using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween
{

    public abstract class Tweener : System.IDisposable, IProgressingYieldInstruction
    {

        #region Events

        public event System.EventHandler OnStep;
        public event System.EventHandler OnWrap;
        public event System.EventHandler OnFinish;

        #endregion

        #region Fields

        private object _id;
        private UpdateSequence _updateType;
        private ITimeSupplier _timeSupplier = SPTime.Normal;
        private TweenWrapMode _wrap;
        private int _wrapCount;
        private bool _reverse;
        private float _speedScale = 1.0f;


        private bool _isPlaying;
        private float _playHeadLength;
        private float _time; //the time since the tween was first played
        private float _unwrappedPlayHeadTime; //we need an unwrapped value so that we can pingpong/loop the playhead
        private float _normalizedPlayHeadTime; //this position the playhead is currently at, with wrap applied

        private int _currentWrapCount;

        #endregion

        #region Configurable Properties

        public object Id
        {
            get { return _id; }
            set { _id = value; }
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
                        return _time >= this.PlayHeadLength;
                    case TweenWrapMode.Loop:
                    case TweenWrapMode.PingPong:
                        if (_wrapCount <= 0)
                            return false;
                        else
                            return _time >= (this.PlayHeadLength * _wrapCount);
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
        }

        public virtual void Kill()
        {
            this.Stop();
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
                    _time = this.PlayHeadLength;
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
                        _time = this.PlayHeadLength * _wrapCount;
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
                        _time = this.PlayHeadLength;
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
                            _time = this.PlayHeadLength * _wrapCount;
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
                        _normalizedPlayHeadTime = Mathf.Clamp(_unwrappedPlayHeadTime, 0, totalDur);
                        break;
                    case TweenWrapMode.Loop:
                        _normalizedPlayHeadTime = Mathf.Repeat(_unwrappedPlayHeadTime, totalDur);
                        break;
                    case TweenWrapMode.PingPong:
                        _normalizedPlayHeadTime = Mathf.PingPong(_unwrappedPlayHeadTime, totalDur);
                        break;
                }
            }
            else
            {
                _normalizedPlayHeadTime = 0f;
            }
        }

        internal void Update()
        {
            this.Scrub(_timeSupplier.Delta * _speedScale);
        }

        #endregion

        #region Tweener Interface

        protected abstract float GetPlayHeadLength();

        protected abstract void DoUpdate(float dt, float t);

        #endregion

        #region IDisposable Interface

        void System.IDisposable.Dispose()
        {
            if (this.IsDead) return;

            this.Kill();
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IProgressingYieldInstruction.IsComplete
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
                        return _time / this.PlayHeadLength;
                    case TweenWrapMode.Loop:
                    case TweenWrapMode.PingPong:
                        if (_wrapCount <= 0)
                            return 0f;
                        else
                            return _time / (this.PlayHeadLength * _wrapCount);
                }
                return 0f;
            }
        }

        bool IRadicalYieldInstruction.ContinueBlocking()
        {
            return !this.IsComplete;
        }

        object IRadicalYieldInstruction.CurrentYieldObject
        {
            get { return null; }
        }

        #endregion

    }

}