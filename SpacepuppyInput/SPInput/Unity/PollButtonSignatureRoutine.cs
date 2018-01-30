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
    public class PollingButtonSignatureRoutine : IRadicalWaitHandle
    {

        public delegate bool PollingCallback(PollingButtonSignatureRoutine targ, out InputToken token);

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

        private System.Action<PollingButtonSignatureRoutine> _onComplete;

        #endregion

        #region CONSTRUCTOR

        public PollingButtonSignatureRoutine()
        {
            this.PollButtons = true;
            this.PollKeyboard = true;
            this.PollFromStandardSPInputs = true;
            this.PollJoyAxes = false;
            this.PollMouseAxes = false;
            this.Joystick = Joystick.All;
            this.AxisConsideration = AxleValueConsideration.Positive;
            this.AxisPollingDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE;
            this.ButtonPollingState = ButtonState.Down;
            this.CancelKey = UnityEngine.KeyCode.Escape;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The resulting InputToken that can be used to create an input signature.
        /// </summary>
        public InputToken InputResult
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
        /// Should we poll for joystick axis tilts. <para/>
        /// Default: False
        /// </summary>
        public bool PollJoyAxes
        {
            get;
            set;
        }

        /// <summary>
        /// Should we poll for mouse axis movement. <param/>
        /// Default: False
        /// </summary>
        public bool PollMouseAxes
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
        /// When polling a button, what state should we check for.
        /// Default: <see cref="ButtonState.Down"/>
        /// </summary>
        public ButtonState ButtonPollingState
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

        public void Start(float delay = 0f)
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
            this.InputResult = InputToken.Unknown;
            _routine = GameLoopEntry.Hook.StartRadicalCoroutine(this.WorkRoutine(delay));
        }

        public void Cancel()
        {
            if (_routine != null)
            {
                _routine.Cancel();
                _routine = null;
            }

            this.InputResult = InputToken.Unknown;
            _state = State.Cancelled;
            this.SignalOnComplete();
        }

        public IInputSignature CreateInputSignature(string id)
        {
            return new DelegatedButtonInputSignature(id, this.InputResult.CreateButtonDelegate(this.Joystick));
        }

        private void SignalOnComplete()
        {
            if (_onComplete != null)
            {
                var d = _onComplete;
                _onComplete = null;
                d(this);
            }
        }

        private System.Collections.IEnumerator WorkRoutine(float delay)
        {
            yield return WaitForDuration.Seconds(delay, SPTime.Real);

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
                
                if (this.CustomPollingCallback != null)
                {
                    InputToken t;
                    if (this.CustomPollingCallback(this, out t))
                    {
                        this.InputResult = t;
                        goto Complete;
                    }
                }

                if (this.PollFromStandardSPInputs)
                {
                    if (this.PollButtons)
                    {
                        SPInputId btn;
                        if (SPInputDirect.TryPollButton(out btn, this.ButtonPollingState, this.Joystick))
                        {
                            this.InputResult = InputToken.CreateButton(btn);
                            goto Complete;
                        }
                    }

                    if (this.PollJoyAxes || this.PollMouseAxes)
                    {
                        SPInputId axis;
                        float value;
                        if (SPInputDirect.TryPollAxis(out axis, out value, this.Joystick, this.PollMouseAxes, this.AxisPollingDeadZone) && TestConsideration(value, this.AxisConsideration, this.AxisPollingDeadZone))
                        {
                            if ((this.PollJoyAxes && axis.IsJoyAxis()) || (this.PollMouseAxes && axis.IsMouseAxis()))
                            {
                                this.InputResult = InputToken.CreateAxleButton(axis, this.AxisConsideration, this.AxisPollingDeadZone);
                                goto Complete;
                            }
                        }
                    }
                }

                if (this.PollKeyboard)
                {
                    UnityEngine.KeyCode key;
                    if (SPInputDirect.TryPollKey(out key, this.ButtonPollingState))
                    {
                        this.InputResult = InputToken.CreateButton(key);
                        goto Complete;
                    }
                }

                yield return null;
            }

            Complete:
            _state = State.Complete;
            _routine = null;
            this.SignalOnComplete();
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

        public void OnComplete(System.Action<PollingButtonSignatureRoutine> callback)
        {
            _onComplete += callback;
        }

        void IRadicalWaitHandle.OnComplete(Action<IRadicalWaitHandle> callback)
        {
            if (callback == null) return;
            _onComplete += (o) => callback(o);
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
            switch (consideration)
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
        public static PollingCallback CreateProfilePollingCallback<TInputId>(params IInputProfile<TInputId>[] profiles) where TInputId : struct, System.IConvertible
        {
            if (profiles == null || profiles.Length == 0) return null;

            return (PollingButtonSignatureRoutine targ, out InputToken token) =>
            {
                if (targ.PollButtons || targ.PollJoyAxes)
                {
                    foreach (var p in profiles)
                    {
                        if (targ.PollButtons)
                        {
                            TInputId btn;
                            if (p.TryPollButton(out btn, ButtonState.Released, targ.Joystick))
                            {
                                token = p.GetMapping(btn);
                                return true;
                            }
                        }

                        if (targ.PollJoyAxes)
                        {
                            TInputId axis;
                            float value;
                            if (p.TryPollAxis(out axis, out value, targ.Joystick, targ.AxisPollingDeadZone))
                            {
                                token = p.GetMapping(axis);
                                return true;
                            }
                        }
                    }
                }

                token = InputToken.Unknown;
                return false;
            };
        }

        #endregion

    }

}
