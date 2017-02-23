
namespace com.spacepuppy
{

    public interface IRadicalWaitHandle : IRadicalYieldInstruction
    {

        bool Cancelled { get; }

        void OnComplete(System.Action<IRadicalWaitHandle> callback);

    }

    public class RadicalWaitHandle : IRadicalWaitHandle, IPooledYieldInstruction
    {

        #region Fields

        private bool _complete;
        private System.Action<IRadicalWaitHandle> _callback;

        #endregion

        #region CONSTRUCTOR

        protected RadicalWaitHandle()
        {

        }

        #endregion

        #region Methods

        public void SignalCancelled()
        {
            if (_complete) return;

            _complete = true;
            this.Cancelled = true;
            if (_callback != null) _callback(this);
        }

        public void SignalComplete()
        {
            if (_complete) return;

            _complete = true;
            if (_callback != null) _callback(this);
        }

        protected virtual bool Tick(out object yieldObject)
        {
            yieldObject = null;
            return !_complete;
        }

        #endregion

        #region IRadicalWaitHandle Interface

        public bool Cancelled
        {
            get;
            private set;
        }

        public bool IsComplete
        {
            get { return _complete; }
        }

        public void OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");
            if (_complete) throw new System.InvalidOperationException("Can not wait for complete on an already completed IRadicalWaitHandle.");
            _callback += callback;
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if (_complete)
            {
                yieldObject = null;
                return false;
            }

            return this.Tick(out yieldObject);
        }

        #endregion

        #region IPooledYieldInstruction Interface

        void System.IDisposable.Dispose()
        {
            if (this.GetType() == typeof(RadicalWaitHandle))
            {
                _complete = false;
                _callback = null;
                _pool.Release(this);
            }
        }

        #endregion

        #region Static Interface

        private static com.spacepuppy.Collections.ObjectCachePool<RadicalWaitHandle> _pool = new com.spacepuppy.Collections.ObjectCachePool<RadicalWaitHandle>(-1, () => new RadicalWaitHandle());

        public static IRadicalWaitHandle Null
        {
            get
            {
                return NullYieldInstruction.Null;
            }
        }

        public static RadicalWaitHandle Create()
        {
            return _pool.GetInstance();
        }

        #endregion

    }

}
