using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors.Visual
{

    public class RightCylindricalVisualSensor : VisualSensor
    {

        #region Fields

        [FormerlySerializedAs("Radius")]
        [MinRange(0f)]
        [SerializeField()]
        private float _radius = 1.0f;

        [Tooltip("An optional radius to carve out the center.")]
        [MinRange(0f)]
        [SerializeField()]
        private float _innerRadius = 0.0f;

        [FormerlySerializedAs("Height")]
        [MinRange(0f)]
        [SerializeField()]
        private float _height = 1.0f;

        [FormerlySerializedAs("Angle")]
        [Range(0f, 360f)]
        [SerializeField()]
        private float _angle = 360.0f;

        [Tooltip("Allows offsetting the cylinder, useful for creating a frustum like view. Line of sight is still from the position of the gameobject this is attached to.")]
        [SerializeField()]
        private Vector3 _center;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public Vector3 Rod { get { return this.transform.right; } }

        public float Radius
        {
            get { return _radius; }
            set { _radius = Mathf.Max(value, 0f); }
        }

        public float InnerRadius
        {
            get { return _innerRadius; }
            set { _innerRadius = Mathf.Clamp(value, 0f, _radius); }
        }

        public float Angle
        {
            get { return _angle; }
            set { _angle = Mathf.Clamp(value, 0f, 360f); }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = Mathf.Max(value, 0f);
            }
        }

        public Vector3 Center
        {
            get { return _center; }
            set { _center = value; }
        }

        #endregion

        #region Methods

        public Vector3 GetCenterInWorldSpace()
        {
            return this.transform.position + (this.transform.rotation * _center);
        }

        public override BoundingSphere GetBoundingSphere()
        {
            Vector3 pos = this.GetCenterInWorldSpace();
            double rad = (_height / 2d);
            rad = System.Math.Sqrt(rad * rad + (double)(_radius * _radius));

            return new BoundingSphere(pos, (float)rad);
        }

        protected override bool TestVisibility(VisualAspect aspect)
        {
            //if not in cylinder, can not see it
            var halfHeight = _height / 2.0f;
            var rod = this.Rod;
            var center = this.GetCenterInWorldSpace();
            var otherPos = aspect.transform.position;

            if (!Cylinder.ContainsPoint(center - (rod * halfHeight),
                                       center + (rod * halfHeight),
                                       _radius,
                                       _innerRadius,
                                       otherPos))
            {
                return false;
            }

            if(this._angle < 360.0f)
            {
                var v = VectorUtil.SetLengthOnAxis(otherPos - center, rod, 0f);
                var a = Vector3.Angle(this.transform.forward, v);
                if (a > this._angle / 2.0f) return false;
            }

            if (this.RequiresLineOfSight)
            {
                var v = otherPos - center;
                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<RaycastHit>())
                {
                    int cnt = PhysicsUtil.RaycastAll(this.transform.position, v, lst, v.magnitude, this.LineOfSightMask);
                    for (int i = 0; i < cnt; i++)
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
