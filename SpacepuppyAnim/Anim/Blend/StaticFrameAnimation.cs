using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim.Blend
{

    /// <summary>
    /// Plays a single frame of animation at a single point in time on the animation until Stop is called. 
    /// The position is a percentage from 0->1, a scale can be applied for quick setting of values in some range from 0->scale.
    /// </summary>
    public class StaticFrameAnimation : ISPAnim
    {

        #region Fields

        private SPAnim _state;
        private float _scale;

        #endregion

        #region CONSTRUCTOR

        public StaticFrameAnimation(SPAnim state)
            : this(state, 1.0f)
        {
        }

        public StaticFrameAnimation(SPAnim state, float scale)
        {
            if (state == null) throw new System.ArgumentNullException("state");
            _state = state;
            _scale = scale;
        }

        #endregion

        #region Properties

        public SPAnim Anim
        {
            get { return _state; }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return;

                float p = this.Position;
                _scale = value;
                this.Position = p;
            }
        }

        public float Position
        {
            get { return (_state.Time / _state.Duration) * _scale; }
            set
            {
                _state.Time = ((value / _scale) % 1f) * _state.Duration;
            }
        }

        #endregion

        #region ISPAnim Interface

        public SPAnimationController Controller
        {
            get { return _state.Controller; }
        }

        public int Layer
        {
            get
            {
                return _state.Layer;
            }
            set
            {
                _state.Layer = value;
            }
        }

        public float Speed
        {
            get { return 0f; }
            set
            {
                //do nothing
            }
        }

        public WrapMode WrapMode
        {
            get
            {
                return WrapMode.Loop;
            }
            set
            {
                //do nothing
            }
        }

        public bool IsPlaying
        {
            get { return _state.IsPlaying; }
        }

        public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            _state.WrapMode = WrapMode.Loop;
            _state.Speed = 0f;
            _state.Play(queueMode, playMode);
        }

        public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            _state.WrapMode = WrapMode.Loop;
            _state.Speed = 0f;
            _state.CrossFade(fadeLength, queueMode, playMode);
        }
        
        public void Stop()
        {
            _state.Stop();
        }

        public void Schedule(System.Action<ISPAnim> callback)
        {
            _state.Schedule((s) => { callback(this); });
        }

        public void Schedule(System.Action<ISPAnim> callback, float timeout, ITimeSupplier supplier)
        {
            _state.Schedule((s) => { callback(this); }, timeout, supplier);
        }

        ITimeSupplier ISPAnim.TimeSupplier
        {
            get { return SPTime.Normal; }
            set
            {
                //do nothing
            }
        }

        float ISPAnim.Time
        {
            get { return 0f; }
            set
            {
                //do nothing
            }
        }

        float IAnimatable.Duration
        {
            get { return 0f; }
        }

        public float ScaledDuration
        {
            get
            {
                return 0f;
            }
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

        #region IDisposable Interface
        
        public bool IsDisposed
        {
            get
            {
                return _state == null;
            }
        }
        
        public void Dispose()
        {
            _state = null;
        }

        #endregion

    }
}
