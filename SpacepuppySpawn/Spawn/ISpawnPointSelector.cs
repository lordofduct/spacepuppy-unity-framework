using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Spawn
{
    public interface ISpawnPointSelector : IComponent
    {

        int Select(ISpawner spawnPoint, int optionCount);

    }
}
