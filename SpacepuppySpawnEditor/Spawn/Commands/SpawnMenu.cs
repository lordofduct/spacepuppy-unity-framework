using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Spawn;

namespace com.spacepuppyeditor.Spawn.Commands
{
    public static class SpawnMenu
    {

        [MenuItem(SPMenu.MENU_NAME_ROOT + "/Create Default SpawnPool", priority = SPMenu.MENU_PRIORITY_SINGLETON + 1)]
        public static void CreateDefaultSpawnPool()
        {
            SpawnPool.CreatePrimaryPool();
        }

        [MenuItem(SPMenu.MENU_NAME_ROOT + "/Create Default SpawnPool", validate = true)]
        public static bool CreateDefaultSpawnPool_Validate()
        {
            if (Application.isPlaying) return false;
            return !SpawnPool.PrimaryPoolExists;
        }

    }
}
