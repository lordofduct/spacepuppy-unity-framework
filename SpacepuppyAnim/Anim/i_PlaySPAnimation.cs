#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Anim;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{

    [Infobox("Be aware that this is a BlockingTriggerableMechanism, if called from a BlockingTrigger (in a coroutine for example), it will block that sequence until complete is fired. If it shouldn't block, be sure to uncheck 'UseAsBlockingYieldInstruction'.", MessageType = InfoBoxMessageType.Info)]
    public class i_PlaySPAnimation : TriggerableMechanism, IObservableTrigger, IBlockingTriggerableMechanism
    {

        private const string TRG_ONANIMCOMPLETE = "OnAnimComplete";

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(ISPAnimationSource))]
        [FormerlySerializedAs("TargetAnimator")]
        private TriggerableTargetObject _targetAnimator;
        [SerializeField()]
        [FormerlySerializedAs("ClipIDs")]
        [OneOrMany()]
        private string[] _clipIDs;
        [SerializeField()]
        [FormerlySerializedAs("PlayMode")]
        private PlayMode _playMode = PlayMode.StopSameLayer;
        [SerializeField()]
        [FormerlySerializedAs("Speed")]
        private float _speed = 1f;
        [SerializeField()]
        private SPTime _timeSupplier;
        [SerializeField()]
        [FormerlySerializedAs("OnAnimComplete")]
        private Trigger _onAnimComplete = new Trigger(TRG_ONANIMCOMPLETE);
        [SerializeField()]
        [Tooltip("If an animation doesn't play, should we signal complete. This is useful if the animation is supposed to be chaining to another i_ that MUST run.")]
        [FormerlySerializedAs("TriggerCompleteIfNoAnim")]
        private bool _triggerCompleteIfNoAnim = true;
        [SerializeField()]
        [Tooltip("If this is called as a BlockingTriggerableMechanims, should it actually block?")]
        private bool _useAsBlockingYieldInstruction = true;
        [SerializeField()]
        [Tooltip("When this mechanism is called as a BlockingTriggerableMechanims, it will block the caller until complete. Set this true to allow the next step in the daisy chain to also block.")]
        private bool _daisyChainBlockingYieldInstruction = true;
        [SerializeField()]
        private float _startTime;

        [System.NonSerialized()]
        private ISPAnim _currentAnimState;

        #endregion

        #region CONSTRUCTOR
        
        #endregion

        #region Properties

        public TriggerableTargetObject TargetAnimator
        {
            get { return _targetAnimator; }
        }

        public PlayMode PlayMode
        {
            get { return _playMode; }
            set { _playMode = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public ITimeSupplier TimeSupplier
        {
            get { return _timeSupplier.TimeSupplier; }
            set { _timeSupplier.TimeSupplier = value; }
        }

        public Trigger OnAnimComplete
        {
            get { return _onAnimComplete; }
        }

        public bool TriggerCompleteIfNoAnim
        {
            get { return _triggerCompleteIfNoAnim; }
            set { _triggerCompleteIfNoAnim = value; }
        }

        public float StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public ISPAnim CurrentAnimState
        {
            get { return _currentAnimState; }
        }

        #endregion

        #region Methods

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var anim = _targetAnimator.GetTarget<ISPAnimationSource>(arg);
            if (anim == null || !anim.CanPlayAnim) return false;

            var id = _clipIDs.PickRandom();
            _currentAnimState = anim.GetAnim(id);
            if (_currentAnimState == null)
            {
                //this.Invoke(() => { this.OnAnimCompleteCallback(null); }, 0f);
                if (_triggerCompleteIfNoAnim) this.Invoke(() => { _onAnimComplete.ActivateTrigger(this, arg); }, 0.01f);
                return false;
            }
            
            _currentAnimState.TimeSupplier = _timeSupplier.TimeSupplier;
            _currentAnimState.Play(_speed, _startTime, QueueMode.PlayNow, _playMode);
            _currentAnimState.Schedule((s) =>
            {
                _currentAnimState = null;
                _onAnimComplete.ActivateTrigger(this, arg);
            });

            return true;
        }

        public bool Trigger(object sender, object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (!_useAsBlockingYieldInstruction || instruction == null) return this.Trigger(sender, arg);
            if (!this.CanTrigger) return false;

            var anim = _targetAnimator.GetTarget<ISPAnimationSource>(arg);
            if (anim == null) return false;

            var id = _clipIDs.PickRandom();
            _currentAnimState = anim.GetAnim(id);
            if (_currentAnimState == null)
            {
                //this.Invoke(() => { this.OnAnimCompleteCallback(null); }, 0f);
                if (_triggerCompleteIfNoAnim) this.Invoke(() => { _onAnimComplete.ActivateTrigger(this, arg); }, 0.01f);
                return false;
            }

            instruction.BeginBlock();
            _currentAnimState.TimeSupplier = _timeSupplier.TimeSupplier;
            _currentAnimState.Play(_speed, _startTime, QueueMode.PlayNow, _playMode);
            _currentAnimState.Schedule((s) =>
            {
                _currentAnimState = null;
                if (_daisyChainBlockingYieldInstruction)
                    _onAnimComplete.DaisyChainTriggerYielding(this, arg, instruction);
                else
                    _onAnimComplete.ActivateTrigger(this, arg);
                instruction.EndBlock();
            });

            return true;
        }

        #endregion

        #region IObservableTrigger Interface
        
        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onAnimComplete };
        }

        #endregion

    }
}
