using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    public interface ISpawnerModifier
    {

        #region Fields

        /// <summary>
        /// Order in which the spawn modifier is applied when others exist in tandem with it on a SpawnPoint.
        /// </summary>
        int order { get; }

        void OnBeforeSpawnNotification(SpawnPointBeforeSpawnNotification n);
        void OnSpawnedNotification(SpawnPointTriggeredNotification n);

        #endregion

    }
}
