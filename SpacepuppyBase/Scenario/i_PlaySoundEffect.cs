#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    /// <summary>
    /// Plays a sound on an AudioSource as a one shot.
    /// </summary>
    [Infobox("Plays a sound on an AudioSource as a one shot.")]
    public class i_PlaySoundEffect : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(AudioSource))]
        private TriggerableTargetObject _targetAudioSource = new TriggerableTargetObject();

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
                            if(src != null && src.isActiveAndEnabled)
                            {
                                _completeRoutine = this.InvokeGuaranteed(this.OnAudioComplete, clip.length, SPTime.Real);
                                src.PlayOneShot(clip);
                            }
                            else
                            {
                                this.OnAudioComplete();
                            }
                        }
                    }, _delay);
                }
                else
                {
                    if (src != null && src.isActiveAndEnabled)
                    {
                        _completeRoutine = this.InvokeGuaranteed(this.OnAudioComplete, clip.length, SPTime.Real);
                        src.PlayOneShot(clip);
                    }
                    else
                    {
                        this.OnAudioComplete();
                    }
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