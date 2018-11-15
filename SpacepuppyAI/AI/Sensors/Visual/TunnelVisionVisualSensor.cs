using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors.Visual
{

    public class TunnelVisionVisualSensor : VisualSensor
    {

        #region Fields

        [FormerlySerializedAs("Range")]
        [MinRange(0f)]
        [SerializeField()]
        private float _range = 5.0f;
        [FormerlySerializedAs("Radius")]
        [MinRange(0f)]
        [SerializeField()]
        private float _startRadius = 1.0f;
        [MinRange(0f)]
        [SerializeField]
        private float _endRadius = 1.0f;
        
        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float Range
        {
            get { return _range; }
            set { _range = Mathf.Max(value, 0f); }
        }

        public float StartRadius
        {
            get { return _startRadius; }
            set { _startRadius = Mathf.Max(value, 0f); }
        }

        public float EndRadius
        {
            get { return _endRadius; }
            set { _endRadius = Mathf.Max(value, 0f); }
        }

        public Vector3 Direction { get { return this.transform.forward; } }

        #endregion

        #region Methods

        public override BoundingSphere GetBoundingSphere()
        {
            var pos = this.transform.position + this.Direction * this.Range * 0.5f;

            double r1 = this.Range / 2d;
            double r2 = System.Math.Max(this.StartRadius, this.EndRadius);
            return new BoundingSphere(pos, (float)System.Math.Sqrt(r1 * r1 + r2 * r2));
        }

        protected override bool TestVisibility(VisualAspect aspect)
        {
            //if not in cylinder, can not see it
            float aspRad = aspect.Radius;
            if (aspRad > MathUtil.EPSILON)
            {
                if (!Cone.ContainsSphere(this.transform.position,
                                       this.transform.position + this.Direction * _range,
                                       _startRadius,
                                       _endRadius,
                                       aspect.transform.position,
                                       aspRad))
                {
                    return false;
                }
            }
            else
            {
                if (!Cone.ContainsPoint(this.transform.position,
                                       this.transform.position + this.Direction * _range,
                                       _startRadius,
                                       _endRadius,
                                       aspect.transform.position))
                {
                    return false;
                }
            }

            if (this.RequiresLineOfSight)
            {
                var v = aspect.transform.position - this.transform.position;
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