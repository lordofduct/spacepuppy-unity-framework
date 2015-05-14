using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    [System.Serializable()]
    public class TriggerTarget
    {

        public Component Triggerable;
        public VariantReference[] TriggerableArgs;
        public TriggerActivationType ActivationType;
        public string MethodName;


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
                        var args = (from a in this.TriggerableArgs select (a != null) ? a.Value : null).ToArray();
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

    }
}
