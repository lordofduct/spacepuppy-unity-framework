using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_AddComponent : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TypeReference.Config(typeof(Component), dropDownStyle=TypeDropDownListingStyle.ComponentMenu)]
        private TypeReference _componentType;

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(GameObject))]
        private TriggerableTargetObject _target;

        [Tooltip("Add a new component even if one already exists.")]
        public bool AddMultipleIfExists;

        #endregion


        #region TriggerableMechanism Interface

        public override object Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<GameObject>(arg);
            if (targ == null) return false;

            try
            {
                if (!this.AddMultipleIfExists && targ.HasComponent(_componentType.Type)) return false;

                var comp = targ.AddComponent(_componentType.Type);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}
