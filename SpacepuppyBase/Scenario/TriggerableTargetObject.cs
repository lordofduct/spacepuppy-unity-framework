using UnityEngine;
using System.Collections.Generic;
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
            Nothing = SearchBy.Nothing,
            WithTag = SearchBy.Tag,
            WithName = SearchBy.Name,
            WithType = SearchBy.Type
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

        public FindCommand Find
        {
            get { return _find; }
            set { _find = value; }
        }

        public ResolveByCommand ResolveBy
        {
            get { return _resolveBy; }
            set { _resolveBy = value; }
        }

        public string ResolveByQuery
        {
            get { return _queryString; }
            set { _queryString = value; }
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
        
        public IEnumerable<T> GetTargets<T>(object triggerArg) where T : class
        {
            foreach(var obj in this.ReduceTargets(triggerArg))
            {
                if (obj == null) continue;

                var result = ObjUtil.GetAsFromSource<T>(obj);
                if (result == null && !_configured && obj == triggerArg && ComponentUtil.IsAcceptableComponentType(typeof(T)))
                {
                    //if not configured, and the triggerArg didn't reduce properly, lets search the entity of the 'triggerArg'
                    var go = GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                    if (go == null) continue;
                    result = go.FindComponent<T>();
                }
                if (result != null) yield return result;
            }
        }

        public System.Collections.IEnumerable GetTargets(System.Type tp, object triggerArg)
        {
            foreach (var obj in this.ReduceTargets(triggerArg))
            {
                if (obj == null) continue;

                var result = ObjUtil.GetAsFromSource(tp, obj);
                if (result == null && !_configured && obj == triggerArg && ComponentUtil.IsAcceptableComponentType(tp))
                {
                    //if not configured, and the triggerArg didn't reduce properly, lets search the entity of the 'triggerArg'
                    var go = GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                    if (go == null) continue;
                    result = go.FindComponent(tp);
                }
                if (result != null) yield return result;
            }
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
                            case ResolveByCommand.WithType:
                                return ObjUtil.GetAsFromSource(TypeUtil.FindType(_queryString), GameObjectUtil.GetGameObjectFromSource(obj)) != null ? obj : null;
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
                            case ResolveByCommand.WithType:
                                {
                                    var tp = TypeUtil.FindType(_queryString);
                                    foreach (var p in GameObjectUtil.GetParents(trans))
                                    {
                                        if (ObjUtil.GetAsFromSource(tp, p) != null) return p;
                                    }
                                    return null;
                                }
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
                                return (trans.childCount > 0) ? trans.GetChild(0) : null;
                            case ResolveByCommand.WithTag:
                                if (trans.childCount > 0)
                                {
                                    foreach (Transform child in GameObjectUtil.GetAllChildren(trans))
                                    {
                                        if (child.HasTag(_queryString)) return child;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithName:
                                if (trans.childCount > 0)
                                {
                                    foreach (Transform child in GameObjectUtil.GetAllChildren(trans))
                                    {
                                        if (child.CompareName(_queryString)) return child;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithType:
                                if(trans.childCount > 0)
                                {
                                    var tp = TypeUtil.FindType(_queryString);
                                    foreach (Transform child in GameObjectUtil.GetAllChildren(trans))
                                    {
                                        if (ObjUtil.GetAsFromSource(tp, child) != null) return child;
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
                            case ResolveByCommand.WithType:
                                {
                                    var tp = TypeUtil.FindType(_queryString);
                                    foreach (var t in GameObjectUtil.GetAllChildrenAndSelf(entity))
                                    {
                                        if (ObjUtil.GetAsFromSource(tp, t) != null) return t;
                                    }
                                    return null;
                                }
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
                                return GameObjectUtil.FindWithMultiTag(_queryString);
                            case ResolveByCommand.WithName:
                                return GameObject.Find(_queryString);
                            case ResolveByCommand.WithType:
                                return ObjUtil.Find(SearchBy.Type, _queryString);
                        }
                    }
                    break;
            }

            return null;
        }

        private System.Collections.IEnumerable ReduceTargets(object triggerArg)
        {
            switch (_find)
            {
                case FindCommand.Direct:
                    {
                        object obj = (_configured) ? _target : triggerArg;
                        if (obj == null) yield break;
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                yield return obj;
                                break;
                            case ResolveByCommand.WithTag:
                                {
                                    var go = GameObjectUtil.GetGameObjectFromSource(obj);
                                    if (go.HasTag(_queryString)) yield return obj;
                                }
                                break;
                            case ResolveByCommand.WithName:
                                {
                                    var go = GameObjectUtil.GetGameObjectFromSource(obj);
                                    if (go.CompareName(_queryString)) yield return obj;
                                }
                                break;
                            case ResolveByCommand.WithType:
                                {
                                    if (ObjUtil.GetAsFromSource(TypeUtil.FindType(_queryString), GameObjectUtil.GetGameObjectFromSource(obj)) != null)
                                        yield return obj;
                                }
                                break;
                        }
                    }
                    break;
                case FindCommand.FindParent:
                    {
                        Transform trans = GameObjectUtil.GetTransformFromSource((_configured) ? _target : triggerArg);
                        if (trans == null) yield break;
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                {
                                    var t = trans.parent;
                                    if (t != null) yield return t;
                                }
                                break;
                            case ResolveByCommand.WithTag:
                                {
                                    foreach(var p in GameObjectUtil.GetParents(trans))
                                    {
                                        if (p.HasTag(_queryString)) yield return p;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithName:
                                {
                                    foreach (var p in GameObjectUtil.GetParents(trans))
                                    {
                                        if (p.CompareName(_queryString)) yield return p;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithType:
                                {
                                    var tp = TypeUtil.FindType(_queryString);
                                    foreach (var p in GameObjectUtil.GetParents(trans))
                                    {
                                        if (ObjUtil.GetAsFromSource(tp, p) != null) yield return p;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case FindCommand.FindInChildren:
                    {
                        Transform trans = GameObjectUtil.GetTransformFromSource((_configured) ? _target : triggerArg);
                        if (trans == null) yield break;
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                if (trans.childCount > 0) yield return trans.GetChild(0);
                                break;
                            case ResolveByCommand.WithTag:
                                if (trans.childCount > 0)
                                {
                                    foreach (Transform child in GameObjectUtil.GetAllChildren(trans))
                                    {
                                        if (child.HasTag(_queryString)) yield return child;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithName:
                                if (trans.childCount > 0)
                                {
                                    foreach (Transform child in GameObjectUtil.GetAllChildren(trans))
                                    {
                                        if (child.CompareName(_queryString)) yield return child;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithType:
                                if (trans.childCount > 0)
                                {
                                    var tp = TypeUtil.FindType(_queryString);
                                    foreach (Transform child in GameObjectUtil.GetAllChildren(trans))
                                    {
                                        if (ObjUtil.GetAsFromSource(tp, child) != null) yield return child;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case FindCommand.FindInEntity:
                    {
                        GameObject entity = GameObjectUtil.GetRootFromSource((_configured) ? _target : triggerArg);
                        if (entity == null) yield break; ;

                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                yield return entity;
                                break;
                            case ResolveByCommand.WithTag:
                                {
                                    foreach(var o in entity.FindAllWithMultiTag(_queryString))
                                    {
                                        yield return o;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithName:
                                {
                                    foreach(var o in GameObjectUtil.FindAllByName(entity.transform, _queryString))
                                    {
                                        yield return o;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithType:
                                {
                                    var tp = TypeUtil.FindType(_queryString);
                                    foreach (var t in GameObjectUtil.GetAllChildrenAndSelf(entity))
                                    {
                                        if (ObjUtil.GetAsFromSource(tp, t) != null) yield return t;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case FindCommand.FindInScene:
                    {
                        switch (_resolveBy)
                        {
                            case ResolveByCommand.Nothing:
                                {
                                    var go = GameObjectUtil.GetGameObjectFromSource((_configured) ? _target : triggerArg);
                                    if (go != null) yield return go;
                                }
                                break;
                            case ResolveByCommand.WithTag:
                                {
                                    foreach(var o in GameObjectUtil.FindGameObjectsWithMultiTag(_queryString))
                                    {
                                        yield return o;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithName:
                                {
                                    foreach (var o in GameObjectUtil.FindAllByName(_queryString))
                                    {
                                        yield return o;
                                    }
                                }
                                break;
                            case ResolveByCommand.WithType:
                                {
                                    foreach (var o in ObjUtil.FindAll(SearchBy.Type, _queryString))
                                    {
                                        yield return o;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        #endregion
        
        #region Special Types

        public class ConfigAttribute : System.Attribute
        {

            public System.Type TargetType;
            public bool SearchChildren;

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
