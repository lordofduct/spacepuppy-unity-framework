using UnityEngine;

using System;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Tween
{

    public abstract class Tweener : IDisposable
    {

        #region Events

        public event System.EventHandler OnStep;
        public event System.EventHandler OnWrap;
        public event System.EventHandler OnFinish;

        #endregion

        #region Fields

        private UpdateSequence _updateType;
        private DeltaTimeType _deltaType;
        private TweenWrapMode _wrap;
        private int _wrapCount;

        private bool _isRunning;
        private bool _isComplete;
        private float _currentTime;
        private float _normalizedTime;
        private int _currentWrapCount;

        #endregion

        #region CONSTRUCTOR

        public Tweener()
        {
        }

        #endregion

        #region Properties

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
            set { _wrap = value; }
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

        public float CurrentTime
        {
            get { return _currentTime; }
        }

        public float NormalizedTime
        {
            get { return _normalizedTime; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
        }

        public int CurrentWrapCount
        {
            get { return _currentWrapCount; }
        }

        public abstract object Target { get; }

        public abstract float TotalDuration { get; }

        #endregion

        #region Methods

        public virtual void Start()
        {
            _isRunning = true;
            SPTween.Instance.AddReference(this);
        }

        public virtual void Stop()
        {
            _isRunning = false;
            SPTween.Instance.RemoveReference(this);
        }

        public virtual void Reset()
        {
            this.Stop();
            _currentWrapCount = 0;
            _currentTime = 0;
            _isComplete = false;
        }

        internal void Update(float dt)
        {
            _currentTime += dt;

            var totalDur = this.TotalDuration;
            if (totalDur > 0f)
            {
                switch (_wrap)
                {
                    case TweenWrapMode.Once:
                        _normalizedTime = Mathf.Clamp(_currentTime, 0, totalDur);
                        break;
                    case TweenWrapMode.Loop:
                        _normalizedTime = Mathf.Repeat(_currentTime, totalDur);
                        break;
                    case TweenWrapMode.PingPong:
                        _normalizedTime = Mathf.PingPong(_currentTime, totalDur);
                        break;
                }
            }
            else
            {
                _normalizedTime = 0f;
            }

            this.DoUpdate(dt, _normalizedTime);

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

        #region IDisposable

        public bool IsDisposed
        {
            get;
            private set;
        }

        public virtual void Dispose()
        {
            SPTween.Instance.RemoveReference(this);
            IsDisposed = true;
        }

        #endregion

    }

}