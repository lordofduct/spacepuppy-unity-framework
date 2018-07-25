using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Anim
{

    public interface IAnimatable
    {

        int Layer { get; set; }
        float Duration { get; }

    }

    /// <summary>
    /// An animation state generated from a clip. Similar to Unity AnimationState.
    /// </summary>
    public interface ISPAnim : IAnimatable, IRadicalWaitHandle, ISPDisposable
    {

        SPAnimationController Controller { get; }

        WrapMode WrapMode { get; set; }
        float Speed { get; set; }
        float ScaledDuration { get; }
        float Time { get; set; }
        ITimeSupplier TimeSupplier { get; set; }
        bool IsPlaying { get; }

        void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);
        void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);
        void Stop();

        /// <summary>
        /// React when this animation is complete.
        /// </summary>
        /// <param name="callback"></param>
        void Schedule(System.Action<ISPAnim> callback);

        /// <summary>
        /// React when this animation is complete, or some timeout is reached, whichever comes first.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="timeout">If the animation is still playing after some duration, stop waiting, and call the callback.</param>
        /// <param name="time">The timesupplier to use, null will use 'Normal' time</param>
        void Schedule(System.Action<ISPAnim> callback, float timeout, ITimeSupplier time);

    }

}
