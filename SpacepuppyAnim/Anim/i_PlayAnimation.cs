#pragma warning disable 0649 // variable declared but not used.
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
    public class i_PlayAnimation : TriggerableMechanism, IObservableTrigger, IBlockingTriggerableMechanism, ISerializationCallbackReceiver
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
        private bool _applyCustomSettings; //OBSOLETE
        [SerializeField]
        private AnimSettingsMask _settingsMask;
        [SerializeField]
        private AnimSettings _settings = AnimSettings.Default;

        [SerializeField]
        private QueueMode _queueMode = QueueMode.PlayNow;
        [SerializeField]
        private PlayMode _playMode = PlayMode.StopSameLayer;
        [SerializeField]
        private float _crossFadeDur = 0f;

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

        private object PlayClip(SPAnimationController controller, UnityEngine.Object clip)
        {
            if (controller == null || !controller.isActiveAndEnabled || clip == null) return null;

            if (clip is AnimationClip)
            {
                if (_crossFadeDur > 0f)
                    return controller.CrossFadeAuxiliary(clip as AnimationClip,
                                                         (_settingsMask != 0) ? AnimSettings.Intersect(AnimSettings.Default, _settings, _settingsMask) : AnimSettings.Default,
                                                         _crossFadeDur, _queueMode, _playMode);
                else
                    return controller.PlayAuxiliary(clip as AnimationClip,
                                                    (_settingsMask != 0) ? AnimSettings.Intersect(AnimSettings.Default, _settings, _settingsMask) : AnimSettings.Default,
                                                    _queueMode, _playMode);
            }
            else if (clip is IScriptableAnimationClip)
            {
                return controller.Play(clip as IScriptableAnimationClip);
            }

            return null;
        }

        private object PlayClip(Animation controller, AnimationClip clip)
        {
            if (controller == null || !controller.isActiveAndEnabled || clip == null) return null;

            var id = "aux*" + clip.GetInstanceID();
            var a = controller[id];
            if (a == null || a.clip != clip)
            {
                controller.AddClip(clip, id);
            }

            AnimationState anim;
            if (_crossFadeDur > 0f)
                anim = controller.CrossFadeQueued(id, _crossFadeDur, _queueMode, _playMode);
            else
                anim = controller.PlayQueued(id, _queueMode, _playMode);
            if (_settingsMask != 0) _settings.Apply(anim, _settingsMask);
            return anim;
        }

        private object TryPlay(object controller)
        {
            if(controller is SPAnimationController)
            {
                switch (_mode)
                {
                    case PlayByMode.PlayAnim:
                        return PlayClip(controller as SPAnimationController, _clip);
                    case PlayByMode.PlayAnimByID:
                        var anim = (controller as SPAnimationController).GetAnim(_id);
                        if (anim != null)
                        {
                            if (_crossFadeDur > 0f)
                                anim.CrossFade(_crossFadeDur, _queueMode, _playMode);
                            else
                                anim.Play(_queueMode, _playMode);
                            if (_settingsMask != 0) _settings.Apply(anim, _settingsMask);
                            return anim;
                        }
                        return null;
                    case PlayByMode.PlayAnimFromResource:
                        return this.PlayClip(controller as SPAnimationController, Resources.Load<UnityEngine.Object>(_id));
                }
            }
            else if(controller is Animation)
            {
                switch (_mode)
                {
                    case PlayByMode.PlayAnim:
                        return PlayClip(controller as Animation, _clip as AnimationClip);
                    case PlayByMode.PlayAnimByID:
                        var comp = controller as Animation;
                        if (comp[_id] != null)
                        {
                            AnimationState anim;
                            if (_crossFadeDur > 0f)
                                anim = comp.CrossFadeQueued(_id, _crossFadeDur, _queueMode, _playMode);
                            else
                                anim = comp.PlayQueued(_id, _queueMode, _playMode);
                            if (_settingsMask != 0) _settings.Apply(anim, _settingsMask);
                            return anim;
                        }
                        return null;
                    case PlayByMode.PlayAnimFromResource:
                        return this.PlayClip(controller as Animation, Resources.Load<AnimationClip>(_id));
                }
            }
            else if(controller is ISPAnimationSource)
            {
                if(_mode == PlayByMode.PlayAnimByID)
                {
                    var anim = (controller as ISPAnimationSource).GetAnim(_id);
                    if (anim != null)
                    {
                        if (_crossFadeDur > 0f)
                            anim.CrossFade(_crossFadeDur, _queueMode, _playMode);
                        else
                            anim.Play(_queueMode, _playMode);
                        if (_settingsMask != 0) _settings.Apply(anim, _settingsMask);
                        return anim;
                    }
                    return null;
                }
            }
            else if(controller is ISPAnimator)
            {
                if (string.IsNullOrEmpty(_id)) return null;
                return com.spacepuppy.Dynamic.DynamicUtil.InvokeMethod(controller, _id);
            }

            return null;
        }

        private object ResolveTargetAnimator(object arg)
        {
            var obj = _targetAnimator.GetTarget<UnityEngine.Object>(arg);

            ISPAnimationSource src = null;
            ISPAnimator spanim = null;
            Animation anim = null;

            if (ObjUtil.GetAsFromSource<ISPAnimationSource>(obj, out src))
                return src;
            if (ObjUtil.GetAsFromSource<ISPAnimator>(obj, out spanim))
                return spanim;
            if (ObjUtil.GetAsFromSource<Animation>(obj, out anim))
                return anim;

            if (obj is SPEntity || _targetAnimator.ImplicityReducesEntireEntity)
            {
                var go = obj is SPEntity ? (obj as SPEntity).gameObject : GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                if (go == null) return null;

                SPAnimationController spcont;
                if (go.FindComponent<SPAnimationController>(out spcont))
                    return spcont;
                
                if (go.FindComponent<Animation>(out anim))
                    return anim;
            }
            
            return null;
        }



        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this.ResolveTargetAnimator(arg);
            if (targ == null) return false;

            var anim = this.TryPlay(targ);
            if (anim.IsNullOrDestroyed())
            {
                if (_triggerCompleteIfNoAnim) this.Invoke(() => { _onAnimComplete.ActivateTrigger(this, arg); }, 0f);
                return false;
            }

            if (_onAnimComplete.Count > 0)
            {
                AnimUtil.TrySchedule(anim, (a) =>
                {
                    _onAnimComplete.ActivateTrigger(this, arg);
                });
            }

            return false;
        }
        
        public bool Trigger(object sender, object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (!_useAsBlockingYieldInstruction || instruction == null) return this.Trigger(sender, arg);
            if (!this.CanTrigger) return false;

            var targ = this.ResolveTargetAnimator(arg);
            if (targ == null) return false;

            var anim = this.TryPlay(targ);
            if (anim == null)
            {
                if (_triggerCompleteIfNoAnim) this.Invoke(() => { _onAnimComplete.ActivateTrigger(this, arg); }, 0f);
                return false;
            }

            instruction.BeginBlock();
            AnimUtil.TrySchedule(anim, (a) =>
            {
                if (_daisyChainBlockingYieldInstruction)
                    _onAnimComplete.DaisyChainTriggerYielding(this, arg, instruction);
                else
                    _onAnimComplete.ActivateTrigger(this, arg);
                instruction.EndBlock();
            });

            return false;
        }

        #endregion

        #region IObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onAnimComplete };
        }

        #endregion

        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(_applyCustomSettings)
            {
                _applyCustomSettings = false;
                _settingsMask = (AnimSettingsMask)(-1);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_applyCustomSettings)
            {
                _applyCustomSettings = false;
                _settingsMask = (AnimSettingsMask)(-1);
            }
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
