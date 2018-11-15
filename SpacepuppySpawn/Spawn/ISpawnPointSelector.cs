using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Spawn
{

    /// <summary>
    /// Only supported by i_Spawner/Spawner
    /// </summary>
    public interface ISpawnPointSelector : IComponent
    {

        int Select(ISpawner spawnPoint, int optionCount);

    }
}
