using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    public abstract class OnSpawnModifier : SPComponent
    {

        #region Fields

        [Tooltip("Order in which the spawn modifier is applied when others exist in tandem with it on a SpawnPoint.")]
        public int order;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        #endregion

        #region Messages

        //TODO - consider allowing these for use on entities, and not just on SpawnPoints

        //protected void OnSpawn()
        //{
        //    //fake the event
        //    this.OnSpawnedNotification(new SpawnPointTriggeredNotification(this.FindRoot()));
        //}


        #endregion

        #region Notification Handlers

        protected internal virtual void OnBeforeSpawnNotification(SpawnPointBeforeSpawnNotification n)
        {
        }

        protected internal virtual void OnSpawnedNotification(SpawnPointTriggeredNotification n)
        {
        }

        #endregion

    }
}
