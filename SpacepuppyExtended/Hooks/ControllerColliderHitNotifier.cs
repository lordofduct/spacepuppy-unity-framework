using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Hooks
{

    [AddComponentMenu("SpacePuppy/Hooks/CharacterControllerHit Notifier")]
    public class ControllerColliderHitNotifier : SPNotifyingComponent
    {

        public bool NotifyThisRoot = true;

        public bool NotifyOther = false;
        public bool NotifyOtherRoot = false;

        protected override void Awake()
        {
            base.Awake();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!(this as Component).IsEnabled()) return;

            var n = new ControllerColliderHitNotification(hit);
            Notification.PostNotification<ControllerColliderHitNotification>(this, n, this.NotifyThisRoot);
            if (this.NotifyOther)
            {
                Notification.PostNotification<ControllerColliderHitNotification>(hit.gameObject, n, this.NotifyOtherRoot);
            }
        }

    }
}
