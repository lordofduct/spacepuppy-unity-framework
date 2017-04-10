using UnityEngine;
using System.Linq;

namespace com.spacepuppy.Anim
{
    public static class AnimUtil
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


        public static void Schedule(this ISPAnim anim, System.Action<ISPAnim> callback, SPTimePeriod period)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");

            anim.Schedule(callback, period.Seconds, period.TimeSupplier);
        }

        public static IRadicalYieldInstruction Wait(this ISPAnim anim, SPTimePeriod period)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");

            if (!anim.IsPlaying || period.Seconds <= 0f) return null; //yielding to when a non-playing anim finishes is just one frame, yield null

            if (period.Seconds == float.PositiveInfinity && anim is IRadicalYieldInstruction)
                return anim as IRadicalYieldInstruction; //SPAnim can be used directly as a yield instruction to its end
            else
            {
                var handle = RadicalWaitHandle.Create();
                anim.Schedule((a) =>
                {
                    handle.SignalComplete();
                }, period.Seconds, period.TimeSupplier);
                return handle;
            }
        }

        public static IRadicalYieldInstruction Wait(this ISPAnim anim, SPTimePeriod period, bool stopAnimOnComplete)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");

            if (!anim.IsPlaying || period.Seconds <= 0f) return null; //yielding to when a non-playing anim finishes is just one frame, yield null

            if (period.Seconds == float.PositiveInfinity && anim is IRadicalYieldInstruction)
                return anim as IRadicalYieldInstruction; //SPAnim can be used directly as a yield instruction to its end
            else
            {
                var handle = RadicalWaitHandle.Create();
                anim.Schedule((a) =>
                {
                    if (stopAnimOnComplete && a.IsPlaying) a.Stop();
                    handle.SignalComplete();
                }, period.Seconds, period.TimeSupplier);
                return handle;
            }
        }

    }
}
