using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Anim;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Anim
{
    public class i_PlayAnimation : TriggerableMechanism, IObservableTrigger, IBlockingTriggerableMechanism
    {

        private const string TRG_ONANIMCOMPLETE = "OnAnimComplete";

        public enum PlayByMode
        {
            PlayAnim,
            PlayAnimByID,
            PlayAnimFromResource
        }

        #region Fields

        [SerializeField]
        private PlayByMode _mode;

        [SerializeField]
        private TriggerableTargetObject _targetAnimator;

        [SerializeField]
        private string _id;

        [SerializeField]
        private UnityEngine.Object _clip;

        [SerializeField]
        private AnimSettings _settings = AnimSettings.Default;

        [SerializeField]
        private QueueMode _queueMode = QueueMode.PlayNow;
        [SerializeField]
        private PlayMode _playMode = PlayMode.StopSameLayer;

        [SerializeField()]
        private Trigger _onAnimComplete = new Trigger(TRG_ONANIMCOMPLETE);
        [SerializeField()]
        [Tooltip("If an animation doesn't play, should we signal complete. This is useful if the animation is supposed to be chaining to another i_ that MUST run.")]
        private bool _triggerCompleteIfNoAnim = true;
        [SerializeField()]
        [Tooltip("If this is called as a BlockingTriggerableMechanims, should it actually block?")]
        private bool _useAsBlockingYieldInstruction = true;
        [SerializeField()]
        [Tooltip("When this mechanism is called as a BlockingTriggerableMechanims, it will block the caller until complete. Set this true to allow the next step in the daisy chain to also block.")]
        private bool _daisyChainBlockingYieldInstruction = true;

        #endregion

        #region Methods

        private object PlayClip(UnityEngine.Object controller, UnityEngine.Object clip)
        {
            if (clip is AnimationClip)
            {
                if (controller is SPAnimationController)
                {
                    var a = (controller as SPAnimationController).CreateAuxiliarySPAnim(clip as AnimationClip);
                    _settings.Apply(a);
                    a.Play(_queueMode, _playMode);
                    return a;
                }
                else if (controller is Animation)
                {
                    var animController = controller as Animation;
                    var id = "aux*" + clip.GetInstanceID();
                    var a = animController[id];
                    if(a == null || a.clip != clip)
                    {
                        animController.AddClip(clip as AnimationClip, id);
                    }

                    var anim = animController.PlayQueued(id, _queueMode, _playMode);
                    _settings.Apply(anim);
                    return anim;
                }
            }
            else if (clip is IScriptableAnimationClip)
            {
                if (controller is SPAnimationController)
                {
                    return (controller as SPAnimationController).Play(clip as IScriptableAnimationClip);
                }
            }

            return null;
        }

        private object TryPlay(UnityEngine.Object controller)
        {
            switch (_mode)
            {
                case PlayByMode.PlayAnim:
                    return PlayClip(controller, _clip);
                case PlayByMode.PlayAnimByID:
                    {
                        if (controller is ISPAnimationSource)
                        {
                            var a = (controller as ISPAnimationSource).GetAnim(_id);
                            if (a != null)
                            {
                                a.Play(_queueMode, _playMode);
                            }
                            return a;
                        }
                        else if(controller is ISPAnimator)
                        {
                            (controller as ISPAnimator).Play(_id, _queueMode, _playMode);
                            return SPAnim.Null;
                        }
                        else if(controller is Animation)
                        {
                            var clip = (controller as Animation)[_id];
                            if(clip != null)
                            {
                                var a = (controller as Animation).PlayQueued(_id, _queueMode, _playMode);
                                _settings.Apply(a);
                                return a;
                            }
                        }

                        return null;
                    }
                case PlayByMode.PlayAnimFromResource:
                    return this.PlayClip(controller, Resources.Load<UnityEngine.Object>(_id));
                default:
                    return null;
            }
        }

        private UnityEngine.Object ResolveTargetAnimator(object arg)
        {
            var obj = _targetAnimator.GetTarget<UnityEngine.Object>(arg);
            if(obj == null || obj is ISPAnimationSource || obj is ISPAnimator || obj is Animation)
            {
                return obj;
            }
            else if (_targetAnimator.TargetsTriggerArg)
            {
                var go = GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                if (go == null) return null;

                SPAnimationController spcont;
                if (go.FindComponent<SPAnimationController>(out spcont))
                    return spcont;

                Animation anim;
                if (go.FindComponent<Animation>(out anim))
                    return anim;
            }
            
            return null;
        }



        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this.ResolveTargetAnimator(arg);
            if (targ == null) return false;

            var anim = this.TryPlay(targ);
            if (anim == null)
            {
                if (_triggerCompleteIfNoAnim) this.Invoke(() => { _onAnimComplete.ActivateTrigger(this, arg); }, 0.01f);
                return false;
            }

            if (_onAnimComplete.Count > 0)
            {
                if (anim is ISPAnim)
                {
                    (anim as ISPAnim).Schedule((s) =>
                    {
                        _onAnimComplete.ActivateTrigger(this, arg);
                    });
                }
                else if (anim is AnimationState)
                {
                    GameLoopEntry.Hook.StartCoroutine((anim as AnimationState).ScheduleLegacy(() =>
                    {
                        _onAnimComplete.ActivateTrigger(this, arg);
                    }));
                }
            }

            return false;
        }
        
        public bool Trigger(object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (!_useAsBlockingYieldInstruction || instruction == null) return this.Trigger(arg);
            if (!this.CanTrigger) return false;

            var targ = this.ResolveTargetAnimator(arg);
            if (targ == null) return false;

            var anim = this.TryPlay(targ);
            if (anim == null)
            {
                if (_triggerCompleteIfNoAnim) this.Invoke(() => { _onAnimComplete.ActivateTrigger(this, arg); }, 0.01f);
                return false;
            }

            instruction.BeginBlock();
            if (anim is ISPAnim)
            {
                (anim as ISPAnim).Schedule((s) =>
                {
                    if (_daisyChainBlockingYieldInstruction)
                        _onAnimComplete.DaisyChainTriggerYielding(this, arg, instruction);
                    else
                        _onAnimComplete.ActivateTrigger(this, arg);
                    instruction.EndBlock();
                });
            }
            else if (anim is AnimationState)
            {
                GameLoopEntry.Hook.StartCoroutine((anim as AnimationState).ScheduleLegacy(() =>
                {
                    if (_daisyChainBlockingYieldInstruction)
                        _onAnimComplete.DaisyChainTriggerYielding(this, arg, instruction);
                    else
                        _onAnimComplete.ActivateTrigger(this, arg);
                    instruction.EndBlock();
                }));
            }

            return false;
        }

        #endregion

        #region IObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onAnimComplete };
        }

        #endregion

        #region Static Interface

        public static bool IsAcceptibleAnimator(object obj)
        {
            return obj is ISPAnimationSource || obj is ISPAnimator || obj is Animation;
        }

        #endregion

    }
}
