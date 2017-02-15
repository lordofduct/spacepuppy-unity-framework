using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [System.Serializable]
    public struct ProxyTarget
    {

        #region Fields

        [SerializeField()]
        private UnityEngine.Object _target;
        [SerializeField()]
        private SearchBy _searchBy;
        [SerializeField()]
        private string _queryString;

        #endregion

        #region CONSTRUCTOR

        public ProxyTarget(SearchBy searchBy)
        {
            _target = null;
            _searchBy = searchBy;
            _queryString = null;
        }

        #endregion

        #region Properties

        public UnityEngine.Object Target
        {
            get { return _target; }
            set { _target = value; }
        }
        
        public SearchBy SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; }
        }

        public string SearchByQuery
        {
            get { return _queryString; }
            set { _queryString = value; }
        }

        #endregion

        #region Methods

        public UnityEngine.Object GetTarget()
        {
            if (!object.ReferenceEquals(_target, null))
                return _target;
            else
                return ObjUtil.Find(_searchBy, _queryString);
        }

        public UnityEngine.Object[] GetTargets()
        {
            if (!object.ReferenceEquals(_target, null))
                return new UnityEngine.Object[] { _target };
            else
                return ObjUtil.FindAll(_searchBy, _queryString);
        }

        public T GetTarget<T>() where T : class
        {
            if (!object.ReferenceEquals(_target, null))
                return _target as T;
            else
                return ObjUtil.Find<T>(_searchBy, _queryString);
        }

        public T[] GetTargets<T>() where T : class
        {
            if (!object.ReferenceEquals(_target, null))
                return (_target is T) ?  new T[] { _target as T } : ArrayUtil.Empty<T>();
            else
                return ObjUtil.FindAll<T>(_searchBy, _queryString);
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {

            public System.Type TargetType;

            public ConfigAttribute()
            {
                this.TargetType = typeof(GameObject);
            }

            public ConfigAttribute(System.Type targetType)
            {
                //if (targetType == null || 
                //    (!TypeUtil.IsType(targetType, typeof(UnityEngine.Object)) && !TypeUtil.IsType(targetType, typeof(IComponent)))) throw new TypeArgumentMismatchException(targetType, typeof(UnityEngine.Object), "targetType");
                if (targetType == null ||
                    (!TypeUtil.IsType(targetType, typeof(UnityEngine.Object)) && !targetType.IsInterface))
                    throw new TypeArgumentMismatchException(targetType, typeof(UnityEngine.Object), "targetType");

                this.TargetType = targetType;
            }

        }

        #endregion

    }
}
