using UnityEngine;

namespace com.spacepuppy
{
    public class CollisionEventArgs : System.EventArgs
    {
        public readonly Collision Collision;

        public CollisionEventArgs(Collision coll)
        {
            this.Collision = coll;
        }
    }

    public class TriggerEventArgs : System.EventArgs
    {
        public readonly Collider Collider;

        public TriggerEventArgs(Collider coll)
        {
            this.Collider = coll;
        }
    }

    public class ControllerColliderHitEventArgs : System.EventArgs
    {
        public readonly ControllerColliderHit Hit;

        public ControllerColliderHitEventArgs(ControllerColliderHit hit)
        {
            this.Hit = hit;
        }
    }
}
