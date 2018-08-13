using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Hooks
{

    [AddComponentMenu("SpacePuppy/Hooks/CharacterController Collision Notifier")]
    public class CharacterControllerCollisionNotifier : SPNotifyingComponent
    {

        public bool ManuallySet = false;
        public bool UseFixedUpdate = false;

        public bool NotifyThisRoot = true;

        public bool NotifyOther = false;
        public bool NotifyOtherRoot = false;

        private CharacterController _controller;
        private HashSet<Collider> _stayedColliders = new HashSet<Collider>();
        private HashSet<Collider> _enteredColliders = new HashSet<Collider>();

        protected override void Awake()
        {
 	         base.Awake();

            _controller = this.GetComponent<CharacterController>();
        }

        void Update()
        {
            if (ManuallySet || UseFixedUpdate) return;

            this.Resolve();
        }

        void FixedUpdate()
        {
            if (ManuallySet || !UseFixedUpdate) return;

            this.Resolve();
        }

        public void Resolve()
        {
            //TODO - reimplement this so that we have all the hit information
            var entered = _enteredColliders.Except(_stayedColliders).ToArray();
            var stayed = _stayedColliders.Intersect(_enteredColliders).ToArray();
            var exited = _stayedColliders.Except(_enteredColliders).ToArray();

            foreach (var c in entered)
            {
                _stayedColliders.Add(c);

                var n = new CharacterControllerCollisionEnterNotification(_controller, c);
                Notification.PostNotification<CharacterControllerCollisionEnterNotification>(this, n, this.NotifyThisRoot);
                if (this.NotifyOther) Notification.PostNotification<CharacterControllerCollisionEnterNotification>(c.gameObject, n, this.NotifyOtherRoot);
            }

            foreach (var c in stayed)
            {
                var n = new CharacterControllerCollisionStayNotification(_controller, c);
                Notification.PostNotification<CharacterControllerCollisionStayNotification>(this, n, this.NotifyThisRoot);
                if (this.NotifyOther) Notification.PostNotification<CharacterControllerCollisionStayNotification>(c.gameObject, n, this.NotifyOtherRoot);
            }

            foreach (var c in exited)
            {
                _stayedColliders.Remove(c);

                var n = new CharacterControllerCollisionExitNotification(_controller, c);
                Notification.PostNotification<CharacterControllerCollisionExitNotification>(this, n, this.NotifyThisRoot);
                if (this.NotifyOther) Notification.PostNotification<CharacterControllerCollisionExitNotification>(c.gameObject, n, this.NotifyOtherRoot);
            }

            _enteredColliders.Clear();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _enteredColliders.Add(hit.collider);
        }

    }

}
