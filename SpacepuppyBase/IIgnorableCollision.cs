using UnityEngine;

namespace com.spacepuppy
{
    public interface IIgnorableCollision
    {

        void IgnoreCollision(Collider coll, bool ignore);
        void IgnoreCollision(IIgnorableCollision coll, bool ignore);

    }
}
