using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim.Blend
{

    /// <summary>
    /// Select an animation across a linear gradient.
    /// 
    /// Assumes that all anims in the collection operate on the same TimeSupplier.
    /// </summary>
    public class LinearAnimationSelector : ICollection<LinearAnimationSelector.StateData>, ISPAnim, System.IDisposable
    {
        
        #region Fields

        private SelectorThresholdStyle _thresholdStyle;
        private List<StateData> _states = new List<StateData>();

        private int _layer;
        private float _speed = 1f;
        private ITimeSupplier _timeSupplier;
        private float _currentT;
        private StateData _currentState;

        private SPAnimationController _controller;
        private AnimEventScheduler _scheduler;

        private Queue<QueueData> _positionQueue = new Queue<QueueData>();
        private bool _queuePositionScheduledCallbackIsActive = false;

        #endregion

        #region CONSTRUCTOR

        public LinearAnimationSelector(SPAnimationController controller, int layer = 0)
        {
            _controller = controller;
            _thresholdStyle = SelectorThresholdStyle.Threshold;
            _layer = layer;
        }

        public LinearAnimationSelector(SPAnimationController controller, SelectorThresholdStyle thresholdStyle, int layer = 0)
        {
            _controller = controller;
            _thresholdStyle = thresholdStyle;
            _layer = layer;
        }

        #endregion

        #region Properties

        public SelectorThresholdStyle ThresholdStyle
        {
            get { return _thresholdStyle; }
            set
            {
                _thresholdStyle = value;
                if (this.IsPlaying) this.PlayAnimationAtCurrentPosition();
            }
        }

        public float Position
        {
            get { return _currentT; }
            set
            {
                if (_states.Count == 0)
                {
                    _currentT = 0;
                }
                else
                {
                    //_currentT = Mathf.Clamp(value, Mathf.Min(_states[0].Threshold, 0f), _states.Last().Threshold);
                    _currentT = value;
                    if (this.IsPlaying) this.PlayAnimationAtCurrentPosition();
                }
            }
        }

        #endregion

        #region Methods

        public StateData Add(ISPAnim state, float threshold, float fadeLength = 0f)
        {
            if (state == null) throw new System.ArgumentNullException("state");

            var data = new StateData(this, state, threshold, fadeLength);
            _states.Add(data);
            _states.Sort(SortSubStates);

            if (this.IsPlaying) this.PlayAnimationAtCurrentPosition();

            return data;
        }

        /// <summary>
        /// Change the position value once the currently selected animation is done. 
        /// </summary>
        /// <param name="pos"></param>
        public void QueuePositionChange(float pos, PlayMode playMode = PlayMode.StopSameLayer, bool overwritePreviouslyQueued = false)
        {
            if(!this.IsPlaying)
            {
                this.Position = pos;
                this.Play(QueueMode.PlayNow, playMode);
            }
            else
            {
                if (overwritePreviouslyQueued) _positionQueue.Clear();
                _positionQueue.Enqueue(new QueueData(pos, playMode));
                if(!_queuePositionScheduledCallbackIsActive)
                {
                    this.Schedule(this.QueuePositionScheduledCallback);
                    _queuePositionScheduledCallbackIsActive = true;
                }
            }
        }

        private void QueuePositionScheduledCallback(ISPAnim state)
        {
            if(_positionQueue.Count == 0) return;

            var data = _positionQueue.Dequeue();
            this.Position = data.Position;
            this.Play(QueueMode.PlayNow, data.PlayMode);

            if(_positionQueue.Count > 0)
            {
                this.Schedule(this.QueuePositionScheduledCallback);
            }
            else
            {
                _queuePositionScheduledCallbackIsActive = false;
            }
        }

        public ISPAnim GetAnimationAtPosition(float pos)
        {
            var data = this.GetAnimationDataAtPosition(pos);
            if (data == null) return null;
            return data.Anim;
        }

        private StateData GetAnimationDataAtPosition(float pos)
        {
            if (_states.Count == 0)
            {
                return null;
            }

            switch (_thresholdStyle)
            {
                case SelectorThresholdStyle.Threshold:
                    for (int i = 0; i < _states.Count; i++)
                    {
                        if (pos <= _states[i].Threshold)
                        {
                            return _states[i];
                        }
                    }
                    break;
                case SelectorThresholdStyle.Discrete:
                    for (int i = _states.Count - 1; i > -1; i--)
                    {
                        if (pos >= _states[i].Threshold)
                        {
                            return _states[i];
                        }
                    }
                    break;
            }
            return null;
        }

        private void PlayAnimationAtCurrentPosition(QueueMode queueMode = QueueMode.CompleteOthers, PlayMode playMode = PlayMode.StopSameLayer, float? crossFadeOverride = null)
        {
            if (_states.Count == 0)
            {
                this.Stop();
                return;
            }

            var state = this.GetAnimationDataAtPosition(_currentT);

            if (state == null) return;
            if (state.Anim == null) return;
            if (state == _currentState && state.Anim.IsPlaying)
            {
                if (state.Anim.Speed != _speed) state.Anim.Speed = _speed;
                return;
            }
            
            _currentState = state;
            _currentState.Anim.TimeSupplier = _timeSupplier;
            _currentState.Anim.Layer = _layer;
            float fade = (crossFadeOverride != null) ? crossFadeOverride.Value : state.FadeLength;
            if (fade <= 0f)
            {
                _currentState.Anim.Play(_speed, queueMode, playMode);
            }
            else
            {
                _currentState.Anim.CrossFade(_speed, fade, queueMode, playMode);
            }
        }

        #endregion

        #region ISPAnim Interface

        public SPAnimationController Controller
        {
            get { return _controller; }
        }

        public int Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                if (_currentState != null) _currentState.Anim.Speed = _speed;
            }
        }

        public ITimeSupplier TimeSupplier
        {
            get { return _timeSupplier ?? SPTime.Normal; }
            set
            {
                if (_timeSupplier == value) return;

                _timeSupplier = value;
                if (_currentState != null)
                    _currentState.Anim.TimeSupplier = _timeSupplier;
            }
        }

        public bool IsPlaying
        {
            get { return _currentState != null && _currentState.Anim.IsPlaying; }
        }

        public float Time
        {
            get
            {
                return (_currentState != null) ? _currentState.Anim.Time : 0f;
            }
            set
            {
                if (_currentState != null) _currentState.Anim.Time = value;
            }
        }

        public float Duration
        {
            get
            {
                return (_currentState != null) ? _currentState.Anim.Duration : 0f;
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

        public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            this.PlayAnimationAtCurrentPosition(queueMode, playMode);
        }
        
        public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            this.PlayAnimationAtCurrentPosition(queueMode, playMode, fadeLength);
        }
        
        public void Stop()
        {
            if (_currentState != null)
            {
                _currentState.Anim.Stop();
                _currentState = null;
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

        #region ICollection Interface

        void ICollection<StateData>.Add(StateData item)
        {
            throw new System.NotSupportedException();
        }

        public bool Contains(StateData state)
        {
            return _states.Contains(state);
        }

        public bool Remove(StateData state)
        {
            if(_states.Remove(state))
            {
                state.Purge();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            this.Stop();
            foreach (var s in _states) s.Purge();
            _states.Clear();
        }

        public void CopyTo(StateData[] array, int arrayIndex)
        {
            _states.CopyTo(array);
        }

        public int Count
        {
            get { return _states.Count; }
        }

        bool ICollection<StateData>.IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<StateData> GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            if(_scheduler != null)
            {
                _scheduler.Dispose();
                _scheduler = null;
            }
        }

        #endregion

        #region Static Utils

        private static int SortSubStates(StateData a, StateData b)
        {
            if (a.Threshold < b.Threshold) return -1;
            else if (a.Threshold > b.Threshold) return 1;
            else return 0;
        }

        #endregion

        #region Special Types

        public class StateData
        {
            private LinearAnimationSelector _owner;
            private ISPAnim _anim;
            private float _threshold;
            private float _fadeLength;


            internal StateData(LinearAnimationSelector owner, ISPAnim anim, float threshold, float fadeLength)
            {
                _owner = owner;
                _anim = anim;
                _threshold = threshold;
                _fadeLength = fadeLength;
            }

            public ISPAnim Anim
            {
                get { return _anim; }
                set
                {
                    if (value == null) throw new System.ArgumentNullException("value");
                    _anim = value;
                    if(_owner != null && _owner._currentState == this)
                    {
                        _anim.Play();
                    }
                }
            }

            public float Threshold
            {
                get { return _threshold; }
                set
                {
                    _threshold = value;
                    if (_owner != null && _owner._currentState != null) _owner.PlayAnimationAtCurrentPosition();
                }
            }

            public float FadeLength
            {
                get { return _fadeLength; }
                set { _fadeLength = value; }
            }




            internal void Purge()
            {
                _owner = null;
            }

        }

        private struct QueueData
        {
            public float Position;
            public PlayMode PlayMode;

            public QueueData(float pos, PlayMode mode)
            {
                this.Position = pos;
                this.PlayMode = mode;
            }
        }

        #endregion

    }
}
