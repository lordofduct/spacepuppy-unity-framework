using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [Infobox("The object spawned will be the object passed to the TriggerableMechanism (i_).")]
    [System.Obsolete("This is outdated, instead use the trigger available in the spawner itself.")]
    public class t_OnSpawn : TriggerComponent
    {

        #region Fields
        
        [DefaultFromSelf(Relativity = EntityRelativity.Entity)]
        [DisableOnPlay()]
        [Tooltip("The GameObject that has the SpawnPoint on it.")]
        [SerializeField()]
        private GameObject _observedTarget;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if (_observedTarget == null) _observedTarget = this.gameObject;
        }

        protected override void OnEnable()
        {
            //this NEEDS to be OnEnable, OnSpawn notification may dispatch prior to Start if this is the first time the object is created.
            base.OnEnable();

            Notification.RegisterObserver<SpawnNotification>(_observedTarget, OnSpawnHandler);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Notification.RemoveObserver<SpawnNotification>(_observedTarget, OnSpawnHandler);
        }

        #endregion

        #region Handlers

        protected virtual void OnSpawnHandler(object sender, SpawnNotification n)
        {
            this.ActivateTrigger(n.SpawnedObject);
        }

        #endregion

    }
}
