using UnityEngine;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Anim
{

    public class SPAnim : ISPAnim, IRadicalWaitHandle, System.ICloneable
    {

        #region Fields

        private SPAnimationController _controller;

        private string _clipId;
        private float _weight = 1.0f;
        private float _speed = 1.0f;
        private int _layer;
        private WrapMode _wrapMode;
        private AnimationBlendMode _blendMode = AnimationBlendMode.Blend;
        private ITimeSupplier _timeSupplier;
        private ISPAnimationMask _mask;

        private AnimationState _state;
        private AnimEventScheduler _scheduler;
        private bool _timeSupplierScaleEventRegistered;

        #endregion

        #region CONSTRUCTOR

        private SPAnim()
        {

        }

        #endregion

        #region Properties

        public string ClipId
        {
            get
            {
                return _clipId;
            }
        }

        public float Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                if (_state != null) _state.weight = value;
            }
        }

        public float Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                if (_state != null) _state.speed = this.RealSpeed;
            }
        }

        public int Layer
        {
            get { return _layer; }
            set
            {
                _layer = Mathf.Max(0, value);
                if (_state != null) _state.layer = value;
            }
        }

        public WrapMode WrapMode
        {
            get { return _wrapMode; }
            set
            {
                _wrapMode = value;
                if (_state != null) _state.wrapMode = value;
            }
        }

        public AnimationBlendMode BlendMode
        {
            get { return _blendMode; }
            set
            {
                _blendMode = value;
                if (_state != null) _state.blendMode = value;
            }
        }
        
        public ITimeSupplier TimeSupplier
        {
            get { return (_timeSupplier == null) ? SPTime.Normal : _timeSupplier; }
            set
            {
                if (_timeSupplier == value) return;

                this.UnregisterTimeScaleChangedEvent();
                _timeSupplier = value;
                if (_timeSupplier != null && this.IsPlaying)
                {
                    this.RegisterTimeScaleChangedEvent();
                    this.OnTimeScaleChanged(null, null);
                }
            }
        }

        /// <summary>
        /// The speed the animation is actually playing at in real time, all timescales applied
        /// </summary>
        public float RealSpeed
        {
            get
            {
                return _controller.Speed * _speed * SPTime.GetInverseScale(_timeSupplier);
            }
        }

        public float Time
        {
            get { return (_state == null) ? 0f : _state.time; }
            set { if (_state != null) _state.time = value; }
        }

        public float Duration
        {
            get
            {
                if (_state != null) return _state.length;

                var st = _controller.States[_clipId];
                return (st != null) ? st.Duration : 0f;
            }
        }

        public float ScaledDuration
        {
            get
            {
                var spd = this.Speed;
                if (spd == 0f) return float.PositiveInfinity;

                return Mathf.Abs(this.Duration / spd);
            }
        }

        public ISPAnimationMask Mask
        {
            get { return _mask; }
            set { _mask = value; }
        }

        ///// <summary>
        ///// Don't actually modify this thing, here for debug purposes.
        ///// </summary>
        //public AnimationState UnityAnimState
        //{
        //    get { return _state; }
        //}

        #endregion

        #region Methods

        public void PlayReverse(PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_controller.ControllerMask != null && !_controller.ControllerMask.CanPlay(this)) return;

            if (this.IsPlaying)
            {
                this.Speed = -this.Speed;
            }
            else
            {
                _speed = -_speed;
                this.Play(QueueMode.PlayNow, playMode);
            }
        }


        public SPAnim Queue(QueueMode queueMode = QueueMode.CompleteOthers, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_controller.ControllerMask != null && !_controller.ControllerMask.CanPlay(this)) return null;

            var a = this.Clone();
            a.Play(queueMode, playMode);
            return a;
        }

        public SPAnim QueueCrossFade(float fadeLength, QueueMode queueMode = QueueMode.CompleteOthers, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_controller.ControllerMask != null && !_controller.ControllerMask.CanPlay(this)) return null;

            var a = this.Clone();
            a.CrossFade(fadeLength, queueMode, playMode);
            return a;
        }

        public SPAnim QueueInReverse(QueueMode queueMode = QueueMode.CompleteOthers, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_controller.ControllerMask != null && !_controller.ControllerMask.CanPlay(this)) return null;

            var a = this.Clone();
            if (this.IsPlaying)
            {
                float t = _state.time;
                float sp = -_state.speed;
                a.Play(queueMode, playMode);
                a.Time = t;
                a.Speed = sp;
            }
            else
            {
                a.Play(queueMode, playMode);
                a.Time = (_speed > 0f) ? a.Duration : 0f;
                a.Speed = -_speed;
            }
            return a;
        }

        #endregion

        #region ISPAnim Interface

        public SPAnimationController Controller
        {
            get { return _controller; }
        }

        public bool IsPlaying
        {
            get
            {
                return _state != null; //&& _state.enabled; //when the anim is done playing... it equates to null
                //return _state != null && _anim.IsPlaying(_state.name);
            }
        }

        public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (this.IsPlaying) return;
            if (_controller.ControllerMask != null && !_controller.ControllerMask.CanPlay(this)) return;

            _state = _controller.PlayQueuedInternal(_clipId, queueMode, playMode, _layer);
            _state.weight = _weight;
            _state.speed = this.RealSpeed;
            _state.time = (_state.speed >= 0f) ? 0f : _state.length;
            _state.layer = _layer;
            _state.wrapMode = _wrapMode;
            _state.blendMode = _blendMode;
            if (_mask != null) _mask.Apply(_controller, _state);
            this.RegisterTimeScaleChangedEvent();
        }
        
        public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (this.IsPlaying) return;
            if (_controller.ControllerMask != null && !_controller.ControllerMask.CanPlay(this)) return;

            _state = _controller.CrossFadeQueuedInternal(_clipId, fadeLength, queueMode, playMode, _layer);
            _state.weight = _weight;
            _state.speed = this.RealSpeed;
            _state.time = (_state.speed >= 0f) ? 0f : _state.length;
            _state.layer = _layer;
            _state.wrapMode = _wrapMode;
            _state.blendMode = _blendMode;
            if (_mask != null) _mask.Apply(_controller, _state);
            this.RegisterTimeScaleChangedEvent();
        }

        public void Stop()
        {
            if (this.IsPlaying)
            {
                //_anim.Stop(_state.name);
                _controller.animation.Stop(_clipId);
                _state = null;
                this.UnregisterTimeScaleChangedEvent();
            }
        }

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

        #region IScalableTimeSupplier ScaleChanged Callback

        private void RegisterTimeScaleChangedEvent()
        {
            if (_timeSupplierScaleEventRegistered) return;
            _timeSupplierScaleEventRegistered = true;
            //if (_timeSupplier != null && _timeSupplier != SPTime.Normal) _timeSupplier.TimeScaleChanged += this.OnTimeScaleChanged;
            if (_timeSupplier is IScalableTimeSupplier && _timeSupplier != SPTime.Normal) (_timeSupplier as IScalableTimeSupplier).TimeScaleChanged += this.OnTimeScaleChanged;
        }

        private void UnregisterTimeScaleChangedEvent()
        {
            if (!_timeSupplierScaleEventRegistered) return;
            _timeSupplierScaleEventRegistered = false;
            //if (_timeSupplier != null && _timeSupplier != SPTime.Normal) _timeSupplier.TimeScaleChanged -= this.OnTimeScaleChanged;
            if (_timeSupplier is IScalableTimeSupplier && _timeSupplier != SPTime.Normal) (_timeSupplier as IScalableTimeSupplier).TimeScaleChanged -= this.OnTimeScaleChanged;
        }

        private void OnTimeScaleChanged(object sender, System.EventArgs e)
        {
            if (!this.IsPlaying)
            {
                this.UnregisterTimeScaleChangedEvent();
                return;
            }

            _state.speed = this.RealSpeed;
        }

        #endregion


        #region ICloneable Interface

        public SPAnim Clone()
        {
            var a = SPAnim.Create(_controller, _clipId);
            a._weight = _weight;
            a._speed = _speed;
            a._layer = _layer;
            a._wrapMode = _wrapMode;
            a._blendMode = _blendMode;
            a._mask = _mask;
            a._timeSupplier = _timeSupplier;
            return a;
        }

        object System.ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IDisposable Interface

        public bool IsDisposed
        {
            get
            {
                if (object.ReferenceEquals(_controller, null)) return true;
                if (_controller == null)
                {
                    //dead but still active
                    this.Dispose();
                    return true;
                }
                return false;
            }
        }

        public void Dispose()
        {
            _controller = null;
            _clipId = null;
            _weight = 1f;
            _speed = 1f;
            _layer = 0;
            _wrapMode = UnityEngine.WrapMode.Default;
            _blendMode = AnimationBlendMode.Blend;
            _mask = null;
            this.UnregisterTimeScaleChangedEvent();
            _timeSupplier = null;

            _state = null;
            if (_scheduler != null) _scheduler.Clear();

            _pool.Release(this);
        }

        #endregion



        #region Static Interface

        private static ObjectCachePool<SPAnim> _pool = new ObjectCachePool<SPAnim>(-1, () => new SPAnim());

        public static SPAnim Create(Animation anim, string clipId)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");
            var a = _pool.GetInstance();
            a._controller = anim.AddOrGetComponent<SPAnimationController>();
            a._clipId = clipId;
            return a;
        }

        public static SPAnim Create(SPAnimationController anim, string clipId)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");
            var a = _pool.GetInstance();
            a._controller = anim;
            a._clipId = clipId;
            a._timeSupplier = anim.TimeSupplier as ITimeSupplier;
            return a;
        }

        

        #endregion

        #region Null Interface

        private static NullSPAnim _null = new NullSPAnim();
        public static ISPAnim Null
        {
            get
            {
                return _null;
            }
        }

        private sealed class NullSPAnim : ISPAnim, IRadicalWaitHandle
        {

            #region ISPAnim Interface

            public SPAnimationController Controller { get { return null; } }

            public int Layer
            {
                get { return 0; }
                set { }
            }

            public float Speed
            {
                get { return 0f; }
                set { }
            }

            public WrapMode WrapMode
            {
                get
                {
                    return WrapMode.Default;
                }
                set
                {
                    //do nothing
                }
            }

            public bool IsPlaying
            {
                get { return false; }
            }

            public float Time
            {
                get { return 0f; }
                set
                {
                    //do nothing
                }
            }

            public float Duration
            {
                get { return 0f; }
            }

            public float ScaledDuration
            {
                get { return 0f; }
            }

            public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
            }

            public void Play(float speed, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
            }

            public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
            }

            public void CrossFade(float speed, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
            }

            public void Stop()
            {
            }

            public void Schedule(System.Action<ISPAnim> callback)
            {
            }

            public void Schedule(System.Action<ISPAnim> callback, float timeout, ITimeSupplier supplier)
            {
            }

            ITimeSupplier ISPAnim.TimeSupplier
            {
                get { return SPTime.Normal; }
                set
                {
                    //do nothing
                }
            }
            
            #endregion

            #region IRadicalWaitHandle Interface

            public void OnComplete(Action<IRadicalWaitHandle> callback)
            {

            }

            public bool Tick(out object yieldObject)
            {
                yieldObject = null;
                return false;
            }

            public bool Cancelled
            {
                get
                {
                    return false;
                }
            }

            public bool IsComplete
            {
                get
                {
                    return true;
                }
            }

            #endregion

            #region IDisposable Interface

            bool ISPDisposable.IsDisposed
            {
                get { return false; }
            }

            void System.IDisposable.Dispose()
            {
                //do nothing
            }

            #endregion

        }

        #endregion

    }

}
