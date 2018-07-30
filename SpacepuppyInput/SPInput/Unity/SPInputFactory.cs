using System;
using System.Collections.Generic;
using System.Linq;
using com.spacepuppy.Collections;

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

        public static ButtonDelegate CreateButtonDelegate(SPInputId button, Joystick joystick = Joystick.All)
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

        public static ButtonDelegate CreateButtonDelegate(SPMouseId id)
        {
            return CreateButtonDelegate(id.ToSPInputId());
        }

        public static ButtonDelegate CreateAxleButtonDelegate(SPInputId axis, AxleValueConsideration consideration, Joystick joystick = Joystick.All, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
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

        public static AxisDelegate CreateAxisDelegate(SPInputId axis, Joystick joystick = Joystick.All, bool invert = false)
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

        public static AxisDelegate CreateAxisDelegate(SPInputId positive, SPInputId negative, Joystick joystick = Joystick.All)
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

        public static AxisDelegate CreateAxisDelegate(SPMouseId id, bool invert = false)
        {
            return CreateAxisDelegate(id.ToSPInputId(), Joystick.All, invert);
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
        
        public static AxisDelegate CreateTriggerDelegate(SPInputId axis, Joystick joystick = Joystick.All, AxleValueConsideration axisConsideration = AxleValueConsideration.Positive)
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
        public static AxisDelegate CreateLongTriggerDelegate(SPInputId axis, Joystick joystick = Joystick.All)
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

        public static ButtonDelegateFactory CreateButtonDelegateFactory(UnityEngine.KeyCode key)
        {
            return (j) =>
            {
                return CreateButtonDelegate(key);
            };
        }

        public static ButtonDelegateFactory CreateButtonDelegateFactory(SPMouseId id)
        {
            var spid = id.ToSPInputId();
            return (j) =>
            {
                return CreateButtonDelegate(spid, j);
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

        public static AxisDelegateFactory CreateAxisDelegateFactory(SPMouseId id, bool invert = false)
        {
            return CreateAxisDelegateFactory(id.ToSPInputId(), invert);
        }



        public static AxisDelegateFactory CreateTriggerDelegateFactory(SPInputId axis, AxleValueConsideration axisConsideration = AxleValueConsideration.Positive)
        {
            return (j) => CreateTriggerDelegate(axis, j, axisConsideration);
        }

        public static AxisDelegateFactory CreateTriggerDelegateFactory(UnityEngine.KeyCode key)
        {
            return (j) => CreateTriggerDelegate(key);
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


        
        /*
         * Merge
         */

        public static AxisDelegate Merge(this AxisDelegate d1, AxisDelegate d2)
        {
            if (d1 == null) return d2;
            if (d2 == null) return d1;
            return () => UnityEngine.Mathf.Clamp(d1() + d2(), -1f, 1f);
        }

        public static AxisDelegate Merge(params AxisDelegate[] arr)
        {
            if (arr == null || arr.Length == 0) return null;
            if (arr.Length == 1) return arr[0];
            if (arr.Length == 2) return arr[0].Merge(arr[1]);

            return () =>
            {
                float v = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] != null) v += arr[i]();
                }
                return UnityEngine.Mathf.Clamp(v, -1f, 1f);
            };
        }

        public static ButtonDelegate Merge(this ButtonDelegate d1, ButtonDelegate d2)
        {
            if (d1 == null) return d2;
            if (d2 == null) return d1;
            return () => d1() || d2();
        }

        public static ButtonDelegate Merge(params ButtonDelegate[] arr)
        {
            if (arr == null || arr.Length == 0) return null;
            if (arr.Length == 1) return arr[0];
            if (arr.Length == 2) return arr[0].Merge(arr[1]);

            return () =>
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] != null && arr[i]()) return true;
                }
                return false;
            };
        }

        public static InputToken MergeAsAxis(this InputToken t1, InputToken t2)
        {
            return InputToken.CreateCustom((j) => t1.CreateAxisDelegate(j).Merge(t2.CreateAxisDelegate(j)));
        }

        public static InputToken MergeAsButton(this InputToken t1, InputToken t2)
        {
            return InputToken.CreateCustom((j) => t1.CreateButtonDelegate(j).Merge(t2.CreateButtonDelegate(j)));
        }

        #endregion

        #region Signature Factory Extension Methods

        //extension

        public static ButtonDelegate CreateButtonDelegate<TInputId>(this IInputProfile<TInputId> profile, TInputId button, Joystick joystick = Joystick.All)
            where TInputId : struct, System.IConvertible
        {
            return profile.GetMapping(button).CreateButtonDelegate(joystick);
        }

        public static AxisDelegate CreateAxisDelegate<TInputId>(this IInputProfile<TInputId> profile, TInputId axis, Joystick joystick = Joystick.All)
            where TInputId : struct, System.IConvertible
        {
            return profile.GetMapping(axis).CreateAxisDelegate(joystick);
        }

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


        //group

        public static AxisDelegate CreateAxisDelegate<TInputId>(this IEnumerable<IInputProfile<TInputId>> profiles, TInputId axis, Joystick joystick = Joystick.All, Comparison<IInputProfile<TInputId>> comparison = null)
            where TInputId : struct, System.IConvertible
        {
            using (var lst = TempCollection.GetList<IInputProfile<TInputId>>(from p in profiles where p != null select p))
            {
                if (comparison != null) lst.Sort(comparison);

                if (lst.Count == 0)
                    return null;
                if (lst.Count == 1)
                    return lst[0].GetMapping(axis).CreateAxisDelegate(joystick);
                if (lst.Count == 2)
                {
                    return lst[0].GetMapping(axis).CreateAxisDelegate(joystick).Merge(
                           lst[1].GetMapping(axis).CreateAxisDelegate(joystick));
                }

                var arr = (from p in lst select p.GetMapping(axis).CreateAxisDelegate(joystick)).ToArray();
                return Merge(arr);
            }
        }

        public static ButtonDelegate CreateButtonDelegate<TInputId>(this IEnumerable<IInputProfile<TInputId>> profiles, TInputId button, Joystick joystick = Joystick.All, Comparison<IInputProfile<TInputId>> comparison = null)
            where TInputId : struct, System.IConvertible
        {
            using (var lst = TempCollection.GetList<IInputProfile<TInputId>>(from p in profiles where p != null select p))
            {
                if (comparison != null) lst.Sort(comparison);

                if (lst.Count == 0)
                    return null;
                if (lst.Count == 1)
                    return lst[0].GetMapping(button).CreateButtonDelegate(joystick);
                if (lst.Count == 2)
                {
                    return lst[0].GetMapping(button).CreateButtonDelegate(joystick).Merge(
                           lst[1].GetMapping(button).CreateButtonDelegate(joystick));
                }

                var arr = (from p in lst select p.GetMapping(button).CreateButtonDelegate(joystick)).ToArray();
                return Merge(arr);
            }
        }

        public static ButtonDelegate CreateAxleButtonDelegate<TInputId>(this IEnumerable<IInputProfile<TInputId>> profiles, 
                                                                        TInputId axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, 
                                                                        Joystick joystick = Joystick.All, Comparison<IInputProfile<TInputId>> comparison = null,
                                                                        float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
            where TInputId : struct, System.IConvertible
        {
            using (var lst = TempCollection.GetList<IInputProfile<TInputId>>(from p in profiles where p != null select p))
            {
                if (comparison != null) lst.Sort(comparison);

                if (lst.Count == 0)
                    return null;
                if (lst.Count == 1)
                    return CreateAxleButtonDelegate(lst[0].GetMapping(axis).CreateAxisDelegate(joystick), consideration, axleButtonDeadZone);
                if (lst.Count == 2)
                {
                    return CreateAxleButtonDelegate(lst[0].GetMapping(axis).CreateAxisDelegate(joystick), consideration, axleButtonDeadZone).Merge(
                           CreateAxleButtonDelegate(lst[1].GetMapping(axis).CreateAxisDelegate(joystick), consideration, axleButtonDeadZone));
                }

                var arr = (from p in lst select CreateAxleButtonDelegate(p.GetMapping(axis).CreateAxisDelegate(joystick), consideration, axleButtonDeadZone)).ToArray();
                return Merge(arr);
            }
        }
        
        public static IButtonInputSignature CreateButtonSignature<TInputId>(this IEnumerable<IInputProfile<TInputId>> profiles, string id, TInputId button, Joystick joystick = Joystick.All, Comparison<IInputProfile<TInputId>> comparison = null)
            where TInputId : struct, System.IConvertible
        {
            return new DelegatedButtonInputSignature(id, CreateButtonDelegate(profiles, button, joystick, comparison));
        }

        public static IButtonInputSignature CreateAxleButtonSignature<TInputId>(this IEnumerable<IInputProfile<TInputId>> profiles, string id, 
                                                                                TInputId axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, 
                                                                                Joystick joystick = Joystick.All, Comparison<IInputProfile<TInputId>> comparison = null, 
                                                                                float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
            where TInputId : struct, System.IConvertible
        {
            return new DelegatedAxleButtonInputSignature(id, CreateAxisDelegate(profiles, axis, joystick, comparison), consideration, axleButtonDeadZone);
        }

        public static IAxleInputSignature CreateAxisSignature<TInputId>(this IEnumerable<IInputProfile<TInputId>> profiles, string id, TInputId axis, Joystick joystick = Joystick.All, Comparison<IInputProfile<TInputId>> comparison = null)
            where TInputId : struct, System.IConvertible
        {
            return new DelegatedAxleInputSignature(id, CreateAxisDelegate(profiles, axis, joystick, comparison));
        }

        public static IDualAxleInputSignature CreateDualAxisSignature<TInputId>(this IEnumerable<IInputProfile<TInputId>> profiles, string id, TInputId axisX, TInputId axisY, Joystick joystick = Joystick.All, Comparison<IInputProfile<TInputId>> comparison = null)
            where TInputId : struct, System.IConvertible
        {
            return new DelegatedDualAxleInputSignature(id, CreateAxisDelegate(profiles, axisX, joystick, comparison), CreateAxisDelegate(profiles, axisY, joystick, comparison));
        }

        #endregion

    }

}
