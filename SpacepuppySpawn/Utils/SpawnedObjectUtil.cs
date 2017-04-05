using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Spawn;

namespace com.spacepuppy.Utils
{
    public static class SpawnedObjectUtil
    {

        public static bool IsSpawnedObject(this GameObject obj)
        {
            //return obj.GetComponent<SpawnedObjectController>() != null;
            return obj.HasComponent<SpawnedObjectController>();
        }

        public static bool InSpawnedEntity(this GameObject obj)
        {
            return obj.FindRoot().HasComponent<SpawnedObjectController>();
        }
        
    }
}
