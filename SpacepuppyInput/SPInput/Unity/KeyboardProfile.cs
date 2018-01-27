using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity
{

    public class KeyboardProfile<TInputId> : IInputProfile<TInputId> where TInputId : struct, System.IConvertible
    {

        #region Fields

        private Dictionary<TInputId, AxisMapping> _axisTable = new Dictionary<TInputId, AxisMapping>();
        private Dictionary<TInputId, ButtonMapping> _buttonTable = new Dictionary<TInputId, ButtonMapping>();

        #endregion

        #region Properties
        
        public Dictionary<TInputId, AxisMapping>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<TInputId, ButtonMapping>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods
        
        public void RegisterAxis(TInputId axis, KeyCode positive, KeyCode negative)
        {
            _axisTable[axis] = new AxisMapping()
            {
                Positive = positive,
                Negative = negative
            };
        }

        public void RegisterButton(TInputId button, KeyCode key)
        {
            _buttonTable[button] = new ButtonMapping()
            {
                Key = key
            };
        }
        
        public AxisMapping GetAxisMapping(TInputId axis)
        {
            AxisMapping result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            throw new KeyNotFoundException("A mapping for axis " + axis.ToString() + " was not found.");
        }

        public ButtonMapping GetButtonMapping(TInputId button)
        {
            ButtonMapping result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            throw new KeyNotFoundException("A mapping for button " + button.ToString() + " was not found.");
        }

        public bool TryGetAxisMapping(TInputId axis, out AxisMapping map)
        {
            return _axisTable.TryGetValue(axis, out map);
        }

        public bool TryGetButtonMapping(TInputId button, out ButtonMapping map)
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
        
        #endregion

        #region IInputProfile Interface

        public bool TryPollButton(out TInputId button, Joystick joystick = Joystick.All)
        {
            ButtonMapping map;

            var e = _buttonTable.Keys.GetEnumerator();
            while(e.MoveNext())
            {
                if(TryGetButtonMapping(e.Current, out map))
                {
                    if(Input.GetKey(map.Key))
                    {
                        button = e.Current;
                        return true;
                    }
                }
            }

            button = default(TInputId);
            return false;
        }

        public bool TryPollAxis(out TInputId axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            AxisMapping map;

            var e = _axisTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (TryGetAxisMapping(e.Current, out map))
                {
                    if (Input.GetKey(map.Positive))
                    {
                        axis = e.Current;
                        value = 1f;
                        return true;
                    }
                    else if(Input.GetKey(map.Negative))
                    {
                        axis = e.Current;
                        value = -1f;
                        return true;
                    }
                }
            }

            axis = default(TInputId);
            value = 0f;
            return false;
        }

        public ButtonDelegate CreateButtonDelegate(TInputId button, Joystick joystick = Joystick.All)
        {
            ButtonMapping map;
            if (!TryGetButtonMapping(button, out map) || map.Key == KeyCode.None) return null;

            return SPInputFactory.CreateButtonDelegate(map.Key);
        }

        public AxisDelegate CreateAxisDelegate(TInputId axis, Joystick joystick = Joystick.All)
        {
            AxisMapping map;
            if (!this.TryGetAxisMapping(axis, out map)) return null;

            return SPInputFactory.CreateAxisDelegate(map.Positive, map.Negative);
        }
        
        #endregion

        #region Special Types

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 0)]
        public struct AxisMapping
        {
            public KeyCode Positive;
            public KeyCode Negative;


            public static AxisMapping Unknown
            {
                get
                {
                    return new AxisMapping()
                    {
                        Positive = KeyCode.None,
                        Negative = KeyCode.None
                    };
                }
            }

        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 0)]
        public struct ButtonMapping
        {
            public KeyCode Key;

            public static ButtonMapping Unknown
            {
                get
                {
                    return new ButtonMapping()
                    {

                    };
                }
            }
        }

        #endregion

    }

}
