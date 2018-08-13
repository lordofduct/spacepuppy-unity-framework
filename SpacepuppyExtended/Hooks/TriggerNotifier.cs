using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Hooks
{

    [AddComponentMenu("SpacePuppy/Hooks/Trigger Notifier")]
    public class TriggerNotifier : SPNotifyingComponent
    {

        public bool NotifyRoot = true;

        void OnTriggerEnter(Collider c)
        {
            Notification.PostNotification<TriggerEnterNotification>(this, new TriggerEnterNotification(this.GetComponent<Collider>(), c), this.NotifyRoot);
        }

        void OnTriggerExit(Collider c)
        {
            Notification.PostNotification<TriggerExitNotification>(this, new TriggerExitNotification(this.GetComponent<Collider>(), c), this.NotifyRoot);
        }

    }

    [AddComponentMenu("SpacePuppy/Hooks/Trigger Stay Notifier")]
    public class TriggerStayNotifier : TriggerNotifier
    {

        void OnTriggerStay(Collider c)
        {
            Notification.PostNotification<TriggerStayNotification>(this, new TriggerStayNotification(this.GetComponent<Collider>(), c), this.NotifyRoot);
        }

    }
}
