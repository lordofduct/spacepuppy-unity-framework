#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Audio
{

    public class i_PlayAmbientAudio : AutoTriggerableMechanism
    {

        #region Fields
        
        [SerializeField()]
        [ReorderableArray()]
        private AudioClip[] _clips;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Interrupt")]
        private AudioInterruptMode _interrupt = AudioInterruptMode.StopIfPlaying;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay;

        [Tooltip("Trigger something at the end of the sound effect. This is NOT perfectly accurate and really just starts a timer for the duration of the sound being played.")]
        [SerializeField()]
        private Trigger _onAudioComplete;

        [System.NonSerialized()]
        private System.IDisposable _completeRoutine;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float Delay
        {
            get { return _delay; }
        }

        public AudioInterruptMode Interrupt
        {
            get { return _interrupt; }
            set { _interrupt = value; }
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

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var manager = Services.Get<IAudioManager>();
            if (manager == null || manager.BackgroundAmbientAudioSource == null)
            {
                Debug.LogWarning("Failed to play audio due to a lack of AudioManager.", this);
                return false;
            }

            var src = manager.BackgroundAmbientAudioSource;
            if (src == null)
            {
                Debug.LogWarning("Failed to play audio due to a lack of BackgroundAmbientAudioSource on the AudioManager.", this);
                return false;
            }

            if (src.isPlaying)
            {
                switch (this.Interrupt)
                {
                    case AudioInterruptMode.StopIfPlaying:
                        if (_completeRoutine != null) _completeRoutine.Dispose();
                        _completeRoutine = null;
                        src.Stop();
                        break;
                    case AudioInterruptMode.DoNotPlayIfPlaying:
                        return false;
                    case AudioInterruptMode.PlayOverExisting:
                        //play one shot over existing audio
                        break;
                }
            }

            var clip = _clips[Random.Range(0, _clips.Length)];
            //src.clip = clip;

            if (clip != null)
            {
                if (_delay > 0)
                {
                    this.InvokeGuaranteed(() =>
                    {
                        if (src != null)
                        {
                            _completeRoutine = this.InvokeGuaranteed(this.OnAudioComplete, clip.length, SPTime.Real);
                            //src.Play();
                            src.PlayOneShot(clip);
                        }
                    }, _delay);
                }
                else
                {
                    _completeRoutine = this.InvokeGuaranteed(this.OnAudioComplete, clip.length, SPTime.Real);
                    //src.Play();
                    src.PlayOneShot(clip);
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
