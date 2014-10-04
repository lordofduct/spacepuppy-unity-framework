using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Scenario
{
    public abstract class TriggerableMechanism : SPComponent, ITriggerableMechanism
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
            get { return this.enabled; }
        }

        public abstract object Trigger(object arg);

        #endregion

    }
}
