using UnityEngine;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// Acts as a token to be attached to any prefab. It can then be used to recognize prefabs.
    /// 
    /// When instantiated, the token deletes itself from the instance.
    /// </summary>
    public class PrefabToken : MonoBehaviour
    {

        protected virtual void Awake()
        {
            Object.Destroy(this);
        }

    }

}
