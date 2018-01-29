using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity
{

    public class KeyboardProfile<TInputId> : IConfigurableInputProfile<TInputId> where TInputId : struct, System.IConvertible
    {

        #region Fields

        private Dictionary<TInputId, InputToken> _axisTable = new Dictionary<TInputId, InputToken>();
        private Dictionary<TInputId, InputToken> _buttonTable = new Dictionary<TInputId, InputToken>();

        #endregion

        #region Properties

        public Dictionary<TInputId, InputToken>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<TInputId, InputToken>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods

        public void RegisterAxis(TInputId axis, KeyCode positive, KeyCode negative)
        {
            _axisTable[axis] = InputToken.CreateEmulatedAxis(positive, negative);
            _buttonTable.Remove(axis);
        }

        public void RegisterMouseAxis(TInputId axis, SPMouseId spaxis)
        {
            _axisTable[axis] = InputToken.CreateAxis(spaxis.ToSPInputId());
            _buttonTable.Remove(axis);
        }

        public void RegisterAxis(TInputId axis, InputToken token)
        {
            _axisTable[axis] = token;
            _buttonTable.Remove(axis);
        }

        public void RegisterButton(TInputId button, KeyCode key)
        {
            _buttonTable[button] = InputToken.CreateButton(key);
            _axisTable.Remove(button);
        }

        public void RegisterMouseButton(TInputId button, SPMouseId spbtn)
        {
            _buttonTable[button] = InputToken.CreateButton(spbtn.ToSPInputId());
            _axisTable.Remove(button);
        }

        public void RegisterButton(TInputId button, InputToken token)
        {
            _buttonTable[button] = token;
            _axisTable.Remove(button);
        }

        public InputToken GetAxisMapping(TInputId axis)
        {
            InputToken result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            return InputToken.Unknown;
        }

        public InputToken GetButtonMapping(TInputId button)
        {
            InputToken result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            return InputToken.Unknown;
        }

        public bool TryGetAxisMapping(TInputId axis, out InputToken map)
        {
            return _axisTable.TryGetValue(axis, out map);
        }

        public bool TryGetButtonMapping(TInputId button, out InputToken map)
        {
            return _buttonTable.TryGetValue(button, out map);
        }

        public bool Contains(TInputId id)
        {
            return _axisTable.ContainsKey(id) || _buttonTable.ContainsKey(id);
        }

        public bool Remove(TInputId id)
        {
            return _axisTable.Remove(id) | _buttonTable.Remove(id);
        }

        public void Clear()
        {
            _axisTable.Clear();
            _buttonTable.Clear();
        }

        #endregion

        #region IInputProfile Interface

        public bool TryPollButton(out TInputId button, Joystick joystick = Joystick.All)
        {
            InputToken map;

            var e = _buttonTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (TryGetButtonMapping(e.Current, out map))
                {
                    if (map.Type == InputType.Keyboard)
                    {
                        if (Input.GetKey((KeyCode)map.Value))
                        {
                            button = e.Current;
                            return true;
                        }
                    }
                    else
                    {
                        var d = map.CreateButtonDelegate(joystick);
                        if (d != null && d())
                        {
                            button = e.Current;
                            return true;
                        }
                    }
                }
            }

            button = default(TInputId);
            return false;
        }

        public bool TryPollAxis(out TInputId axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            InputToken map;

            var e = _axisTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (TryGetAxisMapping(e.Current, out map))
                {
                    if (map.Type == InputType.Keyboard)
                    {
                        if (Input.GetKey((KeyCode)map.Value))
                        {
                            axis = e.Current;
                            value = 1f;
                            return true;
                        }
                        else if (Input.GetKey((KeyCode)map.AltValue))
                        {
                            axis = e.Current;
                            value = -1f;
                            return true;
                        }
                    }
                    else
                    {
                        var d = map.CreateAxisDelegate(joystick);
                        var v = d != null ? d() : 0f;
                        if (d != null && Mathf.Abs(v) > deadZone)
                        {
                            axis = e.Current;
                            value = v;
                            return true;
                        }
                    }
                }
            }

            axis = default(TInputId);
            value = 0f;
            return false;
        }

        public InputToken GetMapping(TInputId id)
        {
            InputToken result;
            if (_axisTable.TryGetValue(id, out result)) return result;
            if (_buttonTable.TryGetValue(id, out result)) return result;

            return InputToken.Unknown;
        }

        void IConfigurableInputProfile<TInputId>.SetAxisMapping(TInputId id, InputToken token)
        {
            this.RegisterAxis(id, token);
        }

        void IConfigurableInputProfile<TInputId>.SetButtonMapping(TInputId id, InputToken token)
        {
            this.RegisterButton(id, token);
        }

        void IConfigurableInputProfile<TInputId>.Reset()
        {
            this.Clear();
        }

        #endregion

    }

}
