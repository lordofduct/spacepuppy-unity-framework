using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{
    public sealed class WaitForLateUpdate : IImmediatelyResumingYieldInstruction
    {

        #region Fields

        private System.EventHandler _handler;

        #endregion

        #region CONSTRUCTOR

        private WaitForLateUpdate()
        {

        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IRadicalYieldInstruction.IsComplete
        {
            get
            {
                return false;
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return true;
        }

        event System.EventHandler IImmediatelyResumingYieldInstruction.Signal
        {
            add { _handler += value; }
            remove { _handler -= value; }
        }

        #endregion

        #region Static Factory

        private static WaitForLateUpdate _handle;
        private static bool _active;
        private static EventHandler _onLateUpdateCallback;
        private static EventHandler OnLateUpdateCallback
        {
            get
            {
                if (_onLateUpdateCallback == null)
                    _onLateUpdateCallback = (s, e) =>
                    {
                        GameLoopEntry.OnLateUpdate -= OnLateUpdateCallback;
                        _active = false;
                        if (_handle != null && _handle._handler != null)
                        {
                            _handle._handler(_handle, EventArgs.Empty);
                        }
                    };
                return _onLateUpdateCallback;
            }
        }

        public static WaitForLateUpdate Create()
        {
            if (_handle == null)
                _handle = new WaitForLateUpdate();
            
            if(!_active)
            {
                _active = true;
                GameLoopEntry.OnLateUpdate += OnLateUpdateCallback;
            }

            return _handle;
        }

        #endregion

    }
}
