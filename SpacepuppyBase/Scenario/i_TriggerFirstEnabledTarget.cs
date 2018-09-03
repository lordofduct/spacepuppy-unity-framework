#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    /// <summary>
    /// 
    /// </summary>
    [Infobox("Cascades through the target list until it finds the first target that is active/enabled and triggers that.")]
    public class i_TriggerFirstEnabledTarget : AutoTriggerableMechanism
    {
        
        [SerializeField]
        private Trigger _cascade;
        [SerializeField]
        private bool _passAlongArg;


        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            int cnt = _cascade.Targets.Count;
            for(int i = 0; i < cnt; i++)
            {
                var targ = _cascade.Targets[i];
                if (targ == null) continue;

                var obj = targ.CalculateTarget(arg);
                if (obj == null) continue;

                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go != null && !go.activeInHierarchy) continue;

                _cascade.ActivateTriggerAt(i, this, _passAlongArg ? arg : null);
                return true;
            }

            return false;
        }
    }

}
