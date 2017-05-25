#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Scenario
{
    public class i_TriggerSequence : AutoTriggerableMechanism
    {

        public enum WrapMode
        {
            Oblivion,
            Clamp,
            Loop,
            PingPong
        }

        public enum SignalMode
        {
            Manual,
            Auto
        }


        #region Fields
        
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("_mode")]
        private WrapMode _wrapMode;

        [SerializeField]
        private SignalMode _signal;

        [SerializeField()]
        private Trigger _trigger;

        [SerializeField()]
        private bool _passAlongTriggerArg;

        [SerializeField]
        [MinRange(0)]
        private int _currentIndex = 0;


        [System.NonSerialized()]
        private RadicalCoroutine _routine;

        #endregion

        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if(this.ActivateOn == ActivateEvent.None)
                this.AttemptAutoStart();
        }

        #endregion

        #region Properties

        public WrapMode Wrap
        {
            get { return _wrapMode; }
            set { _wrapMode = value; }
        }

        public SignalMode Signal
        {
            get { return _signal; }
            set { _signal = value; }
        }

        public Trigger TriggerSequence
        {
            get
            {
                return _trigger;
            }
        }

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }

        public int CurrentIndexNormalized
        {
            get
            {
                switch (_wrapMode)
                {
                    case WrapMode.Oblivion:
                        return _currentIndex;
                    case WrapMode.Clamp:
                        return Mathf.Clamp(_currentIndex, 0, _trigger.Targets.Count - 1);
                    case WrapMode.Loop:
                        return _currentIndex % _trigger.Targets.Count;
                    case WrapMode.PingPong:
                        return (int)Mathf.PingPong(_currentIndex, _trigger.Targets.Count - 1);
                    default:
                        return _currentIndex;
                }
            }
        }

        #endregion

        #region Methods

        public void Reset()
        {
            if (_routine != null)
            {
                _routine.Cancel();
                _routine = null;
            }

            _currentIndex = 0;

            if (Application.isPlaying && this.enabled)
            {
                this.AttemptAutoStart();
            }
        }

        private void AttemptAutoStart()
        {
            int i = this.CurrentIndexNormalized;
            if (i < 0 || i >= _trigger.Targets.Count) return;

            //if (_signal == SignalMode.Auto && _trigger.Targets[i].Target != null)
            //{
            //    var signal = _trigger.Targets[i].Target.GetComponentInChildren<IAutoSequenceSignal>();
            //    if (signal != null)
            //    {
            //        _routine = this.StartRadicalCoroutine(this.DoAutoSequence(signal), RadicalCoroutineDisableMode.Pauses);
            //    }
            //}
            if (_signal == SignalMode.Auto)
            {
                IAutoSequenceSignal signal;
                var targ = GameObjectUtil.GetGameObjectFromSource(_trigger.Targets[i].Target);
                if(targ != null && targ.GetComponentInChildren<IAutoSequenceSignal>(out signal))
                if (signal != null)
                {
                    _routine = this.StartRadicalCoroutine(this.DoAutoSequence(signal), RadicalCoroutineDisableMode.Pauses);
                }
            }
        }
        
        private System.Collections.IEnumerator DoAutoSequence(IAutoSequenceSignal signal)
        {
            if (signal != null)
            {
                yield return signal.Wait();
                _currentIndex++;
            }

            while (true)
            {
                int i = this.CurrentIndexNormalized;
                if (i < 0 || i >= _trigger.Targets.Count) yield break;
                _currentIndex++;

                //if (_trigger.Targets[i].Target != null && _trigger.Targets[i].Target.GetComponentInChildren<IAutoSequenceSignal>(out signal))
                //{
                //    var handle = signal.Wait();
                //    _trigger.ActivateTriggerAt(i, this, null);
                //    yield return handle;
                //}
                //else
                //{
                //    _trigger.ActivateTriggerAt(i, this, null);
                //    yield return null;
                //}
                var go = GameObjectUtil.GetGameObjectFromSource(_trigger.Targets[i].Target);
                if (go != null && go.GetComponentInChildren<IAutoSequenceSignal>(out signal))
                {
                    var handle = signal.Wait();
                    _trigger.ActivateTriggerAt(i, this, null);
                    yield return handle;
                }
                else
                {
                    _trigger.ActivateTriggerAt(i, this, null);
                    yield return null;
                }
            }
        }
        
        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;


            switch (_signal)
            {
                case SignalMode.Manual:
                    {
                        _trigger.ActivateTriggerAt(this.CurrentIndexNormalized, this, _passAlongTriggerArg ? arg : null);
                        _currentIndex++;
                    }
                    break;
                case SignalMode.Auto:
                    {
                        if (_routine != null) _routine.Cancel();
                        _routine = this.StartRadicalCoroutine(this.DoAutoSequence(null), RadicalCoroutineDisableMode.Pauses);
                    }
                    break;
            }

            return true;
        }
        
        #endregion

    }
}
