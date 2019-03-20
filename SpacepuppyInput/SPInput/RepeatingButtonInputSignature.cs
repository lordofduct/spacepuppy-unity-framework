using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput
{

    /// <summary>
    /// Creates a button signature that repeats the 'Down' signal as it is held.
    /// 
    /// Note, only works for Update and not FixedUpdate.
    /// </summary>
    public class RepeatingButtonInputSignature : BaseInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonDelegate _button;
        private float _firstRepeatDelay;
        private float _repeatDelay;
        private float _repeatLerp;
        private float _maxRepeat;

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        private float _delay;
        private int _repeatCount;

        #endregion

        #region CONSTRUCTOR

        public RepeatingButtonInputSignature(string id)
            : base(id)
        {
        }

        public RepeatingButtonInputSignature(string id, ButtonDelegate btn, float repeatDelay)
            : base(id)
        {
            _button = btn;
            this.FirstRepeatDelay = repeatDelay;
            this.RepeatDelay = repeatDelay;
            this.RepeatLerp = 0f;
            this.MaxRepeat = repeatDelay;
        }

        #endregion

        #region Properties

        public ButtonDelegate ButtonDelegate
        {
            get { return _button; }
            set { _button = value; }
        }

        /// <summary>
        /// How long to wait before the first repeat should occur.
        /// </summary>
        public float FirstRepeatDelay
        {
            get { return _firstRepeatDelay; }
            set { _firstRepeatDelay = Mathf.Max(0f, value); }
        }

        /// <summary>
        /// How long to wait between each repeat after the first.
        /// </summary>
        public float RepeatDelay
        {
            get { return _repeatDelay; }
            set { _repeatDelay = Mathf.Max(0f, value); }
        }

        /// <summary>
        /// Set to a value &gt 0 to have the RepeatRate scale over time.
        /// </summary>
        public float RepeatLerp
        {
            get { return _repeatLerp; }
            set { _repeatLerp = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// The value that RepeatRate scales towards over time if RepeatLerp is &gt 0.
        /// </summary>
        public float MaxRepeat
        {
            get { return _maxRepeat; }
            set { _maxRepeat = Mathf.Max(0f, value); }
        }

        /// <summary>
        /// Number of times the Down signal has repeated since last official down.
        /// </summary>
        public int CurrentRepeatCount
        {
            get { return _repeatCount; }
        }

        #endregion

        #region IButtonInputSignature Interface

        public ButtonState CurrentState
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return _currentFixed;
                }
                else
                {
                    return _current;
                }
            }
        }

        public ButtonState GetCurrentState(bool getFixedState)
        {
            return (getFixedState) ? _currentFixed : _current;
        }

        public void Consume()
        {
            if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
            {
                _currentFixed = InputUtil.ConsumeButtonState(_currentFixed);
            }
            else
            {
                _current = InputUtil.ConsumeButtonState(_current);
            }
        }

        public float LastDownTime
        {
            get { return _lastDown; }
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            //determine based on history
            _current = InputUtil.GetNextButtonState(_current, _button != null ? _button() : false);
            if (_current == ButtonState.Down)
            {
                _lastDown = Time.realtimeSinceStartup;
                _delay = _firstRepeatDelay;
                _repeatCount = 0;
            }
            else if (_current == ButtonState.Held && Time.realtimeSinceStartup - _lastDown > _repeatDelay)
            {
                _current = ButtonState.Down;
                _lastDown = Time.realtimeSinceStartup;
                if (_repeatCount == 0)
                    _delay = _repeatDelay;
                else
                    _delay = Mathf.Lerp(_delay, _maxRepeat, _repeatLerp);
                _repeatCount++;
            }
        }

        public override void FixedUpdate()
        {
            //determine based on history
            _currentFixed = InputUtil.GetNextButtonState(_current, _button != null ? _button() : false);
        }

        public override void Reset()
        {
            _current = ButtonState.None;
            _currentFixed = ButtonState.None;
            _lastDown = 0f;
        }

        #endregion

    }

}
