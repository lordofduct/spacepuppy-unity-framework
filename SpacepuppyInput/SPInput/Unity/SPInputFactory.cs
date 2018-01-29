using System;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput.Unity
{

    public delegate ButtonDelegate ButtonDelegateFactory(Joystick joystick);
    public delegate AxisDelegate AxisDelegateFactory(Joystick joystick);

    public static class SPInputFactory
    {

        #region Delegate Factory

        /*
         * Buttons
         */

        public static ButtonDelegate CreateButtonDelegate(SPInputId button, Joystick joystick)
        {
            var inputId = SPInputDirect.GetInputName(button, joystick);
            if (button.IsButton())
                return () => UnityEngine.Input.GetButton(inputId);
            else
                return () => UnityEngine.Input.GetAxisRaw(inputId) > InputUtil.DEFAULT_AXLEBTNDEADZONE;
        }

        public static ButtonDelegate CreateButtonDelegate(UnityEngine.KeyCode key)
        {
            return () => UnityEngine.Input.GetKey(key);
        }

        public static ButtonDelegate CreateAxleButtonDelegate(SPInputId axis, AxleValueConsideration consideration, Joystick joystick, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            var inputId = SPInputDirect.GetInputName(axis, joystick);
            if(axis.IsAxis())
            {
                switch (consideration)
                {
                    case AxleValueConsideration.Positive:
                        return () => UnityEngine.Input.GetAxisRaw(inputId) > axleButtonDeadZone;
                    case AxleValueConsideration.Negative:
                        return () => UnityEngine.Input.GetAxisRaw(inputId) < -axleButtonDeadZone;
                    case AxleValueConsideration.Absolute:
                        return () => Math.Abs(UnityEngine.Input.GetAxisRaw(inputId)) > axleButtonDeadZone;
                    default:
                        return null;
                }
            }
            else
                return () => UnityEngine.Input.GetButton(inputId);
        }

        public static ButtonDelegate CreateAxleButtonDelegate(AxisDelegate axis, AxleValueConsideration consideration, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (axis == null) return null;

            switch (consideration)
            {
                case AxleValueConsideration.Positive:
                    return () => axis() > axleButtonDeadZone;
                case AxleValueConsideration.Negative:
                    return () => axis() < -axleButtonDeadZone;
                case AxleValueConsideration.Absolute:
                    return () => Math.Abs(axis()) > axleButtonDeadZone;
                default:
                    return null;
            }
        }

        /*
         * Axes
         */

        public static AxisDelegate CreateAxisDelegate(SPInputId axis, Joystick joystick, bool invert = false)
        {
            var inputId = SPInputDirect.GetInputName(axis, joystick);
            if(axis.IsAxis())
            {
                if (invert)
                    return () => -UnityEngine.Input.GetAxisRaw(inputId);
                else
                    return () => UnityEngine.Input.GetAxisRaw(inputId);
            }
            else
            {
                if (invert)
                    return () => UnityEngine.Input.GetButton(inputId) ? -1f : 0f;
                else
                    return () => UnityEngine.Input.GetButton(inputId) ? 1f : 0f;
            }
        }

        public static AxisDelegate CreateAxisDelegate(SPInputId positive, SPInputId negative, Joystick joystick)
        {
            return CreateAxisDelegate(CreateButtonDelegate(positive, joystick), CreateButtonDelegate(negative, joystick));
        }

        public static AxisDelegate CreateAxisDelegate(UnityEngine.KeyCode positive, UnityEngine.KeyCode negative)
        {
            if (positive != UnityEngine.KeyCode.None && negative != UnityEngine.KeyCode.None)
            {
                return () =>
                {
                    if (UnityEngine.Input.GetKey(positive))
                        return (UnityEngine.Input.GetKey(negative)) ? 0f : 1f;
                    else if (UnityEngine.Input.GetKey(negative))
                        return -1f;
                    else
                        return 0f;
                };
            }
            else if (positive != UnityEngine.KeyCode.None)
            {
                return () => UnityEngine.Input.GetKey(positive) ? 1f : 0f;
            }
            else if (negative != UnityEngine.KeyCode.None)
            {
                return () => UnityEngine.Input.GetKey(negative) ? -1f : 0f;
            }
            else
                return null;
        }

        public static AxisDelegate CreateAxisDelegate(ButtonDelegate positive, ButtonDelegate negative)
        {
            if (positive != null && negative != null)
            {
                return () =>
                {
                    if (positive())
                        return negative() ? 0f : 1f;
                    else if (negative())
                        return -1f;
                    else
                        return 0f;
                };
            }
            else if (positive != null)
            {
                return () => positive() ? 1f : 0f;
            }
            else if (negative != null)
            {
                return () => positive() ? -1f : 0f;
            }
            else
                return null;
        }

        /*
         * Triggers
         */
        
        public static AxisDelegate CreateTriggerDelegate(SPInputId axis, Joystick joystick, AxleValueConsideration axisConsideration = AxleValueConsideration.Positive)
        {
            var inputId = SPInputDirect.GetInputName(axis, joystick);
            if(axis.IsAxis())
            {
                switch (axisConsideration)
                {
                    case AxleValueConsideration.Positive:
                        return () => UnityEngine.Mathf.Clamp01(UnityEngine.Input.GetAxisRaw(inputId));
                    case AxleValueConsideration.Negative:
                        return () => -UnityEngine.Mathf.Clamp(UnityEngine.Input.GetAxisRaw(inputId), -1f, 0f);
                    case AxleValueConsideration.Absolute:
                        return () => Math.Abs(UnityEngine.Input.GetAxisRaw(inputId));
                    default:
                        return null;
                }
            }
            else
            {
                return () => UnityEngine.Input.GetButton(inputId) ? 1f : 0f;
            }
        }

        public static AxisDelegate CreateTriggerDelegate(UnityEngine.KeyCode key)
        {
            return () => UnityEngine.Input.GetKey(key) ? 1f : 0f;
        }

        public static AxisDelegate CreateTriggerDelegate(ButtonDelegate del)
        {
            if (del == null) return null;
            return () => del() ? 1f : 0f;
        }

        public static AxisDelegate CreateTriggerDelegate(AxisDelegate del, AxleValueConsideration axisConsideration = AxleValueConsideration.Positive)
        {
            if (del == null) return null;
            switch (axisConsideration)
            {
                case AxleValueConsideration.Positive:
                    return () => UnityEngine.Mathf.Clamp01(del());
                case AxleValueConsideration.Negative:
                    return () => -UnityEngine.Mathf.Clamp(del(), -1f, 0f);
                case AxleValueConsideration.Absolute:
                    return () => Math.Abs(del());
                default:
                    return null;
            }
        }

        /*
         * Long Trigers
         */

        /// <summary>
        /// Some controller's triggers register -1 as inactive, and 1 as active. This creates a TriggerDelegate that normalizes this value to 0->1.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="joystick"></param>
        /// <returns></returns>
        public static AxisDelegate CreateLongTriggerDelegate(SPInputId axis, Joystick joystick)
        {
            var inputId = SPInputDirect.GetInputName(axis, joystick);
            if (axis.IsAxis())
            {
                return () => (UnityEngine.Input.GetAxisRaw(inputId) + 1f) / 2f;
            }
            else
            {
                return () => UnityEngine.Input.GetButton(inputId) ? 1f : 0f;
            }
        }
        
        /*
         * Delegate factories
         */

        public static ButtonDelegateFactory CreateButtonDelegateFactory(SPInputId button)
        {
            return (j) =>
            {
                return CreateButtonDelegate(button, j);
            };
        }

        public static ButtonDelegateFactory CreateAxleButtonDelegateFactory(SPInputId axis, AxleValueConsideration consideration, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            return (j) =>
            {
                return CreateAxleButtonDelegate(axis, consideration, j, axleButtonDeadZone);
            };
        }

        public static AxisDelegateFactory CreateAxisDelegateFactory(SPInputId axis, bool invert = false)
        {
            return (j) =>
            {
                return CreateAxisDelegate(axis, j, invert);
            };
        }

        public static AxisDelegateFactory CreateAxisDelegateFactory(SPInputId positive, SPInputId negative)
        {
            return (j) =>
            {
                return CreateAxisDelegate(positive, negative, j);
            };
        }

        public static AxisDelegateFactory CreateAxisDelegateFactory(UnityEngine.KeyCode positive, UnityEngine.KeyCode negative)
        {
            return (j) =>
            {
                return CreateAxisDelegate(positive, negative);
            };
        }

        /// <summary>
        /// The PS4 Controller L2/R2 triggers on some platforms registers -1 when depressed, and 1 when pressed. This creates a factory that normalizes those values to 0->1
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static AxisDelegateFactory CreateLongTriggerDelegateFactory(SPInputId axis)
        {
            return (j) => CreateLongTriggerDelegate(axis, j);
        }

        #endregion

        #region Signature Factory Extension Methods

        //extension

        public static IButtonInputSignature CreateButtonSignature<TInputId>(this IInputProfile<TInputId> profile, string id, TInputId button, Joystick joystick = Joystick.All)
            where TInputId : struct, System.IConvertible
        {
            return new DelegatedButtonInputSignature(id, profile.GetMapping(button).CreateButtonDelegate(joystick));
        }

        public static IButtonInputSignature CreateAxleButtonSignature<TInputId>(this IInputProfile<TInputId> profile, string id, TInputId axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, Joystick joystick = Joystick.All, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
            where TInputId : struct, System.IConvertible
        {
            if (profile == null) return null;

            return new DelegatedAxleButtonInputSignature(id, profile.GetMapping(axis).CreateAxisDelegate(joystick), consideration, axleButtonDeadZone);
        }

        public static IAxleInputSignature CreateAxisSignature<TInputId>(this IInputProfile<TInputId> profile, string id, TInputId axis, Joystick joystick = Joystick.All)
            where TInputId : struct, System.IConvertible
        {
            return new DelegatedAxleInputSignature(id, profile.GetMapping(axis).CreateAxisDelegate(joystick));
        }

        public static IDualAxleInputSignature CreateDualAxisSignature<TInputId>(this IInputProfile<TInputId> profile, string id, TInputId axisX, TInputId axisY, Joystick joystick = Joystick.All)
            where TInputId : struct, System.IConvertible
        {
            return new DelegatedDualAxleInputSignature(id, profile.GetMapping(axisX).CreateAxisDelegate(joystick), profile.GetMapping(axisY).CreateAxisDelegate(joystick));
        }

        #endregion




        #region Signature Factory

        /*

        public static ButtonInputSignature CreateButtonSignature(string id, SPInputId button, Joystick joystick = Joystick.All)
        {
            return new ButtonInputSignature(id, SPInputDirect.GetInputName(button, joystick));
        }
        
        public static AxleButtonInputSignature CreateAxleButtonSignature(string id, SPInputId axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, Joystick joystick = Joystick.All, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            return new AxleButtonInputSignature(id, SPInputDirect.GetInputName(axis, joystick), consideration, axleButtonDeadZone);
        }
        
        public static KeyboardButtonInputSignature CreateKeyCodeButtonSignature(string id, UnityEngine.KeyCode key)
        {
            return new KeyboardButtonInputSignature(id, key);
        }
        
        public static AxleInputSignature CreateAxisSignature(string id, SPInputId axis, Joystick joystick = Joystick.All, bool invert = false)
        {
            return new AxleInputSignature(id, SPInputDirect.GetInputName(axis, joystick))
            {
                Invert = invert
            };
        }
        
        public static KeyboardAxleInputSignature CreateKeyCodeAxisSignature(string id, UnityEngine.KeyCode positiveKey, UnityEngine.KeyCode negativeKey)
        {
            return new KeyboardAxleInputSignature(id, positiveKey, negativeKey);
        }
        
        public static DualAxleInputSignature CreateDualAxisSignature(string id, SPInputId axisX, SPInputId axisY, Joystick joystick = Joystick.All, bool invertX = false, bool invertY = false)
        {
            return new DualAxleInputSignature(id, SPInputDirect.GetInputName(axisX, joystick), SPInputDirect.GetInputName(axisY, joystick))
            {
                InvertX = invertX,
                InvertY = invertY
            };
        }
        
        public static KeyboardDualAxleInputSignature CreateKeyCodeDualAxisSignature(string id, UnityEngine.KeyCode horizontalPositiveKey, UnityEngine.KeyCode horizontalNegativeKey, UnityEngine.KeyCode verticalPositiveKey, UnityEngine.KeyCode verticalNegativeKey)
        {
            return new KeyboardDualAxleInputSignature(id, horizontalPositiveKey, horizontalNegativeKey, verticalPositiveKey, verticalNegativeKey);
        }
        
        public static EmulatedAxleInputSignature CreateEmulatedAxixSignature(string id, SPInputId positive, SPInputId negative, Joystick joystick = Joystick.All)
        {
            return new EmulatedAxleInputSignature(id, SPInputDirect.GetInputName(positive, joystick), SPInputDirect.GetInputName(negative, joystick));
        }

        public static EmulatedDualAxleInputSignature CreateEmulatedDualAxixSignature(string id, SPInputId positiveX, SPInputId negativeX, SPInputId positiveY, SPInputId negativeY, Joystick joystick = Joystick.All)
        {
            return new EmulatedDualAxleInputSignature(id, SPInputDirect.GetInputName(positiveX, joystick), SPInputDirect.GetInputName(negativeX, joystick), SPInputDirect.GetInputName(positiveY, joystick), SPInputDirect.GetInputName(negativeY, joystick));
        }

        */

        #endregion

        #region Polling Factory

        /*
        
        /// <summary>
        /// Polls the input system for a button press. If one is pressed it creates a ButtonDelegate for it.
        /// </summary>
        /// <returns></returns>
        public static bool TryPollCreateButtonDelegate(out ButtonDelegate signature, Joystick joystick = Joystick.All, bool allowKeyboard = false, bool allowAxis = false, bool allowMouseMotionAxis = false, float axleInputDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                SPInputId btn;
                if (SPInputDirect.TryPollButton(out btn, joystick))
                {
                    signature = SPInputFactory.CreateButtonDelegate(btn, joystick);
                    return true;
                }
            }

            if (allowKeyboard)
            {
                UnityEngine.KeyCode key;
                if (SPInputDirect.TryPollKey(out key))
                {
                    signature = SPInputFactory.CreateButtonDelegate(key);
                    return true;
                }
            }

            if (allowAxis || allowMouseMotionAxis)
            {
                SPInputId axis;
                if (SPInputDirect.TryPollAxis(out axis, joystick, allowMouseMotionAxis, axleInputDeadZone))
                {
                    if ((allowMouseMotionAxis && axis.IsMouseAxis()) || (allowAxis && axis.IsJoyAxis()))
                    {
                        float value = SPInputDirect.GetAxis(axis, joystick);
                        if (value > 1f)
                        {
                            signature = SPInputFactory.CreateAxleButtonDelegate(axis, AxleValueConsideration.Positive, joystick, axleInputDeadZone);
                            return true;
                        }
                        else if (value < 0f)
                        {
                            signature = SPInputFactory.CreateAxleButtonDelegate(axis, AxleValueConsideration.Negative, joystick, axleInputDeadZone);
                            return true;
                        }
                    }
                }
            }

            signature = null;
            return false;
        }

        /// <summary>
        /// Polls the input system for a button press. If one is pressed it creates an AxisDelegate for it.
        /// </summary>
        /// <returns></returns>
        public static bool TryPollCreateAxisDelegate(out AxisDelegate signature, Joystick joystick = Joystick.All, bool allowMouseMotionAxis = false, float axleInputDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                SPInputId axis;
                if (SPInputDirect.TryPollAxis(out axis, joystick, allowMouseMotionAxis, axleInputDeadZone))
                {
                    if (allowMouseMotionAxis || axis.IsJoyAxis())
                    {
                        signature = SPInputFactory.CreateAxisDelegate(axis, joystick);
                        return true;
                    }
                }
            }

            signature = null;
            return false;
        }

        /// <summary>
        /// Polls the input system for a button press. If one is pressed it creates an IButtonInputSignature for it.
        /// </summary>
        /// <returns></returns>
        public static bool TryPollCreateButtonSignature(string id, out IButtonInputSignature signature, Joystick joystick = Joystick.All, bool allowKeyboard = false, bool allowAxis = false, bool allowMouseMotionAxis = false, float axleInputDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                SPInputId btn;
                if (SPInputDirect.TryPollButton(out btn, joystick))
                {
                    signature = SPInputFactory.CreateButtonSignature(id, btn, joystick);
                    return true;
                }
            }

            if(allowKeyboard)
            {
                UnityEngine.KeyCode key;
                if(SPInputDirect.TryPollKey(out key))
                {
                    signature = SPInputFactory.CreateKeyCodeButtonSignature(id, key);
                    return true;
                }
            }

            if(allowAxis || allowMouseMotionAxis)
            {
                SPInputId axis;
                if(SPInputDirect.TryPollAxis(out axis, joystick, allowMouseMotionAxis, axleInputDeadZone))
                {
                    if ((allowMouseMotionAxis && axis.IsMouseAxis()) || (allowAxis && axis.IsJoyAxis()))
                    {
                        float value = SPInputDirect.GetAxis(axis, joystick);
                        if (value > 1f)
                        {
                            signature = SPInputFactory.CreateAxleButtonSignature(id, axis, AxleValueConsideration.Positive, joystick, axleInputDeadZone);
                            return true;
                        }
                        else if(value < 0f)
                        {
                            signature = SPInputFactory.CreateAxleButtonSignature(id, axis, AxleValueConsideration.Negative, joystick, axleInputDeadZone);
                            return true;
                        }
                    }
                }
            }
            
            signature = null;
            return false;
        }

        /// <summary>
        /// Polls the input system for a button press. If one is pressed it creates an IAxisInputSignature for it.
        /// </summary>
        /// <returns></returns>
        public static bool TryPollCreateAxisSignature(string id, out IAxleInputSignature signature, Joystick joystick = Joystick.All, bool allowMouseMotionAxis = false, float axleInputDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                SPInputId axis;
                if (SPInputDirect.TryPollAxis(out axis, joystick, allowMouseMotionAxis, axleInputDeadZone))
                {
                    if (allowMouseMotionAxis || axis.IsJoyAxis())
                    {
                        signature = SPInputFactory.CreateAxisSignature(id, axis, joystick);
                        return true;
                    }
                }
            }
           
            signature = null;
            return false;
        }
        
        */

        #endregion

    }

}
