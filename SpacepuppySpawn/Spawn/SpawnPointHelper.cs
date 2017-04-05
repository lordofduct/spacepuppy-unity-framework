using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    public static class SpawnPointHelper
    {

        public static int SelectFromMultiple(ISpawner spawnPoint, int optionCount, bool ignoreSelectionModifiers = false)
        {
            if (optionCount <= 0) return -1;
            if (optionCount == 1) return 0;

            if(!ignoreSelectionModifiers && spawnPoint != null && spawnPoint.component != null)
            {
                using (var lst = TempCollection.GetList<ISpawnPointSelector>())
                {
                    spawnPoint.component.GetComponents<ISpawnPointSelector>(lst);
                    for(int i = 0; i < lst.Count; i++)
                    {
                        if(lst[i].enabled)
                        {
                            int index = lst[i].Select(spawnPoint, optionCount);
                            if (index >= 0) return index;
                        }
                    }
                }
            }

            return RandomUtil.Standard.Range(optionCount);
        }

        public static void GetSpawnModifiers(ISpawner spawnPoint, List<ISpawnerModifier> modifiers)
        {
            if (spawnPoint == null) throw new System.ArgumentNullException("spawnPoint");

            spawnPoint.component.GetComponents<ISpawnerModifier>(modifiers);
            modifiers.Sort(SpawnPointHelper.ModiferSortMethod);
        }

        public static bool SignalOnBeforeSpawnNotification(ISpawner spawnPoint, GameObject prefab, IList<ISpawnerModifier> modifiers)
        {
            var beforeNotif = SpawnPointBeforeSpawnNotification.Create(spawnPoint, prefab);
            
            if (modifiers != null && modifiers.Count > 0)
            {
                for(int i = 0; i < modifiers.Count; i++)
                {
                    if(modifiers[i] != null) modifiers[i].OnBeforeSpawnNotification(beforeNotif);
                }
            }
            Notification.PostNotification<SpawnPointBeforeSpawnNotification>(spawnPoint, beforeNotif, false);
            Notification.Release(beforeNotif);

            return beforeNotif.Cancelled;
        }

        public static void SignalOnSpawnedNotification(ISpawner spawnPoint, ISpawnFactory factory, GameObject spawnedObject, IList<ISpawnerModifier> modifiers)
        {
            var spawnNotif = SpawnPointTriggeredNotification.Create(factory, spawnedObject, spawnPoint);
            if (modifiers != null && modifiers.Count > 0)
            {
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i] != null) modifiers[i].OnSpawnedNotification(spawnNotif);
                }
            }
            Notification.PostNotification<SpawnPointTriggeredNotification>(spawnPoint, spawnNotif, false);
            Notification.Release(spawnNotif);
        }






        #region Internal
        
        private static System.Comparison<ISpawnerModifier> _modifierSort;
        public static System.Comparison<ISpawnerModifier> ModiferSortMethod
        {
            get
            {
                if (_modifierSort == null)
                {
                    _modifierSort = (a, b) => a.order.CompareTo(b.order);
                }
                return _modifierSort;
            }
        }

        #endregion

    }
}
