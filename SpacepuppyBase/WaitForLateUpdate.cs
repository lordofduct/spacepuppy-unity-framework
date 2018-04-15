using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{
    public sealed class WaitForLateUpdate : IImmediatelyResumingYieldInstruction
    {

        #region Fields

        private System.EventHandler _signal;

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
            add
            {
                if (_signal == null)
                    _signal = value;
                else if (_signal == value)
                    return;
                else
                    _signal += value;
            }
            remove
            {
                if (_signal == null)
                    return;
                else if (_signal == value)
                    _signal = null;
                else
                    _signal -= value;
            }
        }

        #endregion

        #region Static Factory

        /*
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
                        if (_handle != null && _handle._signal != null)
                        {
                            _handle._signal(_handle, EventArgs.Empty);
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
        */

        private static WaitForLateUpdate _handle;
        private static PumpToken _token;
        public static WaitForLateUpdate Create()
        {
            if (_handle == null) _handle = new WaitForLateUpdate();
            if (_token == null)
            {
                _token = new PumpToken();
                GameLoopEntry.LateUpdatePump.Add(_token);
            }
            return _handle;
        }

        private class PumpToken : IUpdateable
        {
            void IUpdateable.Update()
            {
                if (_handle != null && _handle._signal != null)
                {
                    _handle._signal(_handle, EventArgs.Empty);
                }
            }
        }

        #endregion

    }
}
