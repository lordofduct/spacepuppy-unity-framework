using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0168 // variable declared but not used.

namespace com.spacepuppy.Utils
{
    public static class ObjUtil
    {

        #region Fields

        private static System.Func<UnityEngine.Object, UnityEngine.Object, bool> _compareBaseObjects;

        #endregion

        #region CONSTRUCTOR

        static ObjUtil()
        {
            try
            {
                var tp = typeof(UnityEngine.Object);
                var meth = tp.GetMethod("CompareBaseObjectsInternal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                _compareBaseObjects = System.Delegate.CreateDelegate(typeof(System.Func<UnityEngine.Object, UnityEngine.Object, bool>), meth) as System.Func<UnityEngine.Object, UnityEngine.Object, bool>;
            }
            catch
            {
                //incase there was a change to the UnityEngine.dll
                _compareBaseObjects = (a, b) => a == b;
                UnityEngine.Debug.LogWarning("This version of Spacepuppy Framework does not support the version of Unity it's being used with.");
                //throw new System.InvalidOperationException("This version of Spacepuppy Framework does not support the version of Unity it's being used with.");
            }
        }

        #endregion


        public static T GetAsFromSource<T>(object obj) where T : class
        {
            if (obj == null) return null;
            if (obj is T) return obj as T;

            var tp = typeof(T);
            if(ComponentUtil.IsComponentType(tp) && ComponentUtil.IsComponentSource(obj))
            {
                return ComponentUtil.GetComponentFromSource<T>(obj);
            }
            
            return null;
        }





        public static void SmartDestroy(UnityEngine.Object obj)
        {
            if (UnityEngine.Application.isPlaying)
            {
                UnityEngine.Object.Destroy(obj);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
        }

        /// <summary>
        /// Returns true if the object is either a null reference or has been destroyed by unity.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDestroyed(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return true;

            if (obj is UnityEngine.Object)
                return _compareBaseObjects(obj as UnityEngine.Object, null);
            else if (obj is IComponent)
                return _compareBaseObjects((obj as IComponent).component, null);
            else if (obj is IGameObjectSource)
                return _compareBaseObjects((obj as IGameObjectSource).gameObject, null);

            return false;
        }

        /// <summary>
        /// Unlike IsNullOrDestroyed, this only returns true if the managed object half of the object still exists, 
        /// but the unmanaged half has been destroyed by unity.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDestroyed(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;

            if (obj is UnityEngine.Object)
                return _compareBaseObjects(obj as UnityEngine.Object, null);
            else if (obj is IComponent)
                return _compareBaseObjects((obj as IComponent).component, null);
            else if (obj is IGameObjectSource)
                return _compareBaseObjects((obj as IGameObjectSource).gameObject, null);

            return false;
        }

        public static bool IsAlive(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;

            if (obj is UnityEngine.Object)
                return !_compareBaseObjects(obj as UnityEngine.Object, null);
            else if (obj is IComponent)
                return !_compareBaseObjects((obj as IComponent).component, null);
            else if (obj is IGameObjectSource)
                return !_compareBaseObjects((obj as IGameObjectSource).gameObject, null);

            return true;
        }



        public static bool EqualsAny(System.Object obj, params System.Object[] others)
        {
            return System.Array.IndexOf(others, obj) >= 0;
        }


        public static T ExtractDelegate<T>(object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false) where T : class
        {
            if (obj == null) throw new System.ArgumentNullException("obj");
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new TypeArgumentMismatchException(delegateType, typeof(System.Delegate), "T");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure) as T;
        }

        public static System.Delegate ExtractDelegate(System.Type delegateType, object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false)
        {
            if (delegateType == null) throw new System.ArgumentNullException("delegateType");
            if (obj == null) throw new System.ArgumentNullException("obj");
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new TypeArgumentMismatchException(delegateType, typeof(System.Delegate), "delegateType");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure);
        }


    }
}
