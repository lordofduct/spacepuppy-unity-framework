using System;
using System.Collections.Generic;
using System.Threading;

namespace com.spacepuppy.Async
{

    /// <summary>
    /// Queue actions up to be called during some other threads update pump.
    /// </summary>
    public class InvokePump : WaitHandle
    {

        #region Fields

        private int _threadId;

        //private Action _invoking;
        private Queue<Action> _invoking;
        private object _invokeLock = new object();
        private EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        //private EventWaitHandle _waitHandleAlt = new EventWaitHandle(false, EventResetMode.AutoReset);

        #endregion

        #region CONSTRUCTOR
        
        public InvokePump()
            : this(null)
        {

        }

        public InvokePump(Thread ownerThread)
        {
            _threadId = (ownerThread != null) ? ownerThread.ManagedThreadId : System.Threading.Thread.CurrentThread.ManagedThreadId;

            _invoking = new Queue<Action>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if on a thread other than the one that owns this pump.
        /// </summary>
        public bool InvokeRequired
        {
            get { return _threadId != 0 && Thread.CurrentThread.ManagedThreadId != _threadId; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Queues an action to be invoked next time Update is called. This method will block until that occurs.
        /// </summary>
        /// <param name="action"></param>
        public void Invoke(Action action)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
            if (action == null) throw new System.ArgumentNullException("action");

            lock (_invokeLock)
            {
                //_invoking += action;
                _invoking.Enqueue(action);
            }
            _waitHandle.WaitOne(); //block until it's called
        }

        /// <summary>
        /// Queues an action to be invoked next time Update is called. This method does not block.
        /// </summary>
        /// <param name="action"></param>
        public void BeginInvoke(Action action)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            if (action == null) throw new System.ArgumentNullException("action");

            lock (_invokeLock)
            {
                //_invoking += action;
                _invoking.Enqueue(action);
            }
        }

        /// <summary>
        /// Can only be called by the thread that owns this InvokePump, this will run all queued actions.
        /// </summary>
        public void Update()
        {
            if (_threadId == 0) return; //we're destroyed
            if (this.InvokeRequired) throw new System.InvalidOperationException("InvokePump.Update can only be updated on the thread that was designated its owner.");
            
            //record the current length so we only activate those actions added at this point
            //any newly added actions should wait until NEXT update
            int cnt = _invoking.Count;
            for(int i = 0; i < cnt; i++)
            {
                Action act;
                lock (_invokeLock)
                {
                    act = _invoking.Dequeue();
                }

                if (act != null) act();
            }

            //release waits
            _waitHandle.Set();
        }

        #endregion

        #region Overrides

        public override void Close()
        {
            base.Close();

            if (_threadId == 0) return; //already was destroyed
            //_invoking = null;
            _invoking.Clear();
            _waitHandle.Close();
            //_waitHandleAlt.Close();
            _threadId = 0;
        }

        protected override void Dispose(bool explicitDisposing)
        {
            base.Dispose(explicitDisposing);

            if (_threadId == 0) return; //already was destroyed
            //_invoking = null;
            _invoking.Clear();
            (_waitHandle as IDisposable).Dispose();
            //(_waitHandleAlt as IDisposable).Dispose();
            _threadId = 0;
        }

        public override bool WaitOne()
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
            return _waitHandle.WaitOne();
        }

        public override bool WaitOne(int millisecondsTimeout)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
            return _waitHandle.WaitOne(millisecondsTimeout);
        }

        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
            return _waitHandle.WaitOne(millisecondsTimeout, exitContext);
        }

        public override bool WaitOne(TimeSpan timeout)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
            return _waitHandle.WaitOne(timeout);
        }

        public override bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
            return _waitHandle.WaitOne(timeout, exitContext);
        }


        #endregion

    }

}
