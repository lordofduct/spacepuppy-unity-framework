using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Place on the root of a GameObject hierarchy, or a prefab, to signify that it is a complete entity.
    /// 
    /// If this class is derived from, make sure to set its execution order to the last executed script! 
    /// Failure to do so will result in IEntityAwakeHandler receivers to be messaged out of order.
    /// </summary>
    [DisallowMultipleComponent()]
    public class SPEntity : SPNotifyingComponent, IIgnorableCollision
    {
        
        #region Static Multiton

        private static Dictionary<GameObject, SPEntity> _pool = new Dictionary<GameObject, SPEntity>(com.spacepuppy.Collections.ObjectInstanceIDEqualityComparer<GameObject>.Default);
        private static List<SPEntity> _findList;

        /// <summary>
        /// A readonly list of all active entities.
        /// </summary>
        public static ICollection<SPEntity> ActiveEntities { get { return _pool.Values; } }

        public static SPEntity[] FindAll(System.Func<SPEntity, bool> predicate)
        {
            if (_findList == null) _findList = new List<SPEntity>();
            if (_findList.Count > 0) throw new System.InvalidOperationException("FindAll can only be called once at a time. Do not call inside the predicate, or from a thread other than the main thread.");

            FindAll(_findList, predicate);
            var arr = _findList.ToArray();
            _findList.Clear();
            return arr;
        }

        public static void FindAll(ICollection<SPEntity> coll, System.Func<SPEntity, bool> predicate)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            var e = _pool.GetEnumerator();
            if(predicate == null)
            {
                while (e.MoveNext())
                {
                    coll.Add(e.Current.Value);
                }
            }
            else
            {
                while (e.MoveNext())
                {
                    if (predicate(e.Current.Value)) coll.Add(e.Current.Value);
                }
            }
        }

        public static void FindAll<T>(ICollection<T> coll, System.Func<T, bool> predicate) where T : SPEntity
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            var e = _pool.GetEnumerator();
            if(predicate == null)
            {
                while (e.MoveNext())
                {
                    if (e.Current.Value is T) coll.Add(e.Current.Value as T);
                }
            }
            else
            {
                while (e.MoveNext())
                {
                    if (e.Current.Value is T && predicate(e.Current.Value as T)) coll.Add(e.Current.Value as T);
                }
            }
        }

        #endregion

        #region Fields
        
        [System.NonSerialized()]
        private bool _isAwake;

        #endregion

        #region CONSTRUCTOR
        
        protected override void Awake()
        {
            this.AddTag(SPConstants.TAG_ROOT);

            base.Awake();

            _isAwake = true;

            using (var lst = TempCollection.GetList<IEntityAwakeHandler>())
            {
                this.gameObject.GetComponentsInChildren<IEntityAwakeHandler>(true, lst);
                if(lst.Count > 0)
                {
                    for(int i = 0; i < lst.Count; i++)
                    {
                        lst[i].OnEntityAwake(this);
                    }
                }
            }
        }

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            _pool[this.gameObject] = this;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _pool.Remove(this.gameObject);
        }

        #endregion

        #region Properties
        
        public bool IsAwake { get { return _isAwake; } }

        #endregion

        #region IIgnorableCollision Interface
        
        public virtual void IgnoreCollision(Collider coll, bool ignore)
        {
            if (coll == null) return;
            
            using (var lst = TempCollection.GetList<Collider>())
            {
                this.GetComponentsInChildren<Collider>(false, lst);
                for (int i = 0; i < lst.Count; i++)
                {
                    Physics.IgnoreCollision(lst[i], coll, ignore);
                }
            }
        }

        public virtual void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            if (coll == null) return;
            
            using (var lst = TempCollection.GetList<Collider>())
            {
                this.GetComponentsInChildren<Collider>(false, lst);
                for (int i = 0; i < lst.Count; i++)
                {
                    coll.IgnoreCollision(lst[i], ignore);
                }
            }
        }

        #endregion



        #region Static Utils

        public static bool IsEntitySource(object obj)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go == null) return false;

            go = go.FindTrueRoot();
            if (go == null) return false;
            return _pool.Contains(go) || go.HasComponent<SPEntity>();
        }

        public static bool IsEntitySource<T>(object obj) where T : SPEntity
        {
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go == null) return false;

            go = go.FindTrueRoot();
            if (go == null) return false;
            SPEntity e;
            return (_pool.TryGetValue(go, out e) && e is T) || go.HasComponent<T>();
        }

        public static SPEntity GetEntityFromSource(object obj)
        {
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return null;

            SPEntity e;
            if (_pool.TryGetValue(go, out e)) return e;
            else return go.GetComponent<SPEntity>();
        }

        public static SPEntity GetEntityFromSource(System.Type tp, object obj)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return null;

            SPEntity e;
            if (_pool.TryGetValue(go, out e) && TypeUtil.IsType(e.GetType(), tp))
            {
                return e;
            }
            else if(TypeUtil.IsType(tp, typeof(SPEntity)))
            {
                return go.GetComponent(tp) as SPEntity;
            }

            return null;
        }

        public static T GetEntityFromSource<T>(object obj) where T : SPEntity
        {
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return null;

            SPEntity e;
            if (_pool.TryGetValue(go, out e) && e is T)
            {
                return e as T;
            }
            else
            {
                return go.GetComponent<T>();
            }
        }

        public static bool GetEntityFromSource(object obj, out SPEntity entity)
        {
            entity = null;
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return false;

            SPEntity e;
            if (_pool.TryGetValue(go, out e))
            {
                entity = e;
                return true;
            }
            else
            {
                entity = go.GetComponent<SPEntity>();
                return entity != null;
            }
        }

        public static bool GetEntityFromSource(System.Type tp, object obj, out SPEntity entity)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            entity = null;
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return false;

            SPEntity e;
            if (_pool.TryGetValue(go, out e) && TypeUtil.IsType(e.GetType(), tp))
            {
                entity = e;
                return true;
            }
            else if (TypeUtil.IsType(tp, typeof(SPEntity)))
            {
                entity = go.GetComponent(tp) as SPEntity;
                return entity != null;
            }

            return false;
        }

        public static bool GetEntityFromSource<T>(object obj, out T entity) where T : SPEntity
        {
            entity = null;
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return false;

            SPEntity e;
            if (_pool.TryGetValue(go, out e) && e is T)
            {
                entity = e as T;
                return true;
            }
            else
            {
                entity = go.GetComponent<T>();
                return entity != null;
            }
        }

        #endregion

    }

}
