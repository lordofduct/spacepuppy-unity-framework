using UnityEngine;
using System.Collections;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_PlaySoundEffect : TriggerableMechanism
    {

        public enum InterruptMode
        {
            StopIfPlaying = 0,
            DoNotPlayIfPlaying = 1
        }

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(AudioSource))]
        private TriggerableTargetObject _targetAudioSource = new TriggerableTargetObject(TriggerableTargetObject.TargetSource.Self);
        [SerializeField()]
        [OneOrMany()]
        private AudioClip[] _clips;
        public InterruptMode Interrupt = InterruptMode.StopIfPlaying;
        public float Delay;

        [Tooltip("Trigger something at the end of the sound effect. This is NOT perfectly accurate and really just starts a timer for the duration of the sound being played.")]
        [SerializeField()]
        private Trigger _onAudioComplete;

        private RadicalCoroutine _completeRoutine;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Methods

        private void OnAudioComplete()
        {
            _completeRoutine = null;
            _onAudioComplete.ActivateTrigger();
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _clips != null && _clips.Length > 0;
            }
        }

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var src = _targetAudioSource.GetTarget<AudioSource>(arg, false);
            if (src == null)
            {
                Debug.LogWarning("Failed to play audio due to a lack of AudioSource on the target.", this);
                return false;
            }
            if (src.isPlaying)
            {
                switch (this.Interrupt)
                {
                    case InterruptMode.StopIfPlaying:
                        if (_completeRoutine != null) _completeRoutine.Cancel();
                        _completeRoutine = null;
                        src.Stop();
                        break;
                    case InterruptMode.DoNotPlayIfPlaying:
                        return false;
                }
            }

            var clip = _clips[Random.Range(0, _clips.Length)];
            src.clip = clip;

            if (clip != null)
            {
                if (this.Delay > 0)
                {
                    this.Invoke(() =>
                    {
                        if (src != null)
                        {
                            _completeRoutine = this.InvokeRadical(this.OnAudioComplete, clip.length);
                            src.Play();
                        }
                    }, this.Delay);
                }
                else
                {
                    _completeRoutine = this.InvokeRadical(this.OnAudioComplete, clip.length);
                    src.Play();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }

}