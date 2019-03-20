#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [Infobox("Plays the audio clip that is configured in an AudioSource. Only one sound can be played at a time, this is primarily used for music.")]
    public class i_PlayAudio : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(AudioSource))]
        private TriggerableTargetObject _targetAudioSource = new TriggerableTargetObject();

        [SerializeField]
        [EnumFlags]
        private AudioSettingsMask _settingsMask;
        [SerializeField]
        private AudioSettings _settings;

        [SerializeField()]
        [EnumPopupExcluding((int)AudioInterruptMode.PlayOverExisting)]
        private AudioInterruptMode _interrupt = AudioInterruptMode.StopIfPlaying;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay;
        
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

        #region ITriggerableMechanism Interface
        
        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var src = _targetAudioSource.GetTarget<AudioSource>(arg);
            if (src == null)
            {
                Debug.LogWarning("Failed to play audio due to a lack of AudioSource on the target.", this);
                return false;
            }

            if (_interrupt == AudioInterruptMode.DoNotPlayIfPlaying && src.isPlaying) return false;

            if (_settingsMask != 0) _settings.Apply(src, _settingsMask);

            if (_delay > 0f)
                src.PlayDelayed(_delay);
            else
                src.Play();

            return true;
        }

        #endregion
        
    }

}
