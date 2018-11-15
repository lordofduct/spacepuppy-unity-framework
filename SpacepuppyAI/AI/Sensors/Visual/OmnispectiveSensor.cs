using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.AI.Sensors.Visual
{

    public class OmnispectiveSensor : VisualSensor
    {

        public override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Vector3.zero, float.PositiveInfinity);
        }

        public override bool SenseAny(System.Func<IAspect, bool> p = null)
        {
            return VisualAspect.Pool.Any(this.GetPredicate(p));
        }

        public override IAspect Sense(System.Func<IAspect, bool> p = null)
        {
            p = this.GetPredicate(p);
            return VisualAspect.Pool.Find(p);
        }

        public override IEnumerable<IAspect> SenseAll(System.Func<IAspect, bool> p = null)
        {
            p = this.GetPredicate(p);
            foreach (var a in VisualAspect.Pool)
            {
                if (p(a)) yield return a;
            }
        }

        public override int SenseAll(ICollection<IAspect> results, System.Func<IAspect, bool> p = null)
        {
            p = this.GetPredicate(p);
            return VisualAspect.Pool.FindAll(results, p);
        }

        public override int SenseAll<T>(ICollection<T> results, System.Func<T, bool> p = null)
        {
            System.Func<T, bool> p2;
            if (p == null)
                p2 = (a) => this.Visible(a);
            else
                p2 = (a) => this.Visible(a) && p(a);

            return VisualAspect.Pool.FindAll<T>(results, p2);
        }

        protected override bool TestVisibility(VisualAspect aspect)
        {
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

    }

}
