using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Async
{

    /// <summary>
    /// Queue actions up to be called during some other threads update pump.
    /// </summary>
    public class InvokePump
    {

        #region Fields

        private int _threadId;

        private Action _invoking;
        private object _invokeLock = new object();
        private InvokeWaitHandle _head;
        private ObjectCachePool<InvokeWaitHandle> _waitHandlePool;

        #endregion

        #region CONSTRUCTOR

        public InvokePump()
            : this(null, 10)
        {
        }

        public InvokePump(int handleCount)
            : this(null, handleCount)
        {
        }

        public InvokePump(Thread ownerThread)
            : this(ownerThread, 10)
        {
        }

        public InvokePump(Thread ownerThread, int handleCount)
        {
            if (handleCount < 1) throw new System.ArgumentException("handleCount must be positive and non-zero.", "handleCount");

            _threadId = (ownerThread != null) ? ownerThread.ManagedThreadId : System.Threading.Thread.CurrentThread.ManagedThreadId;
            _waitHandlePool = new ObjectCachePool<InvokeWaitHandle>(handleCount, InvokeWaitHandleConstructor);
        }

        private InvokeWaitHandle InvokeWaitHandleConstructor()
        {
            return new InvokeWaitHandle();
        }

        #endregion

        #region Properties

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
            if (action == null) throw new System.ArgumentNullException("action");

            InvokeWaitHandle handle;
            lock (_invokeLock)
            {
                handle = _waitHandlePool.GetInstance();
                _invoking += action;

                handle.NextNode = _head;
                _head = handle;
            }

            handle.WaitOne(); //block until it's called
        }

        /// <summary>
        /// Queues an action to be invoked next time Update is called. This method does not block.
        /// </summary>
        /// <param name="action"></param>
        public void BeginInvoke(Action action)
        {
            if (action == null) throw new System.ArgumentNullException("action");

            lock (_invokeLock)
            {
                _invoking += action;
            }
        }

        /// <summary>
        /// Can only be called by the thread that owns this InvokePump, this will run all queued actions.
        /// </summary>
        public void Update()
        {
            if (this.InvokeRequired) throw new System.InvalidOperationException("InvokePump.Update can only be updated on the thread that was designated its owner.");

            if (_invoking != null)
            {
                Action act;
                InvokeWaitHandle node;
                lock (_invokeLock)
                {
                    act = _invoking;
                    node = _head;
                    _invoking = null;
                    _head = null;
                }

                //call delegate
                act();

                //release wait handles
                if(node != null)
                {
                    InvokeWaitHandle tnode;
                    while (node != null)
                    {
                        node.Set();
                        tnode = node;
                        node = node.NextNode;

                        tnode.Clean();
                        _waitHandlePool.Release(tnode);
                    }
                }
            }
        }

        #endregion

        #region Special Types

        private class InvokeWaitHandle : EventWaitHandle
        {

            #region Fields

            public InvokeWaitHandle NextNode;

            #endregion

            public InvokeWaitHandle() : base(false, EventResetMode.AutoReset)
            {

            }


            public void Clean()
            {
                this.NextNode = null;
                this.Reset();
            }

        }

        #endregion

    }
}
