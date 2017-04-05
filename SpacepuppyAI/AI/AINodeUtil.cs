using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI
{
    public static class AINodeUtil
    {

        public static IEnumerable<IAINode> GetAllNodes(IAIActionGroup grp, bool includeGrp)
        {
            if (includeGrp) yield return grp;

            foreach(var n in grp)
            {
                yield return n;

                if (n is IAIActionGroup)
                {
                    foreach (var cn in GetAllNodes(n as IAIActionGroup, false))
                    {
                        yield return cn;
                    }
                }

                if (n is IAIStateMachine)
                {
                    foreach (var st in (n as IAIStateMachine))
                    {
                        yield return st as IAINode;
                        foreach (var cn in GetAllNodes(st as IAIActionGroup, false))
                        {
                            yield return cn;
                        }
                    }
                }
            }
        }

    }
}
