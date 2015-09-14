using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy
{

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

            var e = _pool.Values.GetEnumerator();
            if(predicate == null)
            {
                while (e.MoveNext())
                {
                    coll.Add(e.Current);
                }
            }
            else
            {
                while (e.MoveNext())
                {
                    if (predicate(e.Current)) coll.Add(e.Current);
                }
            }
        }

        public static void FindAll<T>(ICollection<T> coll, System.Func<T, bool> predicate) where T : SPEntity
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            var e = _pool.Values.GetEnumerator();
            if(predicate == null)
            {
                while (e.MoveNext())
                {
                    if (e.Current is T) coll.Add(e.Current as T);
                }
            }
            else
            {
                while (e.MoveNext())
                {
                    if (e.Current is T && predicate(e.Current as T)) coll.Add(e.Current as T);
                }
            }
        }

        #endregion

        #region Fields

        [System.NonSerialized()]
        private Transform _trans;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.SetTag(SPConstants.TAG_ROOT);
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

        public new Transform transform { get { return _trans ?? base.transform; } }

        #endregion

        #region IIgnorableCollision Interface
        
        void IIgnorableCollision.IgnoreCollision(Collider coll, bool ignore)
        {
            if (coll == null) return;

            var arr = this.GetComponentsInChildren<Collider>();
            for(int i = 0; i < arr.Length; i++)
            {
                Physics.IgnoreCollision(arr[i], coll, ignore);
            }
        }

        void IIgnorableCollision.IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            if (coll == null) return;

            var arr = this.GetComponentsInChildren<Collider>();
            for (int i = 0; i < arr.Length; i++)
            {
                coll.IgnoreCollision(arr[i], ignore);
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
