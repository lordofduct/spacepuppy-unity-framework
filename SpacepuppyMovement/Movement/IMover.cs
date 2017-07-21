using UnityEngine;

namespace com.spacepuppy.Movement
{
    public interface IMover
    {

        bool InMotion { get; }
        Vector3 Velocity { get; }

    }
}
