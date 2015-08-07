using UnityEngine;

namespace com.spacepuppy.Hooks
{

    public delegate void OnControllerColliderHitCallback(object sender, ControllerColliderHit hit);

    public sealed class ControllerColliderHitEventHooks : MonoBehaviour
    {

        public event OnControllerColliderHitCallback ControllerColliderHit;

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (this.ControllerColliderHit != null) this.ControllerColliderHit(this, hit);
        }

        private void OnDestroy()
        {
            ControllerColliderHit = null;
        }

    }
}
