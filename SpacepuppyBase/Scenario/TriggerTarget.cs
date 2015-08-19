using UnityEngine;
using System.Collections.Generic;
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
        }

        #endregion

        #region Trigger Methods

        public void Trigger()
        {
            if (this._triggerable == null) return;

            var arg0 = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : null;
            switch (this._activationType)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    foreach (var t in (from t in this._triggerable.GetComponentsAlt<ITriggerableMechanism>() orderby t.Order ascending select t))
                    {
                        t.Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    if (this._triggerable is ITriggerableMechanism)
                    {
                        (this._triggerable as ITriggerableMechanism).Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.SendMessage:
                    var go = GameObjectUtil.GetGameObjectFromSource(this._triggerable);
                    if (go != null && this._methodName != null)
                    {
                        go.SendMessage(this._methodName, arg0, SendMessageOptions.DontRequireReceiver);
                    }
                    break;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    if (this._methodName != null)
                    {
                        //CallMethod does not support using the passed in arg
                        var args = (this._triggerableArgs != null) ? (from a in this._triggerableArgs select (a != null) ? a.Value : null).ToArray() : null;
                        DynamicUtil.InvokeMethod(this._triggerable, this._methodName, args);
                    }
                    break;
            }
        }

        public void Trigger(object arg)
        {
            if (this._triggerable == null) return;

            var arg0 = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : arg;
            switch (this._activationType)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    foreach (var t in (from t in this._triggerable.GetComponentsAlt<ITriggerableMechanism>() orderby t.Order ascending select t))
                    {
                        t.Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    if (this._triggerable is ITriggerableMechanism)
                    {
                        (this._triggerable as ITriggerableMechanism).Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.SendMessage:
                    var go = GameObjectUtil.GetGameObjectFromSource(this._triggerable);
                    if (go != null && this._methodName != null)
                    {
                        go.SendMessage(this._methodName, arg0, SendMessageOptions.DontRequireReceiver);
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

    }

}
