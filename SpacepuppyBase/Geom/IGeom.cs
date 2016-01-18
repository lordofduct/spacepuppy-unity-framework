using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    public interface IGeom
    {

        void Move(Vector3 mv);

        AxisInterval Project(Vector3 axis);
        Bounds GetBounds();
        Sphere GetBoundingSphere();
        IEnumerable<Vector3> GetAxes();

        bool Contains(Vector3 pos);

    }

    public interface IPhysicsGeom : IGeom
    {

        bool TestOverlap(int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal);
        int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal);
        bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal);
        int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal);

    }

}
