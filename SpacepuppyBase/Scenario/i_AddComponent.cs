using UnityEngine;

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

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("AddMultipleIfExists")]
        [Tooltip("Add a new component even if one already exists.")]
        private bool _addMultipleIfExists;

        #endregion

        #region Properties

        public bool AddMultipleIfExists
        {
            get { return _addMultipleIfExists; }
            set { _addMultipleIfExists = value; }
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<GameObject>(arg);
            if (targ == null) return false;

            try
            {
                if (!this._addMultipleIfExists && targ.HasComponent(_componentType.Type)) return false;

                var comp = targ.AddComponent(_componentType.Type);
                return true;
            }
            catch
            {
            }

            return false;
        }

        #endregion

    }
}
