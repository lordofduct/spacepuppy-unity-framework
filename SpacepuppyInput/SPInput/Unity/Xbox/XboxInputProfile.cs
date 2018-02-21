using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity.Xbox
{

    public abstract class XboxInputProfile : IXboxInputProfile
    {

        public const string GENERIC_XBOX360 = "Xbox 360 Controller";
        public const string GENERIC_XBOXONE = "Xbox One Controller";
        public const string GENERIC_PS3 = "PS3 Controller";
        public const string GENERIC_PS4 = "PS4 Controller";

        #region Fields

        private Dictionary<XboxInputId, InputToken> _axisTable = new Dictionary<XboxInputId, InputToken>();
        private Dictionary<XboxInputId, InputToken> _buttonTable = new Dictionary<XboxInputId, InputToken>();

        #endregion

        #region Properties

        public Dictionary<XboxInputId, InputToken>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<XboxInputId, InputToken>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods

        public void RegisterAxis(XboxInputId axis, InputToken token)
        {
            _axisTable[axis] = token;
            _buttonTable.Remove(axis);
        }

        public void RegisterAxis(XboxInputId axis, AxisDelegateFactory del)
        {
            _axisTable[axis] = InputToken.CreateCustom(del);
            _buttonTable.Remove(axis);
        }

        public void RegisterAxis(XboxInputId axis, SPInputId spaxis, bool invert = false)
        {
            _axisTable[axis] = InputToken.CreateAxis(spaxis, invert);
            _buttonTable.Remove(axis);
        }

        public void RegisterAxis(XboxInputId axis, SPInputId positive, SPInputId negative)
        {
            _axisTable[axis] = InputToken.CreateEmulatedAxis(positive, negative);
            _buttonTable.Remove(axis);
        }

        public void RegisterButton(XboxInputId button, InputToken token)
        {
            _buttonTable[button] = token;
            _axisTable.Remove(button);
        }

        public void RegisterButton(XboxInputId button, ButtonDelegateFactory del)
        {
            _buttonTable[button] = InputToken.CreateCustom(del);
            _axisTable.Remove(button);
        }

        public void RegisterButton(XboxInputId button, SPInputId spbtn)
        {
            _buttonTable[button] = InputToken.CreateButton(spbtn);
            _axisTable.Remove(button);
        }

        public void RegisterAxleButton(XboxInputId button, SPInputId axis, AxleValueConsideration consideration, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            _buttonTable[button] = InputToken.CreateAxleButton(axis, consideration, axleButtonDeadZone);
            _axisTable.Remove(button);
        }

        public void RegisterTrigger(XboxInputId trigger, SPInputId axis, AxleValueConsideration consideration = AxleValueConsideration.Positive)
        {
            _axisTable[trigger] = InputToken.CreateTrigger(axis, consideration);
            _buttonTable.Remove(trigger);
        }

        public void RegisterTrigger(XboxInputId trigger, InputToken token)
        {
            _axisTable[trigger] = token;
            _buttonTable.Remove(trigger);
        }



        public InputToken GetAxisMapping(XboxInputId axis)
        {
            InputToken result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            return InputToken.Unknown;
        }

        public InputToken GetButtonMapping(XboxInputId button)
        {
            InputToken result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            return InputToken.Unknown;
        }

        public bool TryGetAxisMapping(XboxInputId axis, out InputToken map)
        {
            return _axisTable.TryGetValue(axis, out map);
        }

        public bool TryGetButtonMapping(XboxInputId button, out InputToken map)
        {
            return _buttonTable.TryGetValue(button, out map);
        }

        public bool Contains(XboxInputId id)
        {
            return _axisTable.ContainsKey(id) || _buttonTable.ContainsKey(id);
        }

        public bool Remove(XboxInputId id)
        {
            return _axisTable.Remove(id) | _buttonTable.Remove(id);
        }

        public void Clear()
        {
            _axisTable.Clear();
            _buttonTable.Clear();
        }

        #endregion

        #region IInputSignatureFactory Interface

        public bool TryPollButton(out XboxInputId button, ButtonState state = ButtonState.Down, Joystick joystick = Joystick.All)
        {
            InputToken map;

            var e = _buttonTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (TryGetButtonMapping(e.Current, out map))
                {
                    if (map.PollButton(state, joystick))
                    {
                        button = e.Current;
                        return true;
                    }
                }
            }

            button = default(XboxInputId);
            return false;
        }

        public bool TryPollAxis(out XboxInputId axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            InputToken map;

            var e = _axisTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (TryGetAxisMapping(e.Current, out map))
                {
                    float v = map.PollAxis(joystick);
                    if (Mathf.Abs(v) > deadZone)
                    {
                        axis = e.Current;
                        value = v;
                        return true;
                    }
                }
            }

            axis = default(XboxInputId);
            value = 0f;
            return false;
        }

        public InputToken GetMapping(XboxInputId id)
        {
            InputToken result;
            if (_axisTable.TryGetValue(id, out result)) return result;
            if (_buttonTable.TryGetValue(id, out result)) return result;

            return InputToken.Unknown;
        }

        void IConfigurableInputProfile<XboxInputId>.SetAxisMapping(XboxInputId id, InputToken token)
        {
            this.RegisterAxis(id, token);
        }

        void IConfigurableInputProfile<XboxInputId>.SetButtonMapping(XboxInputId id, InputToken token)
        {
            this.RegisterButton(id, token);
        }

        void IConfigurableInputProfile<XboxInputId>.Reset()
        {
            this.Clear();
        }

        #endregion

    }

}
