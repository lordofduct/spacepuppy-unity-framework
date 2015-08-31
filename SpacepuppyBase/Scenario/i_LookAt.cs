using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_LookAt : TriggerableMechanism
    {

        #region Fields

        [TriggerableTargetObject.Config(typeof(UnityEngine.Transform))]
        public TriggerableTargetObject Observer;
        [TriggerableTargetObject.Config(typeof(UnityEngine.Transform))]
        public TriggerableTargetObject Target;

        [Tooltip("The axis to rotate around.")]
        private CartesianAxis Axis = CartesianAxis.Y;
        [Tooltip("Rotate around the axis as defined on the transform this is attache to. Otherwise rotate around the axis in global terms.")]
        public bool AxisIsRelative;
        [Tooltip("Flatten the look direction on the defined axis.")]
        public bool FlattenOnAxis = true;

        public bool Slerp;
        public float SlerpAngularSpeed = 180f;

        #endregion

        #region Properties

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var observer = this.Observer.GetTarget<Transform>(arg);
            if (observer == null) return false;
            var targ = this.Target.GetTarget<Transform>(arg);
            if (targ == null) return false;

            var dir = targ.position - observer.position;
            var ax = (this.AxisIsRelative) ? this.transform.GetAxis(this.Axis) : TransformUtil.GetAxis(this.Axis);
            if (this.FlattenOnAxis) dir = dir.SetLengthOnAxis(ax, 0f);
            var q = Quaternion.LookRotation(dir, ax);

            if (this.Slerp)
            {
                observer.rotation = QuaternionUtil.SpeedSlerp(observer.rotation, q, this.SlerpAngularSpeed, Time.deltaTime);
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