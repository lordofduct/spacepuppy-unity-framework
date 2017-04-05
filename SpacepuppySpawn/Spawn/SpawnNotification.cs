using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace com.spacepuppy.Spawn
{

    public class SpawnNotification : Notification
    {

        #region Fields

        protected ISpawnFactory _factory;
        protected GameObject _spawnedObject;
        protected ISpawner _spawnPoint;

        #endregion

        #region CONSTRUCTOR

        public SpawnNotification(ISpawnFactory factory, GameObject spawnedObject)
        {
            if (factory == null) throw new System.ArgumentNullException("factory");
            if (spawnedObject == null) throw new System.ArgumentNullException("spawnedObject");
            _factory = factory;
            _spawnedObject = spawnedObject;
            _spawnPoint = null;
        }

        public SpawnNotification(ISpawnFactory factory, GameObject spawnedObject, ISpawner spawnPoint)
        {
            if (factory == null) throw new System.ArgumentNullException("factory");
            if (spawnedObject == null) throw new System.ArgumentNullException("spawnedObject");
            _factory = factory;
            _spawnedObject = spawnedObject;
            _spawnPoint = spawnPoint;
        }

        protected SpawnNotification()
        {

        }

        public static SpawnNotification Create(ISpawnFactory factory, GameObject spawnedObject, ISpawner spawnPoint)
        {
            if (factory == null) throw new System.ArgumentNullException("factory");
            if (spawnedObject == null) throw new System.ArgumentNullException("spawnedObject");

            SpawnNotification n;
            if(Notification.TryGetCache<SpawnNotification>(out n))
            {
                n._factory = factory;
                n._spawnedObject = spawnedObject;
                n._spawnPoint = spawnPoint;
            }
            else
            {
                n = new SpawnNotification(factory, spawnedObject, spawnPoint);
            }
            return n;
        }

        #endregion

        #region Properties

        public ISpawnFactory SpawnFactory { get { return _factory; } }

        public GameObject SpawnedObject { get { return _spawnedObject; } }

        /// <summary>
        /// Possible spawnpoint that spawned this object, if one exists and was identitified.
        /// </summary>
        public ISpawner SpawnPoint { get { return _spawnPoint; } }

        #endregion

    }

    public class SpawnPointTriggeredNotification : SpawnNotification
    {
        
        public SpawnPointTriggeredNotification(ISpawnFactory factory, GameObject spawnedObject, ISpawner spawnPoint)
            : base(factory, spawnedObject, spawnPoint)
        {
        }
        
        public new static SpawnPointTriggeredNotification Create(ISpawnFactory factory, GameObject spawnedObject, ISpawner spawnPoint)
        {
            if (factory == null) throw new System.ArgumentNullException("factory");
            if (spawnedObject == null) throw new System.ArgumentNullException("spawnedObject");

            SpawnPointTriggeredNotification n;
            if(Notification.TryGetCache<SpawnPointTriggeredNotification>(out n))
            {
                n._factory = factory;
                n._spawnedObject = spawnedObject;
                n._spawnPoint = spawnPoint;
            }
            else
            {
                n = new SpawnPointTriggeredNotification(factory, spawnedObject, spawnPoint);
            }
            return n;
        }

    }

    public class SpawnPointBeforeSpawnNotification : CancellableNotification
    {

        private ISpawner _spawnPoint;
        private GameObject _prefab;

        public SpawnPointBeforeSpawnNotification(ISpawner spawnPoint, GameObject prefab)
        {
            if (spawnPoint == null) throw new System.ArgumentNullException("spawnPoint");
            _spawnPoint = spawnPoint;
            _prefab = prefab;
        }

        protected SpawnPointBeforeSpawnNotification()
        {

        }

        public static SpawnPointBeforeSpawnNotification Create(ISpawner spawnPoint, GameObject prefab)
        {
            if (spawnPoint == null) throw new System.ArgumentNullException("spawnPoint");

            SpawnPointBeforeSpawnNotification n;
            if (Notification.TryGetCache<SpawnPointBeforeSpawnNotification>(out n))
            {
                n._spawnPoint = spawnPoint;
                n._prefab = prefab;
            }
            else
            {
                n = new SpawnPointBeforeSpawnNotification(spawnPoint, prefab);
            }
            return n;
        }

        public ISpawner SpawnPoint { get { return _spawnPoint; } }

        /// <summary>
        /// The prefab, if known. This may be null if the SpawnFactory hasn't determined the prefab.
        /// </summary>
        public GameObject Prefab { get { return _prefab; } }

    }

    public class DeSpawnNotification : Notification
    {

        #region Fields

        private SpawnedObjectController _controller;
        private GameObject _spawnedObject;

        #endregion

        #region CONSTRUCTOR

        public DeSpawnNotification(SpawnedObjectController controller)
        {
            if (controller == null) throw new System.ArgumentNullException("controller");
            _controller = controller;
            _spawnedObject = controller.gameObject;
        }

        protected DeSpawnNotification()
        {

        }

        public static DeSpawnNotification Create(SpawnedObjectController controller)
        {
            if (controller == null) throw new System.ArgumentNullException("controller");

            DeSpawnNotification n;
            if (Notification.TryGetCache<DeSpawnNotification>(out n))
            {
                n._controller = controller;
                n._spawnedObject = controller.gameObject;
            }
            else
            {
                n = new DeSpawnNotification(controller);
            }
            return n;
        }

        #endregion

        #region Properties

        public SpawnedObjectController Controller { get { return _controller; } }

        public GameObject SpawnedObject { get { return _spawnedObject; } }

        #endregion

    }

    public class SpawnPoolPurgedNotification : Notification
    {
        #region Fields

        private SpawnedObjectController _controller;
        private GameObject _spawnedObject;

        #endregion

        #region CONSTRUCTOR

        public SpawnPoolPurgedNotification(SpawnedObjectController controller)
        {
            if (controller == null) throw new System.ArgumentNullException("controller");
            _controller = controller;
            _spawnedObject = controller.gameObject;
        }
        
        public static SpawnPoolPurgedNotification Create(SpawnedObjectController controller)
        {
            if (controller == null) throw new System.ArgumentNullException("controller");

            SpawnPoolPurgedNotification n;
            if (Notification.TryGetCache<SpawnPoolPurgedNotification>(out n))
            {
                n._controller = controller;
                n._spawnedObject = controller.gameObject;
            }
            else
            {
                n = new SpawnPoolPurgedNotification(controller);
            }
            return n;
        }

        #endregion

        #region Properties

        public SpawnedObjectController Controller { get { return _controller; } }

        public GameObject SpawnedObject { get { return _spawnedObject; } }

        #endregion
    }

}
