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
        [System.NonSerialized]
        private System.Delegate _axisDelegate;
        [System.NonSerialized]
        private System.Delegate _buttonDelegate;

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
                        if(_axisDelegate is AxisDelegateFactory)
                        {
                            return (_axisDelegate as AxisDelegateFactory)(joystick);
                        }
                        else if (_axisDelegate is AxisDelegate)
                        {
                            return _axisDelegate as AxisDelegate;
                        }
                        else if(_buttonDelegate is ButtonDelegateFactory)
                        {
                            return SPInputFactory.CreateTriggerDelegate((_buttonDelegate as ButtonDelegateFactory)(joystick));
                        }
                        else if (_buttonDelegate is ButtonDelegate)
                        {
                            return SPInputFactory.CreateTriggerDelegate(_buttonDelegate as ButtonDelegate);
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
                            case InputMode.LongTrigger:
                            case InputMode.Button:
                            case InputMode.AxleButton:
                                return SPInputFactory.CreateButtonDelegate((KeyCode)this.Value);
                        }
                    }
                    break;
                case InputType.Custom:
                    {
                        if (_buttonDelegate is ButtonDelegateFactory)
                        {
                            return (_buttonDelegate as ButtonDelegateFactory)(joystick);
                        }
                        else if (_buttonDelegate is ButtonDelegate)
                        {
                            return _buttonDelegate as ButtonDelegate;
                        }
                        else if (_axisDelegate is AxisDelegateFactory)
                        {
                            return SPInputFactory.CreateAxleButtonDelegate((_axisDelegate as AxisDelegateFactory)(joystick), AxleValueConsideration.Absolute);
                        }
                        else if(_axisDelegate is AxisDelegate)
                        {
                            return SPInputFactory.CreateAxleButtonDelegate(_axisDelegate as AxisDelegate, AxleValueConsideration.Absolute);
                        }
                        else
                        {
                            return null;
                        }
                    }
            }
            return null;
        }
        
        public float PollAxis(Joystick joystick = Joystick.All)
        {
            switch (this.Type)
            {
                case InputType.Unknown:
                    return 0f;
                case InputType.Joystick:
                    {
                        switch (this.Mode)
                        {
                            case InputMode.Axis:
                                {
                                    if (float.IsNaN(this.DeadZone))
                                    {
                                        var d = SPInputFactory.CreateAxisDelegate((SPInputId)this.Value, (SPInputId)this.AltValue, joystick);
                                        return d != null ? d() : 0f;
                                    }
                                    else
                                    {
                                        //return SPInputFactory.CreateAxisDelegate((SPInputId)this.Value, joystick, this.AltValue != 0);
                                        float v = SPInputDirect.GetAxis((SPInputId)this.Value, joystick);
                                        if (this.AltValue != 0) v = -v;
                                        return v;
                                    }
                                }
                            case InputMode.Trigger:
                                {
                                    var d = SPInputFactory.CreateTriggerDelegate((SPInputId)this.Value, joystick, (AxleValueConsideration)this.AltValue);
                                    return d != null ? d() : 0f;
                                }
                            case InputMode.LongTrigger:
                                {
                                    var d = SPInputFactory.CreateLongTriggerDelegate((SPInputId)this.Value, joystick);
                                    return d != null ? d() : 0f;
                                }
                            case InputMode.Button:
                                //return SPInputFactory.CreateTriggerDelegate(SPInputFactory.CreateButtonDelegate((SPInputId)this.Value, joystick));
                                return SPInputDirect.GetButton((SPInputId)this.Value, joystick) ? 1f : 0f;
                            case InputMode.AxleButton:
                                {
                                    var d = SPInputFactory.CreateTriggerDelegate(SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, (AxleValueConsideration)this.AltValue, joystick, this.DeadZone));
                                    return d != null ? d() : 0f;
                                }

                        }
                    }
                    break;
                case InputType.Keyboard:
                    {
                        switch (this.Mode)
                        {
                            case InputMode.Axis:
                                {
                                    //return SPInputFactory.CreateAxisDelegate((KeyCode)this.Value, (KeyCode)this.AltValue);
                                    if (Input.GetKey((KeyCode)this.Value))
                                        return 1f;
                                    else if (Input.GetKey((KeyCode)this.AltValue))
                                        return -1f;
                                    else
                                        return 0f;
                                }
                            case InputMode.Trigger:
                            case InputMode.LongTrigger:
                            case InputMode.Button:
                            case InputMode.AxleButton:
                                //return SPInputFactory.CreateTriggerDelegate((KeyCode)this.Value);
                                return Input.GetKey((KeyCode)this.Value) ? 1f : 0f;
                        }
                    }
                    break;
                case InputType.Custom:
                    {
                        if (_axisDelegate is AxisDelegateFactory)
                        {
                            var d = (_axisDelegate as AxisDelegateFactory)(joystick);
                            return d != null ? d() : 0f;
                        }
                        else if (_axisDelegate is AxisDelegate)
                        {
                            var d = _axisDelegate as AxisDelegate;
                            return d != null ? d() : 0f;
                        }
                        else if (_buttonDelegate is ButtonDelegateFactory)
                        {
                            var d = SPInputFactory.CreateTriggerDelegate((_buttonDelegate as ButtonDelegateFactory)(joystick));
                            return d != null ? d() : 0f;
                        }
                        else if (_buttonDelegate is ButtonDelegate)
                        {
                            var d = SPInputFactory.CreateTriggerDelegate(_buttonDelegate as ButtonDelegate);
                            return d != null ? d() : 0f;
                        }
                        else
                        {
                            return 0f;
                        }
                    }
            }
            return 0f;
        }

        public bool PollButton(ButtonState state = ButtonState.Down, Joystick joystick = Joystick.All)
        {
            switch (this.Type)
            {
                case InputType.Unknown:
                    return false;
                case InputType.Joystick:
                    {
                        switch (this.Mode)
                        {
                            case InputMode.Axis:
                                {
                                    var d = SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, AxleValueConsideration.Absolute, joystick);
                                    return d != null ? d() : false;
                                }
                            case InputMode.Trigger:
                                {
                                    var d = SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, (AxleValueConsideration)this.AltValue, joystick);
                                    return d != null ? d() : false;
                                }
                            case InputMode.LongTrigger:
                                {
                                    var d = SPInputFactory.CreateAxleButtonDelegate(SPInputFactory.CreateLongTriggerDelegate((SPInputId)this.Value, joystick), AxleValueConsideration.Positive);
                                    return d != null ? d() : false;
                                }
                            case InputMode.Button:
                                return SPInputDirect.GetButton((SPInputId)this.Value, state, joystick);
                            case InputMode.AxleButton:
                                {
                                    var d = SPInputFactory.CreateAxleButtonDelegate((SPInputId)this.Value, (AxleValueConsideration)this.AltValue, joystick, this.DeadZone);
                                    return d != null ? d() : false;
                                }
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
                                    //return () => Input.GetKey(p) || Input.GetKey(n);
                                    return SPInputDirect.GetKey(p, state) || SPInputDirect.GetKey(n, state);
                                }
                            case InputMode.Trigger:
                            case InputMode.LongTrigger:
                            case InputMode.Button:
                            case InputMode.AxleButton:
                                return SPInputDirect.GetKey((KeyCode)this.Value, state);
                        }
                    }
                    break;
                case InputType.Custom:
                    {
                        if (_buttonDelegate is ButtonDelegateFactory)
                        {
                            var d = (_buttonDelegate as ButtonDelegateFactory)(joystick);
                            return d != null ? d() : false;
                        }
                        else if (_buttonDelegate is ButtonDelegate)
                        {
                            var d = _buttonDelegate as ButtonDelegate;
                            return d != null ? d() : false;
                        }
                        else if (_axisDelegate is AxisDelegateFactory)
                        {
                            var d = SPInputFactory.CreateAxleButtonDelegate((_axisDelegate as AxisDelegateFactory)(joystick), AxleValueConsideration.Absolute);
                            return d != null ? d() : false;
                        }
                        else if (_axisDelegate is AxisDelegate)
                        {
                            var d = SPInputFactory.CreateAxleButtonDelegate(_axisDelegate as AxisDelegate, AxleValueConsideration.Absolute);
                            return d != null ? d() : false;
                        }
                        else
                        {
                            return false;
                        }
                    }
            }
            return false;
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
                _axisDelegate = del
            };
        }

        public static InputToken CreateCustom(ButtonDelegateFactory del)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _buttonDelegate = del
            };
        }

        public static InputToken CreateCustom(AxisDelegate del)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _axisDelegate = del
            };
        }

        public static InputToken CreateCustom(ButtonDelegate del)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _buttonDelegate = del
            };
        }

        public static InputToken CreateCustom(AxisDelegateFactory axisDel, ButtonDelegateFactory buttonDel)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _axisDelegate = axisDel,
                _buttonDelegate = buttonDel
            };
        }
        
        public static InputToken CreateCustom(AxisDelegate axisDel, ButtonDelegate buttonDel)
        {
            return new InputToken()
            {
                Type = InputType.Custom,
                _axisDelegate = axisDel,
                _buttonDelegate = buttonDel
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
