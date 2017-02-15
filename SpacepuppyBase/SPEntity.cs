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
            _pool.AddReference(this);

            base.OnStartOrEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _pool.RemoveReference(this);
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

        #region Multiton Interface

        private static UniqueToEntityMultitonPool<SPEntity> _pool = new UniqueToEntityMultitonPool<SPEntity>();
        public static UniqueToEntityMultitonPool<SPEntity> Pool { get { return _pool; } }
        
        
        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static SPEntity[] FindAll(System.Func<SPEntity, bool> predicate)
        {
            return SPEntity.Pool.FindAll(predicate);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static void FindAll(ICollection<SPEntity> coll, System.Func<SPEntity, bool> predicate)
        {
            SPEntity.Pool.FindAll(coll, predicate);
        }
        
        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static void FindAll<T>(ICollection<T> coll, System.Func<T, bool> predicate) where T : SPEntity
        {
            SPEntity.Pool.FindAll<T>(coll, predicate);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static bool IsEntitySource(object obj)
        {
            return SPEntity.Pool.IsSource(obj);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static bool IsEntitySource<T>(object obj) where T : SPEntity
        {
            return SPEntity.Pool.IsSource<T>(obj);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static SPEntity GetEntityFromSource(object obj)
        {
            return SPEntity.GetEntityFromSource(obj);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static SPEntity GetEntityFromSource(System.Type tp, object obj)
        {
            return SPEntity.Pool.GetFromSource(tp, obj);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static T GetEntityFromSource<T>(object obj) where T : SPEntity
        {
            return SPEntity.Pool.GetFromSource<T>(obj);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static bool GetEntityFromSource(object obj, out SPEntity entity)
        {
            return SPEntity.Pool.GetFromSource(obj, out entity);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static bool GetEntityFromSource(System.Type tp, object obj, out SPEntity entity)
        {
            return SPEntity.Pool.GetFromSource(tp, obj, out entity);
        }

        [System.Obsolete("Access SPEntity.Pool instead.")]
        public static bool GetEntityFromSource<T>(object obj, out T entity) where T : SPEntity
        {
            return SPEntity.GetEntityFromSource<T>(obj, out entity);
        }

        #endregion

    }

}
