using UnityEngine;

namespace com.spacepuppy.Scenario
{
    public class i_DebugLog : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private VariantReference _message = new VariantReference(string.Empty);

        #endregion


        #region TriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            Debug.Log(_message.StringValue, this);
            return true;
        }

        #endregion

    }
}
