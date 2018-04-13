using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI.Sensors.Audible
{

    public class SirenToken : IRadicalWaitHandle
    {

        #region Fields

        private AudibleAspect _aspect;
        private bool _complete;
        private Action<IRadicalWaitHandle> _callback;

        #endregion

        #region CONSTRUCTOR

        public SirenToken(AudibleAspect aspect)
        {
            if (object.ReferenceEquals(aspect, null)) throw new System.ArgumentNullException("aspect");
            _aspect = aspect;
        }

        #endregion

        #region Properties

        public AudibleAspect Aspect
        {
            get { return _aspect; }
        }

        #endregion

        #region Methods

        public void SignalComplete()
        {
            if (_complete) return;

            _complete = true;
            _aspect.EndSiren();
            if (_callback != null) _callback(this);
        }

        #endregion

        #region IRadicalWaitHandle Interface
        
        bool IRadicalWaitHandle.Cancelled { get { return false; } }

        public bool IsComplete { get { return _complete; } }

        public void OnComplete(Action<IRadicalWaitHandle> callback)
        {
            _callback += callback;
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return !_complete;
        }

        #endregion

    }

}
