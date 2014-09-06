using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Spawn
{
    public interface ISpawnPointPrefabSelector : IComponent
    {

        GameObject SelectPrefab(GameObject[] prefabs);

    }
}
