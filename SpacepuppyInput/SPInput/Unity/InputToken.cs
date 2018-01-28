using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity
{

    [System.Serializable]
    public struct InputToken
    {

        public InputType Type;
        public InputMode Mode;
        public int Value;
        public int AltValue;
        public float DeadZone;
        private System.Delegate _customDelegate;

        public AxisDelegate CreateAxisDelegate(Joystick joystick = Joystick.All)
        {
            switch(this.Type)
            {
                case InputType.Unknown:
                    return null;
                case InputType.Joystick:
                    {
                        switch(this.Mode)
                        {
                            case InputMode.Axis:
                                {
                                    if (float.IsNaN(this.DeadZone))
                                        return SPInputFactory.CreateAxisDelegate((SPInputId)this.Value, (SPInputId)this.AltValue, joystick);
                                    else
                                        return SPInputFactory.CreateAxisDelegate((SPInputId)this.Value, joystick, this.AltValue != 0);
                                }
                            case InputMode.Trigger:
                                return SPInputFactory.CreateTriggerDelegate((SPInputId)this.Value, joystick, (AxleValueConsideration)this.AltValue);
                            case InputMode.LongTrigger:
                                return SPInputFactory.CreateLongTriggerDelegate((SPInputId)this.Value, joystick);
                            case InputMode.Button:
                                return SPInputFactory.CreateTriggerDelegate(SPInputFactory.CreateButtonDelegate((SPInputId)this.Value, joystick));
                            case InputMode.AxleButton:
                                return SPInputFactory.CreateTriggerDelegate(SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, (AxleValueConsideration)this.AltValue, joystick, this.DeadZone));

                        }
                    }
                    break;
                case InputType.Keyboard:
                    {
                        switch (this.Mode)
                        {
                            case InputMode.Axis:
                                return SPInputFactory.CreateAxisDelegate((KeyCode)this.Value, (KeyCode)this.AltValue);
                            case InputMode.Trigger:
                            case InputMode.LongTrigger:
                            case InputMode.Button:
                            case InputMode.AxleButton:
                                return SPInputFactory.CreateTriggerDelegate((KeyCode)this.Value);
                        }
                    }
                    break;
                case InputType.Custom:
                    {
                        if(_customDelegate is AxisDelegateFactory)
                        {
                            return (_customDelegate as AxisDelegateFactory)(joystick);
                        }
                        else if(_customDelegate is ButtonDelegateFactory)
                        {
                            return SPInputFactory.CreateTriggerDelegate((_customDelegate as ButtonDelegateFactory)(joystick));
                        }
                        else if (_customDelegate is AxisDelegate)
                        {
                            return _customDelegate as AxisDelegate;
                        }
                        else if (_customDelegate is ButtonDelegate)
                        {
                            return SPInputFactory.CreateTriggerDelegate(_customDelegate as ButtonDelegate);
                        }
                        else
                        {
                            return null;
                        }
                    }
            }
            return null;
        }

        public ButtonDelegate CreateButtonDelegate(Joystick joystick = Joystick.All)
        {
            switch (this.Type)
            {
                case InputType.Unknown:
                    return null;
                case InputType.Joystick:
                    {
                        switch (this.Mode)
                        {
                            case InputMode.Axis:
                                return SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, AxleValueConsideration.Absolute, joystick);
                            case InputMode.Trigger:
                                return SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, (AxleValueConsideration)this.AltValue, joystick);
                            case InputMode.LongTrigger:
                                return SPInputFactory.CreateAxleButtonDelegate(SPInputFactory.CreateLongTriggerDelegate((SPInputId)this.Value, joystick), AxleValueConsideration.Positive);
                            case InputMode.Button:
                                return SPInputFactory.CreateButtonDelegate((SPInputId)this.Value, joystick);
                            case InputMode.AxleButton:
                                return SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, (AxleValueConsideration)this.AltValue, joystick, this.DeadZone);
                        }
                    }
                    break;
                case InputType.Keyboard:
                    {
                        switch (this.Mode)
                        {
                            case InputMode.Axis:
                                {
                                    KeyCode p = (KeyCode)Value;
                                    KeyCode n = (KeyCode)AltValue;
                                    return () => Input.GetKey(p) || Input.GetKey(n);
                                }
                            case InputMode.Trigger:
                                return SPInputFactory.CreateButtonDelegate((KeyCode)this.Value);
                            case InputMode.LongTrigger:
                                return SPInputFactory.CreateAxleButtonDelegate(SPInputFactory.CreateLongTriggerDelegate((SPInputId)this.Value, joystick), AxleValueConsideration.Positive);
                            case InputMode.Button:
                            case InputMode.AxleButton:
                                return SPInputFactory.CreateButtonDelegate((KeyCode)this.Value);
                        }
                    }
                    break;
                case InputType.Custom:
                    {
                        if (_customDelegate is AxisDelegateFactory)
                        {
                            return SPInputFactory.CreateAxleButtonDelegate((_customDelegate as AxisDelegateFactory)(joystick), AxleValueConsideration.Absolute);
                        }
                        else if (_customDelegate is ButtonDelegateFactory)
                        {
                            return (_customDelegate as ButtonDelegateFactory)(joystick);
                        }
                        else if(_customDelegate is AxisDelegate)
                        {
                            return SPInputFactory.CreateAxleButtonDelegate(_customDelegate as AxisDelegate, AxleValueConsideration.Absolute);
                        }
                        else if(_customDelegate is ButtonDelegate)
                        {
                            return _customDelegate as ButtonDelegate;
                        }
                        else
                        {
                            return null;
                        }
                    }
            }
            return null;
        }
        


        public static InputToken CreateButton(SPInputId button)
        {
            return new InputToken()
            {
                Type = InputType.Joystick,
                Mode = InputMode.Button,
                Value = (int)button
            };
        }

        public static InputToken CreateButton(KeyCode key)
        {
            return new InputToken()
            {
                Type = InputType.Keyboard,
                Mode = InputMode.Button,
                Value = (int)key
            };
        }

        public static InputToken CreateAxleButton(SPInputId axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            return new InputToken()
            {
                Type = InputType.Joystick,
                Mode = InputMode.AxleButton,
                Value = (int)axis,
                AltValue = (int)consideration,
                DeadZone = deadZone
            };
        }

        public static InputToken CreateTrigger(SPInputId axis, AxleValueConsideration consideration = AxleValueConsideration.Positive)
        {
            return new InputToken()
            {
                Type = InputType.Joystick,
                Mode = InputMode.Trigger,
                Value = (int)axis,
                AltValue = (int)consideration
            };
        }

        public static InputToken CreateTrigger(KeyCode key)
        {
            return new InputToken()
            {
                Type = InputType.Keyboard,
                Mode = InputMode.Trigger,
                Value = (int)key
            };
        }

        public static InputToken CreateLongTrigger(SPInputId axis)
        {
            return new InputToken()
            {
                Type = InputType.Joystick,
                Mode = InputMode.LongTrigger,
                Value = (int)axis
            };
        }

        public static InputToken CreateAxis(SPInputId axis, bool invert = false)
        {
            return new InputToken()
            {
                Type = InputType.Joystick,
                Mode = InputMode.Axis,
                Value = (int)axis,
                AltValue = invert ? 1 : 0
            };
        }

        public static InputToken CreateEmulatedAxis(KeyCode positive, KeyCode negative)
        {
            return new InputToken()
            {
                Type = InputType.Keyboard,
                Mode = InputMode.Axis,
                Value = (int)positive,
                AltValue = (int)negative
            };
        }

        public static InputToken CreateEmulatedAxis(SPInputId positiveButton, SPInputId negativeButton)
        {
            return new InputToken()
            {
                Type = InputType.Joystick,
                Mode = InputMode.Axis,
                Value = (int)positiveButton,
                AltValue = (int)negativeButton
            };
        }

        public static InputToken CreateCustom(AxisDelegateFactory del)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _customDelegate = del
            };
        }

        public static InputToken CreateCustom(ButtonDelegateFactory del)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _customDelegate = del
            };
        }

        public static InputToken CreateCustom(AxisDelegate del)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _customDelegate = del
            };
        }

        public static InputToken CreateCustom(ButtonDelegate del)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _customDelegate = del
            };
        }

        public static InputToken Unknown
        {
            get
            {
                return new InputToken()
                {
                    Type = InputType.Unknown
                };
            }
        }

    }

}
