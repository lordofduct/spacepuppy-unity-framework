using System;
using System.Collections.Generic;
using System.Threading;

namespace com.spacepuppy.Async
{

    /// <summary>
    /// SPThreadPool manages a set pool of threads to be used within your game just like System.Threading.ThreadPool. 
    /// Unlike System.Threading.ThreadPool though, it reacts to the various events in Unity when managing its threads. 
    /// This way a task can be cancelled automatically if needed.
    /// 
    /// The default thread counts are as followed:
    /// WebGL - not supported, 1 thread
    /// Mobile - Max(coreCount, 5)
    /// PC/Mac/Console - Max(coreCount, 10)
    /// 
    /// More threads are managed than cores in most cases. This is because a thread isn't necessarily meant to eek out more 
    /// speed from a processor by using another core (though it can be used for that), but rather to allow asynchronous coding 
    /// of two tasks.
    /// 
    /// Better speed could be accomplished by matching the core count if so desired. You can resize the number of available 
    /// threads by calling the 'Resize' method.
    /// </summary>
#if SP_LIB
    [Singleton.Config(DefaultLifeCycle = SingletonLifeCycleRule.LivesForever, ExcludeFromSingletonManager = true, LifeCycleReadOnly = true)]
    public class SPThreadPool : Singleton
    {

    #region Singleton Interface

        private static SPThreadPool _instance;
        private static SPThreadPool CreateInstance()
        {
            if (object.ReferenceEquals(_instance, null))
            {
                _instance = Singleton.CreateSpecialInstance<SPThreadPool>("Spacepuppy.SPThreadPool", SingletonLifeCycleRule.LivesForever);
            }

            return _instance;
        }

    #endregion

#else
    public class SPThreadPool : SPComponent
    {


        #region Singleton Interface

        private static SPThreadPool _instance;
        private static SPThreadPool CreateInstance()
        {
            if (object.ReferenceEquals(_instance, null))
            {
                var go = new UnityEngine.GameObject("Spacepuppy.SPThreadPool");
                _instance = go.AddComponent<SPThreadPool>();
                UnityEngine.Object.DontDestroyOnLoad(go);
            }

            return _instance;
        }

        #endregion


#endif



        #region Static Interface

        public static bool Initialized
        {
            get { return _instance != null; }
        }

        public static void Resize(int maxThreads)
        {
            if(!Initialized)
            {
                CreateInstance();
            }
            _instance.ResizePool(maxThreads);
        }

        public static bool QueueUserWorkItem(WaitCallback callback)
        {
            if (!Initialized)
            {
                CreateInstance();
                _instance.ResizePool(0);
            }
            return _instance.EnqueueTask(callback, null, false);
        }

        public static bool QueueUserWorkItem(WaitCallback callback, bool cancelOnLevelLoaded)
        {
            if (!Initialized)
            {
                CreateInstance();
                _instance.ResizePool(0);
            }
            return _instance.EnqueueTask(callback, null, cancelOnLevelLoaded);
        }

        public static bool QueueUserWorkItem(WaitCallback callback, object state)
        {
            if (!Initialized)
            {
                CreateInstance();
                _instance.ResizePool(0);
            }
            return _instance.EnqueueTask(callback, state, false);
        }

        public static bool QueueUserWorkItem(WaitCallback callback, object state, bool cancelOnLevelLoaded)
        {
            if (!Initialized)
            {
                CreateInstance();
                _instance.ResizePool(0);
            }
            return _instance.EnqueueTask(callback, state, cancelOnLevelLoaded);
        }

        public static int GetMaxThreadCount()
        {
            if (!Initialized)
            {
                CreateInstance();
                _instance.ResizePool(0);
            }
            return _instance._maxThreadCount;
        }

        public static int GetAvailableThreadCount()
        {
            if (!Initialized)
            {
                CreateInstance();
                _instance.ResizePool(0);
            }
            lock (_instance._lock)
            {
                return _instance._maxThreadCount - _instance._activeThreads.Count;
            }
        }
        
#endregion

#region Instance Interface
        
#region Fields
        
        private int _maxThreadCount;
        private Stack<ThreadState> _openThreads;
        private HashSet<ThreadState> _activeThreads;
        private Queue<TaskInfo> _taskQueue;

        private object _lock = new object();

#endregion

#region CONSTRUCTOR

        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.PurgeThreads();
        }

        private void OnLevelWasLoaded()
        {
            this.PurgeCancellableTasks();
        }

#endregion
        
#region Methods

        private void ResizePool(int maxThreads)
        {
            if (maxThreads <= 0)
            {
                //thread-count higher than core-count can be useful on low-core systems
                //otherwise a scheduled task will have to wait for a previously scheduled task
                //async isn't necessarily about using all the cores, but more about performing a task asynchronously
                //of course, you can set the 'maxThreads' manually if you prefer
                if (UnityEngine.Application.isMobilePlatform)
                {
                    maxThreads = Math.Max(UnityEngine.SystemInfo.processorCount, 5);
                }
                else if(UnityEngine.Application.isWebPlayer)
                {
                    //WARNING - this mode technically not supported!
                    maxThreads = 1;
                }
                else
                {
                    maxThreads = Math.Max(UnityEngine.SystemInfo.processorCount, 10);
                }
            }

            lock (_lock)
            {
                if(_activeThreads != null)
                {
                    _maxThreadCount = maxThreads;
                    //remove any unneeded waiting threads
                    while(_openThreads.Count > _maxThreadCount)
                    {
                        var thread = _openThreads.Pop();
                        thread.Thread.Interrupt();
                    }

                    //terminate any threads that are over count
                    int cnt = _maxThreadCount - _openThreads.Count;
                    if(_activeThreads.Count > cnt)
                    {
                        var old = _activeThreads;
                        _activeThreads = new HashSet<ThreadState>();
                        var e = old.GetEnumerator();
                        int i = 0;
                        while(i < cnt && e.MoveNext())
                        {
                            _activeThreads.Add(e.Current);
                            i++;
                        }
                        while(e.MoveNext())
                        {
                            e.Current.IsTerminating = true;
                        }
                    }
                }
                else
                {
                    _maxThreadCount = maxThreads;
                    _openThreads = new Stack<ThreadState>(_maxThreadCount);
                    _activeThreads = new HashSet<ThreadState>();
                    _taskQueue = new Queue<TaskInfo>();
                }
            }
        }

        private bool EnqueueTask(WaitCallback callback, object state, bool cancellable)
        {
            lock(_lock)
            {
                _taskQueue.Enqueue(new TaskInfo(callback, state, cancellable));
                if(_openThreads.Count > 0)
                {
                    var thread = _openThreads.Pop();
                    thread.WaitHandle.Set();
                }
                else if(_activeThreads.Count < _maxThreadCount)
                {
                    var thread = new ThreadState();
                    thread.Thread = new Thread(MultiThreadedTaskCallback);
                    thread.WaitHandle = new AutoResetEvent(true);
                    _activeThreads.Add(thread);
                    thread.Thread.Start(thread);
                }
            }

            return true;
        }

        private void PurgeThreads()
        {
            lock(_lock)
            {
                if(_activeThreads != null && _activeThreads.Count > 0)
                {
                    var e = _activeThreads.GetEnumerator();
                    while (e.MoveNext())
                    {
                        e.Current.IsTerminating = true;
                    }

                    _activeThreads.Clear();
                }
                if(_openThreads != null && _openThreads.Count > 0)
                {
                    var e = _openThreads.GetEnumerator();
                    while(e.MoveNext())
                    {
                        e.Current.Thread.Interrupt();
                    }

                    _openThreads.Clear();
                }

                _maxThreadCount = 0;
                _activeThreads = null;
                _taskQueue = null;
            }
        }

        private void PurgeCancellableTasks()
        {
            lock (_lock)
            {
                if (_taskQueue.Count > 0)
                {
                    bool anyCancellable = false;
                    var e = _taskQueue.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.Cancellable)
                        {
                            anyCancellable = true;
                            break;
                        }
                    }

                    if (anyCancellable)
                    {
                        var old = _taskQueue;
                        _taskQueue = new Queue<TaskInfo>();
                        e = old.GetEnumerator();
                        while (e.MoveNext())
                        {
                            if (!e.Current.Cancellable) _taskQueue.Enqueue(e.Current);
                        }
                    }
                }
            }
        }



        private void MultiThreadedTaskCallback(object data)
        {
            ThreadState state = data as ThreadState;
            if (state == null) return;

WaitUntilNextTask:
            state.WaitHandle.WaitOne();

GetNextTask:
            if (state.IsTerminating) return;

            TaskInfo task = default(TaskInfo);
            lock (_lock)
            {
                if (_taskQueue.Count > 0)
                {
                    task = _taskQueue.Dequeue();
                }
                else
                {
                    _activeThreads.Remove(state);
                    if (state.IsTerminating) return;
                    _openThreads.Push(state);
                    goto WaitUntilNextTask;
                }
            }
            if (task.Callback != null) task.Callback(task.State);

            goto GetNextTask;
        }

#endregion

        private struct TaskInfo
        {
            public WaitCallback Callback;
            public object State;
            public bool Cancellable;

            public TaskInfo(WaitCallback callback, object state, bool cancellable)
            {
                this.Callback = callback;
                this.State = state;
                this.Cancellable = cancellable;
            }
        }

        private class ThreadState
        {
            
            public Thread Thread;
            public AutoResetEvent WaitHandle;
            public volatile bool IsTerminating;
            
        }
        
#endregion

    }
}
