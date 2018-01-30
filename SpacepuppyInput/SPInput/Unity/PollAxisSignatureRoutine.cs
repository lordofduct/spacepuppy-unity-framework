using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.SPInput.Unity
{

    /// <summary>
    /// Use this to poll for an InputSignature defined by the user. You start by creating this object, starting it and waiting for it to complete. 
    /// It can also be used as a yield instruction to wait for its completion.<para/>
    /// Behaviour of this routine:<para/>
    /// 1) The system expects the player to press in the 'up' direction to register an axis. If they press down it interprets this as wanting inverted controls. <para/>
    /// 2) Use the PollAsTrigger property to control registering only positive presses for triggers. <para/>
    /// 3) If PollButtons or PollKeyboard is set true, it will register the first button press as the positive value, and the second as the negative. 
    /// Unless PollAsTrigger is true, in which case it immediately registers as a trigger axis on first button press.
    /// </summary>
    /// <typeparam name="TButton"></typeparam>
    /// <typeparam name="TAxis"></typeparam>
    public class PollingAxisSignatureRoutine : IRadicalWaitHandle
    {

        public const float DEFAULT_ButtonPressMonitorDuration = 5f;

        public delegate bool PollingCallback(PollingAxisSignatureRoutine targ, out InputToken token);

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

        private System.Action<PollingAxisSignatureRoutine> _onComplete;

        #endregion

        #region CONSTRUCTOR

        public PollingAxisSignatureRoutine()
        {
            this.PollButtons = false;
            this.PollKeyboard = false;
            this.PollFromStandardSPInputs = true;
            this.PollJoyAxes = true;
            this.PollMouseAxes = false;
            this.Joystick = Joystick.All;
            this.AxisConsideration = AxleValueConsideration.Absolute;
            this.AxisPollingDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE;
            this.ButtonPollingState = ButtonState.Down;
            this.ButtonPressMonitorDuration = 5.0f;
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
        /// Default: False
        /// </summary>
        public bool PollButtons
        {
            get;
            set;
        }

        /// <summary>
        /// Should we poll key presses. <para/>
        /// Default: False
        /// </summary>
        public bool PollKeyboard
        {
            get;
            set;
        }

        /// <summary>
        /// Should we poll for joystick axis tilts. <para/>
        /// Default: True
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
        /// Default: <see cref="AxleValueConsideration.Absolute"/>
        /// </summary>
        public AxleValueConsideration AxisConsideration
        {
            get;
            set;
        }

        /// <summary>
        /// When polling an axis what is considered the 'not pressed' dead zone. <para/>
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
        /// If monitoring for button/key presses to emulate an axis this is how long in seconds one should wait before giving up on waiting for the second button press.<para/>
        /// Note - If AxisConsideration is Positive/Negative, it'll register on the first button press.
        /// Default: <see cref="PollingAxisSignatureRoutine.DEFAULT_ButtonPressMonitorDuration"/>
        /// </summary>
        public float ButtonPressMonitorDuration
        {
            get;
            set;
        }

        /// <summary>
        /// When polling for the axis, consider it as a trigger. In this case it only registers when the value is positive. This can be used to deal with 2 key issues.<para/>
        /// 1) When monitoring for 0->1 presses for triggers, this will allow you to ignore negative presses by monitoring only Positive. <para/>
        /// 2) For strange input devices that register -1 for depressed (PS4 controller on some platforms), this allows you to 
        /// ignore that depressed state by setting it to only monitor Positive. <para/>
        /// Default: True
        /// </summary>
        public bool PollAsTrigger
        {
            get;
            set;
        }
        
        /// <summary>
        /// A key the user can press to cancel out of the polling. <para/>
        /// Default: <see cref="UnityEngine.KeyCode.Escape"/>
        /// </summary>
        public UnityEngine.KeyCode CancelKey
        {
            get;
            set;
        }

        /// <summary>
        /// A custom button press that can be used to register a cancel instead of the CancelKey. 
        /// Useful for allowing a gamepad button press to cancel with. <para/>
        /// Default: Null
        /// </summary>
        public ButtonDelegate CancelDelegate
        {
            get;
            set;
        }

        /// <summary>
        /// Set this to allow to poll for some custom inputs.
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

            _state = State.Cancelled;
            this.InputResult = InputToken.Unknown;
            this.SignalOnComplete();
        }

        public IAxleInputSignature CreateInputSignature(string id)
        {
            return new DelegatedAxleInputSignature(id, this.InputResult.CreateAxisDelegate(this.Joystick));
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

            SPInputId positiveBtn = SPInputId.Unknown;
            UnityEngine.KeyCode positiveKey = UnityEngine.KeyCode.None;
            float t = float.NegativeInfinity;

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
                    InputToken token;
                    if(this.CustomPollingCallback(this, out token))
                    {
                        this.InputResult = token;
                        goto Complete;
                    }
                }

                if (this.PollFromStandardSPInputs)
                {
                    if (this.PollJoyAxes || this.PollMouseAxes)
                    {
                        SPInputId axis;
                        float value;
                        if (SPInputDirect.TryPollAxis(out axis, out value, this.Joystick, this.PollMouseAxes, this.AxisPollingDeadZone) && TestConsideration(value, this.AxisConsideration, this.AxisPollingDeadZone))
                        {
                            if ((this.PollJoyAxes && axis.IsJoyAxis()) || (this.PollMouseAxes && axis.IsMouseAxis()))
                            {
                                if (this.PollAsTrigger)
                                {
                                    this.InputResult = InputToken.CreateTrigger(axis);
                                }
                                else
                                {
                                    this.InputResult = InputToken.CreateAxis(axis, value < 0f);
                                }
                                goto Complete;
                            }
                        }
                    }

                    if (this.PollButtons)
                    {
                        SPInputId btn;
                        if (SPInputDirect.TryPollButton(out btn, this.ButtonPollingState, this.Joystick))
                        {
                            if (this.PollAsTrigger)
                            {
                                this.InputResult = InputToken.CreateTrigger(btn, AxleValueConsideration.Positive);
                                goto Complete;
                            }
                            if (positiveBtn != SPInputId.Unknown)
                            {
                                this.InputResult = InputToken.CreateEmulatedAxis(positiveBtn, btn);
                                goto Complete;
                            }
                            else
                            {
                                positiveKey = UnityEngine.KeyCode.None;
                                positiveBtn = btn;
                                t = UnityEngine.Time.realtimeSinceStartup;
                                goto Restart;
                            }
                        }
                    }
                }

                if (this.PollKeyboard)
                {
                    UnityEngine.KeyCode key;
                    if (SPInputDirect.TryPollKey(out key, this.ButtonPollingState))
                    {
                        if (this.PollAsTrigger)
                        {
                            this.InputResult = InputToken.CreateTrigger(key);
                            goto Complete;
                        }
                        if (positiveKey != UnityEngine.KeyCode.None)
                        {
                            this.InputResult = InputToken.CreateEmulatedAxis(positiveKey, key);
                            goto Complete;
                        }
                        else
                        {
                            positiveBtn = SPInputId.Unknown;
                            positiveKey = key;
                            t = UnityEngine.Time.realtimeSinceStartup;
                            goto Restart;
                        }
                    }
                }

                Restart:
                yield return null;

                if (UnityEngine.Time.realtimeSinceStartup - t > this.ButtonPressMonitorDuration)
                {
                    positiveBtn = SPInputId.Unknown;
                    positiveKey = UnityEngine.KeyCode.None;
                }
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

        public void OnComplete(System.Action<PollingAxisSignatureRoutine> callback)
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
        public static PollingCallback CreateProfileAxisPollingCallback<TInputId>(params IInputProfile<TInputId>[] profiles) where TInputId : struct, System.IConvertible
        {
            if (profiles == null || profiles.Length == 0) return null;

            return (PollingAxisSignatureRoutine targ, out InputToken token) =>
            {
                if (targ.PollJoyAxes)
                {
                    foreach (var p in profiles)
                    {
                        TInputId axis;
                        float value;
                        if (p.TryPollAxis(out axis, out value, targ.Joystick, targ.AxisPollingDeadZone))
                        {
                            token = p.GetMapping(axis);
                            if(value < 0f && token.Mode == InputMode.Axis)
                            {
                                token.AltValue = (token.AltValue == 0) ? 1 : 0;
                            }
                            return true;
                        }
                    }
                }

                if (targ.PollButtons && targ.PollAsTrigger)
                {
                    foreach (var p in profiles)
                    {
                        TInputId btn;
                        if (p.TryPollButton(out btn, ButtonState.Released, targ.Joystick))
                        {
                            token = p.GetMapping(btn);
                            return true;
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
