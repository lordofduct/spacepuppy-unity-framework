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
        private float _radius = 1.0f;
        
        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float Range
        {
            get { return _range; }
            set { _range = Mathf.Max(value, 0f); }
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = Mathf.Max(value, 0f); }
        }

        public Vector3 Direction { get { return this.transform.forward; } }

        #endregion

        #region Methods

        protected override bool TestVisibility(VisualAspect aspect)
        {
            //if not in cylinder, can not see it
            if(!Cylinder.ContainsPoint(this.transform.position,
                                       this.transform.position + this.Direction * _range,
                                       _radius, 
                                       aspect.transform.position))
            {
                return false;
            }

            if (this.RequiresLineOfSight)
            {
                var v = aspect.transform.position - this.transform.position;
                //RaycastHit[] hits = Physics.RaycastAll(this.transform.position, v, v.magnitude, this.LineOfSightMask);
                //foreach (var hit in hits)
                //{
                //    //we ignore ourself
                //    var r = hit.collider.FindRoot();
                //    if (r != aspect.entityRoot && r != this.entityRoot) return false;
                //}
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