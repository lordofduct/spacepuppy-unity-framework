using UnityEngine;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{

    public enum AnimatorType
    {
        Unknown = 0,
        Animation,
        Animator,
        SPAnimController,
        SPAnimator,
        SPAnimSource
    }

    public static class AnimUtil
    {

        #region AnimatorType Enum

        public static AnimatorType GetAnimatorType(System.Type tp)
        {
            if (tp == null)
                return AnimatorType.Unknown;
            else if (TypeUtil.IsType(tp, typeof(Animation)))
                return AnimatorType.Animation;
            else if (TypeUtil.IsType(tp, typeof(Animator)))
                return AnimatorType.Animator;
            else if (TypeUtil.IsType(tp, typeof(SPAnimationController)))
                return AnimatorType.SPAnimController;
            else if (TypeUtil.IsType(tp, typeof(ISPAnimator)))
                return AnimatorType.SPAnimator;
            else if (TypeUtil.IsType(tp, typeof(ISPAnimationSource)))
                return AnimatorType.SPAnimSource;
            else
                return AnimatorType.Unknown;
        }

        public static AnimatorType GetAnimatorType(object obj)
        {
            if (obj == null) return AnimatorType.Unknown;
            if (obj is System.Type) return GetAnimatorType(obj as System.Type);

            if (obj is Animation)
                return AnimatorType.Animation;
            else if (obj is Animator)
                return AnimatorType.Animator;
            else if (obj is SPAnimationController)
                return AnimatorType.SPAnimController;
            else if (obj is ISPAnimator)
                return AnimatorType.SPAnimator;
            else if (obj is ISPAnimationSource)
                return AnimatorType.SPAnimSource;
            else if (obj is IProxy)
                return GetAnimatorType((obj as IProxy).GetTargetType());
            else
                return AnimatorType.Unknown;
        }
        
        #endregion

        #region Animation Extension Methods

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

        public static void Stop(this Animation animation, int layer)
        {
            foreach (AnimationState a in animation)
            {
                if (a.enabled && a.layer == layer)
                {
                    var nm = a.name;
                    if (animation.IsPlaying(nm))
                    {
                        //if it was a queued animation, we need to redact... otherwise the Animation can't stop it... cause it's stupid
                        int i = nm.IndexOf(" - Queued Clone");
                        if (i >= 0) nm = nm.Substring(0, i);
                        animation.Stop(nm);
                    }
                }
            }
        }

        #endregion

        #region ISPAnim Extension Methods

        public static void Play(this ISPAnim anim, float speed, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");
            anim.Speed = speed;
            anim.Play(queueMode, playMode);
        }

        public static void Play(this ISPAnim anim, float speed, float startTime, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");
            anim.Speed = speed;
            anim.Play(queueMode, playMode);
            anim.Time = Mathf.Clamp(startTime, 0f, anim.Duration);
        }

        public static void CrossFade(this ISPAnim anim, float speed, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");
            anim.Speed = speed;
            anim.CrossFade(fadeLength, queueMode, playMode);
        }

        public static void CrossFade(this ISPAnim anim, float speed, float fadeLength, float startTime, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");
            anim.Speed = speed;
            anim.CrossFade(fadeLength, queueMode, playMode);
            anim.Time = Mathf.Clamp(startTime, 0f, anim.Duration);
        }

        #endregion

        #region Scheduling Methods

        public static bool TrySchedule(object animtoken, System.Action<object> callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");
            if (animtoken == null) return false;

            if (animtoken is ISPAnim)
            {
                (animtoken as ISPAnim).Schedule((a) => callback(a));
                return true;
            }
            else if (animtoken is IRadicalWaitHandle)
            {
                var handle = animtoken as IRadicalWaitHandle;
                if (handle.IsComplete)
                {
                    callback(animtoken);
                }
                else
                {
                    handle.OnComplete((h) => callback(h));
                }

                return true;
            }
            else if (animtoken is AnimationState)
            {
                //GameLoopEntry.Hook.StartCoroutine((animtoken as AnimationState).ScheduleLegacy((a) => callback(a)));
                InvokeHandle.Begin(GameLoopEntry.UpdatePump, () => callback(animtoken), ScheduleLegacyForInvokeHandle(animtoken as AnimationState));
                return true;
            }
            else
            {
                GameLoopEntry.Hook.StartCoroutine(CoroutineUtil.Wait(animtoken, callback));
            }

            return false;
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

        public static System.Collections.IEnumerator ScheduleLegacy(this AnimationState state, System.Action<AnimationState> callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");

            while (state != null)
            {
                yield return null;
            }

            callback(state);
        }

        private static System.Collections.IEnumerator ScheduleLegacyForInvokeHandle(AnimationState state)
        {
            while (state != null) yield return null;
        }

        #endregion

    }
}
