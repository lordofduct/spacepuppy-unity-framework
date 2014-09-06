using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Spawn
{

    public class SpawnNotification : Notification
    {

        #region Fields

        private GameObject _spawnedObject;

        #endregion

        #region CONSTRUCTOR

        public SpawnNotification(GameObject spawnedObject)
        {
            if(spawnedObject == null) throw new System.ArgumentNullException("spawnedObject");
            _spawnedObject = spawnedObject;
        }

        #endregion

        #region Properties

        public GameObject SpawnedObject { get { return _spawnedObject; } }

        #endregion

    }

    public class DeSpawnNotification : Notification
    {

        #region Fields

        private GameObject _spawnedObject;

        #endregion

        #region CONSTRUCTOR

        public DeSpawnNotification(GameObject spawnedObject)
        {
            if(spawnedObject == null) throw new System.ArgumentNullException("spawnedObject");
            _spawnedObject = spawnedObject;
        }

        #endregion

        #region Properties

        public GameObject SpawnedObject { get { return _spawnedObject; } }

        #endregion

    }

    public class SpawnPointBeforeSpawnNotification : CancellableNotification
    {

        private GameObject _prefab;

        public SpawnPointBeforeSpawnNotification(GameObject prefab)
        {
            if (prefab == null) throw new System.ArgumentNullException("prefab");
            _prefab = prefab;
        }

        public GameObject Options { get { return _prefab; } }

    }

    public class SpawnPointTriggeredNotification : SpawnNotification
    {
        public SpawnPointTriggeredNotification(GameObject spawnedObject)
            : base(spawnedObject)
        {

        }
    }


}
