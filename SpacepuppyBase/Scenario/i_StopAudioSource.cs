#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;

using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_StopAudioSource : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(AudioSource))]
        private TriggerableTargetObject _target;

        [SerializeField]
        [TimeUnitsSelector()]
        private float _fadeOutDur;

        [SerializeField]
        private DisableMode _disableAudioSource;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public float FadeOutDur
        {
            get { return _fadeOutDur; }
            set { _fadeOutDur = value; }
        }

        public DisableMode DisableAudioSource
        {
            get { return _disableAudioSource; }
            set { _disableAudioSource = value; }
        }

        #endregion


        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<AudioSource>(arg);
            if (targ == null) return false;
            if (!targ.isPlaying) return false;

            if(_fadeOutDur > 0f)
            {
                float cache = targ.volume;
                SPTween.Tween(targ)
                       .To("volume", _fadeOutDur, 0f)
                       .OnFinish((s,e) =>
                       {
                           targ.Stop();
                           targ.volume = cache;
                           switch(_disableAudioSource)
                           {
                               case DisableMode.DisableComponent:
                                   targ.enabled = false;
                                   break;
                               case DisableMode.DisableGameObject:
                                   targ.gameObject.SetActive(false);
                                   break;
                           }
                       })
                       .Play(true);
            }
            else
            {
                targ.Stop();
                switch (_disableAudioSource)
                {
                    case DisableMode.DisableComponent:
                        targ.enabled = false;
                        break;
                    case DisableMode.DisableGameObject:
                        targ.gameObject.SetActive(false);
                        break;
                }
            }

            return true;
        }
    }
}
