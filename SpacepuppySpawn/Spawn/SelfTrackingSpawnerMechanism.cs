using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [System.Serializable()]
    public class SelfTrackingSpawnerMechanism
    {

        #region Fields

        [SerializeField()]
        [Tooltip("If left empty the default SpawnPool will be used instead.")]
        private SpawnPool _spawnPool;

        [System.NonSerialized()]
        private ISpawner _spawnPoint;
        [System.NonSerialized()]
        private bool _active;

        [System.NonSerialized()]
        private NotificationPool _spawnedObjects = new NotificationPool();

        [System.NonSerialized()]
        private int _totalCount;

        [System.NonSerialized]
        private BinaryHeap<ISpawnerModifier> _modifiers;

        #endregion

        #region CONSTRUCTOR
        
        public void Init(ISpawner owner, bool active = false)
        {
            if (!object.ReferenceEquals(_spawnPoint, null)) throw new System.InvalidOperationException("SpawnerMechanism has already been initialized.");

            _spawnedObjects.RegisterNotificationType<DeSpawnNotification>();
            _spawnedObjects.RegisterNotificationType<SpawnPoolPurgedNotification>();

            _spawnPoint = owner;
            this.Active = active;
        }

        #endregion

        #region Properties

        public bool UsesDefaultSpawnPool
        {
            get { return object.ReferenceEquals(_spawnPool, null) || object.ReferenceEquals(_spawnPool, SpawnPool.DefaultPool); }
        }

        public SpawnPool SpawnPool
        {
            get
            {
                if (_spawnPool == null) _spawnPool = SpawnPool.DefaultPool;
                return _spawnPool;
            }
            set { _spawnPool = value; }
        }

        public int ActiveCount { get { return _spawnedObjects.Count; } }

        public int TotalCount { get { return _totalCount; } }

        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active == value) return;
                _active = value;
                if(_active)
                {
                    _spawnedObjects.OnNotification += this.OnSpawnedObjectNotification;
                }
                else
                {
                    _spawnedObjects.OnNotification -= this.OnSpawnedObjectNotification;
                }
            }
        }

        #endregion

        #region Methods

        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            var controller = this.SpawnAsController(prefab, pos, rot, par, initializeProperties);
            return (controller != null) ? controller.gameObject : null;
        }
        
        public SpawnedObjectController SpawnAsController(GameObject prefab, Vector3 pos, Quaternion rot, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            if (_spawnPoint == null) throw new System.InvalidOperationException("SpawnerMechanism must be initialized before calling Spawn.");
            if (!_active) return null;
            if (prefab == null) return null;

            /*
            using (var modifiers = TempCollection.GetList<ISpawnerModifier>())
            {
                SpawnPointHelper.GetSpawnModifiers(_spawnPoint, modifiers);

                if(SpawnPointHelper.SignalOnBeforeSpawnNotification(_spawnPoint, prefab, modifiers))
                {
                    return null;
                }

                //perform actual spawn
                var controller = this.SpawnPool.SpawnAsController(prefab, pos, rot, par, initializeProperties, _spawnPoint);
                if (controller == null) return null;
                _spawnedObjects.Add(controller);
                _totalCount++;
                //end actual spawn

                SpawnPointHelper.SignalOnSpawnedNotification(_spawnPoint, this.SpawnPool, controller.gameObject, modifiers);

                return controller;
            }
            */

            //on before spawn
            var beforeNotif = SpawnPointBeforeSpawnNotification.Create(_spawnPoint, prefab);
            if (_modifiers != null && _modifiers.Count > 0)
            {
                var e = _modifiers.GetEnumerator();
                while(e.MoveNext())
                {
                    e.Current.OnBeforeSpawnNotification(beforeNotif);
                }
            }
            Notification.PostNotification<SpawnPointBeforeSpawnNotification>(_spawnPoint, beforeNotif, false);
            Notification.Release(beforeNotif);

            if (beforeNotif.Cancelled)
                return null;

            //perform actual spawn
            var controller = this.SpawnPool.SpawnAsController(prefab, pos, rot, par, initializeProperties, _spawnPoint);
            if (controller == null) return null;
            _spawnedObjects.Add(controller);
            _totalCount++;
            //end actual spawn

            //on post spawn
            var spawnNotif = SpawnPointTriggeredNotification.Create(this.SpawnPool, controller.gameObject, _spawnPoint);
            if (_modifiers != null && _modifiers.Count > 0)
            {
                var e = _modifiers.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.OnSpawnedNotification(spawnNotif);
                }
            }
            Notification.PostNotification<SpawnPointTriggeredNotification>(_spawnPoint, spawnNotif, false);
            Notification.Release(spawnNotif);

            return controller;
        }

        public GameObject[] GetActiveGameObjects()
        {
            return (from c in _spawnedObjects where c is SpawnedObjectController select (c as SpawnedObjectController).gameObject).ToArray();
        }

        public SpawnedObjectController[] GetActiveObjectControllers()
        {
            return (from c in _spawnedObjects where c is SpawnedObjectController select c as SpawnedObjectController).ToArray();
        }


        public void RegisterModifier(ISpawnerModifier modifier)
        {
            if (modifier == null) throw new System.ArgumentNullException("modifier");
            if(_modifiers == null)
            {
                _modifiers = new BinaryHeap<ISpawnerModifier>(SpawnerModifierComparer.Default);
            }

            if(!_modifiers.Contains(modifier))
                _modifiers.Add(modifier);
        }

        public bool UnRegisterModifier(ISpawnerModifier modifier)
        {
            if (_modifiers == null) return false;
            if (modifier == null) return false;

            return _modifiers.Remove(modifier);
        }

        #endregion

        #region Notification handlers

        private void OnSpawnedObjectNotification(object sender, Notification n)
        {
            if(n is DeSpawnNotification)
            {
                _spawnedObjects.Remove((n as DeSpawnNotification).Controller);
            }
            else if(n is SpawnPoolPurgedNotification)
            {
                _spawnedObjects.Remove((n as SpawnPoolPurgedNotification).Controller);
            }
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {
            private string _label;

            public ConfigAttribute(string label)
            {
                _label = label;
            }

            public string Label { get { return _label; } }
        }

        #endregion

    }
}
