using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Hooks
{
    public abstract class CharacterControllerCollisionNotification : Notification
    {

        private CharacterController _controller;
        private Collider _collider;

        public CharacterControllerCollisionNotification(CharacterController controller, Collider collider)
        {
            _controller = controller;
            _collider = collider;
        }

        public CharacterController Controller { get { return _controller; } }
        public Collider Collider { get { return _collider; } }

    }

    public class CharacterControllerCollisionEnterNotification : CharacterControllerCollisionNotification
    {

        public CharacterControllerCollisionEnterNotification(CharacterController controller, Collider collider)
            : base(controller, collider)
        {

        }

    }

    public class CharacterControllerCollisionStayNotification : CharacterControllerCollisionNotification
    {

        public CharacterControllerCollisionStayNotification(CharacterController controller, Collider collider)
            : base(controller, collider)
        {

        }

    }

    public class CharacterControllerCollisionExitNotification : CharacterControllerCollisionNotification
    {

        public CharacterControllerCollisionExitNotification(CharacterController controller, Collider collider)
            : base(controller, collider)
        {

        }

    }
}
