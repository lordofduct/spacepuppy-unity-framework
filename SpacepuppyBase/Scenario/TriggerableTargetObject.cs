using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public class TriggerableTargetObject
    {

        #region Fields

        [SerializeField()]
        private TargetSource _source;
        [SerializeField()]
        private UnityEngine.Object _target;

        #endregion

        #region CONSTRUCTOR

        public TriggerableTargetObject()
        {
        }

        public TriggerableTargetObject(TargetSource src)
        {
            _source = src;
        }

        #endregion

        #region Methods

        public T GetTarget<T>(object triggerArg, bool searchEntity = true) where T : class
        {
            return GetTarget_Imp(typeof(T), triggerArg, searchEntity) as T;
        }

        public object GetTarget(System.Type tp, object triggerArg, bool searchEntity = true)
        {
            if (tp == null || !TypeUtil.IsType(tp, typeof(UnityEngine.Object))) throw new TypeArgumentMismatchException(tp, typeof(UnityEngine.Object), "tp");

            return GetTarget_Imp(tp, triggerArg, searchEntity);
        }

        private object GetTarget_Imp(System.Type tp, object triggerArg, bool searchEntity = true)
        {
            var targ = this.ReduceTarget(triggerArg);

            if (targ != null && TypeUtil.IsType(targ.GetType(), tp))
            {
                return targ;
            }
            else
            {
                if (searchEntity)
                {
                    if (tp == typeof(GameObject))
                    {
                        if (GameObjectUtil.IsGameObjectSource(targ))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(targ).FindRoot();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (TypeUtil.IsType(tp, typeof(Component)))
                    {
                        if (GameObjectUtil.IsGameObjectSource(targ))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(targ).FindComponent(tp);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (TypeUtil.IsType(tp, typeof(IComponent)))
                    {
                        if (GameObjectUtil.IsGameObjectSource(targ))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(targ).FindLikeComponent(tp);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (tp == typeof(GameObject))
                    {
                        if (GameObjectUtil.IsGameObjectSource(targ))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(targ);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (TypeUtil.IsType(tp, typeof(Component)))
                    {
                        if (GameObjectUtil.IsGameObjectSource(targ))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(targ).GetComponent(tp);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (TypeUtil.IsType(tp, typeof(IComponent)))
                    {
                        if (GameObjectUtil.IsGameObjectSource(targ))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(targ).GetFirstLikeComponent(tp);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private UnityEngine.Object ReduceTarget(object triggerArg)
        {
            switch (_source)
            {
                case TargetSource.TriggerArg:
                    return (triggerArg is UnityEngine.Object) ? triggerArg as UnityEngine.Object : null;
                case TargetSource.Self:
                case TargetSource.Root:
                case TargetSource.Configurable:
                    return _target;
            }

            return null;
        }

        #endregion

        #region Special Types

        public enum TargetSource
        {
            TriggerArg = 0,
            Self = 1,
            Root = 2,
            Configurable = 3
        }

        public class ConfigAttribute : System.Attribute
        {

            public System.Type TargetType;

            public ConfigAttribute()
            {
                this.TargetType = typeof(GameObject);
            }

            public ConfigAttribute(System.Type targetType)
            {
                if (targetType == null || 
                    (!TypeUtil.IsType(targetType, typeof(UnityEngine.Object)) && !TypeUtil.IsType(targetType, typeof(IComponent)))) throw new TypeArgumentMismatchException(targetType, typeof(UnityEngine.Object), "targetType");

                this.TargetType = targetType;
            }

        }

        #endregion

    }
}
