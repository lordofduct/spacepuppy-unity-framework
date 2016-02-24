using System;

namespace com.spacepuppy.Tween
{

    public delegate void TweenerUpdateCallback(Tweener tween, float dt, float t);

    /// <summary>
    /// A tweener that calls an update function tick by tick.
    /// 
    /// When AutoKilling Id must be set.
    /// </summary>
    public class CallbackTweener : Tweener, ITweenHash
    {

        #region Fields

        private object _id;
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

        public override object Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

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

        protected internal override float GetPlayHeadLength()
        {
 	        return _dur;
        }

        protected internal override void DoUpdate(float dt, float t)
        {
            if (_callback == null) return;
            _callback(this, dt, _ease(t, 0, _dur, _dur));
        }

        #endregion
        
        #region ITweenHash Interface

        ITweenHash ITweenHash.SetId(object id)
        {
            this.Id = id;
            return this;
        }

        ITweenHash ITweenHash.Ease(Ease ease)
        {
            _ease = ease;
            return this;
        }

        ITweenHash ITweenHash.Delay(float delay)
        {
            this.Delay = delay;
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
            this.TimeSupplier = SPTime.Normal;
            return this;
        }

        ITweenHash ITweenHash.UseRealTime()
        {
            this.TimeSupplier = SPTime.Real;
            return this;
        }

        ITweenHash ITweenHash.UseSmoothTime()
        {
            this.TimeSupplier = SPTime.Smooth;
            return this;
        }

        ITweenHash ITweenHash.Use(ITimeSupplier time)
        {
            this.TimeSupplier = time;
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

        ITweenHash ITweenHash.SpeedScale(float scale)
        {
            this.SpeedScale = scale;
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

        ITweenHash ITweenHash.OnStopped(EventHandler d)
        {
            if (d == null) return this;
            this.OnStopped += d;
            return this;
        }

        ITweenHash ITweenHash.OnStopped(Action<Tweener> d)
        {
            if (d == null) return this;
            this.OnStopped += (s, e) => d(this);
            return this;
        }
        
        Tweener ITweenHash.Play(bool autoKill, object autoKillToken)
        {
            this.Play();
            if (autoKill)
            {
                this.AutoKillToken = autoKillToken;
                SPTween.AutoKill(this);
            }
            return this;
        }

        Tweener ITweenHash.Play(float playHeadPosition, bool autoKill, object autoKillToken)
        {
            this.Play(playHeadPosition);
            if (autoKill)
            {
                this.AutoKillToken = autoKillToken;
                SPTween.AutoKill(this);
            }
            return this;
        }

        #endregion

        #region Special Types

        private struct TokenPairing
        {
            public object Target;
            public object TokenUid;

            public TokenPairing(object targ, object uid)
            {
                this.Target = targ;
                this.TokenUid = uid;
            }

            //public override bool Equals(object obj)
            //{
            //    if (obj == null) return false;
            //    if (!(obj is TokenPairing)) return false;

            //    var token = (TokenPairing)obj;
            //    return token.Target == this.Target && token.TokenUid == this.TokenUid;
            //}
        }

        #endregion

    }
}
