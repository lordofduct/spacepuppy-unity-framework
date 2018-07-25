#pragma warning disable 0649 // variable declared but not used.
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
        System.Type GetTargetType();

        object GetTarget();
        object GetTarget(object arg);
    }

    [System.Serializable]
    public struct Proxy : IProxy
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

        public object GetTarget()
        {
            if (_searchBy == SearchBy.Nothing)
            {
                return (_target is IProxy) ? (_target as IProxy).GetTarget() : _target;
            }
            else
            {
                return ObjUtil.Find(_searchBy, _queryString);
            }
        }

        public object[] GetTargets()
        {
            if (_searchBy == SearchBy.Nothing)
            {
                return new object[] { (_target is IProxy) ? (_target as IProxy).GetTarget() : _target };
            }
            else
            {
                return ObjUtil.FindAll(_searchBy, _queryString);
            }
        }

        public T GetTarget<T>() where T : class
        {
            if (_searchBy == SearchBy.Nothing)
            {
                return ObjUtil.GetAsFromSource<T>(_target, true);
            }
            else
            {
                return ObjUtil.Find<T>(_searchBy, _queryString);
            }
        }

        public T[] GetTargets<T>() where T : class
        {
            if (_searchBy == SearchBy.Nothing)
            {
                var targ = ObjUtil.GetAsFromSource<T>(_target, true);
                return targ != null ? new T[] { targ } : ArrayUtil.Empty<T>();
            }
            else
            {
                return ObjUtil.FindAll<T>(_searchBy, _queryString);
            }
        }

        #endregion

        #region IProxy Interface

        object IProxy.GetTarget()
        {
            return this.GetTarget();
        }

        object IProxy.GetTarget(object arg)
        {
            return this.GetTarget();
        }

        public System.Type GetTargetType()
        {
            if (_target == null) return typeof(object);
            return (_target is IProxy) ? (_target as IProxy).GetTargetType() : _target.GetType();
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {

            public System.Type TargetType;
            public bool AllowProxy = true;

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

        [Space()]
        [SerializeField]
        [Tooltip("Cache the target when it's first retrieved. This is useful for speeding up any 'Find' commands if called repeatedly, but is hindered if the target is changing.")]
        private bool _cache;
        [System.NonSerialized]
        private UnityEngine.Object _object;

        [Space()]
        [SerializeField]
        private bool _treatAsTriggerable = true;
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

        public object GetTarget()
        {
            if(_cache)
            {
                if (_object != null) return _object;

                _object = _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), null) as UnityEngine.Object;
                return _object;
            }
            else
            {
                return _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), null) as UnityEngine.Object;
            }
        }

        public object GetTarget(object arg)
        {
            if (_cache)
            {
                if (_object != null) return _object;

                if (_componentTypeOnTarget == null) return null;
                _object = _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), arg) as UnityEngine.Object;
                return _object;
            }
            else
            {
                if (_componentTypeOnTarget == null) return null;
                return _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), arg) as UnityEngine.Object;
            }
        }

        public System.Type GetTargetType()
        {
            if(_componentTypeOnTarget.Type != null) return _componentTypeOnTarget.Type;
            return (_cache && _object != null) ? _object.GetType() : typeof(UnityEngine.Object);
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool CanTrigger
        {
            get { return _treatAsTriggerable && base.CanTrigger; }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this.GetTarget(arg);
            if (targ == null) return false;

            switch (_triggerAction)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    EventTriggerEvaluator.Current.TriggerAllOnTarget(targ, sender, arg);
                    return true;
                case TriggerActivationType.TriggerSelectedTarget:
                    EventTriggerEvaluator.Current.TriggerSelectedTarget(targ, sender, arg);
                    return true;
                case TriggerActivationType.DestroyTarget:
                    EventTriggerEvaluator.Current.DestroyTarget(targ);
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

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            return DynamicUtil.GetMemberNames(this.GetTarget(), includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            return DynamicUtil.GetMember(this.GetTarget(), sMemberName, includeNonPublic);
        }

        #endregion

    }

    [CreateAssetMenu(fileName = "TargetProxy", menuName = "Spacepuppy/TargetProxy")]
    public class TargetProxyToken : ScriptableObject, IProxy
    {

        #region Fields

        [SerializeField]
        private TriggerableTargetObject _target = new TriggerableTargetObject(TriggerableTargetObject.FindCommand.FindInScene, TriggerableTargetObject.ResolveByCommand.Nothing, string.Empty);
        [SerializeField]
        [TypeReference.Config(typeof(Component), allowAbstractClasses = true, allowInterfaces = true)]
        private TypeReference _componentTypeOnTarget = new TypeReference();

        [Space()]
        [SerializeField]
        [Tooltip("Cache the target when it's first retrieved. This is useful for speeding up any 'Find' commands if called repeatedly, but is hindered if the target is changing.")]
        private bool _cache;
        [System.NonSerialized]
        private UnityEngine.Object _object;

        #endregion

        #region IProxy Interface

        public object GetTarget()
        {
            if (_cache)
            {
                if (_object != null) return _object;

                _object = _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), null) as UnityEngine.Object;
                return _object;
            }
            else
            {
                return _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), null) as UnityEngine.Object;
            }
        }

        public object GetTarget(object arg)
        {
            if (_cache)
            {
                if (_object != null) return _object;

                if (_componentTypeOnTarget == null) return null;
                _object = _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), arg) as UnityEngine.Object;
                return _object;
            }
            else
            {
                if (_componentTypeOnTarget == null) return null;
                return _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), arg) as UnityEngine.Object;
            }
        }

        public System.Type GetTargetType()
        {
            if (_componentTypeOnTarget.Type != null) return _componentTypeOnTarget.Type;
            return (_cache && _object != null) ? _object.GetType() : typeof(UnityEngine.Object);
        }

        #endregion

    }


    [CreateAssetMenu(fileName = "ProxyMediator", menuName = "Spacepuppy/ProxyMediator")]
    public class ProxyMediator : ScriptableObject, ITriggerableMechanism
    {

        public System.EventHandler OnTriggered;

        public void Trigger()
        {
            if (this.OnTriggered != null) this.OnTriggered(this, System.EventArgs.Empty);
        }

        #region ITriggerableMechanism Interface

        bool ITriggerableMechanism.CanTrigger
        {
            get
            {
                return true;
            }
        }

        int ITriggerableMechanism.Order
        {
            get
            {
                return 0;
            }
        }

        bool ITriggerableMechanism.Trigger(object sender, object arg)
        {
            this.Trigger();
            return true;
        }

        #endregion

    }

    public class t_OnProxyMediatorTriggered : TriggerComponent
    {

        [SerializeField]
        private ProxyMediator _mediator;

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            base.OnEnable();

            if(_mediator != null)
            {
                _mediator.OnTriggered += this.OnMediatorTriggered;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if(_mediator != null)
            {
                _mediator.OnTriggered -= this.OnMediatorTriggered;
            }
        }

        #endregion

        #region Properties

        public ProxyMediator Mediator
        {
            get { return _mediator; }
            set
            {
                if (_mediator == value) return;

                if(Application.isPlaying && this.enabled)
                {
                    if(_mediator != null) _mediator.OnTriggered -= this.OnMediatorTriggered;
                    _mediator = value;
                    if (_mediator != null) _mediator.OnTriggered += this.OnMediatorTriggered;
                }
                else
                {
                    _mediator = value;
                }
            }
        }

        #endregion

        #region Methods

        private void OnMediatorTriggered(object sender, System.EventArgs e)
        {
            this.ActivateTrigger(null);
        }

        #endregion

    }

}
