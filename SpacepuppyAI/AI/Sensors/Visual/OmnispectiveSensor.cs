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
        
        protected override bool TestVisibility(VisualAspect aspect)
        {
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

    }

}
