#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Tween;

namespace com.spacepuppy.Waypoints
{

    /// <summary>
    /// TODO - remove obsolete deltaType
    /// </summary>
    public class i_MoveOnPath : AutoTriggerableMechanism, IObservableTrigger
    {

        public const string TRG_ONFINISH = "OnFinish";

        #region Fields
        
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Speed")]
        [Tooltip("Speed as units per second.")]
        private float _speed = 1.0f;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Ease")]
        private EaseStyle _ease;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("WrapMode")]
        private TweenWrapMode _wrapMode = TweenWrapMode.Once;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("WrapCount")]
        [Tooltip("How many time should the tween wrap. If WrapMode is 'Once', this is ignored.")]
        [DiscreteFloat.NonNegative()]
        private DiscreteFloat _wrapCount = DiscreteFloat.PositiveInfinity;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("UpdateType")]
        [Tooltip("Update should be used in most cases. FixedUpdate is useful if you need to animate physics. LateUpdate is useful for the camera.")]
        private UpdateSequence _updateType = UpdateSequence.Update;

        [SerializeField()]
        [Tooltip("What sort of delta time to use when updating. Normal should be used in most cases. Real is useful if gametime is every scaled and you want to ignore it. Smooth is for if you want the tween to be smooth over bad framerate.")]
        private SPTime _timeSupplier;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Reverse")]
        [Tooltip("Should we move down the path in a backwards direction.")]
        private bool _reverse;

        [SerializeField()]
        private AdvancedWaypointPathTweenCurve.TranslationOptions _updateTranslation = AdvancedWaypointPathTweenCurve.TranslationOptions.Position;

        [SerializeField()]
        private AdvancedWaypointPathTweenCurve.RotationOptions _updateRotation;

        [SerializeField()]
        [ReorderableArray()]
        [TypeReference.Config(typeof(com.spacepuppy.Dynamic.IStateModifier), allowAbstractClasses = false, allowInterfaces = false)]
        [Tooltip("Modifier types to search nodes for to also update by.")]
        private TypeReference[] _updateModifierTypes;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("StopMovementOnDisable")]
        [Tooltip("If this component is disabled, will stop any active tweens it created while this value was true.")]
        private bool _stopMovementOnDisable;

        [SerializeField()]
        private WaypointPathComponent _path;

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        private Trigger _onFinish = new Trigger(TRG_ONFINISH);

        [System.NonSerialized()]
        private HashSet<Tweener> _activeTweens;

        #endregion

        #region CONSTRUCTOR
        
        protected override void OnDisable()
        {
            base.OnDisable();

            if (this._stopMovementOnDisable && _activeTweens != null && _activeTweens.Count > 0)
            {
                var arr = _activeTweens.ToArray();
                _activeTweens.Clear();
                foreach (var tween in arr)
                {
                    tween.Stop();
                }
            }
        }

        #endregion

        #region Properties

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public EaseStyle Ease
        {
            get { return _ease; }
            set { _ease = value; }
        }

        public TweenWrapMode WrapMode
        {
            get { return _wrapMode; }
            set { _wrapMode = value; }
        }

        public DiscreteFloat WrapCount
        {
            get { return _wrapCount; }
            set { _wrapCount = (value < 0f) ? DiscreteFloat.Zero : value; }
        }

        public UpdateSequence UpdateType
        {
            get { return _updateType; }
            set { _updateType = value; }
        }

        public ITimeSupplier TimeSupplier
        {
            get { return _timeSupplier.TimeSupplier; }
            set { _timeSupplier.TimeSupplier = value; }
        }

        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }

        public AdvancedWaypointPathTweenCurve.TranslationOptions UpdateTranslation
        {
            get { return _updateTranslation; }
            set { _updateTranslation = value; }
        }

        public AdvancedWaypointPathTweenCurve.RotationOptions UpdateRotation
        {
            get { return _updateRotation; }
            set { _updateRotation = value; }
        }

        public bool StopMovementOnDisable
        {
            get { return _stopMovementOnDisable; }
            set { _stopMovementOnDisable = value; }
        }

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        #endregion

        #region Methods

        private void OnFinishHandler(object sender, System.EventArgs e)
        {
            _onFinish.ActivateTrigger(this, null);

            var tween = sender as Tweener;
            if (tween != null)
            {
                tween.OnFinish -= this.OnFinishHandler;
                if(_activeTweens != null) _activeTweens.Remove(tween);
            }
        }

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _path != null && this._speed > 0f;
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if(!this.CanTrigger) return false;

            var targ = _target.GetTarget<Transform>(arg);
            if(targ == null) return false;

            //var curve = new WaypointPathTweenCurve("position", EaseMethods.GetEase(this._ease), _path.Path.GetArcLength() / this._speed, _path.Path);
            var curve = new AdvancedWaypointPathTweenCurve(EaseMethods.GetEase(this._ease), _path.Path.GetArcLength() / this._speed, _path)
            {
                UpdateTranslation = _updateTranslation,
                UpdateRotation = _updateRotation
            };
            if (_updateModifierTypes != null && _updateModifierTypes.Length > 0)
            {
                for (int i = 0; i < _updateModifierTypes.Length; i++)
                {
                    var tp = _updateModifierTypes[i].Type;
                    if (tp != null) curve.AddNodeModifierType(tp);
                }
            }

            var tween = SPTween.Tween(targ)
                               .UseCurve(curve)
                               .Wrap(this._wrapMode, ((DiscreteFloat.IsInfinity(this._wrapCount)) ? 0 : Mathf.RoundToInt(this._wrapCount)))
                               .Use(this._updateType)
                               .Use(_timeSupplier.TimeSupplier)
                               .Reverse(this._reverse)
                               .OnFinish(this.OnFinishHandler)
                               .Play(false);
            if(this._stopMovementOnDisable)
            {
                if (_activeTweens == null) _activeTweens = new HashSet<Tweener>();
                _activeTweens.Add(tween);
            }

            return true;
        }

        #endregion

        #region IObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onFinish };
        }

        #endregion
        
    }

}
