using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Hooks
{
    public class ControllerColliderHitNotification : Notification
    {

        private ControllerColliderHit _hit;

        public ControllerColliderHitNotification(ControllerColliderHit hit)
        {
            _hit = hit;
        }

        public ControllerColliderHit Hit { get { return _hit; } }
        public CharacterController Controller { get { return _hit.controller; } }
        public Collider Collider { get { return _hit.collider; } }

    }
}
