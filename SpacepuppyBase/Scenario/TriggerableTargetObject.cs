using UnityEngine;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public class TriggerableTargetObject
    {
        
        public enum FindCommand
        {
            Direct = 0,
            FindParent = 1,
            FindInChildren = 2,
            FindInEntity = 3,
            FindInScene = 4
        }

        public enum ResolveByCommand
        {
            Nothing = 0,
            WithTag = 1,
            WithName = 2
        }

        #region Fields
        
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("_source")]
        private bool _configured = true;
        [SerializeField()]
        private UnityEngine.Object _target;
        [SerializeField()]
        private FindCommand _find;
        [SerializeField()]
        private ResolveByCommand _resolveBy;
        [SerializeField()]
        private string _queryString;
        
        #endregion

        #region CONSTRUCTOR

        public TriggerableTargetObject()
        {
        }

        public TriggerableTargetObject(bool defaultTriggerArg)
        {
            _configured = !defaultTriggerArg;
        }

        #endregion

        #region Properties

        public bool TargetsTriggerArg
        {
            get { return !_configured; }
            set
            {
                _configured = !value;
            }
        }

        public UnityEngine.Object Target
        {
            get { return _target; }
            set { _target = value; }
        }
        
        #endregion

        #region Methods

        public T GetTarget<T>(object triggerArg) where T : class
        {
            var obj = this.ReduceTarget(triggerArg);
            if (obj == null) return null;

            var result = ObjUtil.GetAsFromSource<T>(obj);
            if(result == null && !_configured && ComponentUtil.IsAcceptableComponentType(typeof(T)))
            {
                //if not configured, and the triggerArg didn't reduce properly, lets search the entity of the 'triggerArg'
                var go = GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                if (go == null) return null;
                result = go.FindComponent<T>();
            }
            return result;
        }

        public object GetTarget(System.Type tp, object triggerArg)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            
            var obj = this.ReduceTarget(triggerArg);
            if (obj == null) return null;

            var result = ObjUtil.GetAsFromSource(tp, obj);
            if(result == null && !_configured && ComponentUtil.IsAcceptableComponentType(tp))
            {
                //if not configured, and the triggerArg didn't reduce properly, lets search the entity of the 'triggerArg'
                var go = GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                if (go == null) return null;
                result = go.FindComponent(tp);
            }
            return result;
        }


        private object ReduceTarget(object triggerArg)
        {
            switch (_find)
            {
                case FindCommand.Direct:
                    {
                        object obj = (_configured) ? _target : triggerArg;
                        if (obj == null) return null;
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                return obj;
                            case ResolveByCommand.WithTag:
                                return GameObjectUtil.GetGameObjectFromSource(obj).HasTag(_queryString) ? obj : null;
                            case ResolveByCommand.WithName:
                                return GameObjectUtil.GetGameObjectFromSource(obj).CompareName(_queryString) ? obj : null;
                        }
                    }
                    break;
                case FindCommand.FindParent:
                    {
                        Transform trans = GameObjectUtil.GetTransformFromSource((_configured) ? _target : triggerArg);
                        if (trans == null) return null;
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                return trans.parent;
                            case ResolveByCommand.WithTag:
                                return trans.FindParentWithTag(_queryString);
                            case ResolveByCommand.WithName:
                                return trans.FindParentWithName(_queryString);
                        }
                    }
                    break;
                case FindCommand.FindInChildren:
                    {
                        Transform trans = GameObjectUtil.GetTransformFromSource((_configured) ? _target : triggerArg);
                        if (trans == null) return null;
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                if (trans.childCount > 0)
                                {
                                    foreach (Transform child in trans)
                                    {
                                        return child;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithTag:
                                if (trans.childCount > 0)
                                {
                                    foreach (Transform child in trans)
                                    {
                                        if (child.HasTag(_queryString)) return child;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithName:
                                if (trans.childCount > 0)
                                {
                                    foreach (Transform child in trans)
                                    {
                                        if (child.CompareName(_queryString)) return child;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case FindCommand.FindInEntity:
                    {
                        GameObject entity = GameObjectUtil.GetRootFromSource((_configured) ? _target : triggerArg);
                        if (entity == null) return null;

                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                return entity;
                            case ResolveByCommand.WithTag:
                                return entity.FindWithMultiTag(_queryString);
                            case ResolveByCommand.WithName:
                                return entity.FindByName(_queryString);
                        }
                    }
                    break;
                case FindCommand.FindInScene:
                    {
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                return GameObjectUtil.GetGameObjectFromSource((_configured) ? _target : triggerArg);
                            case ResolveByCommand.WithTag:
                                return GameObjectUtil.FindGameObjectsWithMultiTag(_queryString).FirstOrDefault();
                            case ResolveByCommand.WithName:
                                return GameObject.Find(_queryString);
                        }
                    }
                    break;
            }

            return null;
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
