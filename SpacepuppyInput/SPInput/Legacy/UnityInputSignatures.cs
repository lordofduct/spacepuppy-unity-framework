using UnityEngine;
using System.Collections.Generic;
using System;

namespace com.spacepuppy.SPInput.Legacy
{

    public class ButtonInputSignature : BaseInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #endregion

        #region CONSTRUCTOR

        public ButtonInputSignature(string id, string unityInputId)
            : base(id)
        {
            this.UnityInputId = unityInputId;
        }
        
        #endregion

        #region Properties

        public string UnityInputId
        {
            get;
            set;
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
            _current = InputUtil.GetNextButtonState(_current, Input.GetButton(this.UnityInputId));
            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
        }

        public override void FixedUpdate()
        {
            //determine based on history
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId));
        }

        #endregion

    }

    /// <summary>
    /// Dual mode input signature. It allows for axes to act like a button. It will prefer button presses over it. 
    /// Very useful if you have dual inputs for the same action that should act like buttons. For instance both pressing 
    /// up on left stick, or pressing A to jump.
    /// </summary>
    public class AxleButtonInputSignature : BaseInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #endregion

        #region CONSTRUCTOR

        public AxleButtonInputSignature(string id, string unityInputId, AxleValueConsideration consideration = AxleValueConsideration.Positive, float axisButtnDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
            : base(id)
        {
            this.AxisButtonDeadZone = axisButtnDeadZone;
            this.UnityInputId = unityInputId;
            this.Consideration = consideration;
        }
        
        #endregion

        #region Properties

        public string UnityInputId
        {
            get;
            set;
        }

        /// <summary>
        /// If input is treated as an axis, where is the dead-zone of the button.
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
            ////determine based on history
            //_current = InputUtil.GetNextButtonState(_current, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);

            switch(this.Consideration)
            {
                case AxleValueConsideration.Positive:
                    _current = InputUtil.GetNextButtonState(_current, Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Negative:
                    _current = InputUtil.GetNextButtonState(_current, Input.GetAxis(this.UnityInputId) <= -this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Absolute:
                    //_current = InputUtil.GetNextButtonState(_current, Input.GetButton(this.UnityInputId) || Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    _current = InputUtil.GetNextButtonState(_current, Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    break;
            }

            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
        }

        public override void FixedUpdate()
        {
            //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);

            switch (this.Consideration)
            {
                case AxleValueConsideration.Positive:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Negative:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) <= -this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetAxis(this.UnityInputId) <= -this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Absolute:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    break;
            }
        }

        #endregion

    }

    public class AxleInputSignature : BaseInputSignature, IAxleInputSignature
    {

        #region Fields

        //private float _current;

        #endregion

        #region CONSTRUCTOR

        public AxleInputSignature(string id, string unityInputId)
            : base(id)
        {
            this.UnityInputId = unityInputId;
        }
        
        #endregion

        #region Properties

        public string UnityInputId
        {
            get;
            set;
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

                var v = Input.GetAxis(this.UnityInputId);
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
            /*
            var v = Input.GetAxis(this.UnityInputId);
            if (this.Invert) v *= -1;
            _current = InputUtil.CutoffAxis(v, this.DeadZone, this.Cutoff);
            */
        }

        #endregion

    }

    public class DualAxleInputSignature : BaseInputSignature, IDualAxleInputSignature
    {

        #region Fields

        private string _xAxisId;
        private string _yAxisId;

        //private Vector2 _current;

        #endregion

        #region CONSTRUCTOR

        public DualAxleInputSignature(string id, string xAxisId, string yAxisId)
            : base(id)
        {
            _xAxisId = xAxisId;
            _yAxisId = yAxisId;
        }
        
        #endregion

        #region Properties

        public string XAxisId
        {
            get { return _xAxisId; }
            set { _xAxisId = value; }
        }

        public string YAxisId
        {
            get { return _yAxisId; }
            set { _yAxisId = value; }
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

        /// <summary>
        /// If you accept keyboard/button input as opposed to analogue input, the values that come out are not radial in nature. 
        /// This will ensure that the button/keyboard inputs acts similar to a joystick.
        /// </summary>
        public bool RadialNormalizeButtonInput
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
                Vector2 v = new Vector2(Input.GetAxis(_xAxisId), Input.GetAxis(_yAxisId));
                if (this.InvertX) v.x = -v.x;
                if (this.InvertY) v.y = -v.y;
                if (this.RadialNormalizeButtonInput && (Input.GetButton(_xAxisId) || Input.GetButton(_yAxisId)))
                {
                    if (v.sqrMagnitude > 1f) v.Normalize();
                }
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
            /*
            Vector2 v = new Vector2(Input.GetAxis(_xAxisId), Input.GetAxis(_yAxisId));
            if (this.RadialNormalizeButtonInput && (Input.GetButton(_xAxisId) || Input.GetButton(_yAxisId)))
            {
                if (v.sqrMagnitude > 1f) v.Normalize();
            }
            _current = InputUtil.CutoffDualAxis(v, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            */
        }

        #endregion

    }

    public class MouseCursorInputSignature : BaseInputSignature, ICursorInputSignature
    {


        public MouseCursorInputSignature(string id)
            : base(id)
        {

        }
        



        public Vector2 CurrentState
        {
            get
            {
                return Input.mousePosition;
            }
        }
        
        public override void Update()
        {
            //do nothing
        }
    }

    public class MouseClickInputSignature : BaseInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #endregion

        #region CONSTRUCTOR

        public MouseClickInputSignature(string id, int mouseButton)
            : base(id)
        {
            this.MouseButton = mouseButton;
        }
        
        #endregion

        #region Properties

        public int MouseButton
        {
            get;
            set;
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

        public bool Active
        {
            get
            {
                return this.CurrentState > ButtonState.None;
            }
        }

        public override void Update()
        {
            //determine based on history
            _current = InputUtil.GetNextButtonState(_current, Input.GetMouseButton(this.MouseButton));

            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
        }

        public override void FixedUpdate()
        {
            //determine based on history
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetMouseButton(this.MouseButton));
        }

        #endregion

    }

    public class KeyboardButtonInputSignature : BaseInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #endregion

        #region CONSTRUCTOR

        public KeyboardButtonInputSignature(string id, KeyCode key)
            : base(id)
        {
            this.Key = key;
        }
        
        #endregion

        #region Properties

        public KeyCode Key
        {
            get;
            set;
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
            ////use classic way of determining
            //if (Input.GetKeyDown(this.Key))
            //    _current = ButtonState.Down;
            //else if (Input.GetKeyUp(this.Key))
            //    _current = ButtonState.Released;
            //else if (Input.GetKey(this.Key))
            //    _current = ButtonState.Held;
            //else
            //    _current = ButtonState.None;


            //determine based on history
            _current = InputUtil.GetNextButtonState(_current, Input.GetKey(this.Key));

            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
        }

        public override void FixedUpdate()
        {
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetKey(this.Key));
        }

        #endregion

    }

    public class KeyboardAxleInputSignature : BaseInputSignature, IAxleInputSignature
    {

        #region Fields
        
        //private float _state;

        #endregion

        #region CONSTRUCTOR

        public KeyboardAxleInputSignature(string id, KeyCode positiveKey, KeyCode negativeKey)
            : base(id)
        {
            this.PositiveKey = positiveKey;
            this.NegativeKey = negativeKey;
        }
        
        #endregion

        #region Properties

        public KeyCode PositiveKey
        {
            get;
            set;
        }

        public KeyCode NegativeKey
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
                //return _state;

                if (this.DeadZone > 1f) return 0f;

                if (Input.GetKey(this.PositiveKey))
                    return Input.GetKey(this.NegativeKey) ? 0f : 1f;
                else if (Input.GetKey(this.NegativeKey))
                    return -1f;
                else
                    return 0f;
            }
        }

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            /*
            if (Input.GetKey(this.PositiveKey))
                _state = Input.GetKey(this.NegativeKey) ? 0f : 1f;
            else if (Input.GetKey(this.NegativeKey))
                _state = -1f;
            else
                _state = 0f;

            if (this.DeadZone > 1f) _state = 0f;
            */
        }

        public override void FixedUpdate()
        {
            /*
            if (Input.GetKey(this.PositiveKey))
                _state = Input.GetKey(this.NegativeKey) ? 0f : 1f;
            else if (Input.GetKey(this.NegativeKey))
                _state = -1f;
            else
                _state = 0f;

            if (this.DeadZone > 1f) _state = 0f;
            */
        }

        #endregion

    }

    public class KeyboardDualAxleInputSignature : BaseInputSignature, IDualAxleInputSignature
    {

        #region Fields

        //private Vector2 _state;

        #endregion

        #region CONSTRUCTOR

        public KeyboardDualAxleInputSignature(string id, KeyCode horizontalPositiveKey, KeyCode horizontalNegativeKey, KeyCode verticalPositiveKey, KeyCode verticalNegativeKey)
            : base(id)
        {
            this.HorizontalPositiveKey = horizontalPositiveKey;
            this.HorizontalNegativeKey = horizontalNegativeKey;
            this.VerticalPositiveKey = verticalPositiveKey;
            this.VerticalNegativeKey = verticalNegativeKey;
        }
        
        #endregion

        #region Properties

        public KeyCode HorizontalPositiveKey
        {
            get;
            set;
        }

        public KeyCode HorizontalNegativeKey
        {
            get;
            set;
        }

        public KeyCode VerticalPositiveKey
        {
            get;
            set;
        }

        public KeyCode VerticalNegativeKey
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
                //return _state;

                Vector2 result = Vector2.zero;
                if (Input.GetKey(this.HorizontalPositiveKey))
                    result.x = Input.GetKey(this.HorizontalNegativeKey) ? 0f : 1f;
                else if (Input.GetKey(this.HorizontalNegativeKey))
                    result.x = -1f;
                else
                    result.x = 0f;

                if (Input.GetKey(this.VerticalPositiveKey))
                    result.y = Input.GetKey(this.VerticalNegativeKey) ? 0f : 1f;
                else if (Input.GetKey(this.VerticalNegativeKey))
                    result.y = -1f;
                else
                    result.y = 0f;

                return InputUtil.CutoffDualAxis(result, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            }
        }

        public float DeadZone
        {
            get;
            set;
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

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            /*
            if (Input.GetKey(this.HorizontalPositiveKey))
                _state.x = Input.GetKey(this.HorizontalNegativeKey) ? 0f : 1f;
            else if (Input.GetKey(this.HorizontalNegativeKey))
                _state.x = -1f;
            else
                _state.x = 0f;

            if (Input.GetKey(this.VerticalPositiveKey))
                _state.y = Input.GetKey(this.VerticalNegativeKey) ? 0f : 1f;
            else if (Input.GetKey(this.VerticalNegativeKey))
                _state.y = -1f;
            else
                _state.y = 0f;

            _state = InputUtil.CutoffDualAxis(_state, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            */
        }

        public override void FixedUpdate()
        {
            /*
            if (Input.GetKey(this.HorizontalPositiveKey))
                _state.x = Input.GetKey(this.HorizontalNegativeKey) ? 0f : 1f;
            else if (Input.GetKey(this.HorizontalNegativeKey))
                _state.x = -1f;
            else
                _state.x = 0f;

            if (Input.GetKey(this.VerticalPositiveKey))
                _state.y = Input.GetKey(this.VerticalNegativeKey) ? 0f : 1f;
            else if (Input.GetKey(this.VerticalNegativeKey))
                _state.y = -1f;
            else
                _state.y = 0f;

            _state = InputUtil.CutoffDualAxis(_state, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            */
        }

        #endregion

    }

    public class EmulatedAxleInputSignature : BaseInputSignature, IAxleInputSignature
    {

        #region Fields

        //private float _state;

        #endregion

        #region CONSTRUCTOR

        public EmulatedAxleInputSignature(string id, string positiveBtn, string negativeBtn)
            : base(id)
        {
            this.PositiveButton = positiveBtn;
            this.NegativeButton = negativeBtn;
        }

        #endregion

        #region Properties

        public string PositiveButton
        {
            get;
            set;
        }

        public string NegativeButton
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
                //return _state;

                if (this.DeadZone > 1f) return 0f;
                
                if (this.PositiveButton != null && Input.GetButton(this.PositiveButton))
                     return (this.NegativeButton != null && Input.GetButton(this.NegativeButton)) ? 0f : 1f;
                else if (this.NegativeButton != null && Input.GetButton(this.NegativeButton))
                    return -1f;
                else
                    return 0f;
            }
        }

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            /*
            if (Input.GetButton(this.PositiveButton))
                _state = Input.GetButton(this.NegativeButton) ? 0f : 1f;
            else if (Input.GetButton(this.NegativeButton))
                _state = -1f;
            else
                _state = 0f;

            if (this.DeadZone > 1f) _state = 0f;
            */
        }

        public override void FixedUpdate()
        {
            /*
            if (Input.GetButton(this.PositiveButton))
                _state = Input.GetButton(this.NegativeButton) ? 0f : 1f;
            else if (Input.GetButton(this.NegativeButton))
                _state = -1f;
            else
                _state = 0f;

            if (this.DeadZone > 1f) _state = 0f;
            */
        }

        #endregion

    }

    public class EmulatedDualAxleInputSignature : BaseInputSignature, IDualAxleInputSignature
    {

        #region Fields

        //private Vector2 _state;

        #endregion

        #region CONSTRUCTOR

        public EmulatedDualAxleInputSignature(string id, string horizontalPositiveBtn, string horizontalNegativeBtn, string verticalPositiveBtn, string verticalNegativeBtn)
            : base(id)
        {
            this.HorizontalPositiveButton = horizontalPositiveBtn;
            this.HorizontalNegativeButton = horizontalNegativeBtn;
            this.VerticalPositiveButton = verticalPositiveBtn;
            this.VerticalNegativeButton = verticalNegativeBtn;
        }

        #endregion

        #region Properties

        public string HorizontalPositiveButton
        {
            get;
            set;
        }

        public string HorizontalNegativeButton
        {
            get;
            set;
        }

        public string VerticalPositiveButton
        {
            get;
            set;
        }

        public string VerticalNegativeButton
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
                //return _state;
                Vector2 result = Vector2.zero;
                if (this.HorizontalPositiveButton != null && Input.GetButton(this.HorizontalPositiveButton))
                    result.x = (this.HorizontalNegativeButton != null && Input.GetButton(this.HorizontalNegativeButton)) ? 0f : 1f;
                else if (this.HorizontalNegativeButton != null && Input.GetButton(this.HorizontalNegativeButton))
                    result.x = -1f;
                else
                    result.x = 0f;

                if (this.VerticalPositiveButton != null && Input.GetButton(this.VerticalPositiveButton))
                    result.y = (this.VerticalNegativeButton != null && Input.GetButton(this.VerticalNegativeButton)) ? 0f : 1f;
                else if (this.VerticalNegativeButton != null && Input.GetButton(this.VerticalNegativeButton))
                    result.y = -1f;
                else
                    result.y = 0f;

                return InputUtil.CutoffDualAxis(result, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            }
        }

        public float DeadZone
        {
            get;
            set;
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

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            /*
            if (Input.GetButton(this.HorizontalPositiveButton))
                _state.x = Input.GetButton(this.HorizontalNegativeButton) ? 0f : 1f;
            else if (Input.GetButton(this.HorizontalNegativeButton))
                _state.x = -1f;
            else
                _state.x = 0f;

            if (Input.GetButton(this.VerticalPositiveButton))
                _state.y = Input.GetButton(this.VerticalNegativeButton) ? 0f : 1f;
            else if (Input.GetButton(this.VerticalNegativeButton))
                _state.y = -1f;
            else
                _state.y = 0f;

            _state = InputUtil.CutoffDualAxis(_state, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            */
        }

        public override void FixedUpdate()
        {
            /*
            if (Input.GetButton(this.HorizontalPositiveButton))
                _state.x = Input.GetButton(this.HorizontalNegativeButton) ? 0f : 1f;
            else if (Input.GetButton(this.HorizontalNegativeButton))
                _state.x = -1f;
            else
                _state.x = 0f;

            if (Input.GetButton(this.VerticalPositiveButton))
                _state.y = Input.GetButton(this.VerticalNegativeButton) ? 0f : 1f;
            else if (Input.GetButton(this.VerticalNegativeButton))
                _state.y = -1f;
            else
                _state.y = 0f;

            _state = InputUtil.CutoffDualAxis(_state, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            */
        }

        #endregion

    }
    
}
