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

        public override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(this.transform.position, _radius);
        }

        protected override bool TestVisibility(VisualAspect aspect)
        {
            float aspRad = aspect.Radius;
            float sqrRadius = _radius * _radius;
            Vector3 v = aspect.transform.position - this.transform.position;
            float sqrDist = v.sqrMagnitude;

            if (sqrDist - (aspRad * aspRad) > sqrRadius) return false;
            if (this._innerRadius > aspRad && sqrDist < this._innerRadius * this._innerRadius) return false;

            if(this._horizontalAngle < 360.0f && this._verticalAngle < 360.0f)
            {
                Vector3 directionOfAspectInLocalSpace = this.transform.InverseTransformDirection(v); //Quaternion.Inverse(this.transform.rotation) * v;
                float a;
                if (aspRad > MathUtil.EPSILON)
                {
                    float k = 2f * Mathf.Asin(aspRad / (Mathf.Sqrt(sqrDist + (aspRad * aspRad) / 4f))) * Mathf.Rad2Deg;
                    a = VectorUtil.AngleBetween(new Vector2(1f, 0f), new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.x));
                    
                    if (a > (this._horizontalAngle / 2f) - k)
                        return false;
                    a = VectorUtil.AngleBetween(new Vector2(1f, 0f), new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.y));
                    if (a > (this._verticalAngle / 2f) - k)
                        return false;
                }
                else
                {
                    a = VectorUtil.AngleBetween(new Vector2(1f, 0f), new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.x));
                    if (a > this._horizontalAngle / 2f)
                        return false;
                    a = VectorUtil.AngleBetween(new Vector2(1f, 0f), new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.y));
                    if (a > this._verticalAngle / 2f)
                        return false;
                }

            }

            if(this.RequiresLineOfSight)
            {
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
