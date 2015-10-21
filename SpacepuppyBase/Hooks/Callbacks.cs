using UnityEngine;

namespace com.spacepuppy.Hooks
{

    public delegate void OnCollisionCallback(GameObject sender, Collision collision);

    public delegate void OnTriggerCallback(GameObject sender, Collider otherCollider);

    public delegate void OnStrikeCallback(GameObject sender, Collider otherCollider);

}
