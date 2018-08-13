using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Hooks
{
    public abstract class TriggerNotification : Notification
    {

        private Collider _hitbox;
        private Collider _other;

        public TriggerNotification(Collider hitbox, Collider other)
        {
            _hitbox = hitbox;
            _other = other;
        }

        public Collider HitBox { get { return _hitbox; } }
        public Collider Other { get { return _other; } }

    }

    public class TriggerEnterNotification : TriggerNotification
    {
        public TriggerEnterNotification(Collider hitbox, Collider other)
            : base(hitbox, other)
        {

        }
    }

    public class TriggerStayNotification : TriggerNotification
    {
        public TriggerStayNotification(Collider hitbox, Collider other)
            : base(hitbox, other)
        {

        }
    }

    public class TriggerExitNotification : TriggerNotification
    {
        public TriggerExitNotification(Collider hitbox, Collider other)
            : base(hitbox, other)
        {

        }
    }
}
