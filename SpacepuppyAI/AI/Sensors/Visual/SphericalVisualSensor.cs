using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors.Visual
{
    public class SphericalVisualSensor : VisualSensor
    {

        #region Fields

        [FormerlySerializedAs("HorizontalAngle")]
        [Range(0f, 360f)]
        [SerializeField()]
        private float _horizontalAngle = 360f;
        [FormerlySerializedAs("VerticalAngle")]
        [Range(0f, 180f)]
        [SerializeField()]
        private float _verticalAngle = 180f;
        [FormerlySerializedAs("Range")]
        [MinRange(0f)]
        [SerializeField()]
        private float _radius = 5.0f;
        [Tooltip("An optional radius to carve out the center.")]
        [MinRange(0f)]
        [SerializeField()]
        private float _innerRadius = 0f;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float HorizontalAngle
        {
            get { return _horizontalAngle; }
            set { _horizontalAngle = Mathf.Clamp(value, 0, 360f); }
        }

        public float VerticalAngle
        {
            get { return _verticalAngle; }
            set { _verticalAngle = Mathf.Clamp(value, 0f, 180f); }
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public float InnerRadius
        {
            get { return _innerRadius; }
            set { _innerRadius = Mathf.Clamp(value, 0f, _radius); }
        }

        #endregion

        #region Methods

        protected override bool TestVisibility(VisualAspect aspect)
        {
            var sqrRadius = _radius * _radius;
            var v = aspect.transform.position - this.transform.position;
            if (v.sqrMagnitude > sqrRadius) return false;
            if (this._innerRadius > 0f && v.sqrMagnitude < this._innerRadius * this._innerRadius) return false;

            if(this._horizontalAngle != 360.0f && this._verticalAngle != 360.0f)
            {
                Vector3 directionOfAspectInLocalSpace = this.transform.InverseTransformDirection(v); //Quaternion.Inverse(this.transform.rotation) * v;
                //var lookAtAngles = (VectorUtil.NearZeroVector(directionOfAspectInLocalSpace)) ? Vector3.zero : Quaternion.LookRotation(directionOfAspectInLocalSpace).eulerAngles;
                //if (Mathf.Abs(lookAtAngles.y * 2.0f) > this._horizontalAngle)
                //    return false;
                //else if (Mathf.Abs(lookAtAngles.x * 2.0f) > this._verticalAngle)
                //    return false;

                float a;
                a = VectorUtil.AngleBetween(new Vector2(1f, 0f), new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.x));
                if (a > this._horizontalAngle)
                    return false;
                a = VectorUtil.AngleBetween(new Vector2(1f, 0f), new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.y));
                if (a > this._verticalAngle)
                    return false;
            }

            if(this.RequiresLineOfSight)
            {
                //RaycastHit[] hits = Physics.RaycastAll(this.transform.position, v, v.magnitude, this.LineOfSightMask);
                //foreach(var hit in hits)
                //{
                //    //we ignore ourself
                //    var r = hit.collider.FindRoot();
                //    if (r != aspect.entityRoot && r != this.entityRoot) return false;
                //}
                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<RaycastHit>())
                {
                    int cnt = PhysicsUtil.RaycastAll(this.transform.position, v, lst, v.magnitude, this.LineOfSightMask);
                    for(int i = 0; i < cnt; i++)
                    {
                        //we ignore ourself
                        var r = lst[i].collider.FindRoot();
                        if (r != aspect.entityRoot && r != this.entityRoot) return false;
                    }
                }
            }

            return true;
        }

        #endregion

    }
}
