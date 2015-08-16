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
    public class InvokePump : WaitHandle
    {

        #region Fields

        private int _threadId;

        private Action _invoking;
        private object _invokeLock = new object();
        private EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private EventWaitHandle _waitHandleAlt = new EventWaitHandle(false, EventResetMode.AutoReset);

        #endregion

        #region CONSTRUCTOR
        
        public InvokePump()
            : this(null)
        {

        }

        public InvokePump(Thread ownerThread)
        {
            _threadId = (ownerThread != null) ? ownerThread.ManagedThreadId : System.Threading.Thread.CurrentThread.ManagedThreadId;
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
            if (action == null) throw new System.ArgumentNullException("action");

            lock (_invokeLock)
            {
                _invoking += action;
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
                _invoking += action;
            }
        }

        /// <summary>
        /// Can only be called by the thread that owns this InvokePump, this will run all queued actions.
        /// </summary>
        public void Update()
        {
            if (_threadId == 0) return; //we're destroyed
            if (this.InvokeRequired) throw new System.InvalidOperationException("InvokePump.Update can only be updated on the thread that was designated its owner.");

            if (_invoking != null)
            {
                Action act;
                EventWaitHandle handle;
                lock (_invokeLock)
                {
                    act = _invoking;
                    handle = _waitHandle;
                    _invoking = null;
                    _waitHandle = _waitHandleAlt;
                    _waitHandleAlt = handle;
                }

                //call delegate
                act();

                //release waits
                handle.Set();
            }
        }

        #endregion

        #region Overrides

        public override void Close()
        {
            base.Close();

            if (_threadId == 0) return; //already was destroyed
            _waitHandle.Close();
            _waitHandleAlt.Close();
            _threadId = 0;
        }

        protected override void Dispose(bool explicitDisposing)
        {
            base.Dispose(explicitDisposing);

            if (_threadId == 0) return; //already was destroyed
            (_waitHandle as IDisposable).Dispose();
            (_waitHandleAlt as IDisposable).Dispose();
            _threadId = 0;
        }

        public override bool WaitOne()
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            return _waitHandle.WaitOne();
        }

        public override bool WaitOne(int millisecondsTimeout)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            return _waitHandle.WaitOne(millisecondsTimeout);
        }

        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            return _waitHandle.WaitOne(millisecondsTimeout, exitContext);
        }

        public override bool WaitOne(TimeSpan timeout)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            return _waitHandle.WaitOne(timeout);
        }

        public override bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
            return _waitHandle.WaitOne(timeout, exitContext);
        }


        #endregion

    }
}
