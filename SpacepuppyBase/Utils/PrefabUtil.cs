using UnityEngine;

namespace com.spacepuppy.Utils
{
    public static class PrefabUtil
    {


        public static GameObject Create(GameObject prefab)
        {
            return UnityEngine.Object.Instantiate(prefab) as GameObject;
        }

        public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            return UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;
        }

        public static GameObject Create(GameObject prefab, Transform parent)
        {
            if (parent == null) return Create(prefab);

            //NOTE - old method, no longer works in unity 5.3.x, replaced with new version
            //prefab.AddOrGetComponent<EarlyParentSetter>().Init(parent);
            //return UnityEngine.Object.Instantiate(prefab, parent.position, parent.rotation) as GameObject;

            //NOTE - this appears to work, thanks to help from @Polymorphik
            bool isActive = prefab.activeSelf;
            prefab.SetActive(false);
            var result = UnityEngine.Object.Instantiate(prefab, parent.position, parent.rotation) as GameObject;
            result.transform.parent = parent;
            result.SetActive(isActive);
            prefab.SetActive(isActive);
            return result;
        }

        public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            if (parent == null) return Create(prefab, pos, rot);

            //NOTE - old method, no longer works in unity 5.3.x, replaced with new version
            //prefab.AddOrGetComponent<EarlyParentSetter>().Init(parent);
            //return UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;

            //NOTE - this appears to work, thanks to help from @Polymorphik
            bool isActive = prefab.activeSelf;
            prefab.SetActive(false);
            var result = UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;
            result.transform.parent = parent;
            result.SetActive(isActive);
            prefab.SetActive(isActive);
            return result;
        }
        
    }
}
