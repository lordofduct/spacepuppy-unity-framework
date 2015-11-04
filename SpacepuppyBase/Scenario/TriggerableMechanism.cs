using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public abstract class TriggerableMechanism : SPNotifyingComponent, ITriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private int _order;

        #endregion

        #region ITriggerableMechanism Interface

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public virtual bool CanTrigger
        {
            get { return this.IsActiveAndEnabled(); }
        }

        public void Trigger()
        {
            this.Trigger(null);
        }

        public abstract bool Trigger(object arg);

        #endregion

    }
}
