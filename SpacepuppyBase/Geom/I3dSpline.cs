using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    public interface I3dSpline : IEnumerable<Vector3>
    {

        int Count { get; }
        Vector3 ControlPoint(int index);

        void AddControlPoint(Vector3 w);
        void MoveControlPoint(int index, Vector3 w);
        void RemoveControlPoint(int index);

        float GetArcLength();
        Vector3 GetPosition(float t);
        Vector3 GetPositionAfter(int index, float t);

    }
}
