using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Hooks
{

    public sealed class EarlyStartHook : MonoBehaviour
    {

        private System.Action _callback;

        private void Start()
        {
            if (_callback != null) _callback();
            Destroy(this);
        }
        
        public static void Invoke(GameObject obj, System.Action callback)
        {
            if (callback == null) return;

            var hook = obj.GetComponent<EarlyStartHook>();
            if (hook == null) hook = obj.AddComponent<EarlyStartHook>();
            hook._callback += callback;
        }

    }

}
