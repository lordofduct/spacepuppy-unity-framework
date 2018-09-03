using UnityEngine;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public class TriggerTarget
    {

        #region Fields

        [SerializeField()]
        private float _weight = 1f;

        /**
         * These values are named like public properties because this was originally an internal class and they were public. 
         * But now it's a public class, and they need to be protected. BUT they are live and serialized out there by the original 
         * name. So to ensure backwards compatability we must keep these names.
         */

        [UnityEngine.Serialization.FormerlySerializedAs("Triggerable")]
        [SerializeField()]
        private UnityEngine.Object _triggerable;

        [UnityEngine.Serialization.FormerlySerializedAs("TriggerableArgs")]
        [SerializeField()]
        private VariantReference[] _triggerableArgs;

        [UnityEngine.Serialization.FormerlySerializedAs("ActivationType")]
        [SerializeField()]
        private TriggerActivationType _activationType;

        [UnityEngine.Serialization.FormerlySerializedAs("MethodName")]
        [SerializeField()]
        private string _methodName;
        
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

        //public GameObject Target { get { return (this._triggerable != null) ? _triggerable.gameObject : null; } }
        public UnityEngine.Object Target { get { return _triggerable; } }

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
        }

        public void ConfigureTriggerAll(GameObject targ, object arg = null)
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
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
        }

        public void ConfigureTriggerAll(ITriggerableMechanism mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            if (GameObjectUtil.IsGameObjectSource(mechanism))
                _triggerable = GameObjectUtil.GetGameObjectFromSource(mechanism).transform;
            else
                _triggerable = mechanism as UnityEngine.Object;
            if (arg == null || _triggerable == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
        }

        public void ConfigureTriggerAll(UnityEngine.Object targ, object arg = null)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            if(GameObjectUtil.IsGameObjectSource(targ))
            {
                this.ConfigureTriggerAll(GameObjectUtil.GetGameObjectFromSource(targ));
                return;
            }
            else if (!TriggerTarget.IsValidTriggerTarget(targ, TriggerActivationType.TriggerAllOnTarget))
            {
                throw new System.ArgumentException("Must be a game object source of some sort.", "targ");
            }

            this._triggerable = targ;
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
        }

        public void ConfigureTriggerTarget(ITriggerableMechanism mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            
            this._triggerable = mechanism as UnityEngine.Object;
            if (arg == null || _triggerable == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerSelectedTarget;
            this._methodName = null;
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
        }

        public void ConfigureCallMethod(UnityEngine.Object targ, string methodName, params object[] args)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this._triggerable = targ;
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
        }

        public object CalculateTarget(object arg)
        {
            return (_triggerable is IProxy) ? (_triggerable as IProxy).GetTarget(arg) : _triggerable;
        }

        #endregion

        #region Trigger Methods

        [System.Obsolete()]
        public void Trigger(object sender)
        {
            if (this._triggerable == null) return;

            var arg = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : null;
            this.Trigger_Imp(sender, null, arg, null);
        }

        [System.Obsolete()]
        public void TriggerYielding(object sender, BlockingTriggerYieldInstruction instruction)
        {
            if (this._triggerable == null) return;

            var arg = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : null;
            this.Trigger_Imp(sender, null, arg, instruction);
        }




        public void Trigger(object sender, object arg)
        {
            if (this._triggerable == null) return;

            var arg0 = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : arg;
            this.Trigger_Imp(sender, arg, arg0, null);
        }

        public void TriggerYielding(object sender, object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (this._triggerable == null) return;

            var arg0 = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : arg;
            this.Trigger_Imp(sender, arg, arg0, instruction);
        }
        
        private void Trigger_Imp(object sender, object incomingArg, object outgoingArg, BlockingTriggerYieldInstruction instruction)
        {
            try
            {
                switch (this._activationType)
                {
                    case TriggerActivationType.TriggerAllOnTarget:
                        {
                            EventTriggerEvaluator.Current.TriggerAllOnTarget((_triggerable is IProxy) ? (_triggerable as IProxy).GetTarget(incomingArg) : _triggerable, 
                                                                             sender, outgoingArg, instruction);
                        }
                        break;
                    case TriggerActivationType.TriggerSelectedTarget:
                        {
                            EventTriggerEvaluator.Current.TriggerSelectedTarget((_triggerable is IProxy) ? (_triggerable as IProxy).GetTarget(incomingArg) : _triggerable,
                                                                                sender, outgoingArg, instruction);
                        }
                        break;
                    case TriggerActivationType.SendMessage:
                        {
                            EventTriggerEvaluator.Current.SendMessageToTarget((_triggerable is IProxy) ? (_triggerable as IProxy).GetTarget(incomingArg) : _triggerable,
                                                                              _methodName, outgoingArg);
                        }
                        break;
                    case TriggerActivationType.CallMethodOnSelectedTarget:
                        {
                            EventTriggerEvaluator.Current.CallMethodOnSelectedTarget((_triggerable is IProxy) ? (_triggerable as IProxy).GetTarget(incomingArg) : _triggerable,
                                                                                     _methodName, _triggerableArgs);
                        }
                        break;
                    case TriggerActivationType.EnableTarget:
                        {
                            EventTriggerEvaluator.Current.EnableTarget((_triggerable is IProxy) ? (_triggerable as IProxy).GetTarget(incomingArg) : _triggerable,
                                                                       ConvertUtil.ToEnum<EnableMode>(_methodName));
                        }
                        break;
                    case TriggerActivationType.DestroyTarget:
                        {
                            EventTriggerEvaluator.Current.DestroyTarget((_triggerable is IProxy) ? (_triggerable as IProxy).GetTarget(incomingArg) : _triggerable);
                        }
                        break;
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex, sender as UnityEngine.Object);
            }
        }

        #endregion


        #region Static Utils

        public static bool IsValidTriggerTarget(UnityEngine.Object obj, TriggerActivationType act)
        {
            if (obj == null) return true;

            switch (act)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                case TriggerActivationType.TriggerSelectedTarget:
                    return (GameObjectUtil.IsGameObjectSource(obj) || obj is ITriggerableMechanism || obj is IProxy);
                case TriggerActivationType.SendMessage:
                    return GameObjectUtil.IsGameObjectSource(obj) || obj is IProxy;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    return true;
                case TriggerActivationType.EnableTarget:
                case TriggerActivationType.DestroyTarget:
                    return GameObjectUtil.IsGameObjectSource(obj) || obj is IProxy;
            }

            return false;
        }

        #endregion

    }

}
