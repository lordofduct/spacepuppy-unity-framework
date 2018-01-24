using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace com.spacepuppy.UserInput.SPInput.Xbox
{

    public abstract class XboxInputProfile
    {

        #region Fields

        private Dictionary<XboxAxis, SPInputAxis> _axisTable = new Dictionary<XboxAxis, SPInputAxis>();
        private Dictionary<XboxButton, SPInputButton> _buttonTable = new Dictionary<XboxButton, SPInputButton>();

        #endregion

        #region Properties

        public SPInputAxis this[XboxAxis axis]
        {
            get
            {
                return this.GetMapping(axis);
            }
        }

        public SPInputButton this[XboxButton button]
        {
            get
            {
                return this.GetMapping(button);
            }
        }

        public Dictionary<XboxAxis, SPInputAxis>.KeyCollection Axes
        {
            get { return _axisTable.Keys; }
        }

        public Dictionary<XboxButton, SPInputButton>.KeyCollection Buttons
        {
            get { return _buttonTable.Keys; }
        }

        #endregion

        #region Methods

        public void Register(XboxAxis axis, SPInputAxis map)
        {
            _axisTable[axis] = map;
        }

        public void Register(XboxButton button, SPInputButton map)
        {
            _buttonTable[button] = map;
        }

        public SPInputAxis GetMapping(XboxAxis axis)
        {
            SPInputAxis result;
            if (_axisTable.TryGetValue(axis, out result))
                return result;

            return SPInputAxis.Unknown;
        }

        public SPInputButton GetMapping(XboxButton button)
        {
            SPInputButton result;
            if (_buttonTable.TryGetValue(button, out result))
                return result;

            return SPInputButton.Unknown;
        }




        
        public IButtonInputSignature CreateButtonSignature(string id, XboxButton button, SPJoystick joystick = SPJoystick.All)
        {
            var spbtn = this.GetMapping(button);
            if (spbtn != SPInputButton.Unknown)
            {
                return SPInputFactory.CreateButtonSignature(id, spbtn, joystick);
            }

            switch (button)
            {
                case XboxButton.DPadUp:
                    {
                        var spaxis = this.GetMapping(XboxAxis.DPadY);
                        if (spaxis != SPInputAxis.Unknown)
                        {
                            return SPInputFactory.CreateAxleButtonSignature(id, spaxis, AxleButtonInputSignature.AxleValueConsideration.Positive, joystick);
                        }
                    }
                    break;
                case XboxButton.DPadDown:
                    {
                        var spaxis = this.GetMapping(XboxAxis.DPadY);
                        if (spaxis != SPInputAxis.Unknown)
                        {
                            return SPInputFactory.CreateAxleButtonSignature(id, spaxis, AxleButtonInputSignature.AxleValueConsideration.Negative, joystick);
                        }
                    }
                    break;
                case XboxButton.DPadRight:
                    {
                        var spaxis = this.GetMapping(XboxAxis.DPadX);
                        if (spaxis != SPInputAxis.Unknown)
                        {
                            return SPInputFactory.CreateAxleButtonSignature(id, spaxis, AxleButtonInputSignature.AxleValueConsideration.Positive, joystick);
                        }
                    }
                    break;
                case XboxButton.DPadLeft:
                    {
                        var spaxis = this.GetMapping(XboxAxis.DPadX);
                        if (spaxis != SPInputAxis.Unknown)
                        {
                            return SPInputFactory.CreateAxleButtonSignature(id, spaxis, AxleButtonInputSignature.AxleValueConsideration.Negative, joystick);
                        }
                    }
                    break;
            }

            return null;
        }

        //public IButtonInputSignature CreateButtonSignature(string id, XboxAxis axis, SPJoystick joystick = SPJoystick.All)
        //{

        //}

        #endregion

    }

}
