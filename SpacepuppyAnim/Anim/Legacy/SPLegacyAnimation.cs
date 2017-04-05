#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

namespace com.spacepuppy.Anim.Legacy
{

    /// <summary>
    /// Use in tandem with the Animation class to have more control over Animation at both design time and runtime. 
    /// Note, when this is being used DO NOT directly access the Animation class for adding and removing clips, this will cause 
    /// the two to go out of sync and require this to be resynced.
    /// </summary>
    /// <remarks>
    /// Note - when refreshing animator at design time this attempts to clear out animations. Thing is the Animation component 
    /// from unity initializes the states on load, but not when the animation list is edited. So if you clear the animations 
    /// in the Animation component then select to Sync you will get a warning message from unity. There is nothing that can be 
    /// done about it. It doesn't hurt anything, so just ignore.
    /// </remarks>
    [System.Obsolete("Use SPAnimationController instead!")]
    public class SPLegacyAnimation : SPAnimationController
    {

        #region Fields

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ISPAnim CreateAnimatableState(string name)
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();
            var state = this.States[name];
            if (state == null) throw new UnknownStateException(name);

            return state.CreateAnimatableState();
        }

        public void PlayDirectly(string name, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();
            var state = this.States[name];
            if (state == null) throw new UnknownStateException(name);

            this.animation.Play(state.Name, playMode);
        }

        public AnimationState QueueDirectly(string name, QueueMode queueMode = QueueMode.CompleteOthers, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();
            var state = this.States[name];
            if (state == null) throw new UnknownStateException(name);

            var s = this.animation.PlayQueued(state.Name, queueMode, playMode);
            s.weight = state.Weight;
            s.speed = state.Speed;
            s.layer = state.Layer;
            s.wrapMode = state.WrapMode;
            s.blendMode = state.BlendMode;
            if (state.Masks.Count > 0) state.Masks.Apply(s);
            return s;
        }

        public void CrossFadeDirectly(string name, float fadeLength, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();
            var state = this.States[name];
            if (state == null) throw new UnknownStateException(name);

            this.animation.CrossFade(state.Name, fadeLength, playMode);
        }

        public AnimationState QueueCrossFadeDirectly(string name, float fadeLength, QueueMode queueMode = QueueMode.CompleteOthers, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();
            var state = this.States[name];
            if (state == null) throw new UnknownStateException(name);

            var s = this.animation.CrossFadeQueued(state.Name, fadeLength, queueMode, playMode);
            s.weight = state.Weight;
            s.speed = state.Speed;
            s.layer = state.Layer;
            s.wrapMode = state.WrapMode;
            s.blendMode = state.BlendMode;
            if (state.Masks.Count > 0) state.Masks.Apply(s);
            return s;
        }

        public void Stop(string name)
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();
            var state = this.States[name];
            if (state == null) throw new UnknownStateException(name);

            this.animation.Stop(state.Name);
        }

        public void StopAll()
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();
            this.animation.Stop();
        }

        public void StopLayer(int layer)
        {
            if (this.animation == null) throw new AnimationInvalidAccessException();

            foreach (AnimationState a in this.animation)
            {
                if (a.layer == layer) this.animation.Stop(a.name);
            }
        }

        public bool IsPlaying(string name)
        {
            return (this.animation != null) ? this.animation.IsPlaying(name) : false;
        }

        #endregion

        #region Event Callback

        public void AddEvent(string anim, float time, AnimationEventCallback callback, object token)
        {
            var state = this.States[anim];
            if (state == null) throw new UnknownStateException(name);

            state.AddEvent(time, callback, token);
        }

        #endregion

    }

}
