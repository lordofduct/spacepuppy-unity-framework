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


    public class SpawnerModifierComparer : IComparer<ISpawnerModifier>
    {

        private static SpawnerModifierComparer _default;
        public static SpawnerModifierComparer Default
        {
            get
            {
                if (_default == null)
                    _default = new SpawnerModifierComparer();
                return _default;
            }
        }

        public int Compare(ISpawnerModifier x, ISpawnerModifier y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return x == null ? 0 : 1;
            return x.order.CompareTo(y.order);
        }
    }

}
