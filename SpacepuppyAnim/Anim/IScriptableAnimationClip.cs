using UnityEngine;
using System.Collections.Generic;
using System;

namespace com.spacepuppy.Anim
{


    /// <summary>
    /// Represents a non-standard AnimationClip that isn't a UnityEngine.AnimationClip. 
    /// This could be a animation blend tree, or a scriptable animation, or any other custom implementation of a clip.
    /// </summary>
    public interface IScriptableAnimationClip
    {

        float Length { get; }

        ISPAnim CreateState(SPAnimationController controller);

    }
    
    public interface IScriptableAnimationCallback : IDisposable
    {

        int Layer { get; }
        bool Tick(bool layerIsObscured);

    }

    
    public abstract class ScriptableAnimState : ISPAnim
    {

        #region Fields

        private SPAnimationController _controller;
        private AnimEventScheduler _scheduler;

        #endregion

        #region CONSTRUCTOR

        public ScriptableAnimState(SPAnimationController controller)
        {
            if (object.ReferenceEquals(controller, null)) throw new System.ArgumentNullException("controller");
            _controller = controller;
        }

        #endregion

        #region ISPAnim Interface

        public SPAnimationController Controller { get { return _controller; } }
        public virtual int Layer { get; set; }
        public virtual float Speed { get; set; }
        public virtual ITimeSupplier TimeSupplier { get; set; }

        public abstract float Duration { get; }
        public abstract bool IsPlaying { get; }
        public abstract float Time { get; set; }

        public abstract void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);
        public abstract void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);
        public abstract void Stop();


        public void Schedule(System.Action<ISPAnim> callback)
        {
            if (_scheduler == null) _scheduler = new AnimEventScheduler(this);
            _scheduler.Schedule(callback);
        }

        public void Schedule(System.Action<ISPAnim> callback, float timeout, ITimeSupplier supplier)
        {
            if (_scheduler == null) _scheduler = new AnimEventScheduler(this);
            _scheduler.Schedule(callback, timeout, supplier);
        }

        #endregion

        #region IRadicalWaitHandle Interface

        bool IRadicalYieldInstruction.IsComplete
        {
            get { return !this.IsPlaying; }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return this.IsPlaying;
        }

        void IRadicalWaitHandle.OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");
            if (!this.IsPlaying) throw new System.InvalidOperationException("Can not wait for complete on an already completed IRadicalWaitHandle.");

            this.Schedule((a) => callback(this));
        }

        bool IRadicalWaitHandle.Cancelled
        {
            get { return false; }
        }

        #endregion 

    }


}
