using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI
{
    public interface IAIActionWeightSupplier
    {

        float GetWeight(IAIAction action);

        void OnActionSuccess(IAIAction action);
        void OnActionFailure(IAIAction action);

    }
}
