using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.SPInput.Unity
{

    /// <summary>
    /// Use this to poll for an InputSignature defined by the user. You start by creating this object, starting it and waiting for it to complete. 
    /// It can also be used as a yield instruction to wait for its completion.<para/>
    /// </summary>
    /// <typeparam name="TButton"></typeparam>
    /// <typeparam name="TAxis"></typeparam>
    public class PollButtonSignatureRoutine : IRadicalWaitHandle
    {

        public delegate bool PollingCallback(PollButtonSignatureRoutine targ, out ButtonDelegate del);

        private enum State
        {
            Unknown,
            Running,
            Cancelled,
            Complete
        }

        #region Fields

        private State _state;
        private RadicalCoroutine _routine;

        #endregion

        #region CONSTRUCTOR

        public PollButtonSignatureRoutine()
        {
            this.PollButtons = true;
            this.PollKeyboard = true;
            this.PollFromStandardSPInputs = true;
            this.Joystick = Joystick.All;
            this.AxisConsideration = AxleValueConsideration.Positive;
            this.AxisPollingDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE;
            this.AllowMouseAsAxis = false;
            this.CancelKey = UnityEngine.KeyCode.Escape;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// The resulting ButtonDelegate that can be used to poll for the input.
        /// </summary>
        public ButtonDelegate DelegateResult
        {
            get;
            set;
        }
        
        /// <summary>
        /// Joystick # to poll for. <param/>
        /// Default: <see cref="Joystick.All"/>
        /// </summary>
        public Joystick Joystick
        {
            get;
            set;
        }

        /// <summary>
        /// Should we poll all the standard inputs defined by SPInputDirect. 
        /// If no profiles are defined and this is false, than no gamepads will be polled for. <para/>
        /// Default: True
        /// </summary>
        public bool PollFromStandardSPInputs
        {
            get;
            set;
        }

        /// <summary>
        /// Should we poll button presses. <para/>
        /// Default: True
        /// </summary>
        public bool PollButtons
        {
            get;
            set;
        }

        /// <summary>
        /// Should we poll key presses. <param/>
        /// Default: True
        /// </summary>
        public bool PollKeyboard
        {
            get;
            set;
        }

        /// <summary>
        /// Should we poll for axis tilts. <param/>
        /// Default: False
        /// </summary>
        public bool PollAxes
        {
            get;
            set;
        }

        /// <summary>
        /// When polling an axis what directions should be considered. <param/>
        /// Default: <see cref="AxleValueConsideration.Positive"/>
        /// </summary>
        public AxleValueConsideration AxisConsideration
        {
            get;
            set;
        }

        /// <summary>
        /// When polling an axis what is considered the 'not pressed' dead zone. <param/>
        /// Default: <see cref="InputUtil.DEFAULT_AXLEBTNDEADZONE"/>
        /// </summary>
        public float AxisPollingDeadZone
        {
            get;
            set;
        }

        /// <summary>
        /// Allow the mouse to register as an axis when pulling Standard SPInputs (does not work for profiles). <param/>
        /// Default: False
        /// </summary>
        public bool AllowMouseAsAxis
        {
            get;
            set;
        }

        /// <summary>
        /// A key the user can press to cancel out of the polling. <param/>
        /// Default: <see cref="UnityEngine.KeyCode.Escape"/>
        /// </summary>
        public UnityEngine.KeyCode CancelKey
        {
            get;
            set;
        }

        /// <summary>
        /// A custom button press that can be used to register a cancel instead of the CancelKey. 
        /// Useful for allowing a gamepad button press to cancel with.<param/>
        /// Default: Null
        /// </summary>
        public ButtonDelegate CancelDelegate
        {
            get;
            set;
        }

        /// <summary>
        /// Set this to allow to poll for some custom inputs to register buttons.
        /// </summary>
        public PollingCallback CustomPollingCallback
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void Start()
        {
            if (_routine != null)
            {
                if (_routine.Finished)
                {
                    _routine = null;
                    _state = State.Unknown;
                }
                else
                {
                    //already running
                    return;
                }
            }

            _state = State.Running;
            this.DelegateResult = null;
            _routine = GameLoopEntry.Hook.StartRadicalCoroutine(this.WorkRoutine());
        }

        public void Cancel()
        {
            if (_routine != null)
            {
                _routine.Cancel();
                _routine = null;
            }

            this.DelegateResult = null;
            _state = State.Cancelled;
        }

        public IInputSignature CreateInputSignature(string id)
        {
            return new DelegatedButtonInputSignature(id, this.DelegateResult);
        }

        private System.Collections.IEnumerator WorkRoutine()
        {
            while (_state == State.Running)
            {
                if (UnityEngine.Input.GetKeyDown(this.CancelKey))
                {
                    this.Cancel();
                    yield break;
                }
                if (this.CancelDelegate != null && this.CancelDelegate())
                {
                    this.Cancel();
                    yield break;
                }
                
                if(this.CustomPollingCallback != null)
                {
                    ButtonDelegate d;
                    if(this.CustomPollingCallback(this, out d))
                    {
                        this.DelegateResult = d;
                        goto Complete;
                    }
                }

                if (this.PollFromStandardSPInputs)
                {
                    if (this.PollButtons)
                    {
                        SPInputButton btn;
                        if(SPInputDirect.TryPollButton(out btn, this.Joystick))
                        {
                            this.DelegateResult = SPInputFactory.CreateButtonDelegate(btn, this.Joystick);
                            goto Complete;
                        }
                    }

                    if(this.PollAxes)
                    {
                        SPInputAxis axis;
                        float value;
                        if(SPInputDirect.TryPollAxis(out axis, out value, this.Joystick, this.AxisPollingDeadZone) && TestConsideration(value, this.AxisConsideration, this.AxisPollingDeadZone))
                        {
                            if(this.AllowMouseAsAxis || axis < SPInputAxis.MouseAxis1)
                            {
                                this.DelegateResult = SPInputFactory.CreateButtonDelegate(axis, this.AxisConsideration, this.Joystick, this.AxisPollingDeadZone);
                                goto Complete;
                            }
                        }
                    }
                }

                if (this.PollKeyboard)
                {
                    UnityEngine.KeyCode key;
                    if (SPInputDirect.TryPollKey(out key))
                    {
                        this.DelegateResult = SPInputFactory.CreateButtonDelegate(key);
                        goto Complete;
                    }
                }

                yield return null;
            }

            Complete:
            _state = State.Complete;
            _routine = null;
        }
        
        #endregion

        #region IRadicalWaitHandle Interface

        public bool Cancelled
        {
            get { return _state == State.Cancelled; }
        }

        public bool IsComplete
        {
            get { return _state >= State.Cancelled; }
        }

        void IRadicalWaitHandle.OnComplete(Action<IRadicalWaitHandle> callback)
        {

        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if (_state == State.Running)
            {
                yieldObject = _routine;
                return true;
            }
            else
            {
                yieldObject = null;
                return false;
            }
        }

        #endregion

        #region Static Utils
        
        private static bool TestConsideration(float value, AxleValueConsideration consideration, float deadZone)
        {
            switch(consideration)
            {
                case AxleValueConsideration.Positive:
                    return value > deadZone;
                case AxleValueConsideration.Negative:
                    return value < -deadZone;
                case AxleValueConsideration.Absolute:
                    return Math.Abs(value) > deadZone;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Create a custom polling callback that polls an array of IInputProfiles. 
        /// This can be useful for input devices that act strange, yet the profile fixes that behaviour.
        /// For example the PS4 controller's L2/R2 buttons are weird and return -1 when depressed, but its profile fixes it.
        /// </summary>
        /// <typeparam name="TButton"></typeparam>
        /// <typeparam name="TAxis"></typeparam>
        /// <param name="profiles"></param>
        /// <returns></returns>
        public static PollingCallback CreateProfilePollingCallback<TButton, TAxis>(params IInputProfile<TButton, TAxis>[] profiles) where TButton : struct, System.IConvertible where TAxis : struct, System.IConvertible
        {
            if (profiles == null || profiles.Length == 0) return null;

            return (PollButtonSignatureRoutine targ, out ButtonDelegate del) =>
            {
                if (targ.PollButtons || targ.PollAxes)
                {
                    foreach (var p in profiles)
                    {
                        if (targ.PollButtons)
                        {
                            TButton btn;
                            if (p.TryPollButton(out btn, targ.Joystick))
                            {
                                del = p.CreateButtonDelegate(btn, targ.Joystick);
                                return true;
                            }
                        }

                        if (targ.PollAxes)
                        {
                            TAxis axis;
                            float value;
                            if (p.TryPollAxis(out axis, out value, targ.Joystick, targ.AxisPollingDeadZone) && TestConsideration(value, targ.AxisConsideration, targ.AxisPollingDeadZone))
                            {
                                del = SPInputFactory.CreateButtonDelegate(p.CreateAxisDelegate(axis, targ.Joystick), targ.AxisConsideration, targ.AxisPollingDeadZone);
                                return true;
                            }
                        }
                    }
                }

                del = null;
                return false;
            };
        }

        #endregion

    }

}
