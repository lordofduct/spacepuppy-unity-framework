#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace com.spacepuppy.Scenario
{
    public class i_PostNotification : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TypeReference.Config(typeof(Notification))]
        private TypeReference _notification;

        [System.NonSerialized()]
        private Notification _n;

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if(_n == null || _n.GetType() != _notification.Type)
            {
                try
                {
                    _n = System.Activator.CreateInstance(_notification.Type) as Notification;
                    if (_n == null) return false;
                }
                catch
                {
                    return false;
                }
            }

            Notification.UnsafePostNotification(this, _n);

            return true;
        }

        #endregion

    }
}
