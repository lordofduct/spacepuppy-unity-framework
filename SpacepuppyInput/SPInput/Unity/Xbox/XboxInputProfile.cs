using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity.Xbox
{

    public abstract class XboxInputProfile : IXboxInputProfile
    {

        public const string GENERIC_XBOX360 = "Xbox 360 Controller";
        public const string GENERIC_XBOXONE = "Xbox One Controller";
        public const string GENERIC_PS4 = "PS4 Controller";

        #region Fields

        private Dictionary<XboxAxis, AxisDelegateFactory> _axisTable = new Dictionary<XboxAxis, AxisDelegateFactory>();
        private Dictionary<XboxButton, ButtonDelegateFactory> _buttonTable = new Dictionary<XboxButton, ButtonDelegateFactory>();
        
        #endregion

        #region Properties

        public AxisDelegateFactory this[XboxAxis axis]
        {
            get
            {
                return this.GetMapping(axis);
            }
        }

        public ButtonDelegateFactory this[XboxButton button]
        {
            get
            {
                return this.GetMapping(button);
            }
        }

        public Dictionary<XboxAxis, AxisDelegateFactory>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<XboxButton, ButtonDelegateFactory>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods

        public void Register(XboxAxis axis, AxisDelegateFactory del)
        {
            _axisTable[axis] = del;
        }

        public void Register(XboxAxis axis, SPInputId spaxis, bool invert = false)
        {
            _axisTable[axis] = SPInputFactory.CreateAxisDelegateFactory(spaxis, invert);
        }

        public void Register(XboxAxis axis, SPInputId positive, SPInputId negative)
        {
            _axisTable[axis] = SPInputFactory.CreateAxisDelegateFactory(positive, negative);
        }

        public void Register(XboxButton button, ButtonDelegateFactory del)
        {
            _buttonTable[button] = del;
        }

        public void Register(XboxButton button, SPInputId spbtn)
        {
            _buttonTable[button] = SPInputFactory.CreateButtonDelegateFactory(spbtn);
        }

        public void Register(XboxButton button, SPInputId axis, AxleValueConsideration consideration, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            _buttonTable[button] = SPInputFactory.CreateAxleButtonDelegateFactory(axis, consideration, axleButtonDeadZone);
        }



        public AxisDelegateFactory GetMapping(XboxAxis axis)
        {
            AxisDelegateFactory result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            throw new KeyNotFoundException("A mapping for axis " + axis.ToString() + " was not found.");
        }

        public ButtonDelegateFactory GetMapping(XboxButton button)
        {
            ButtonDelegateFactory result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            throw new KeyNotFoundException("A mapping for button " + button.ToString() + " was not found.");
        }

        public bool TryGetMapping(XboxAxis axis, out AxisDelegateFactory map)
        {
            return _axisTable.TryGetValue(axis, out map);
        }

        public bool TryGetMapping(XboxButton button, out ButtonDelegateFactory map)
        {
            return _buttonTable.TryGetValue(button, out map);
        }

        public bool Contains(XboxAxis axis)
        {
            return _axisTable.ContainsKey(axis);
        }

        public bool Contains(XboxButton button)
        {
            return _buttonTable.ContainsKey(button);
        }

        public bool Remove(XboxAxis axis)
        {
            return _axisTable.Remove(axis);
        }

        public bool Remove(XboxButton button)
        {
            return _buttonTable.Remove(button);
        }

        #endregion

        #region IInputSignatureFactory Interface

        public bool TryPollButton(out XboxButton button, Joystick joystick = Joystick.All)
        {
            var e = _buttonTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                var d = this.CreateButtonDelegate(e.Current, joystick);
                if(d != null && d())
                {
                    button = e.Current;
                    return true;
                }
            }

            button = default(XboxButton);
            return false;
        }

        public bool TryPollAxis(out XboxAxis axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            var e = _axisTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                var d = this.CreateAxisDelegate(e.Current, joystick);
                var v = d != null ? d() : 0f;
                if(d != null && Mathf.Abs(v) > deadZone)
                {
                    axis = e.Current;
                    value = v;
                    return true;
                }
            }

            axis = default(XboxAxis);
            value = 0f;
            return false;
        }

        public ButtonDelegate CreateButtonDelegate(XboxButton button, Joystick joystick = Joystick.All)
        {
            ButtonDelegateFactory map;
            if (!this.TryGetMapping(button, out map)) return null;

            return map != null ? map(joystick) : null;
        }
        
        public AxisDelegate CreateAxisDelegate(XboxAxis axis, Joystick joystick = Joystick.All)
        {
            AxisDelegateFactory map;
            if (!this.TryGetMapping(axis, out map)) return null;

            return map != null ? map(joystick) : null;
        }
        
        #endregion
        
    }

}
