using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{

    /// <summary>
    /// In what way should the Threshold of a selector be treated.
    /// A = 0.1, B = 0.5, C = 1.0
    /// 
    /// Threshold - standard where the threshold is the max value the entry should be selected at, 0.4 would result in selection B
    /// Discrete - the threshold is the initial value that it should be selected at, 0.4 would result in selection A
    /// </summary>
    public enum SelectorThresholdStyle
    {
        Threshold = 0,
        Discrete = 1
    }

    public enum SelectorIndexStyle
    {
        Clamp = 0,
        Wrap = 1
    }

    public delegate void AnimationEventCallback(object token);

}
