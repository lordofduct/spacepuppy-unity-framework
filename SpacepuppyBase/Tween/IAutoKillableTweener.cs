using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Tween
{

    /// <summary>
    /// A Tweener that implements IAutoKillableTweener can be passed into the SPTween.AutoKill method. 
    /// Until the Tweener is either killed, or finished, it will be eligible for being automatically 
    /// killed if another Tweener starts playing that tweens the same target object. Note that other tweener 
    /// must implement IAutoKillableTweener as well (though doesn't have to be flagged to AutoKill).
    /// </summary>
    public interface IAutoKillableTweener
    {

        object Token { get; set; }

        void Kill();

    }

}
