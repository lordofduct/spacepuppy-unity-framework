using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Scenario
{
    public class TriggerableMechanismOrderComparer : System.Collections.IComparer, System.Collections.Generic.IComparer<ITriggerableMechanism>
    {

        private static TriggerableMechanismOrderComparer _default;
        public static TriggerableMechanismOrderComparer Default
        {
            get
            {
                if (_default == null) _default = new TriggerableMechanismOrderComparer();
                return _default;
            }
        }

        int System.Collections.IComparer.Compare(object x, object y)
        {
            ITriggerableMechanism a = x as ITriggerableMechanism;
            ITriggerableMechanism b = y as ITriggerableMechanism;
            if (a == null) return b == null ? 0 : -1;
            if (b == null) return -1;
            return a.Order.CompareTo(b.Order);
        }

        public int Compare(ITriggerableMechanism x, ITriggerableMechanism y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return -1;
            return x.Order.CompareTo(y.Order);
        }
    }
}
