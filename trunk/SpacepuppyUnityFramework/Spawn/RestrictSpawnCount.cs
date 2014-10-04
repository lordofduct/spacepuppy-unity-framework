using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Spawn
{
    [RequireComponent(typeof(SpawnPoint))]
    public class RestrictSpawnCount : OnSpawnModifier
    {

        #region Fields

        public int MaxCount = 1;

        [System.NonSerialized()]
        private int _currentCount = 0;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Notification Handlers

        protected internal override void OnBeforeSpawnNotification(SpawnPointBeforeSpawnNotification n)
        {
            if (_currentCount >= this.MaxCount) n.Cancelled = true;
        }

        protected internal override void OnSpawnedNotification(SpawnPointTriggeredNotification n)
        {
            _currentCount++;
        }

        #endregion

    }
}
