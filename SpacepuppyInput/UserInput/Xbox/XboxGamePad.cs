using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.UserInput.Xbox
{
    public class XboxGamePad : IXboxGamePad
    {

        #region Const IDS

        private const string ID_LEFT_X = "pall.left.x";
        private const string ID_LEFT_Y = "pall.left.y";
        private const string ID_RIGHT_X = "pall.right.x";
        private const string ID_RIGHT_Y = "pall.right.y";
        private const string ID_TRIGGER_LEFT = "pall.trigger.left";
        private const string ID_TRIGGER_RIGHT = "pall.trigger.right";

        private const string ID_DPAD_UP = "pall.dpad.up";
        private const string ID_DPAD_RIGHT = "pall.dpad.right";
        private const string ID_DPAD_DOWN = "pall.dpad.down";
        private const string ID_DPAD_LEFT = "pall.dpad.left";

        private const string ID_BTN_A = "pall.btn.a";
        private const string ID_BTN_B = "pall.btn.b";
        private const string ID_BTN_X = "pall.btn.x";
        private const string ID_BTN_Y = "pall.btn.y";
        private const string ID_BTN_LB = "pall.btn.lb";
        private const string ID_BTN_RB = "pall.btn.rb";
        private const string ID_BTN_START = "pall.btn.start";
        private const string ID_BTN_BACK = "pall.btn.back";
        private const string ID_BTN_LEFTSTICK = "pall.btn.leftstick";
        private const string ID_BTN_RIGHTSTICK = "pall.btn.rightstick";


        private const string XID_LEFT_X = "p{0}.left.x";
        private const string XID_LEFT_Y = "p{0}.left.y";
        private const string XID_RIGHT_X = "p{0}.right.x";
        private const string XID_RIGHT_Y = "p{0}.right.y";
        private const string XID_TRIGGER_LEFT = "p{0}.trigger.left";
        private const string XID_TRIGGER_RIGHT = "p{0}.trigger.right";

        private const string XID_DPAD_UP = "p{0}.dpad.up";
        private const string XID_DPAD_RIGHT = "p{0}.dpad.right";
        private const string XID_DPAD_DOWN = "p{0}.dpad.down";
        private const string XID_DPAD_LEFT = "p{0}.dpad.left";

        private const string XID_BTN_A = "p{0}.btn.a";
        private const string XID_BTN_B = "p{0}.btn.b";
        private const string XID_BTN_X = "p{0}.btn.x";
        private const string XID_BTN_Y = "p{0}.btn.y";
        private const string XID_BTN_LB = "p{0}.btn.lb";
        private const string XID_BTN_RB = "p{0}.btn.rb";
        private const string XID_BTN_START = "p{0}.btn.start";
        private const string XID_BTN_BACK = "p{0}.btn.back";
        private const string XID_BTN_LEFTSTICK = "p{0}.btn.leftstick";
        private const string XID_BTN_RIGHTSTICK = "p{0}.btn.rightstick";

        #endregion

        #region Fields

        private int _gamepadIndex;
        private string _id;
        private bool _active = true;
        private int _hash;
        private float _precedence;

        private Dictionary<XboxButtons, string> _btnToId;
        private Dictionary<XboxAxleInputs, string> _axleToId;

        private XboxGamePadState _current;
        private XboxGamePadState _currentFixed;

        #endregion

        #region CONSTRUCTOR

        public XboxGamePad(int gamepadIndex = -1)
        {
            _gamepadIndex = gamepadIndex;
            _id = (_gamepadIndex >= 0) ? "Xbox GamePad " + _gamepadIndex.ToString() : "All Xbox GamePads";
            _hash = _id.GetHashCode();

            this.Init();
        }

        public XboxGamePad(string id, int gamepadIndex = -1)
        {
            _gamepadIndex = gamepadIndex;
            _id = id;
            _hash = _id.GetHashCode();

            this.Init();
        }

        private void Init()
        {
            if(_gamepadIndex >= 0)
            {
                _axleToId = new Dictionary<XboxAxleInputs, string>();
                _btnToId = new Dictionary<XboxButtons, string>();

                var sindex = _gamepadIndex.ToString();

                _axleToId.Add(XboxAxleInputs.LeftStickX, string.Format(XID_LEFT_X, sindex));
                _axleToId.Add(XboxAxleInputs.LeftStickY, string.Format(XID_LEFT_Y, sindex));
                _axleToId.Add(XboxAxleInputs.RightStickX, string.Format(XID_RIGHT_X, sindex));
                _axleToId.Add(XboxAxleInputs.RightStickY, string.Format(XID_RIGHT_X, sindex));
                _axleToId.Add(XboxAxleInputs.LeftTrigger, string.Format(XID_TRIGGER_LEFT, sindex));
                _axleToId.Add(XboxAxleInputs.RightTrigger, string.Format(XID_TRIGGER_RIGHT, sindex));

                _btnToId.Add(XboxButtons.DPadUp, string.Format(XID_DPAD_UP));
                _btnToId.Add(XboxButtons.DPadRight, string.Format(XID_DPAD_RIGHT));
                _btnToId.Add(XboxButtons.DPadDown, string.Format(XID_DPAD_DOWN));
                _btnToId.Add(XboxButtons.DPadLeft, string.Format(XID_DPAD_LEFT));
                _btnToId.Add(XboxButtons.A, string.Format(XID_BTN_A));
                _btnToId.Add(XboxButtons.B, string.Format(XID_BTN_B));
                _btnToId.Add(XboxButtons.X, string.Format(XID_BTN_X));
                _btnToId.Add(XboxButtons.Y, string.Format(XID_BTN_Y));
                _btnToId.Add(XboxButtons.LB, string.Format(XID_BTN_LB));
                _btnToId.Add(XboxButtons.RB, string.Format(XID_BTN_RB));
                _btnToId.Add(XboxButtons.Start, string.Format(XID_BTN_START));
                _btnToId.Add(XboxButtons.Back, string.Format(XID_BTN_BACK));
                _btnToId.Add(XboxButtons.LeftStick, string.Format(XID_BTN_LEFTSTICK));
                _btnToId.Add(XboxButtons.RightStick, string.Format(XID_BTN_RIGHTSTICK));
            }
            else
            {
                _axleToId = null;
                _btnToId = null;
            }
        }

        #endregion

        #region IInputSignature Interface

        public string Id
        {
            get { return _id; }
        }

        public int Hash
        {
            get { return _hash; }
        }

        public float Precedence
        {
            get
            {
                return _precedence;
            }
            set
            {
                _precedence = value;
            }
        }

        void IInputSignature.Update()
        {
            this.UpdateState(ref _current);
        }

        void IInputSignature.FixedUpdate()
        {
            this.UpdateState(ref _currentFixed);
        }

        private void UpdateState(ref XboxGamePadState state)
        {
            if (_gamepadIndex < 0)
            {
                state.LeftX = Input.GetAxis(ID_LEFT_X);
                state.LeftY = Input.GetAxis(ID_LEFT_Y);
                state.LeftTrigger = Input.GetAxis(ID_TRIGGER_LEFT);
                state.RightX = Input.GetAxis(ID_RIGHT_X);
                state.RightY = Input.GetAxis(ID_RIGHT_Y);
                state.RightTrigger = Input.GetAxis(ID_TRIGGER_RIGHT);

                state.DPadUp = InputUtil.GetNextButtonState(state.DPadUp, Input.GetButton(ID_DPAD_UP));
                state.DPadRight = InputUtil.GetNextButtonState(state.DPadRight, Input.GetButton(ID_DPAD_RIGHT));
                state.DPadDown = InputUtil.GetNextButtonState(state.DPadDown, Input.GetButton(ID_DPAD_DOWN));
                state.DPadLeft = InputUtil.GetNextButtonState(state.DPadLeft, Input.GetButton(ID_DPAD_LEFT));

                state.A = InputUtil.GetNextButtonState(state.A, Input.GetButton(ID_BTN_A));
                state.B = InputUtil.GetNextButtonState(state.B, Input.GetButton(ID_BTN_B));
                state.X = InputUtil.GetNextButtonState(state.X, Input.GetButton(ID_BTN_X));
                state.Y = InputUtil.GetNextButtonState(state.Y, Input.GetButton(ID_BTN_Y));
                state.LB = InputUtil.GetNextButtonState(state.LB, Input.GetButton(ID_BTN_LB));
                state.RB = InputUtil.GetNextButtonState(state.RB, Input.GetButton(ID_BTN_RB));
                state.Start = InputUtil.GetNextButtonState(state.Start, Input.GetButton(ID_BTN_START));
                state.Back = InputUtil.GetNextButtonState(state.Back, Input.GetButton(ID_BTN_BACK));
                state.LeftStickClick = InputUtil.GetNextButtonState(state.LeftStickClick, Input.GetButton(ID_BTN_LEFTSTICK));
                state.RightStickClick = InputUtil.GetNextButtonState(state.RightStickClick, Input.GetButton(ID_BTN_RIGHTSTICK));
            }
            else
            {
                state.LeftX = Input.GetAxis(_axleToId[XboxAxleInputs.LeftStickX]);
                state.LeftY = Input.GetAxis(_axleToId[XboxAxleInputs.LeftStickY]);
                state.LeftTrigger = Input.GetAxis(_axleToId[XboxAxleInputs.LeftTrigger]);
                state.RightX = Input.GetAxis(_axleToId[XboxAxleInputs.RightStickX]);
                state.RightY = Input.GetAxis(_axleToId[XboxAxleInputs.RightStickY]);
                state.RightTrigger = Input.GetAxis(_axleToId[XboxAxleInputs.RightTrigger]);

                state.DPadUp = InputUtil.GetNextButtonState(state.DPadUp, Input.GetButton(_btnToId[XboxButtons.DPadUp]));
                state.DPadRight = InputUtil.GetNextButtonState(state.DPadRight, Input.GetButton(_btnToId[XboxButtons.DPadRight]));
                state.DPadDown = InputUtil.GetNextButtonState(state.DPadDown, Input.GetButton(_btnToId[XboxButtons.DPadDown]));
                state.DPadLeft = InputUtil.GetNextButtonState(state.DPadLeft, Input.GetButton(_btnToId[XboxButtons.DPadLeft]));

                state.A = InputUtil.GetNextButtonState(state.A, Input.GetButton(_btnToId[XboxButtons.A]));
                state.B = InputUtil.GetNextButtonState(state.B, Input.GetButton(_btnToId[XboxButtons.B]));
                state.X = InputUtil.GetNextButtonState(state.X, Input.GetButton(_btnToId[XboxButtons.X]));
                state.Y = InputUtil.GetNextButtonState(state.Y, Input.GetButton(_btnToId[XboxButtons.Y]));
                state.LB = InputUtil.GetNextButtonState(state.LB, Input.GetButton(_btnToId[XboxButtons.LB]));
                state.RB = InputUtil.GetNextButtonState(state.RB, Input.GetButton(_btnToId[XboxButtons.RB]));
                state.Start = InputUtil.GetNextButtonState(state.Start, Input.GetButton(_btnToId[XboxButtons.Start]));
                state.Back = InputUtil.GetNextButtonState(state.Back, Input.GetButton(_btnToId[XboxButtons.Back]));
                state.LeftStickClick = InputUtil.GetNextButtonState(state.LeftStickClick, Input.GetButton(_btnToId[XboxButtons.LeftStick]));
                state.RightStickClick = InputUtil.GetNextButtonState(state.RightStickClick, Input.GetButton(_btnToId[XboxButtons.RightStick]));
            }
        }

        #endregion

        #region IPlayerInputDevice Interface

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        ButtonState IPlayerInputDevice.GetCurrentButtonState(string id)
        {
            try
            {
                var e = (XboxButtons)System.Enum.Parse(typeof(XboxButtons), id, true);
                return this.GetButtonState(e);
            }
            catch
            {
                return ButtonState.None;
            }
        }

        ButtonState IPlayerInputDevice.GetCurrentButtonState(int hash)
        {
            try
            {
                var e = (XboxButtons)hash;
                return this.GetButtonState(e);
            }
            catch
            {
                return ButtonState.None;
            }
        }

        float IPlayerInputDevice.GetCurrentAxleState(string id)
        {
            try
            {
                var e = (XboxAxleInputs)System.Enum.Parse(typeof(XboxAxleInputs), id, true);
                return this.GetAxleState(e);
            }
            catch
            {
                return 0f;
            }
        }

        float IPlayerInputDevice.GetCurrentAxleState(int hash)
        {
            try
            {
                var e = (XboxAxleInputs)hash;
                return this.GetAxleState(e);
            }
            catch
            {
                return 0f;
            }
        }

        Vector2 IPlayerInputDevice.GetCurrentDualAxleState(string id)
        {
            return Vector2.zero;
        }

        Vector2 IPlayerInputDevice.GetCurrentDualAxleState(int hash)
        {
            return Vector2.zero;
        }

        public Vector2 GetCurrentCursorState(string id)
        {
            return Vector2.zero;
        }

        public Vector2 GetCurrentCursorState(int hash)
        {
            return Vector2.zero;
        }

        #endregion

        #region IXboxGamePad Interface

        public ButtonState GetButtonState(XboxButtons btn)
        {
            if(GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
            {
                return _currentFixed.GetButtonState(btn);
            }
            else
            {
                return _current.GetButtonState(btn);
            }
        }

        public float GetAxleState(XboxAxleInputs axis)
        {
            if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
            {
                return _currentFixed.GetAxleState(axis);
            }
            else
            {
                return _current.GetAxleState(axis);
            }
        }

        public XboxGamePadState GetCurrentState()
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

        #endregion

        #region HashCodeOverride

        public override int GetHashCode()
        {
            return _hash;
        }

        #endregion






    }
}
