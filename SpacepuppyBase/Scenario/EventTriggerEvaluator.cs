using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public sealed class EventTriggerEvaluator : EventTriggerEvaluator.IEvaluator
    {

        #region Singleton Interface

        private static EventTriggerEvaluator _default = new EventTriggerEvaluator();
        private static IEvaluator _evaluator;

        public static EventTriggerEvaluator Default
        {
            get { return _default; }
        }

        public static IEvaluator Current
        {
            get { return _evaluator; }
        }

        public static void SetCurrentEvaluator(IEvaluator ev)
        {
            _evaluator = ev ?? _default;
        }

        static EventTriggerEvaluator()
        {
            _evaluator = _default;
        }
        
        #endregion
        
        #region Methods

        private ITriggerableMechanism[] GetCache(GameObject go)
        {
            //we don't trigger inactive GameObjects unless they are prefabs

            EventTriggerCache cache;
            if (go.activeInHierarchy)
            {
                cache = go.AddOrGetComponent<EventTriggerCache>();
                return cache.Targets ?? cache.RefreshCache();
            }
            else if (go.HasComponent<PrefabToken>())
            {
                cache = go.GetComponent<EventTriggerCache>();
                if (cache != null) return cache.Targets ?? cache.RefreshCache();

                return go.GetComponents<ITriggerableMechanism>();
            }
            else
            {
                return ArrayUtil.Empty<ITriggerableMechanism>();
            }
        }


        public void GetAllTriggersOnTarget(object target, List<ITriggerableMechanism> outputColl)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null)
            {
                outputColl.AddRange(this.GetCache(go));
            }
            else if (target is ITriggerableMechanism)
                outputColl.Add(target as ITriggerableMechanism);
        }

        public void TriggerAllOnTarget(object target, object sender, object arg, BlockingTriggerYieldInstruction instruction = null)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null)
            {
                var arr = this.GetCache(go);

                if (instruction != null)
                {
                    foreach (var t in arr)
                    {
                        if (t.CanTrigger)
                        {
                            if (t is IBlockingTriggerableMechanism)
                                (t as IBlockingTriggerableMechanism).Trigger(sender, arg, instruction);
                            else
                                t.Trigger(sender, arg);
                        }
                    }
                }
                else
                {
                    foreach (var t in arr)
                    {
                        if (t.CanTrigger)
                        {
                            t.Trigger(sender, arg);
                        }
                    }
                }
            }
            else
            {
                var targ = target as ITriggerableMechanism;
                if (targ != null)
                {
                    if (instruction != null)
                    {
                        if (targ.CanTrigger)
                        {
                            if (targ is IBlockingTriggerableMechanism)
                                (targ as IBlockingTriggerableMechanism).Trigger(sender, arg, instruction);
                            else
                                targ.Trigger(sender, arg);
                        }
                    }
                    else
                    {
                        if (targ.CanTrigger)
                        {
                            targ.Trigger(sender, arg);
                        }
                    }
                }
            }
        }

        public void TriggerSelectedTarget(object target, object sender, object arg, BlockingTriggerYieldInstruction instruction = null)
        {
            if (target != null && target is ITriggerableMechanism)
            {
                if (instruction != null && target is IBlockingTriggerableMechanism)
                {
                    var t = target as IBlockingTriggerableMechanism;
                    if (t.CanTrigger) t.Trigger(sender, arg);
                }
                else
                {
                    var t = target as ITriggerableMechanism;
                    if (t.CanTrigger) t.Trigger(sender, arg);
                }
            }
        }

        public void SendMessageToTarget(object target, string message, object arg)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null && message != null)
            {
                go.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
            }
        }

        public void CallMethodOnSelectedTarget(object target, string methodName, VariantReference[] methodArgs)
        {
            if (methodName != null)
            {
                //CallMethod does not support using the passed in arg
                //var args = (from a in this._triggerableArgs select (a != null) ? a.Value : null).ToArray();

                object[] args = null;
                if (methodArgs != null && methodArgs.Length > 0)
                {
                    args = new object[methodArgs.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (methodArgs[i] != null) args[i] = methodArgs[i].Value;
                    }
                }

                if (args != null && args.Length == 1)
                {
                    DynamicUtil.SetValue(target, methodName, args[0]);
                }
                else
                {
                    DynamicUtil.InvokeMethod(target, methodName, args);
                }
            }
        }

        public void EnableTarget(object target, EnableMode mode)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null)
            {
                switch (mode)
                {
                    case EnableMode.Disable:
                        go.SetActive(false);
                        break;
                    case EnableMode.Enable:
                        go.SetActive(true);
                        break;
                    case EnableMode.Toggle:
                        go.SetActive(!go.activeSelf);
                        break;
                }
            }
        }

        public void DestroyTarget(object target)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null)
            {
                ObjUtil.SmartDestroy(go);
            }
            else if (target is UnityEngine.Object)
            {
                ObjUtil.SmartDestroy(target as UnityEngine.Object);
            }
        }

        #endregion

        #region Special Types

        private class EventTriggerCache : MonoBehaviour
        {

            #region Fields

            private ITriggerableMechanism[] _targets;

            #endregion

            #region Properties

            public ITriggerableMechanism[] Targets
            {
                get { return _targets; }
            }

            #endregion

            #region Methods

            private void Awake()
            {
                this.RefreshCache();
            }

            public ITriggerableMechanism[] RefreshCache()
            {
                _targets = this.gameObject.GetComponents<ITriggerableMechanism>();
                if (_targets.Length > 1)
                    System.Array.Sort(_targets, TriggerableMechanismOrderComparer.Default);
                return _targets;
            }

            #endregion

        }

        public interface IEvaluator
        {

            void GetAllTriggersOnTarget(object target, List<ITriggerableMechanism> outputColl);

            void TriggerAllOnTarget(object target, object sender, object arg, BlockingTriggerYieldInstruction instruction = null);
            void TriggerSelectedTarget(object target, object sender, object arg, BlockingTriggerYieldInstruction instruction = null);
            void SendMessageToTarget(object target, string message, object arg);
            void CallMethodOnSelectedTarget(object target, string methodName, VariantReference[] methodArgs);
            void EnableTarget(object target, EnableMode mode);
            void DestroyTarget(object target);

        }

        #endregion

    }

}
