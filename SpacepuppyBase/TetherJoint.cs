using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public class TetherJoint : SPComponent
    {

        public enum JointDampingStyle
        {
            Ease,
            Linear,
            Elastic
        }
        
        #region Fields

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("_target")]
        private Transform _linkTarget;

        [SerializeField]
        [EnumPopupExcluding(new int[] { (int)UpdateSequence.None })]
        private UpdateSequence _updateMode = UpdateSequence.Update;

        [SerializeField]
        private Constraint _constraint = Constraint.All;

        [SerializeField]
        private bool _doPositionDamping;
        [SerializeField]
        private JointDampingStyle _positionDampingStyle;
        [SerializeField]
        private float _positionDampingStrength;
        [SerializeField]
        private bool _doRotationDamping;
        [SerializeField]
        private JointDampingStyle _rotationDampingStyle;
        [SerializeField]
        private float _rotationDampingStrength;


        [System.NonSerialized]
        private RadicalCoroutine _updateRoutine;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            base.Start();

            _updateRoutine = this.StartRadicalCoroutine(this.DoUpdateRoutine(), RadicalCoroutineDisableMode.Pauses);
        }

        #endregion

        #region Properties

        public Transform LinkTarget
        {
            get { return _linkTarget; }
            set { _linkTarget = value; }
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

        #region Methods

        private System.Collections.IEnumerator DoUpdateRoutine()
        {
            while(true)
            {
                if(_linkTarget != null)
                {
                    Constraint c;
                    var trans = this.transform;

                    //constrain position
                    c = (_constraint & Constraint.Position);
                    if (c == Constraint.Position)
                    {
                        if (_doPositionDamping)
                        {
                            switch (_positionDampingStyle)
                            {
                                case JointDampingStyle.Ease:
                                    trans.position = Vector3.Lerp(trans.position, _linkTarget.position, Time.deltaTime * _positionDampingStrength);
                                    break;
                                case JointDampingStyle.Linear:
                                    trans.position = Vector3.MoveTowards(trans.position, _linkTarget.position, Time.deltaTime * _positionDampingStrength);
                                    break;
                                case JointDampingStyle.Elastic:
                                    {
                                        var tpos = _linkTarget.position;
                                        var pos = trans.position;
                                        if (_positionDampingStrength < 0.0001f)
                                        {
                                            trans.position = tpos;
                                            break;
                                        }

                                        float t = 1f - Mathf.Clamp01(1f / _positionDampingStrength) * 0.99f;
                                        tpos += (tpos - pos);
                                        trans.position = Vector3.LerpUnclamped(pos, tpos, t);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            trans.position = _linkTarget.position;
                        }
                    }
                    else if(c != 0)
                    {
                        Vector3 pos = trans.position;
                        Vector3 tpos = _linkTarget.position;

                        if ((_constraint & Constraint.XPosition) != 0)
                        {
                            if (_doPositionDamping)
                                pos.x = DampFloat(pos.x, tpos.x, Time.deltaTime, _positionDampingStrength, _positionDampingStyle);
                            else
                                pos.x = tpos.x;
                        }
                        if ((_constraint & Constraint.YPosition) != 0)
                        {
                            if (_doPositionDamping)
                                pos.y = DampFloat(pos.y, tpos.y, Time.deltaTime, _positionDampingStrength, _positionDampingStyle);
                            else
                                pos.y = tpos.y;
                        }
                        if ((_constraint & Constraint.ZPosition) != 0)
                        {
                            if (_doPositionDamping)
                                pos.z = DampFloat(pos.z, tpos.z, Time.deltaTime, _positionDampingStrength, _positionDampingStyle);
                            else
                                pos.z = tpos.z;
                        }

                        trans.position = pos;
                    }

                    //constrain rotation
                    c = (_constraint & Constraint.Rotation);
                    if (c == Constraint.Rotation)
                    {
                        if (_doRotationDamping)
                        {
                            switch (_positionDampingStyle)
                            {
                                case JointDampingStyle.Ease:
                                    trans.rotation = Quaternion.Slerp(trans.rotation, _linkTarget.rotation, Time.deltaTime * _positionDampingStrength);
                                    break;
                                case JointDampingStyle.Linear:
                                    trans.rotation = Quaternion.RotateTowards(trans.rotation, _linkTarget.rotation, Time.deltaTime * _positionDampingStrength);
                                    break;
                                case JointDampingStyle.Elastic:
                                    {
                                        var tpos = _linkTarget.rotation;
                                        var pos = trans.rotation;
                                        if (_positionDampingStrength < 0.0001f)
                                        {
                                            trans.rotation = tpos;
                                            break;
                                        }

                                        float t = 1f - Mathf.Clamp01(1f / _positionDampingStrength) * 0.99f;
                                        tpos *= QuaternionUtil.FromToRotation(pos, tpos);
                                        trans.rotation = Quaternion.SlerpUnclamped(pos, tpos, t);
                                    }
                                    break;
                            }
                        }
                        else
                            trans.rotation = _linkTarget.rotation;
                    }
                    else if(c != 0)
                    {
                        var rot = trans.eulerAngles;
                        var targRot = _linkTarget.eulerAngles;
                        if ((_constraint & Constraint.XRotation) != 0)
                        {
                            if (_doRotationDamping)
                                rot.x = DampFloat(rot.x, targRot.x, Time.deltaTime, _rotationDampingStrength, _rotationDampingStyle);
                            else
                                rot.x = targRot.x;
                        }
                        if ((_constraint & Constraint.YRotation) != 0)
                        {
                            if (_doRotationDamping)
                                rot.y = DampFloat(rot.y, targRot.y, Time.deltaTime, _rotationDampingStrength, _rotationDampingStyle);
                            else
                                rot.y = targRot.y;
                        }
                        if ((_constraint & Constraint.ZRotation) != 0)
                        {
                            if (_doRotationDamping)
                                rot.z = DampFloat(rot.z, targRot.z, Time.deltaTime, _rotationDampingStrength, _rotationDampingStyle);
                            else
                                rot.z = targRot.z;
                        }
                        trans.eulerAngles = rot;
                    }
                }
                
                switch(_updateMode)
                {
                    case UpdateSequence.None:
                    case UpdateSequence.Update:
                        yield return null;
                        break;
                    case UpdateSequence.FixedUpdate:
                        yield return RadicalCoroutine.WaitForFixedUpdate;
                        break;
                    case UpdateSequence.LateUpdate:
                        yield return WaitForLateUpdate.Create();
                        break;
                }
            }
        }


        private static float DampFloat(float a, float b, float dt, float strength, JointDampingStyle style)
        {
            switch(style)
            {
                case JointDampingStyle.Ease:
                    return Mathf.Lerp(a, b, dt * strength);
                case JointDampingStyle.Linear:
                    return Mathf.MoveTowards(a, b, dt * strength);
                case JointDampingStyle.Elastic:
                    {
                        if (strength < 0.0001f) return b;

                        float t = 1f - Mathf.Clamp01(1f / strength) * 0.99f;
                        b += (b - a);
                        return Mathf.LerpUnclamped(a, b, t);
                    }
                default:
                    return b;
            }
        }

        #endregion

    }

}
