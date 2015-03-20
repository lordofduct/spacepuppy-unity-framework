using UnityEngine;

using System;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Tween
{

    public abstract class Tweener
    {

        #region Events

        public event System.EventHandler OnPlay;
        public event System.EventHandler OnStep;
        public event System.EventHandler OnWrap;
        public event System.EventHandler OnFinish;

        #endregion

        #region Fields

        private UpdateSequence _updateType;
        private DeltaTimeType _deltaType;
        private TweenWrapMode _wrap;
        private int _wrapCount;
        private bool _reverse;

        private bool _isRunning;
        private bool _isComplete;

        private float _currentTime;
        private float _playHeadTime;
        private float _normalizedPlayHeadTime;

        private int _currentWrapCount;

        #endregion

        #region CONSTRUCTOR

        public Tweener()
        {
        }

        #endregion

        #region Configurable Properties

        public UpdateSequence UpdateType
        {
            get { return _updateType; }
            set { _updateType = value; }
        }

        public DeltaTimeType DeltaType
        {
            get { return _deltaType; }
            set { _deltaType = value; }
        }

        public TweenWrapMode WrapMode
        {
            get { return _wrap; }
            set
            {
                if (_wrap == value) return;

                _wrap = value;
                //normalized time is dependent on WrapMode, so we force update the play head
                this.Scrub(0f);
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

        #endregion

        #region Status Properties

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
        }

        public float CurrentTime
        {
            get { return _currentTime; }
        }

        public float PlayHeadTime
        {
            get { return _playHeadTime; }
        }

        public float NormalizedPlayHeadTime
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

        public abstract object Target { get; }

        public abstract float TotalDuration { get; }

        #endregion

        #region Methods

        public void Play()
        {
            if (_isRunning) return;
            _isRunning = true;
            if (_currentTime == 0f && _reverse)
            {
                //if this the first time we're playing, and we're in reverse, then make sure we set the playhead correctly
                _playHeadTime = this.TotalDuration;
                _normalizedPlayHeadTime = this.TotalDuration;
            }
            SPTween.AddReference(this);
            if (this.OnPlay != null) this.OnPlay(this, System.EventArgs.Empty);
        }

        public void Play(float playHeadPosition)
        {
            _isRunning = true;
            if (_currentTime == 0f && _reverse)
            {
                //if this the first time we're playing, and we're in reverse, then make sure we set the playhead correctly
                _playHeadTime = this.TotalDuration;
                _normalizedPlayHeadTime = this.TotalDuration;
            }
            SPTween.AddReference(this);
            if (this.OnPlay != null) this.OnPlay(this, System.EventArgs.Empty);
        }

        public virtual void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;
            SPTween.RemoveReference(this);
        }

        public virtual void Reset()
        {
            this.Stop();
            _currentWrapCount = 0;
            _currentTime = 0f;
            _playHeadTime = 0f;
            _normalizedPlayHeadTime = 0f;
            _isComplete = false;
        }

        public void Scrub(float dt)
        {
            _currentTime += Mathf.Abs(dt);
            if (_reverse)
                _playHeadTime -= dt;
            else
                _playHeadTime += dt;

            var totalDur = this.TotalDuration;
            if (totalDur > 0f)
            {
                switch (_wrap)
                {
                    case TweenWrapMode.Once:
                        _normalizedPlayHeadTime = Mathf.Clamp(_playHeadTime, 0, totalDur);
                        break;
                    case TweenWrapMode.Loop:
                        _normalizedPlayHeadTime = Mathf.Repeat(_playHeadTime, totalDur);
                        break;
                    case TweenWrapMode.PingPong:
                        _normalizedPlayHeadTime = Mathf.PingPong(_playHeadTime, totalDur);
                        break;
                }
            }
            else
            {
                _normalizedPlayHeadTime = 0f;
            }
        }

        internal void Update(float dt)
        {
            this.Scrub(dt);

            this.DoUpdate(dt, _normalizedPlayHeadTime);

            switch (_wrap)
            {
                case TweenWrapMode.Once:
                    if (_currentTime > this.TotalDuration)
                    {
                        _currentTime = this.TotalDuration;
                        this.Stop();
                        _isComplete = true;
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
                    if (_currentTime > this.TotalDuration * (_currentWrapCount + 1))
                    {
                        _currentWrapCount++;
                        if (_wrapCount > 0 && _currentWrapCount >= _wrapCount)
                        {
                            _currentTime = this.TotalDuration * _wrapCount;
                            this.Stop();
                            _isComplete = true;
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

        /// <summary>
        /// Update the position of all values.
        /// </summary>
        /// <param name="dt">The delta/change in time since last update.</param>
        /// <param name="t">The current position of time normalized by WrapMode.</param>
        protected abstract void DoUpdate(float dt, float ct);

        #endregion

    }

}