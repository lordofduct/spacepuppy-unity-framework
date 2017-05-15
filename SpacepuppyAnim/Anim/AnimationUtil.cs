using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{
    internal static class AnimationUtil
    {

        public static void ClearAnimations(Animation anim)
        {
            if (anim != null)
            {
                var clips = (from a in anim.Cast<AnimationState>() select a.clip).Distinct().ToArray();
                foreach (var c in clips)
                {
                    anim.RemoveClip(c);
                }
            }
        }

    }
}
