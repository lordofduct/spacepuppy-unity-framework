using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Scenario
{
    [System.Serializable()]
    public class TriggerTarget
    {

        #region Fields

        [SerializeField()]
        private float _weight;

        /**
         * These values are named like public properties because this was originally an internal class and they were public. 
         * But now it's a public class, and they need to be protected. BUT they are live and serialized out there by the original 
         * name. So to ensure backwards compatability we must keep these names.
         */

        [UnityEngine.Serialization.FormerlySerializedAs("Triggerable")]
        [SerializeField()]
        private Component _triggerable;

        [UnityEngine.Serialization.FormerlySerializedAs("TriggerableArgs")]
        [SerializeField()]
        private VariantReference[] _triggerableArgs;

        [UnityEngine.Serialization.FormerlySerializedAs("ActivationType")]
        [SerializeField()]
        private TriggerActivationType _activationType;

        [UnityEngine.Serialization.FormerlySerializedAs("MethodName")]
        [SerializeField()]
        private string _methodName;


        [System.NonSerialized()]
        private ITriggerableMechanism[] _triggerAllCache;

        #endregion

        #region Properties

        /// <summary>
        /// A value that can represent the probability weight of the TriggerTarget. This is used by Trigger when configured for probability.
        /// </summary>
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public GameObject Target { get { return (this._triggerable != null) ? _triggerable.gameObject : null; } }

        public TriggerActivationType ActivationType { get { return this._activationType; } }

        //public bool CanBlock
        //{
        //    get
        //    {
        //        switch(_activationType)
        //        {
        //            case TriggerActivationType.TriggerAllOnTarget:
        //                {
        //                    return _triggerable.HasComponent<IBlockingTriggerableMechanism>();
        //                }
        //            case TriggerActivationType.TriggerSelectedTarget:
        //                {
        //                    return _triggerable is IBlockingTriggerableMechanism;
        //                }
        //            case TriggerActivationType.SendMessage:
        //                {
        //                    return false;
        //                }
        //            case TriggerActivationType.CallMethodOnSelectedTarget:
        //                {
        //                    return false;
        //                }
        //            default:
        //                return false;
        //        }
        //    }
        //}

        #endregion

        #region Configure Methods

        public void Clear()
        {
            this._triggerable = null;
            this._triggerableArgs = null;
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureTriggerAll(GameObject targ, object arg = null)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this._triggerable = targ.transform;
            if(arg == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureTriggerAll(ITriggerableMechanism mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            this._triggerable = mechanism.transform;
            if (arg == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureTriggerTarget(ITriggerableMechanism mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            this._triggerable = mechanism.component;
            if (arg == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerSelectedTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureSendMessage(GameObject targ, string message, object arg = null)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this._triggerable = targ.transform;
            if (arg == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._methodName = message;
            this._activationType = TriggerActivationType.SendMessage;
            _triggerAllCache = null;
        }

        public void ConfigureCallMethod(GameObject targ, string methodName, params object[] args)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this._triggerable = targ.transform;
            if (args == null || args.Length == 0)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = (from a in args select new VariantReference(a)).ToArray();
            }
            this._methodName = methodName;
            this._activationType = TriggerActivationType.CallMethodOnSelectedTarget;
            _triggerAllCache = null;
        }

        #endregion

        #region Trigger Methods

        public void Trigger()
        {
            if (this._triggerable == null) return;

            var arg = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : null;
            this.Trigger_Imp(arg, null);
        }

        public void Trigger(object arg)
        {
            if (this._triggerable == null) return;

            var arg0 = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : arg;
            this.Trigger_Imp(arg0, null);
        }

        public void TriggerYielding(BlockingTriggerYieldInstruction instruction)
        {
            if (this._triggerable == null) return;

            var arg = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : null;
            this.Trigger_Imp(arg, instruction);
        }

        public void TriggerYielding(object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (this._triggerable == null) return;

            var arg0 = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : arg;
            this.Trigger_Imp(arg0, instruction);
        }

        private void Trigger_Imp(object arg, BlockingTriggerYieldInstruction instruction)
        {
            switch (this._activationType)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    if(_triggerAllCache == null)
                    {
                        //_triggerAllCache = (from t in this._triggerable.GetComponentsAlt<ITriggerableMechanism>() orderby t.Order ascending select t).ToArray();
                        _triggerAllCache = _triggerable.GetComponentsAlt<ITriggerableMechanism>();
                        System.Array.Sort(_triggerableArgs, MechanismComparer.Default);
                    }
                    if(instruction != null)
                    {
                        foreach (var t in _triggerAllCache)
                        {
                            if (t.component != null && t.CanTrigger)
                            {
                                if (t is IBlockingTriggerableMechanism)
                                    (t as IBlockingTriggerableMechanism).Trigger(arg, instruction);
                                else
                                    t.Trigger(arg);
                            }
                        }
                    }
                    else
                    {
                        foreach (var t in _triggerAllCache)
                        {
                            if (t.component != null && t.CanTrigger)
                            {
                                t.Trigger(arg);
                            }
                        }
                    }
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    if (_triggerable != null && _triggerable is ITriggerableMechanism)
                    {
                        if(instruction != null && _triggerable is IBlockingTriggerableMechanism)
                        {
                            var t = _triggerable as IBlockingTriggerableMechanism;
                            if (t.CanTrigger) t.Trigger(arg);
                        }
                        else
                        {
                            var t = _triggerable as ITriggerableMechanism;
                            if (t.CanTrigger) t.Trigger(arg);
                        }
                    }
                    break;
                case TriggerActivationType.SendMessage:
                    var go = GameObjectUtil.GetGameObjectFromSource(this._triggerable);
                    if (go != null && this._methodName != null)
                    {
                        go.SendMessage(this._methodName, arg, SendMessageOptions.DontRequireReceiver);
                    }
                    break;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    if (this._methodName != null)
                    {
                        //CallMethod does not support using the passed in arg
                        var args = (from a in this._triggerableArgs select (a != null) ? a.Value : null).ToArray();
                        DynamicUtil.InvokeMethod(this._triggerable, this._methodName, args);
                    }
                    break;
            }
        }
        

        #endregion



        #region Special Types

        private class MechanismComparer : System.Collections.IComparer
        {

            private static MechanismComparer _default;
            public static MechanismComparer Default
            {
                get
                {
                    if (_default == null) _default = new MechanismComparer();
                    return _default;
                }
            }


            public int Compare(object x, object y)
            {
                return (x as ITriggerableMechanism).Order.CompareTo((y as ITriggerableMechanism).Order);
            }
        }

        #endregion

    }

}
