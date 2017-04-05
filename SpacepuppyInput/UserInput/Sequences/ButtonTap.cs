using UnityEngine;

namespace com.spacepuppy.UserInput.Sequences
{
    public class ButtonTap : ISequence
    {

        #region Fields

        private IButtonInputSignature _button;
        private float _tapBufferDur;
        private int _tapCount;
        private float _nextTapBufferDur;
        private System.Action _callback;

        private bool _lastWasDown;
        private float _t;
        private int _currentCount;

        #endregion

        #region CONSTRUCTOR

        public ButtonTap(IButtonInputSignature button, float tapBufferDur, System.Action callback)
        {
            _button = button;
            _tapBufferDur = tapBufferDur;
            _tapCount = 1;
            _nextTapBufferDur = 0f;
            _callback = callback;
        }

        public ButtonTap(IButtonInputSignature button, float tapBufferDur, int tapCount, float nextTapBufferDur, System.Action callback)
        {
            _button = button;
            _tapBufferDur = tapBufferDur;
            _tapCount = tapCount;
            _nextTapBufferDur = nextTapBufferDur;
            _callback = callback;
        }

        #endregion

        #region ISequence Interface

        public void OnStart()
        {
            _lastWasDown = (_button != null && _button.CurrentState > ButtonState.None);
            _t = 0f;
            _currentCount = 0;
        }

        public bool Update()
        {
            if (_button == null) return true;
            if (_t >= _tapBufferDur) return true;

            if(_lastWasDown)
            {
                if (_button.CurrentState <= ButtonState.None)
                {
                    _lastWasDown = false;
                    _currentCount++;
                    if (_currentCount >= _tapCount)
                    {
                        if (_callback != null) _callback();
                        return true;
                    }
                    else
                    {
                        _t = 0f;
                        return false;
                    }
                }
            }
            else
            {
                if (_currentCount > 0 && _t >= _nextTapBufferDur) return true;
                
                if (_button.CurrentState > ButtonState.None)
                {
                    _lastWasDown = true;
                    _t = 0f;
                    return false;
                }
            }

            _t += Time.unscaledDeltaTime;
            return false;
        }

        #endregion

    }
}
