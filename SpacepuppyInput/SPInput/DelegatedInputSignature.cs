﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput
{

    public delegate bool ButtonDelegate();
    public delegate float AxisDelegate();

    public class DelegatedButtonInputSignature : BaseInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonDelegate _delegate;
        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #endregion

        #region CONSTRUCTOR

        public DelegatedButtonInputSignature(string id, ButtonDelegate del)
            : base(id)
        {
            _delegate = del;
        }

        #endregion

        #region Properties

        public ButtonDelegate Delegate
        {
            get { return _delegate; }
            set { _delegate = value; }
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
            _current = InputUtil.GetNextButtonState(_current, _delegate != null ? _delegate() : false);
            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
        }

        public override void FixedUpdate()
        {
            //determine based on history
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, _delegate != null ? _delegate() : false);
        }

        public override void Reset()
        {
            _current = ButtonState.None;
            _currentFixed = ButtonState.None;
            _lastDown = 0f;
        }

        #endregion

    }

    public class DelegatedAxleInputSignature : BaseInputSignature, IAxleInputSignature
    {

        #region Fields

        private AxisDelegate _delegate;

        #endregion

        #region CONSTRUCTOR

        public DelegatedAxleInputSignature(string id, AxisDelegate del)
            : base(id)
        {
            _delegate = del;
        }

        #endregion

        #region Properties

        public AxisDelegate Delegate
        {
            get { return _delegate; }
            set { _delegate = value; }
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public bool Invert
        {
            get;
            set;
        }

        #endregion

        #region IAxleInputSignature Interface

        public float CurrentState
        {
            get
            {
                //return _current;

                var v = _delegate != null ? _delegate() : 0f;
                if (this.Invert) v *= -1;
                return InputUtil.CutoffAxis(v, this.DeadZone, this.Cutoff);
            }
        }

        public float DeadZone
        {
            get;
            set;
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {

        }

        public override void Reset()
        {
        }

        #endregion

    }

    /// <summary>
    /// Dual mode input signature. It allows for axes to act like a button. It behaves exactly as a DelegatedAxleInputSignature, 
    /// but will return appropriate values when treated as a button. Very useful if you have dual inputs for the same 
    /// action that should act like buttons. For instance both pressing up on left stick, or pressing A to jump.
    /// </summary>
    public class DelegatedAxleButtonInputSignature : DelegatedAxleInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #endregion

        #region CONSTRUCTOR

        public DelegatedAxleButtonInputSignature(string id, AxisDelegate del, AxleValueConsideration consideration = AxleValueConsideration.Positive, float axisButtnDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
            : base(id, del)
        {
            this.AxisButtonDeadZone = axisButtnDeadZone;
            this.Consideration = consideration;
        }

        #endregion

        #region Properties

        /// <summary>
        /// If input is treated as a axis-button, where is the dead-zone of the button. As opposed to the axes' DeadZone.
        /// </summary>
        public float AxisButtonDeadZone
        {
            get;
            set;
        }

        /// <summary>
        /// How should we consider the value returned for the axis.
        /// </summary>
        public AxleValueConsideration Consideration
        {
            get;
            set;
        }

        #endregion

        #region IButtonInputSignature Interface

        public new ButtonState CurrentState
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
            base.Update();

            float v = base.CurrentState;
            switch (this.Consideration)
            {
                case AxleValueConsideration.Positive:
                    _current = InputUtil.GetNextButtonState(_current, v >= this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Negative:
                    _current = InputUtil.GetNextButtonState(_current, v <= -this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Absolute:
                    //_current = InputUtil.GetNextButtonState(_current, Input.GetButton(this.UnityInputId) || Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    _current = InputUtil.GetNextButtonState(_current, v >= this.AxisButtonDeadZone);
                    break;
            }

            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float v = base.CurrentState;
            switch (this.Consideration)
            {
                case AxleValueConsideration.Positive:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, v >= this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Negative:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) <= -this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, v <= -this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Absolute:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, v >= this.AxisButtonDeadZone);
                    break;
            }
        }

        public override void Reset()
        {
            base.Reset();

            _current = ButtonState.None;
            _currentFixed = ButtonState.None;
            _lastDown = 0f;
        }

        #endregion

    }

    public class DelegatedDualAxleInputSignature : BaseInputSignature, IDualAxleInputSignature
    {

        #region Fields

        private AxisDelegate _horizontal;
        private AxisDelegate _vertical;

        #endregion

        #region CONSTRUCTOR

        public DelegatedDualAxleInputSignature(string id, AxisDelegate hor, AxisDelegate ver)
            : base(id)
        {
            _horizontal = hor;
            _vertical = ver;
        }

        #endregion

        #region Properties
        
        public AxisDelegate HorizontalDelegate
        {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        public AxisDelegate VerticalDelegate
        {
            get { return _vertical; }
            set { _vertical = value; }
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public float RadialDeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff RadialCutoff
        {
            get;
            set;
        }

        public bool InvertX
        {
            get;
            set;
        }

        public bool InvertY
        {
            get;
            set;
        }

        #endregion

        #region IDualAxleInputSignature Interface

        public Vector2 CurrentState
        {
            get
            {
                //return _current;
                Vector2 v = new Vector2(_horizontal != null ? _horizontal() : 0f,
                                        _vertical != null ? _vertical() : 0f);
                if (this.InvertX) v.x = -v.x;
                if (this.InvertY) v.y = -v.y;
                return InputUtil.CutoffDualAxis(v, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            }
        }

        public float DeadZone
        {
            get;
            set;
        }

        #endregion

        #region IInputSignature Interface

        public override void Update()
        {

        }

        public override void Reset()
        {
        }

        #endregion

    }

    /// <summary>
    /// Dual mode input signature. It allows for dual axes to act like a button. It behaves exactly as a DelegateDualAxleInputSignature, 
    /// but will return ButtonStates based on the magnitude from center. Very useful if you want to know if the user popped their dual axes. 
    /// </summary>
    public class DelegatedDualAxleButtonInputSignature : DelegatedDualAxleInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #endregion

        #region CONSTRUCTOR

        public DelegatedDualAxleButtonInputSignature(string id, AxisDelegate hor, AxisDelegate ver, float axisButtnDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
            : base(id, hor, ver)
        {
            this.AxisButtonDeadZone = axisButtnDeadZone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// If input is treated as a axis-button, where is the dead-zone of the button. As opposed to the axes' DeadZone.
        /// </summary>
        public float AxisButtonDeadZone
        {
            get;
            set;
        }

        #endregion

        #region IButtonInputSignature Interface

        public new ButtonState CurrentState
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
            base.Update();

            float v = base.CurrentState.sqrMagnitude;
            float dz = this.AxisButtonDeadZone;
            _current = InputUtil.GetNextButtonState(_current, v >= (dz * dz));

            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float v = base.CurrentState.sqrMagnitude;
            float dz = this.AxisButtonDeadZone;
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, v >= (dz * dz));
        }

        public override void Reset()
        {
            base.Reset();

            _current = ButtonState.None;
            _currentFixed = ButtonState.None;
            _lastDown = 0f;
        }

        #endregion

    }

}
