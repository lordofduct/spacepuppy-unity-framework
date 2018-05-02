#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_StartTetherJoint : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(Transform))]
        [Tooltip("Target that gets tethered to 'Link Target'.")]
        private TriggerableTargetObject _target;

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(Transform))]
        [Tooltip("Target to be tethered to.")]
        private TriggerableTargetObject _linkTarget;

        [SerializeField]
        [EnumPopupExcluding(new int[] { (int)UpdateSequence.None })]
        private UpdateSequence _updateMode = UpdateSequence.Update;

        [SerializeField]
        private Constraint _constraint = Constraint.All;

        [SerializeField]
        private bool _doPositionDamping;
        [SerializeField]
        private TetherJoint.JointDampingStyle _positionDampingStyle;
        [SerializeField]
        private float _positionDampingStrength;
        [SerializeField]
        private bool _doRotationDamping;
        [SerializeField]
        private TetherJoint.JointDampingStyle _rotationDampingStyle;
        [SerializeField]
        private float _rotationDampingStrength;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public TriggerableTargetObject LinkTarget
        {
            get { return _linkTarget; }
        }

        public UpdateSequence UpdateMode
        {
            get { return _updateMode; }
            set { _updateMode = value; }
        }

        public Constraint Constraint
        {
            get { return _constraint; }
            set { _constraint = value; }
        }

        public bool DoPositionDamping
        {
            get { return _doPositionDamping; }
            set { _doPositionDamping = value; }
        }

        public TetherJoint.JointDampingStyle PositionDampingStyle
        {
            get { return _positionDampingStyle; }
            set { _positionDampingStyle = value; }
        }

        public float PositionDampingStrength
        {
            get { return _positionDampingStrength; }
            set { _positionDampingStrength = value; }
        }

        public bool DoRotationDamping
        {
            get { return _doRotationDamping; }
            set { _doRotationDamping = value; }
        }

        public TetherJoint.JointDampingStyle RotationDampingStyle
        {
            get { return _rotationDampingStyle; }
            set { _rotationDampingStyle = value; }
        }

        public float RotationDampingStrength
        {
            get { return _rotationDampingStrength; }
            set { _rotationDampingStrength = value; }
        }

        #endregion

        #region Triggerable Mechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<Transform>(arg);
            if (targ == null) return false;

            var link = _linkTarget.GetTarget<Transform>(arg);
            if (link == null) return false;

            var tether = targ.AddOrGetComponent<TetherJoint>();
            tether.LinkTarget = link;
            tether.UpdateMode = _updateMode;
            tether.Constraint = _constraint;
            tether.DoPositionDamping = _doPositionDamping;
            tether.PositionDampingStyle = _positionDampingStyle;
            tether.PositionDampingStrength = _positionDampingStrength;
            tether.DoRotationDamping = _doRotationDamping;
            tether.RotationDampingStyle = _rotationDampingStyle;
            tether.RotationDampingStrength = _rotationDampingStrength;
            tether.enabled = true;

            return true;
        }

        #endregion

    }
}
