using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    public class ProxySpawnPointManager : SPComponent, ISpawnerModifier
    {

        #region Fields

        public int order;
        public bool BlockSpawnIfAllBusy = false;
        public bool WaitToSpawnIfAllBusy = true;


        [System.NonSerialized()]
        private NotificationPool _onReleaseBusyStatusPool = new NotificationPool();
        [System.NonSerialized()]
        private Queue<SpawnPointTriggeredNotification> _delayedQueue = new Queue<SpawnPointTriggeredNotification>();

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _onReleaseBusyStatusPool.RegisterNotificationType<ProxySpawnPoint.OnReleaseBusyStatus>();
            _onReleaseBusyStatusPool.OnNotification += this.OnReleaseBusyStatusHandler;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public IEnumerable<ProxySpawnPoint> GetAllProxies()
        {
            return this.GetChildComponents<ProxySpawnPoint>();
        }

        private void OnReleaseBusyStatusHandler(object sender, Notification n)
        {
            var sn = n as ProxySpawnPoint.OnReleaseBusyStatus;
            if (sn == null) return;

            while (_delayedQueue.Peek() == null) _delayedQueue.Dequeue(); //clear out any that have died
            if(_delayedQueue.Count > 0)
            {
                var delayedNotif = _delayedQueue.Dequeue();
                delayedNotif.SpawnedObject.SetActive(true);
                sn.SpawnPoint.PlaceOnSpawn(delayedNotif.SpawnPoint, delayedNotif.SpawnFactory, delayedNotif.SpawnedObject);
            }

            if(_delayedQueue.Count == 0)
            {
                _onReleaseBusyStatusPool.Clear();
            }
        }

        #endregion

        #region ISpawnerModifier Interface

        int ISpawnerModifier.order
        {
            get { return this.order; }
        }

        void ISpawnerModifier.OnBeforeSpawnNotification(SpawnPointBeforeSpawnNotification n)
        {
            var nodes = this.GetAllProxies().ToArray();

            if (nodes.Length == 0)
            {
                n.Cancelled = true;
                Debug.LogWarning("ProxySpawnPointManager is configured improperly, you must add child nodes.", this);
                return;
            }

            if (this.BlockSpawnIfAllBusy)
            {
                if (!nodes.Any((p) => !p.Busy))
                {
                    n.Cancelled = true;
                    return;
                }
            }
        }

        void ISpawnerModifier.OnSpawnedNotification(SpawnPointTriggeredNotification n)
        {
            if (this.WaitToSpawnIfAllBusy)
            {
                var nodes = this.GetAllProxies().ToArray();
                var node = (from p in nodes where !p.Busy select p).PickRandom();
                if (node != null)
                {
                    node.PlaceOnSpawn(n.SpawnPoint, n.SpawnFactory, n.SpawnedObject);
                }
                else
                {
                    n.SpawnedObject.SetActive(false);
                    _delayedQueue.Enqueue(n);
                    _onReleaseBusyStatusPool.AddRange(nodes);
                }
            }
            else
            {
                var node = this.GetAllProxies().PickRandom();
                if (node != null)
                {
                    node.PlaceOnSpawn(n.SpawnPoint, n.SpawnFactory, n.SpawnedObject);
                }
                else
                {
                    Debug.LogWarning("ProxySpawnPointManager is configured improperly, you must add child nodes.", this);
                }

            }
        }

        #endregion

    }
}
