using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Base contract for any interface contract that should be considered a Component
    /// </summary>
    public interface IComponent : IGameObjectSource
    {
        event System.EventHandler ComponentDestroyed;

        bool enabled { get; set; }
        Component component { get; }

    }

}
