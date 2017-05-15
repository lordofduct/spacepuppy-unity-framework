using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public interface IProxy
    {
        UnityEngine.Object GetTarget();
        UnityEngine.Object GetTarget(object arg);
    }

    [System.Serializable]
    public struct Proxy : IProxy
    {

        #region Fields

        [SerializeField()]
        [SelectableObject(AllowSceneObjects = true)]
        private UnityEngine.Object _target;
        [SerializeField()]
        private SearchBy _searchBy;
        [SerializeField()]
        private string _queryString;

        #endregion

        #region CONSTRUCTOR

        public Proxy(SearchBy searchBy)
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
            if (_searchBy == SearchBy.Nothing)
                return _target;
            else
            {
                return ObjUtil.Find(_searchBy, _queryString);
            }
        }

        public UnityEngine.Object[] GetTargets()
        {
            if (_searchBy == SearchBy.Nothing)
                return new UnityEngine.Object[] { _target };
            else
            {
                return ObjUtil.FindAll(_searchBy, _queryString);
            }
        }

        public T GetTarget<T>() where T : class
        {
            if (_searchBy == SearchBy.Nothing)
                return ObjUtil.GetAsFromSource<T>(_target);
            else
            {
                return ObjUtil.Find<T>(_searchBy, _queryString);
            }
        }

        public T[] GetTargets<T>() where T : class
        {
            if (_searchBy == SearchBy.Nothing)
            {
                //return (_target is T) ? new T[] { _target as T } : ArrayUtil.Empty<T>();
                var targ = ObjUtil.GetAsFromSource<T>(_target);
                return targ != null ? new T[] { targ } : ArrayUtil.Empty<T>();
            }
            else
            {
                return ObjUtil.FindAll<T>(_searchBy, _queryString);
            }
        }

        #endregion

        #region IProxy Interface

        UnityEngine.Object IProxy.GetTarget()
        {
            return this.GetTarget();
        }

        UnityEngine.Object IProxy.GetTarget(object arg)
        {
            return this.GetTarget();
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

    public class TargetProxy : TriggerableMechanism, IDynamic, IProxy
    {

        #region Fields

        [SerializeField]
        private TriggerableTargetObject _target;
        [SerializeField]
        [TypeReference.Config(typeof(Component), allowAbstractClasses = true, allowInterfaces = true)]
        private TypeReference _componentTypeOnTarget = new TypeReference();
        [SerializeField]
        [EnumPopupExcluding((int)TriggerActivationType.SendMessage, (int)TriggerActivationType.CallMethodOnSelectedTarget, (int)TriggerActivationType.EnableTarget)]
        private TriggerActivationType _triggerAction;

        #endregion

        #region Properties

        public TriggerActivationType TriggerAction
        {
            get { return _triggerAction; }
            set
            {
                switch (value)
                {
                    case TriggerActivationType.SendMessage:
                    case TriggerActivationType.CallMethodOnSelectedTarget:
                    case TriggerActivationType.EnableTarget:
                        throw new System.ArgumentOutOfRangeException("TriggerActivationType not supported.");
                    default:
                        _triggerAction = value;
                        break;
                }
            }
        }

        #endregion

        #region IProxy Interface

        public UnityEngine.Object GetTarget()
        {
            return _target.GetTarget(_componentTypeOnTarget, null) as UnityEngine.Object;
        }

        public UnityEngine.Object GetTarget(object arg)
        {
            return _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), arg) as UnityEngine.Object;
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this.GetTarget(arg);
            if (targ == null) return false;

            switch (_triggerAction)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    TriggerTarget.TriggerAllOnTarget(targ, sender, arg);
                    return true;
                case TriggerActivationType.TriggerSelectedTarget:
                    TriggerTarget.TriggerSelectedTarget(targ, sender, arg);
                    return true;
                case TriggerActivationType.DestroyTarget:
                    TriggerTarget.DestroyTarget(targ);
                    return true;
            }

            return false;
        }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get
            {
                return (this as IDynamic).GetValue(sMemberName);
            }
            set
            {
                (this as IDynamic).SetValue(sMemberName, value);
            }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return targ.SetValue(sMemberName, value, index);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return targ.GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            var targ = this.GetTarget();
            if (targ == null)
            {
                result = null;
                return false;
            }
            return targ.TryGetValue(sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return targ.InvokeMethod(sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return DynamicUtil.HasMember(targ, sMemberName, includeNonPublic);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            return DynamicUtil.GetMembers(this.GetTarget(), includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            return DynamicUtil.GetMember(this.GetTarget(), sMemberName, includeNonPublic);
        }

        #endregion

    }

    
    /*
public class TargetProxy : MonoBehaviour, IDynamic
{

    #region Fields

    [SerializeField]
    private Proxy _target;
    [SerializeField]
    [TypeReference.Config(typeof(Component), allowAbstractClasses = true, allowInterfaces = true)]
    private TypeReference _componentTypeOnTarget = new TypeReference();

    [System.NonSerialized]
    private object _cachedTarg;

    #endregion

    #region Properties

    public Proxy Target
    {
        get { return _target; }
        set { _target = value; }
    }

    #endregion


    #region IDynamic Interface

    object IDynamic.this[string sMemberName]
    {
        get
        {
            return (this as IDynamic).GetValue(sMemberName);
        }
        set
        {
            (this as IDynamic).SetValue(sMemberName, value);
        }
    }

    bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
    {
        if(_cachedTarg == null)
        {
            _cachedTarg = _target.GetTarget();
            if (_componentTypeOnTarget.Type != null)
                _cachedTarg = ObjUtil.GetAsFromSource(_componentTypeOnTarget.Type, _cachedTarg);
        }
        return _cachedTarg.SetValue(sMemberName, value, index);
    }

    object IDynamic.GetValue(string sMemberName, params object[] args)
    {
        if (_cachedTarg == null)
        {
            _cachedTarg = _target.GetTarget();
            if (_componentTypeOnTarget.Type != null)
                _cachedTarg = ObjUtil.GetAsFromSource(_componentTypeOnTarget.Type, _cachedTarg);
        }
        return _cachedTarg.GetValue(sMemberName, args);
    }

    bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
    {
        if (_cachedTarg == null)
        {
            _cachedTarg = _target.GetTarget();
            if (_componentTypeOnTarget.Type != null)
                _cachedTarg = ObjUtil.GetAsFromSource(_componentTypeOnTarget.Type, _cachedTarg);
        }
        return _cachedTarg.TryGetValue(sMemberName, out result, args);
    }

    object IDynamic.InvokeMethod(string sMemberName, params object[] args)
    {
        if (_cachedTarg == null)
        {
            _cachedTarg = _target.GetTarget();
            if (_componentTypeOnTarget.Type != null)
                _cachedTarg = ObjUtil.GetAsFromSource(_componentTypeOnTarget.Type, _cachedTarg);
        }
        return _cachedTarg.InvokeMethod(sMemberName, args);
    }

    bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
    {
        if (_cachedTarg == null)
        {
            _cachedTarg = _target.GetTarget();
            if (_componentTypeOnTarget.Type != null)
                _cachedTarg = ObjUtil.GetAsFromSource(_componentTypeOnTarget.Type, _cachedTarg);
        }
        return DynamicUtil.HasMember(_cachedTarg, sMemberName, includeNonPublic);
    }

    IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
    {
        if (_cachedTarg == null)
        {
            _cachedTarg = _target.GetTarget();
            if (_componentTypeOnTarget.Type != null)
                _cachedTarg = ObjUtil.GetAsFromSource(_componentTypeOnTarget.Type, _cachedTarg);
        }
        return DynamicUtil.GetMembers(_cachedTarg, includeNonPublic);
    }

    System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
    {
        if (_cachedTarg == null)
        {
            _cachedTarg = _target.GetTarget();
            if (_componentTypeOnTarget.Type != null)
                _cachedTarg = ObjUtil.GetAsFromSource(_componentTypeOnTarget.Type, _cachedTarg);
        }
        return DynamicUtil.GetMember(_cachedTarg, sMemberName, includeNonPublic);
    }

    #endregion


}
*/

}
