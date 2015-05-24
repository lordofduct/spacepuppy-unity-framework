using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        [SerializeField()]
        private Component Triggerable;
        [SerializeField()]
        private VariantReference[] TriggerableArgs;
        [SerializeField()]
        private TriggerActivationType ActivationType;
        [SerializeField()]
        private string MethodName;

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

        public GameObject Target { get { return (this.Triggerable != null) ? Triggerable.gameObject : null; } }

        public TriggerActivationType ActivationMode { get { return this.ActivationType; } }

        #endregion

        #region Configure Methods

        public void Clear()
        {
            this.Triggerable = null;
            this.TriggerableArgs = null;
            this.ActivationType = TriggerActivationType.TriggerAllOnTarget;
            this.MethodName = null;
        }

        public void ConfigureTriggerAll(GameObject targ, object arg = null)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this.Triggerable = targ.transform;
            if(arg == null)
            {
                this.TriggerableArgs = null;
            }
            else
            {
                this.TriggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this.ActivationType = TriggerActivationType.TriggerAllOnTarget;
            this.MethodName = null;
        }

        public void ConfigureTriggerAll(ITriggerableMechanism mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            this.Triggerable = mechanism.transform;
            if (arg == null)
            {
                this.TriggerableArgs = null;
            }
            else
            {
                this.TriggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this.ActivationType = TriggerActivationType.TriggerAllOnTarget;
            this.MethodName = null;
        }

        public void ConfigureTriggerTarget(ITriggerableMechanism mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            this.Triggerable = mechanism.component;
            if (arg == null)
            {
                this.TriggerableArgs = null;
            }
            else
            {
                this.TriggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this.ActivationType = TriggerActivationType.TriggerSelectedTarget;
            this.MethodName = null;
        }

        public void ConfigureSendMessage(GameObject targ, string message, object arg = null)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this.Triggerable = targ.transform;
            if (arg == null)
            {
                this.TriggerableArgs = null;
            }
            else
            {
                this.TriggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this.MethodName = message;
            this.ActivationType = TriggerActivationType.SendMessage;
        }

        public void ConfigureCallMethod(GameObject targ, string methodName, params object[] args)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this.Triggerable = targ.transform;
            if (args == null || args.Length == 0)
            {
                this.TriggerableArgs = null;
            }
            else
            {
                this.TriggerableArgs = (from a in args select new VariantReference(a)).ToArray();
            }
            this.MethodName = methodName;
            this.ActivationType = TriggerActivationType.CallMethodOnSelectedTarget;
        }

        #endregion

        #region Trigger Methods

        public void Trigger()
        {
            if (this.Triggerable == null) return;

            var arg0 = (this.TriggerableArgs != null && this.TriggerableArgs.Length > 0) ? this.TriggerableArgs[0].Value : null;
            switch (this.ActivationType)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    foreach (var t in (from t in this.Triggerable.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                    {
                        t.Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    if (this.Triggerable is ITriggerableMechanism)
                    {
                        (this.Triggerable as ITriggerableMechanism).Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.SendMessage:
                    var go = GameObjectUtil.GetGameObjectFromSource(this.Triggerable);
                    if (go != null && this.MethodName != null)
                    {
                        go.SendMessage(this.MethodName, arg0, SendMessageOptions.DontRequireReceiver);
                    }
                    break;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    if (this.MethodName != null)
                    {
                        //CallMethod does not support using the passed in arg
                        var args = (this.TriggerableArgs != null) ? (from a in this.TriggerableArgs select (a != null) ? a.Value : null).ToArray() : null;
                        ObjUtil.CallMethod(this.Triggerable, this.MethodName, args);
                    }
                    break;
            }
        }

        public void Trigger(object arg)
        {
            if (this.Triggerable == null) return;

            var arg0 = (this.TriggerableArgs != null && this.TriggerableArgs.Length > 0) ? this.TriggerableArgs[0].Value : arg;
            switch (this.ActivationType)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    foreach (var t in (from t in this.Triggerable.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                    {
                        t.Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    if (this.Triggerable is ITriggerableMechanism)
                    {
                        (this.Triggerable as ITriggerableMechanism).Trigger(arg0);
                    }
                    break;
                case TriggerActivationType.SendMessage:
                    var go = GameObjectUtil.GetGameObjectFromSource(this.Triggerable);
                    if (go != null && this.MethodName != null)
                    {
                        go.SendMessage(this.MethodName, arg0, SendMessageOptions.DontRequireReceiver);
                    }
                    break;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    if (this.MethodName != null)
                    {
                        //CallMethod does not support using the passed in arg
                        var args = (from a in this.TriggerableArgs select (a != null) ? a.Value : null).ToArray();
                        ObjUtil.CallMethod(this.Triggerable, this.MethodName, args);
                    }
                    break;
            }
        }

        #endregion

    }

}
