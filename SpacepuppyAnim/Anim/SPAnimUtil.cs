using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{
    public static class SPAnimUtil
    {

        public static bool IsValid(this SPAnimClip clip)
        {
            return clip != null && clip.Clip != null && clip.Initialized;
        }

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

    }
}
