using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Tween
{

    public delegate void TweenerUpdateCallback(Tweener tween, float dt, float t);

    public class CallbackTweener : Tweener, ITweenHash
    {

        #region Fields

        private TweenerUpdateCallback _callback;
        private Ease _ease;
        private float _dur;

        #endregion
        
        #region CONSTRUCTOR

        public CallbackTweener(TweenerUpdateCallback callback, float dur)
        {
            _callback = callback;
            _ease = EaseMethods.LinearEaseNone;
            _dur = dur;
        }

        public CallbackTweener(TweenerUpdateCallback callback, Ease ease, float dur)
        {
            _callback = callback;
            _ease = ease ?? EaseMethods.LinearEaseNone;
            _dur = dur;
        }

        #endregion

        #region Properties

        public Ease Ease
        {
            get { return _ease; }
            set
            {
                _ease = value ?? EaseMethods.LinearEaseNone;
            }
        }

        #endregion

        #region Tweener Interface

        protected override float GetPlayHeadLength()
        {
 	        return _dur;
        }

        protected override void DoUpdate(float dt, float t)
        {
            if (_callback == null) return;
            _callback(this, dt, _ease(t, 0, _dur, _dur));
        }

        #endregion

        #region ITweenHash Interface

        ITweenHash ITweenHash.Ease(Ease ease)
        {
            _ease = ease;
            return this;
        }

        ITweenHash ITweenHash.UseUpdate()
        {
            this.UpdateType = UpdateSequence.Update;
            return this;
        }

        ITweenHash ITweenHash.UseFixedUpdate()
        {
            this.UpdateType = UpdateSequence.FixedUpdate;
            return this;
        }

        ITweenHash ITweenHash.UseLateUpdate()
        {
            this.UpdateType = UpdateSequence.LateUpdate;
            return this;
        }

        ITweenHash ITweenHash.Use(UpdateSequence type)
        {
            this.UpdateType = type;
            return this;
        }

        ITweenHash ITweenHash.UseNormalTime()
        {
            this.DeltaType = DeltaTimeType.Normal;
            return this;
        }

        ITweenHash ITweenHash.UseRealTime()
        {
            this.DeltaType = DeltaTimeType.Real;
            return this;
        }

        ITweenHash ITweenHash.UseSmoothTime()
        {
            this.DeltaType = DeltaTimeType.Smooth;
            return this;
        }

        ITweenHash ITweenHash.Use(DeltaTimeType type)
        {
            this.DeltaType = type;
            return this;
        }

        ITweenHash ITweenHash.PlayOnce()
        {
            this.WrapMode = TweenWrapMode.Once;
            return this;
        }

        ITweenHash ITweenHash.Loop(int count)
        {
            this.WrapMode = TweenWrapMode.Loop;
            this.WrapCount = count;
            return this;
        }

        ITweenHash ITweenHash.PingPong(int count)
        {
            this.WrapMode = TweenWrapMode.PingPong;
            this.WrapCount = count;
            return this;
        }

        ITweenHash ITweenHash.Wrap(TweenWrapMode wrap, int count)
        {
            this.WrapMode = wrap;
            this.WrapCount = count;
            return this;
        }

        ITweenHash ITweenHash.Reverse()
        {
            this.Reverse = true;
            return this;
        }

        ITweenHash ITweenHash.Reverse(bool reverse)
        {
            this.Reverse = reverse;
            return this;
        }

        ITweenHash ITweenHash.OnStep(EventHandler d)
        {
            if (d == null) return this;
            this.OnStep += d;
            return this;
        }

        ITweenHash ITweenHash.OnStep(Action<Tweener> d)
        {
            if (d == null) return this;
            this.OnStep += (s, e) => d(this);
            return this;
        }

        ITweenHash ITweenHash.OnWrap(EventHandler d)
        {
            if (d == null) return this;
            this.OnWrap += d;
            return this;
        }

        ITweenHash ITweenHash.OnWrap(Action<Tweener> d)
        {
            if (d == null) return this;
            this.OnWrap += (s, e) => d(this);
            return this;
        }

        ITweenHash ITweenHash.OnFinish(EventHandler d)
        {
            if (d == null) return this;
            this.OnFinish += d;
            return this;
        }

        ITweenHash ITweenHash.OnFinish(Action<Tweener> d)
        {
            if (d == null) return this;
            this.OnFinish += (s, e) => d(this);
            return this;
        }

        Tweener ITweenHash.Play()
        {
            this.Play();
            return this;
        }

        Tweener ITweenHash.Play(float playHeadPosition)
        {
            this.Play(playHeadPosition);
            return this;
        }

        #endregion

    }
}
