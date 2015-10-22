using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Tween
{

    public abstract class Tweener : ISPDisposable, IProgressingYieldInstruction, IRadicalWaitHandle
    {

        #region Events

        public event System.EventHandler OnStep;
        public event System.EventHandler OnWrap;
        public event System.EventHandler OnFinish;
        public event System.EventHandler OnStopped;

        #endregion

        #region Fields
        
        private UpdateSequence _updateType;
        private ITimeSupplier _timeSupplier = SPTime.Normal;
        private TweenWrapMode _wrap;
        private int _wrapCount;
        private bool _reverse;
        private float _speedScale = 1.0f;
        private float _delay;


        private bool _isPlaying;
        private float _playHeadLength;
        private float _time; //the time since the tween was first played
        private float _unwrappedPlayHeadTime; //we need an unwrapped value so that we can pingpong/loop the playhead
        private float _normalizedPlayHeadTime; //this position the playhead is currently at, with wrap applied

        private int _currentWrapCount;

        private object _autoKillToken;

        #endregion

        #region Configurable Properties

        /// <summary>
        /// An identifier for this tween to relate it to other tweens. Usually its the object being tweened, if one exists.
        /// </summary>
        public abstract object Id
        {
            get;
            set;
        }

        public object AutoKillToken
        {
            get { return _autoKillToken; }
            set
            {
                if (this.IsPlaying) throw new System.InvalidOperationException("Can only chnage the AutoKillToken on a Tweener that is not currently playing.");

                _autoKillToken = value;
            }
        }
        
        public UpdateSequence UpdateType
        {
            get { return _updateType; }
            set { _updateType = value; }
        }

        public ITimeSupplier TimeSupplier
        {
            get { return _timeSupplier; }
            set { _timeSupplier = value ?? SPTime.Normal; }
        }

        public DeltaTimeType DeltaType
        {
            get { return SPTime.GetDeltaType(_timeSupplier); }
        }

        public TweenWrapMode WrapMode
        {
            get { return _wrap; }
            set
            {
                if (_wrap == value) return;

                _wrap = value;
                //normalized time is dependent on WrapMode, so we force update the play head
                this.MovePlayHeadPosition(0f);
            }
        }

        /// <summary>
        /// Amount of times the tween should wrap if WrapMode loops or pingpongs. 
        /// A value of 0 or less will wrap infinitely.
        /// </summary>
        public int WrapCount
        {
            get { return _wrapCount; }
            set { _wrapCount = value; }
        }

        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }

        public float SpeedScale
        {
            get { return _speedScale; }
            set
            {
                _speedScale = value;
                if (_speedScale < 0f || float.IsNaN(_speedScale)) _speedScale = 0f;
                else if (float.IsInfinity(_speedScale)) _speedScale = float.MaxValue;
            }
        }

        public float Delay
        {
            get { return _delay; }
            set
            {
                _delay = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        #endregion

        #region Status Properties

        public bool IsDead
        {
            get { return float.IsNaN(_time); }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public bool IsComplete
        {
            get
            {
                if (float.IsNaN(_time)) return true;
                switch(_wrap)
                {
                    case TweenWrapMode.Once:
                        return _time >= this.PlayHeadLength + _delay;
                    case TweenWrapMode.Loop:
                    case TweenWrapMode.PingPong:
                        if (_wrapCount <= 0)
                            return false;
                        else
                            return (_time - _delay) >= (this.PlayHeadLength * _wrapCount);
                }
                return false;
            }
        }

        public float PlayHeadLength
        {
            get
            {
                return (_isPlaying) ? _playHeadLength : this.GetPlayHeadLength();
            }
        }

        /// <summary>
        /// The amount of time that has passed for the tween (sum of all calls to update).
        /// </summary>
        public float Time
        {
            get { return _time; }
        }

        public float TotalTime
        {
            get
            {
                switch(_wrap)
                {
                    case TweenWrapMode.Once:
                        return this.PlayHeadLength;
                    case TweenWrapMode.Loop:
                    case TweenWrapMode.PingPong:
                        return (_wrapCount <= 0) ? float.PositiveInfinity : this.PlayHeadLength * _wrapCount;
                }
                return 0f;
            }
        }

        /// <summary>
        /// The position of the play-head relative to PlayHeadLength of the tween.
        /// </summary>
        public float PlayHeadTime
        {
            get { return _normalizedPlayHeadTime; }
        }

        public int CurrentWrapCount
        {
            get { return _currentWrapCount; }
        }

        /// <summary>
        /// Does the configuration of this tween result in a tween that plays forever.
        /// </summary>
        public bool PlaysIndefinitely
        {
            get { return _wrap != TweenWrapMode.Once && _wrapCount <= 0; }
        }

        #endregion

        #region Methods

        public void Play()
        {
            if (_isPlaying) return;
            this.Play((_reverse) ? this.PlayHeadLength : 0f);
        }

        public virtual void Play(float playHeadPosition)
        {
            if (this.IsDead) throw new System.InvalidOperationException("Cannot play a dead Tweener.");
            _isPlaying = true;
            _playHeadLength = this.GetPlayHeadLength();
            SPTween.AddReference(this);

            if (_time == 0f && _reverse)
            {
                //if this the first time we're playing, and we're in reverse, then make sure we set the playhead correctly
                _unwrappedPlayHeadTime = playHeadPosition;
                _normalizedPlayHeadTime = playHeadPosition;
            }
        }

        public virtual void Stop()
        {
            if (!_isPlaying) return;
            _isPlaying = false;
            SPTween.RemoveReference(this);
            if (this.OnStopped != null) this.OnStopped(this, System.EventArgs.Empty);
        }

        public virtual void Kill()
        {
            this.Stop();
            _time = float.NaN;
        }

        /// <summary>
        /// Called internally if killed by SPTween.
        /// </summary>
        internal void SetKilled()
        {
            _isPlaying = false;
            _time = float.NaN;
        }

        public virtual void Reset()
        {
            this.Stop();
            _currentWrapCount = 0;
            _time = 0f;
            _unwrappedPlayHeadTime = 0f;
            _normalizedPlayHeadTime = 0f;
        }

        public bool CompleteImmediately()
        {
            if (!this.IsPlaying) return false;

            switch (_wrap)
            {
                case TweenWrapMode.Once:
                    float odt = this.PlayHeadLength - _time;
                    _time = this.PlayHeadLength + _delay + 0.0001f;
                    _normalizedPlayHeadTime = (_reverse) ? 0f : _time;
                    _unwrappedPlayHeadTime = _normalizedPlayHeadTime;
                    this.DoUpdate(odt, _normalizedPlayHeadTime);
                    this.Stop();
                    if (this.OnFinish != null) this.OnFinish(this, System.EventArgs.Empty);
                    return true;
                case TweenWrapMode.Loop:
                case TweenWrapMode.PingPong:
                    if (_wrapCount <= 0)
                    {
                        //this doesn't make sense... you can't complete an infinite tween
                    }
                    else
                    {
                        float pdt = (this.PlayHeadLength * _wrapCount) - _time;
                        _time = this.PlayHeadLength * _wrapCount + _delay + 0.0001f;
                        _normalizedPlayHeadTime = (_reverse) ? 0f : (_wrapCount % 2 == 0) ? 0f : this.PlayHeadLength;
                        _unwrappedPlayHeadTime = _normalizedPlayHeadTime;
                        this.DoUpdate(pdt, _normalizedPlayHeadTime);
                        this.Stop();
                        if (this.OnFinish != null) this.OnFinish(this, System.EventArgs.Empty);
                        return true;
                    }
                    break;
            }

            return false;
        }

        public virtual void Scrub(float dt)
        {
            if (this.IsDead) return;

            this.MovePlayHeadPosition(dt);

            this.DoUpdate(dt, _normalizedPlayHeadTime);

            switch (_wrap)
            {
                case TweenWrapMode.Once:
                    if (this.IsComplete)
                    {
                        _time = this.PlayHeadLength + _delay + 0.0001f;
                        this.Stop();
                        if (this.OnFinish != null) this.OnFinish(this, System.EventArgs.Empty);
                        break;
                    }
                    else
                    {
                        if (this.OnStep != null) this.OnStep(this, System.EventArgs.Empty);
                    }
                    break;
                case TweenWrapMode.Loop:
                case TweenWrapMode.PingPong:
                    if (_time > this.PlayHeadLength * (_currentWrapCount + 1))
                    {
                        _currentWrapCount++;
                        if (this.IsComplete)
                        {
                            _time = this.PlayHeadLength * _wrapCount + _delay + 0.0001f;
                            this.Stop();
                            if (this.OnFinish != null) this.OnFinish(this, System.EventArgs.Empty);
                        }
                        else
                        {
                            if (this.OnStep != null) this.OnStep(this, System.EventArgs.Empty);
                            if (this.OnWrap != null) this.OnWrap(this, System.EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if (this.OnStep != null) this.OnStep(this, System.EventArgs.Empty);
                    }
                    break;
            }
        }

        private void MovePlayHeadPosition(float dt)
        {
            _time += Mathf.Abs(dt);
            if (_reverse)
                _unwrappedPlayHeadTime -= dt;
            else
                _unwrappedPlayHeadTime += dt;

            var totalDur = this.PlayHeadLength;
            if (totalDur > 0f)
            {
                switch (_wrap)
                {
                    case TweenWrapMode.Once:
                        _normalizedPlayHeadTime = Mathf.Clamp(_unwrappedPlayHeadTime - _delay, 0, totalDur);
                        break;
                    case TweenWrapMode.Loop:
                        if (_unwrappedPlayHeadTime < _delay)
                            _normalizedPlayHeadTime = 0f;
                        else
                            _normalizedPlayHeadTime = Mathf.Repeat(_unwrappedPlayHeadTime - _delay, totalDur);
                        break;
                    case TweenWrapMode.PingPong:
                        if (_normalizedPlayHeadTime < _delay)
                            _normalizedPlayHeadTime = 0f;
                        else
                            _normalizedPlayHeadTime = Mathf.PingPong(_unwrappedPlayHeadTime - _delay, totalDur);
                        break;
                }
            }
            else
            {
                _normalizedPlayHeadTime = 0f;
            }
        }

        internal virtual void Update()
        {
            this.Scrub(_timeSupplier.Delta * _speedScale);
        }

        #endregion

        #region Tweener Interface

        protected internal abstract float GetPlayHeadLength();

        protected internal abstract void DoUpdate(float dt, float t);

        #endregion

        #region IDisposable Interface

        bool ISPDisposable.IsDisposed
        {
            get { return this.IsDead; }
        }

        void System.IDisposable.Dispose()
        {
            if (this.IsDead) return;

            this.Kill();
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IRadicalYieldInstruction.IsComplete
        {
            get { return this.IsComplete; }
        }

        float IProgressingYieldInstruction.Progress
        {
            get
            {
                if (this.IsComplete) return 1f;

                switch (_wrap)
                {
                    case TweenWrapMode.Once:
                        return _time / (this.PlayHeadLength + _delay);
                    case TweenWrapMode.Loop:
                    case TweenWrapMode.PingPong:
                        if (_wrapCount <= 0)
                            return 0f;
                        else
                            return _time / (this.PlayHeadLength * _wrapCount + _delay);
                }
                return 0f;
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return !this.IsComplete;
        }

        void IRadicalWaitHandle.OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            System.EventHandler d = null;
            d = (s, e) =>
            {
                this.OnStopped -= d;
                callback(this);
            };

            this.OnStopped += d;
        }

        #endregion

    }


    public class TweenerGroup : Tweener
    {

        #region Fields

        private object _id;
        private Tweener[] _tweens;

        #endregion

        #region CONSTRUCTOR

        public TweenerGroup(IEnumerable<Tweener> tweens)
        {
            _tweens = tweens.ToArray();
        }

        internal TweenerGroup(Tweener[] tweens)
        {
            _tweens = tweens;
        }

        #endregion


        public override object Id
        {
            get
            {
                if (_id != null) return _id;

                foreach (var twn in _tweens)
                {
                    var id = twn.Id;
                    if (id != null) return id;
                }

                return null;
            }
            set
            {
                _id = value;
            }
        }

        internal override void Update()
        {
            base.Update();

            foreach (var twn in _tweens)
            {
                twn.Update();
            }
        }

        protected internal override void DoUpdate(float dt, float t)
        {
            //do nothing
        }

        protected internal override float GetPlayHeadLength()
        {
            float length = 0f;

            foreach (var twn in _tweens)
            {
                float l = twn.GetPlayHeadLength();
                if (l > length) length = l;
            }

            return length;
        }

    }

}