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

        public IButtonInputSignature CreateButtonSignature(string id, TButton button, SPJoystick joystick = SPJoystick.All)
        {
            ButtonMapping map;
            if (!TryGetMapping(button, out map)) return null;

            return SPInputFactory.CreateKeyCodeButtonSignature(id, map.Key);
        }

        public IButtonInputSignature CreateButtonSignature(string id, TAxis axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, SPJoystick joystick = SPJoystick.All, float axleButtonDeadZone = 0.707F)
        {
            AxisMapping map;
            if (!TryGetMapping(axis, out map)) return null;

            switch(consideration)
            {
                case AxleValueConsideration.Positive:
                    return SPInputFactory.CreateKeyCodeButtonSignature(id, map.Positive);
                case AxleValueConsideration.Negative:
                    return SPInputFactory.CreateKeyCodeButtonSignature(id, map.Negative);
                case AxleValueConsideration.Absolute:
                    {
                        IButtonInputSignature pos = null;
                        IButtonInputSignature neg = null;
                        if(map.Positive != KeyCode.None) pos = SPInputFactory.CreateKeyCodeButtonSignature(id, map.Positive);
                        if(map.Negative != KeyCode.None) neg = SPInputFactory.CreateKeyCodeButtonSignature(id, map.Negative);

                        if (pos != null && neg != null)
                            return new MergedButtonInputSignature(id, pos, neg);
                        else if (pos != null)
                            return pos;
                        else if (neg != null)
                            return neg;
                        else
                            return SPInputFactory.CreateKeyCodeButtonSignature(id, KeyCode.None);
                    }
                default:
                    return null;
            }
        }

        public IAxleInputSignature CreateAxisSignature(string id, TAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            AxisMapping map;
            if (!TryGetMapping(axis, out map)) return null;

            return SPInputFactory.CreateKeyCodeAxisSignature(id, map.Positive, map.Negative);
        }

        public IDualAxleInputSignature CreateDualAxisSignature(string id, TAxis axisX, TAxis axisY, SPJoystick joystick = SPJoystick.All)
        {
            AxisMapping mapX;
            if (!TryGetMapping(axisX, out mapX)) mapX = AxisMapping.Unknown;
            AxisMapping mapY;
            if (!TryGetMapping(axisY, out mapY)) mapY = AxisMapping.Unknown;

            return SPInputFactory.CreateKeyCodeDualAxisSignature(id, mapX.Positive, mapX.Negative, mapY.Positive, mapY.Negative);
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
