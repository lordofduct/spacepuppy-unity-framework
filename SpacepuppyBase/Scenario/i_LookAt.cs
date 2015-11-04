using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_LookAt : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Transform))]
        [UnityEngine.Serialization.FormerlySerializedAs("Observer")]
        private TriggerableTargetObject _observer = new TriggerableTargetObject();
        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Transform))]
        [UnityEngine.Serialization.FormerlySerializedAs("Target")]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [Tooltip("The axis to rotate around.")]
        [UnityEngine.Serialization.FormerlySerializedAs("Axis")]
        private CartesianAxis _axis = CartesianAxis.Y;
        [SerializeField()]
        [Tooltip("Rotate around the axis as defined on the transform this is attache to. Otherwise rotate around the axis in global terms.")]
        [UnityEngine.Serialization.FormerlySerializedAs("AxisIsRelative")]
        private bool _axisIsRelative;
        [SerializeField()]
        [Tooltip("Flatten the look direction on the defined axis.")]
        [UnityEngine.Serialization.FormerlySerializedAs("FlattenOnAxis")]
        private bool _flattenOnAxis = true;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Slerp")]
        private bool _slerp;
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("SlerpAngularSpeed")]
        private float _slerpAngularSpeed = 180f;

        #endregion

        #region Properties

        public Transform Observer
        {
            get
            {
                return _observer.GetTarget<Transform>(null);
            }
            set
            {
                _observer.SetTarget(value);
            }
        }

        public Transform Target
        {
            get
            {
                return _observer.GetTarget<Transform>(null);
            }
            set
            {
                _observer.SetTarget(value);
            }
        }

        /// <summary>
        /// Axis of the transform to use for rotating around.
        /// </summary>
        public CartesianAxis Axis
        {
            get { return _axis; }
            set { _axis = value; }
        }

        /// <summary>
        /// Rotate around the axis as defined on teh transform that is attached to. Otherwise rotate around the axis in global terms.
        /// </summary>
        public bool AxisIsRelative
        {
            get { return _axisIsRelative; }
            set { _axisIsRelative = value; }
        }

        /// <summary>
        /// Flatten the look direction on the defined axis.
        /// </summary>
        public bool FlattenOnAxis
        {
            get { return _flattenOnAxis; }
            set { _flattenOnAxis = value; }
        }

        /// <summary>
        /// Should the lookat interpolation be spherical.
        /// </summary>
        public bool Slerp
        {
            get { return _slerp; }
            set { _slerp = value; }
        }

        /// <summary>
        /// If Slerp is true, the speed at which the rotation is slerped.
        /// </summary>
        public float SlerpAngularSpeed
        {
            get { return _slerpAngularSpeed; }
            set { _slerpAngularSpeed = value; }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var observer = this._observer.GetTarget<Transform>(arg);
            if (observer == null) return false;
            var targ = this._target.GetTarget<Transform>(arg);
            if (targ == null) return false;

            var dir = targ.position - observer.position;
            var ax = (this._axisIsRelative) ? this.transform.GetAxis(this._axis) : TransformUtil.GetAxis(this._axis);
            if (this._flattenOnAxis) dir = dir.SetLengthOnAxis(ax, 0f);
            var q = Quaternion.LookRotation(dir, ax);

            if (this._slerp)
            {
                observer.rotation = QuaternionUtil.SpeedSlerp(observer.rotation, q, this._slerpAngularSpeed, Time.deltaTime);
            }
            else
            {
                observer.rotation = q;
            }
            return true;
        }

        #endregion

    }

}