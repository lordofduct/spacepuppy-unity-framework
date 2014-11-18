using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public interface IGameObjectSource
    {

        GameObject gameObject { get; }
        Transform transform { get; }

    }

}
