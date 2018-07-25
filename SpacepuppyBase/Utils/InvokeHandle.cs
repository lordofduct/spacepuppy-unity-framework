using System;
using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{

    /// <summary>
    /// Waits a duration of time and then calls a System.Action.
    /// 
    /// This can accept a System.Collections.IEnumerator just like a Coroutine, but it does not respect any yield instructions. All yielded objects wait a single frame.
    /// </summary>
    public sealed class InvokeHandle : IUpdateable, IRadicalWaitHandle, IDisposable
    {

        #region Fields

        private UpdatePump _pump;
        private System.Action _callback;
        private System.Collections.IEnumerator _handle;

        #endregion

        #region CONSTRUCTOR

        private InvokeHandle()
        {

        }

        #endregion

        #region IUpdateable Interface

        void IUpdateable.Update()
        {
            if(_handle == null || !_handle.MoveNext())
            {
                var d = _callback;
                this.Dispose();
                if (d != null) d();
            }
        }

        #endregion

        #region IRadicalWaitHandle Interface

        bool IRadicalWaitHandle.Cancelled
        {
            get { return false; }
        }

        public bool IsComplete
        {
            get { return _handle == null; }
        }

        void IRadicalWaitHandle.OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            if (_handle == null || callback == null) return;

            _callback += () => callback(this);
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return _handle != null;
        }

        #endregion

        #region ISPDisposable Interface

        public void Dispose()
        {
            if (_pump != null) _pump.Remove(this);
            _pump = null;
            _callback = null;
            if (_handle is IPooledYieldInstruction) (_handle as IPooledYieldInstruction).Dispose();
            _handle = null;
            _pool.Release(this);
        }

        #endregion

        #region Static Factory

        private static ObjectCachePool<InvokeHandle> _pool = new ObjectCachePool<InvokeHandle>(-1, () => new InvokeHandle());

        public static InvokeHandle Begin(UpdatePump pump, System.Action callback, float duration, ITimeSupplier time)
        {
            if (pump == null) throw new System.ArgumentNullException("pump");

            var handle = _pool.GetInstance();
            handle._callback = callback;
            handle._handle = WaitForDuration.Seconds(duration, time);
            handle._pump = pump;

            pump.Add(handle);

            return handle;
        }

        public static InvokeHandle Begin(UpdatePump pump, System.Action callback, System.Collections.IEnumerator e)
        {
            if (pump == null) throw new System.ArgumentNullException("pump");

            var handle = _pool.GetInstance();
            handle._callback = callback;
            handle._handle = e;
            handle._pump = pump;

            pump.Add(handle);

            return handle;
        }

        #endregion

    }

}
