#pragma warning disable 0168 // variable declared but not used.

namespace com.spacepuppy.Utils
{
    public static class ObjUtil
    {

        #region Fields

        private static System.Func<UnityEngine.Object, bool> _isObjectAlive;

        #endregion

        #region CONSTRUCTOR

        static ObjUtil()
        {
            try
            {
                var tp = typeof(UnityEngine.Object);
                var meth = tp.GetMethod("IsNativeObjectAlive", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (meth != null)
                    _isObjectAlive = System.Delegate.CreateDelegate(typeof(System.Func<UnityEngine.Object, bool>), meth) as System.Func<UnityEngine.Object, bool>;
                else
                    _isObjectAlive = (a) => a != null;
            }
            catch
            {
                //incase there was a change to the UnityEngine.dll
                _isObjectAlive = (a) => a != null;
                UnityEngine.Debug.LogWarning("This version of Spacepuppy Framework does not support the version of Unity it's being used with.");
                //throw new System.InvalidOperationException("This version of Spacepuppy Framework does not support the version of Unity it's being used with.");
            }
        }
        
        #endregion

        public static T GetAsFromSource<T>(object obj) where T : class
        {
            if (obj == null) return null;
            if (obj is T) return obj as T;
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (c is T) return c as T;
            }
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go is T) return go as T;
            
            //if (go != null && ComponentUtil.IsAcceptableComponentType(typeof(T))) return go.GetComponentAlt<T>();
            if (go != null)
            {
                var tp = typeof(T);
                if (typeof(SPEntity).IsAssignableFrom(tp))
                    return SPEntity.Pool.GetFromSource(tp, go) as T;
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                    return go.GetComponent(tp) as T;
            }

            return null;
        }

        public static object GetAsFromSource(System.Type tp, object obj)
        {
            if (obj == null) return null;

            var otp = obj.GetType();
            if (TypeUtil.IsType(otp, tp)) return obj;
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (!object.ReferenceEquals(c, null) && TypeUtil.IsType(c.GetType(), tp)) return c;
            }

            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (tp == typeof(UnityEngine.GameObject)) return go;

            if(go != null)
            {
                if (typeof(SPEntity).IsAssignableFrom(tp))
                    return SPEntity.Pool.GetFromSource(tp, go);
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                    return go.GetComponent(tp);
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

        public static System.Func<UnityEngine.Object, bool> IsObjectAlive
        {
            get { return _isObjectAlive; }
        }

        public static bool IsDisposed(this ISPDisposable obj)
        {
            if (object.ReferenceEquals(obj, null)) return true;

            return obj.IsDisposed;
        }

        /// <summary>
        /// Returns true if the object is either a null reference or has been destroyed by unity. 
        /// This will respect ISPDisposable over all else.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDestroyed(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return true;

            if (obj is ISPDisposable)
                return (obj as ISPDisposable).IsDisposed;
            else if (obj is UnityEngine.Object)
                return !_isObjectAlive(obj as UnityEngine.Object);
            else if (obj is IComponent)
                return !_isObjectAlive((obj as IComponent).component);
            else if (obj is IGameObjectSource)
                return !_isObjectAlive((obj as IGameObjectSource).gameObject);

            //if (obj is UnityEngine.Object)
            //    return (obj as UnityEngine.Object) == null;
            //else if (obj is IComponent)
            //    return (obj as IComponent).component == null;
            //else if (obj is IGameObjectSource)
            //    return (obj as IGameObjectSource).gameObject == null;

            return false;
        }

        /// <summary>
        /// Unlike IsNullOrDestroyed, this only returns true if the managed object half of the object still exists, 
        /// but the unmanaged half has been destroyed by unity. 
        /// This will respect ISPDisposable over all else.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDestroyed(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;

            if (obj is ISPDisposable)
                return (obj as ISPDisposable).IsDisposed;
            else if (obj is UnityEngine.Object)
                return !_isObjectAlive(obj as UnityEngine.Object);
            else if (obj is IComponent)
                return !_isObjectAlive((obj as IComponent).component);
            else if (obj is IGameObjectSource)
                return !_isObjectAlive((obj as IGameObjectSource).gameObject);


            //if (obj is UnityEngine.Object)
            //    return (obj as UnityEngine.Object) == null;
            //else if (obj is IComponent)
            //    return (obj as IComponent).component == null;
            //else if (obj is IGameObjectSource)
            //    return (obj as IGameObjectSource).gameObject == null;

            return false;
        }

        public static bool IsAlive(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;

            if (obj is ISPDisposable)
                return !(obj as ISPDisposable).IsDisposed;
            else if (obj is UnityEngine.Object)
                return _isObjectAlive(obj as UnityEngine.Object);
            else if (obj is IComponent)
                return _isObjectAlive((obj as IComponent).component);
            else if (obj is IGameObjectSource)
                return _isObjectAlive((obj as IGameObjectSource).gameObject);


            //if (obj is UnityEngine.Object)
            //    return (obj as UnityEngine.Object) != null;
            //else if (obj is IComponent)
            //    return (obj as IComponent).component != null;
            //else if (obj is IGameObjectSource)
            //    return (obj as IGameObjectSource).gameObject != null;

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
