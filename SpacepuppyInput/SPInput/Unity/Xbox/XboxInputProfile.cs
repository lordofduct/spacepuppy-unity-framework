using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace com.spacepuppy.SPInput.Unity.Xbox
{

    public abstract class XboxInputProfile : IXboxInputProfile
    {

        public const string GENERIC_XBOX360 = "Xbox 360 Controller";
        public const string GENERIC_XBOXONE = "Xbox One Controller";
        public const string GENERIC_PS4 = "PS4 Controller";

        #region Fields

        private Dictionary<XboxAxis, AxisMapping> _axisTable = new Dictionary<XboxAxis, AxisMapping>();
        private Dictionary<XboxButton, ButtonMapping> _buttonTable = new Dictionary<XboxButton, ButtonMapping>();

        #endregion

        #region Properties

        public AxisMapping this[XboxAxis axis]
        {
            get
            {
                return this.GetMapping(axis);
            }
        }

        public ButtonMapping this[XboxButton button]
        {
            get
            {
                return this.GetMapping(button);
            }
        }

        public Dictionary<XboxAxis, AxisMapping>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<XboxButton, ButtonMapping>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods

        public void Register(XboxAxis axis, SPInputAxis spaxis, bool invert = false)
        {
            _axisTable[axis] = new AxisMapping()
            {
                Emulated = false,
                InvertAxis = invert,
                Axis = spaxis,
                Positive = SPInputButton.Unknown,
                Negative = SPInputButton.Unknown
            };
        }

        public void Register(XboxAxis axis, SPInputButton positive, SPInputButton negative)
        {
            _axisTable[axis] = new AxisMapping()
            {
                Emulated = true,
                Axis = SPInputAxis.Unknown,
                Positive = positive,
                Negative = negative
            };
        }

        public void Register(XboxButton button, SPInputButton spbtn)
        {
            _buttonTable[button] = new ButtonMapping()
            {
                Emulated = false,
                Button = spbtn,
                Axis = SPInputAxis.Unknown
            };
        }

        public void Register(XboxButton button, SPInputAxis axis, AxleValueConsideration consideration)
        {
            _buttonTable[button] = new ButtonMapping()
            {
                Emulated = true,
                Button = SPInputButton.Unknown,
                Axis = axis,
                Consideration = consideration
            };
        }

        public AxisMapping GetMapping(XboxAxis axis)
        {
            AxisMapping result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            throw new KeyNotFoundException("A mapping for axis " + axis.ToString() + " was not found.");
        }

        public ButtonMapping GetMapping(XboxButton button)
        {
            ButtonMapping result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            throw new KeyNotFoundException("A mapping for button " + button.ToString() + " was not found.");
        }

        public bool TryGetMapping(XboxAxis axis, out AxisMapping map)
        {
            return _axisTable.TryGetValue(axis, out map);
        }

        public bool TryGetMapping(XboxButton button, out ButtonMapping map)
        {
            return _buttonTable.TryGetValue(button, out map);
        }

        #endregion

        #region IInputSignatureFactory Interface

        public IButtonInputSignature CreateButtonSignature(string id, XboxButton button, SPJoystick joystick = SPJoystick.All)
        {
            ButtonMapping map;
            if (!this.TryGetMapping(button, out map)) return null;

            if(map.Emulated)
            {
                if(map.Axis != SPInputAxis.Unknown)
                {
                    return SPInputFactory.CreateAxleButtonSignature(id, map.Axis, map.Consideration, joystick, AxleButtonInputSignature.DEFAULT_BTNDEADZONE);
                }
            }
            else
            {
                if(map.Button != SPInputButton.Unknown)
                {
                    return SPInputFactory.CreateButtonSignature(id, map.Button, joystick);
                }
            }

            return null;
        }

        public IButtonInputSignature CreateButtonSignature(string id, XboxAxis axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, SPJoystick joystick = SPJoystick.All, float axleButtonDeadZone = AxleButtonInputSignature.DEFAULT_BTNDEADZONE)
        {
            AxisMapping map;
            if (!this.TryGetMapping(axis, out map)) return null;

            if(map.Emulated)
            {
                switch(consideration)
                {
                    case AxleValueConsideration.Positive:
                        if (map.Positive != SPInputButton.Unknown) return SPInputFactory.CreateButtonSignature(id, map.Positive, joystick);
                        break;
                    case AxleValueConsideration.Negative:
                        if (map.Negative != SPInputButton.Unknown) return SPInputFactory.CreateButtonSignature(id, map.Negative, joystick);
                        break;
                    case AxleValueConsideration.Absolute:
                        {
                            var pos = (map.Positive != SPInputButton.Unknown) ? SPInputFactory.CreateButtonSignature(id, map.Positive, joystick) : null;
                            var neg = (map.Negative != SPInputButton.Unknown) ? SPInputFactory.CreateButtonSignature(id, map.Negative, joystick) : null;

                            if (pos != null)
                            {
                                if (neg != null)
                                    return new MergedButtonInputSignature(id, pos, neg);
                                else
                                    return pos;
                            }
                            else if (neg != null)
                                return neg;
                            
                        }
                        break;
                }
            }
            else
            {
                if (map.Axis != SPInputAxis.Unknown)
                {
                    if (map.InvertAxis)
                    {
                        switch (consideration)
                        {
                            case AxleValueConsideration.Positive:
                                consideration = AxleValueConsideration.Negative;
                                break;
                            case AxleValueConsideration.Negative:
                                consideration = AxleValueConsideration.Positive;
                                break;
                        }
                    }
                    return SPInputFactory.CreateAxleButtonSignature(id, map.Axis, consideration, joystick, axleButtonDeadZone);
                }
            }

            return null;
        }

        public IAxleInputSignature CreateAxisSignature(string id, XboxAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            AxisMapping map;
            if (!this.TryGetMapping(axis, out map)) return null;

            //CHANGE FROM && TO || - this is to support emulating half-functioning axes, like triggers.
            if (map.Emulated)
            {
                if (map.Positive != SPInputButton.Unknown || map.Negative != SPInputButton.Unknown)
                {
                    return SPInputFactory.CreateEmulatedAxixSignature(id, map.Positive, map.Negative, joystick);
                }
                return SPInputFactory.CreateEmulatedAxixSignature(id, map.Positive, map.Negative, joystick);
            }
            else
            {
                if(map.Axis != SPInputAxis.Unknown)
                {
                    return SPInputFactory.CreateAxisSignature(id, map.Axis, joystick, map.InvertAxis);
                }
            }

            return null;
        }
        
        public IDualAxleInputSignature CreateDualAxisSignature(string id, XboxAxis axisX, XboxAxis axisY, SPJoystick joystick = SPJoystick.All)
        {
            AxisMapping hmap;
            AxisMapping vmap;
            if (!this.TryGetMapping(axisX, out hmap)) return null;
            if (!this.TryGetMapping(axisY, out vmap)) return null;
            

            //CHANGE FROM && TO || - this is to support emulating half-functioning axes, like triggers.
            if(hmap.Emulated)
            {
                if(vmap.Emulated)
                {
                    if (hmap.Positive != SPInputButton.Unknown || hmap.Negative != SPInputButton.Unknown)
                    {
                        if (vmap.Positive != SPInputButton.Unknown || vmap.Negative != SPInputButton.Unknown)
                        {
                            return SPInputFactory.CreateEmulatedDualAxixSignature(id, hmap.Positive, hmap.Negative, vmap.Positive, vmap.Negative, joystick);
                        }
                        else
                        {
                            var hor = SPInputFactory.CreateEmulatedAxixSignature(id, hmap.Positive, hmap.Negative, joystick);
                            return new CompositeDualAxleInputSignature(id, hor, null);
                        }
                    }
                    else if(vmap.Positive != SPInputButton.Unknown || vmap.Negative != SPInputButton.Unknown)
                    {
                        var ver = SPInputFactory.CreateEmulatedAxixSignature(id, vmap.Positive, vmap.Negative, joystick);
                        return new CompositeDualAxleInputSignature(id, null, ver);
                    }
                }
                else
                {
                    IAxleInputSignature hor = null;
                    if (hmap.Positive != SPInputButton.Unknown || hmap.Negative != SPInputButton.Unknown)
                    {
                        hor = SPInputFactory.CreateEmulatedAxixSignature(id, hmap.Positive, hmap.Negative, joystick);
                    }

                    IAxleInputSignature ver = null;
                    if (vmap.Axis != SPInputAxis.Unknown)
                    {
                        ver = SPInputFactory.CreateAxisSignature(id, vmap.Axis, joystick, vmap.InvertAxis);
                    }

                    if(hor != null || ver != null)
                    {
                        return new CompositeDualAxleInputSignature(id, hor, ver);
                    }
                }
            }
            else
            {
                if(vmap.Emulated)
                {
                    IAxleInputSignature hor = null;
                    if (hmap.Axis != SPInputAxis.Unknown)
                    {
                        hor = SPInputFactory.CreateAxisSignature(id, hmap.Axis, joystick, hmap.InvertAxis);
                    }

                    IAxleInputSignature ver = null;
                    if (vmap.Positive != SPInputButton.Unknown || vmap.Negative != SPInputButton.Unknown)
                    {
                        ver = SPInputFactory.CreateEmulatedAxixSignature(id, vmap.Positive, vmap.Negative, joystick);
                    }

                    if (hor != null || ver != null)
                    {
                        return new CompositeDualAxleInputSignature(id, hor, ver);
                    }
                }
                else
                {
                    if (hmap.Axis != SPInputAxis.Unknown)
                    {
                        if (vmap.Axis != SPInputAxis.Unknown)
                        {
                            return SPInputFactory.CreateDualAxisSignature(id, hmap.Axis, vmap.Axis, joystick, hmap.InvertAxis, vmap.InvertAxis);
                        }
                        else
                        {
                            var hor = SPInputFactory.CreateAxisSignature(id, hmap.Axis, joystick, hmap.InvertAxis);
                            return new CompositeDualAxleInputSignature(id, hor, null);
                        }
                    }
                    else if(vmap.Axis != SPInputAxis.Unknown)
                    {
                        var ver = SPInputFactory.CreateAxisSignature(id, vmap.Axis, joystick, vmap.InvertAxis);
                        return new CompositeDualAxleInputSignature(id, null, ver);
                    }
                }
            }

            return null;
        }

        #endregion




        #region Special Types

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 0)]
        public struct AxisMapping
        {
            public bool Emulated;
            public bool InvertAxis;
            public SPInputAxis Axis;
            public SPInputButton Positive;
            public SPInputButton Negative;


            public static AxisMapping Unknown
            {
                get
                {
                    return new AxisMapping()
                    {
                        Axis = SPInputAxis.Unknown,
                        Positive = SPInputButton.Unknown,
                        Negative = SPInputButton.Unknown
                    };
                }
            }

        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 0)]
        public struct ButtonMapping
        {
            public bool Emulated;
            public SPInputButton Button;
            public SPInputAxis Axis;
            public AxleValueConsideration Consideration;

            public static ButtonMapping Unknown
            {
                get
                {
                    return new ButtonMapping()
                    {
                        Axis = SPInputAxis.Unknown,
                        Button = SPInputButton.Unknown
                    };
                }
            }
        }

        #endregion

    }

}
