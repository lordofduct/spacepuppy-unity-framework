using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity
{

    public class KeyboardProfile<TButton, TAxis> : IInputProfile<TButton, TAxis> where TButton : struct, System.IConvertible where TAxis : struct, System.IConvertible
    {

        #region Fields

        private Dictionary<TAxis, AxisMapping> _axisTable = new Dictionary<TAxis, AxisMapping>();
        private Dictionary<TButton, ButtonMapping> _buttonTable = new Dictionary<TButton, ButtonMapping>();

        #endregion

        #region Properties

        public AxisMapping this[TAxis axis]
        {
            get
            {
                return this.GetMapping(axis);
            }
        }

        public ButtonMapping this[TButton button]
        {
            get
            {
                return this.GetMapping(button);
            }
        }

        public Dictionary<TAxis, AxisMapping>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<TButton, ButtonMapping>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods
        
        public void Register(TAxis axis, KeyCode positive, KeyCode negative)
        {
            _axisTable[axis] = new AxisMapping()
            {
                Positive = positive,
                Negative = negative
            };
        }

        public void Register(TButton button, KeyCode key)
        {
            _buttonTable[button] = new ButtonMapping()
            {
                Key = key
            };
        }
        
        public AxisMapping GetMapping(TAxis axis)
        {
            AxisMapping result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            throw new KeyNotFoundException("A mapping for axis " + axis.ToString() + " was not found.");
        }

        public ButtonMapping GetMapping(TButton button)
        {
            ButtonMapping result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            throw new KeyNotFoundException("A mapping for button " + button.ToString() + " was not found.");
        }

        public bool TryGetMapping(TAxis axis, out AxisMapping map)
        {
            return _axisTable.TryGetValue(axis, out map);
        }

        public bool TryGetMapping(TButton button, out ButtonMapping map)
        {
            return _buttonTable.TryGetValue(button, out map);
        }

        public bool Contains(TAxis axis)
        {
            return _axisTable.ContainsKey(axis);
        }

        public bool Contains(TButton button)
        {
            return _buttonTable.ContainsKey(button);
        }

        public bool Remove(TAxis axis)
        {
            return _axisTable.Remove(axis);
        }

        public bool Remove(TButton button)
        {
            return _buttonTable.Remove(button);
        }

        #endregion

        #region IInputProfile Interface

        public bool TryPollButton(out TButton button, Joystick joystick = Joystick.All)
        {
            ButtonMapping map;

            var e = _buttonTable.Keys.GetEnumerator();
            while(e.MoveNext())
            {
                if(TryGetMapping(e.Current, out map))
                {
                    if(Input.GetKey(map.Key))
                    {
                        button = e.Current;
                        return true;
                    }
                }
            }

            button = default(TButton);
            return false;
        }

        public bool TryPollAxis(out TAxis axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            AxisMapping map;

            var e = _axisTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (TryGetMapping(e.Current, out map))
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

            axis = default(TAxis);
            value = 0f;
            return false;
        }

        public ButtonDelegate CreateButtonDelegate(TButton button, Joystick joystick = Joystick.All)
        {
            ButtonMapping map;
            if (!TryGetMapping(button, out map) || map.Key == KeyCode.None) return null;

            return SPInputFactory.CreateButtonDelegate(map.Key);
        }

        public AxisDelegate CreateAxisDelegate(TAxis axis, Joystick joystick = Joystick.All)
        {
            AxisMapping map;
            if (!this.TryGetMapping(axis, out map)) return null;

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
