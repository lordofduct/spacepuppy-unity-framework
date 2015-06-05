using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_TriggerChildren : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private bool _passAlongTriggerArg = true;

        [SerializeField()]
        [Tooltip("If you want to target only a specific child by its name, put that here. Otherwise leave blank to trigger all children.")]
        private string _specificTargetName;

        #endregion

        #region Properties

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public string SpecificTargetName
        {
            get { return _specificTargetName; }
            set { _specificTargetName = value; }
        }

        #endregion

        #region Triggerable Interface

        public override object Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            if(!string.IsNullOrEmpty(_specificTargetName))
            {
                var child = this.transform.FindChild(_specificTargetName);
                if (child != null && child.gameObject.activeInHierarchy)
                {
                    foreach (var t in (from t in child.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                    {
                        if (_passAlongTriggerArg)
                            t.Trigger(arg);
                        else
                            t.Trigger(null);
                    }
                }
            }
            else
            {
                foreach(Transform child in this.transform)
                {
                    if(child != null && child.gameObject.activeInHierarchy)
                    {
                        foreach (var t in (from t in child.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                        {
                            if (_passAlongTriggerArg)
                                t.Trigger(arg);
                            else
                                t.Trigger(null);
                        }
                    }
                }
            }

            return true;
        }

        #endregion

    }

}
