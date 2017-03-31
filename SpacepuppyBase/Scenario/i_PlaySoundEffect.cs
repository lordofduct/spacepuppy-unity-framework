#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
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
        private TriggerableTargetObject _targetAudioSource = new TriggerableTargetObject();

        [SerializeField()]
        [OneOrMany()]
        private AudioClip[] _clips;
        
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Interrupt")]
        private InterruptMode Interrupt = InterruptMode.StopIfPlaying;
        
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay;

        [Tooltip("Trigger something at the end of the sound effect. This is NOT perfectly accurate and really just starts a timer for the duration of the sound being played.")]
        [SerializeField()]
        private Trigger _onAudioComplete;

        [System.NonSerialized()]
        private RadicalCoroutine _completeRoutine;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float Delay
        {
            get { return _delay; }
        }

        #endregion

        #region Methods

        private void OnAudioComplete()
        {
            _completeRoutine = null;
            _onAudioComplete.ActivateTrigger(this, null);
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

            var src = _targetAudioSource.GetTarget<AudioSource>(arg);
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
                if (this._delay > 0)
                {
                    this.Invoke(() =>
                    {
                        if (src != null)
                        {
                            _completeRoutine = this.InvokeRadical(this.OnAudioComplete, clip.length);
                            src.Play();
                        }
                    }, this._delay);
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