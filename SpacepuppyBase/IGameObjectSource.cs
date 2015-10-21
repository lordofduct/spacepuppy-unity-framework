using UnityEngine;

namespace com.spacepuppy
{

    public interface IGameObjectSource
    {

        GameObject gameObject { get; }
        Transform transform { get; }

    }

}
