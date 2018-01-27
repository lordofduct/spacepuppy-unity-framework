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

        private Dictionary<XboxInputId, AxisDelegateFactory> _axisTable = new Dictionary<XboxInputId, AxisDelegateFactory>();
        private Dictionary<XboxInputId, ButtonDelegateFactory> _buttonTable = new Dictionary<XboxInputId, ButtonDelegateFactory>();
        
        #endregion

        #region Properties
        
        public Dictionary<XboxInputId, AxisDelegateFactory>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<XboxInputId, ButtonDelegateFactory>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods

        public void RegisterAxis(XboxInputId axis, AxisDelegateFactory del)
        {
            _axisTable[axis] = del;
        }

        public void RegisterAxis(XboxInputId axis, SPInputId spaxis, bool invert = false)
        {
            _axisTable[axis] = SPInputFactory.CreateAxisDelegateFactory(spaxis, invert);
        }

        public void RegisterAxis(XboxInputId axis, SPInputId positive, SPInputId negative)
        {
            _axisTable[axis] = SPInputFactory.CreateAxisDelegateFactory(positive, negative);
        }

        public void RegisterButton(XboxInputId button, ButtonDelegateFactory del)
        {
            _buttonTable[button] = del;
        }

        public void RegisterButton(XboxInputId button, SPInputId spbtn)
        {
            _buttonTable[button] = SPInputFactory.CreateButtonDelegateFactory(spbtn);
        }

        public void RegisterAxleButton(XboxInputId button, SPInputId axis, AxleValueConsideration consideration, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            _buttonTable[button] = SPInputFactory.CreateAxleButtonDelegateFactory(axis, consideration, axleButtonDeadZone);
        }



        public AxisDelegateFactory GetAxisMapping(XboxInputId axis)
        {
            AxisDelegateFactory result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            throw new KeyNotFoundException("A mapping for axis " + axis.ToString() + " was not found.");
        }

        public ButtonDelegateFactory GetButtonMapping(XboxInputId button)
        {
            ButtonDelegateFactory result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            throw new KeyNotFoundException("A mapping for button " + button.ToString() + " was not found.");
        }

        public bool TryGetAxisMapping(XboxInputId axis, out AxisDelegateFactory map)
        {
            return _axisTable.TryGetValue(axis, out map);
        }

        public bool TryGetButtonMapping(XboxInputId button, out ButtonDelegateFactory map)
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
        
        #endregion

        #region IInputSignatureFactory Interface

        public bool TryPollButton(out XboxInputId button, Joystick joystick = Joystick.All)
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

            button = default(XboxInputId);
            return false;
        }

        public bool TryPollAxis(out XboxInputId axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
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

            axis = default(XboxInputId);
            value = 0f;
            return false;
        }

        public ButtonDelegate CreateButtonDelegate(XboxInputId button, Joystick joystick = Joystick.All)
        {
            ButtonDelegateFactory map;
            if (!this.TryGetButtonMapping(button, out map)) return null;

            return map != null ? map(joystick) : null;
        }
        
        public AxisDelegate CreateAxisDelegate(XboxInputId axis, Joystick joystick = Joystick.All)
        {
            AxisDelegateFactory map;
            if (!this.TryGetAxisMapping(axis, out map)) return null;

            return map != null ? map(joystick) : null;
        }
        
        #endregion
        
    }

}
