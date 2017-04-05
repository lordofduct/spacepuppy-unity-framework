using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Spawn
{

    public class ProxySpawnPoint : SPNotifyingComponent
    {

        #region Fields

        public bool FlagBusyOnSpawn = true;

        [System.NonSerialized()]
        private bool _busy;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public bool Busy { get { return _busy; } }

        #endregion

        #region Methods

        public void ReleaseBusyStatus()
        {
            _busy = false;

            Notification.PostNotification<OnReleaseBusyStatus>(this, new OnReleaseBusyStatus(this), false);
        }

        internal void PlaceOnSpawn(ISpawner spawnPoint, ISpawnFactory spawnPool, GameObject spawnedObject)
        {
            if (_busy) return;
            if (this.FlagBusyOnSpawn) _busy = true;

            spawnedObject.transform.position = this.transform.position;
            spawnedObject.transform.rotation = this.transform.rotation;

            var n = SpawnPointTriggeredNotification.Create(spawnPool, spawnedObject, spawnPoint);
            Notification.PostNotification<SpawnPointTriggeredNotification>(this, n, false);
            Notification.Release(n);
        }

        #endregion

        #region Notification Types

        public class OnReleaseBusyStatus : Notification
        {

            private ProxySpawnPoint _spawnPoint;

            public OnReleaseBusyStatus(ProxySpawnPoint spawnPoint)
            {
                _spawnPoint = spawnPoint;
            }

            public ProxySpawnPoint SpawnPoint { get { return _spawnPoint; } }

        }

        #endregion

    }

}
