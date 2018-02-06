using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim.Blend
{

    /// <summary>
    /// Select an animation from a indexed collection.
    /// 
    /// Assumes that all anims in the collection operate on the same TimeSupplier.
    /// </summary>
    public class IndexedAnimationSelector : ICollection<ISPAnim>, ISPAnim, System.IDisposable
    {

        #region Fields

        private SelectorIndexStyle _indexStyle;
        private List<ISPAnim> _states = new List<ISPAnim>();

        private int _layer;
        private float _speed = 1f;
        private ITimeSupplier _timeSupplier;
        private float _fadeLength;
        private int _currentIndex;
        private ISPAnim _currentState;

        private SPAnimationController _controller;
        private AnimEventScheduler _scheduler;

        #endregion
        
        #region CONSTRUCTOR

        public IndexedAnimationSelector(SPAnimationController controller, int layer = 0)
        {
            _controller = controller;
            _indexStyle = SelectorIndexStyle.Clamp;
            _layer = layer;
        }

        public IndexedAnimationSelector(SPAnimationController controller, SelectorIndexStyle indexStyle, int layer = 0)
        {
            _controller = controller;
            _indexStyle = indexStyle;
            _layer = layer;
        }

        #endregion

        #region Properties

        public SelectorIndexStyle IndexStyle
        {
            get { return _indexStyle; }
            set
            {
                _indexStyle = value;
            }
        }

        public float FadeLength
        {
            get { return _fadeLength; }
            set { _fadeLength = value; }
        }

        public int Index
        {
            get { return _currentIndex; }
            set
            {
                if (_states.Count == 0)
                {
                    _currentIndex = 0;
                }
                else
                {
                    switch(_indexStyle)
                    {
                        case SelectorIndexStyle.Clamp:
                            _currentIndex = Mathf.Clamp(value, _states.Count - 1, 0);
                            break;
                        case SelectorIndexStyle.Wrap:
                            _currentIndex = MathUtil.Wrap(value, _states.Count, 0);
                            break;
                        default:
                            _currentIndex = 0;
                            break;
                    }
                    if (this.IsPlaying) this.PlayAnimationAtCurrentPosition();
                }
            }
        }

        public ISPAnim this[int index]
        {
            get { return _states[index]; }
        }

        #endregion

        #region Methods

        public ISPAnim GetAnimAtIndex(int index)
        {
            return _states[index];
        }

        private void PlayAnimationAtCurrentPosition(QueueMode queueMode = QueueMode.CompleteOthers, PlayMode playMode = PlayMode.StopSameLayer, float? crossFadeOverride = null)
        {
            if (_states.Count == 0)
            {
                this.Stop();
                return;
            }

            switch (_indexStyle)
            {
                case SelectorIndexStyle.Clamp:
                    _currentIndex = Mathf.Clamp(_currentIndex, _states.Count - 1, 0);
                    break;
                case SelectorIndexStyle.Wrap:
                    _currentIndex = MathUtil.Wrap(_currentIndex, _states.Count, 0);
                    break;
                default:
                    _currentIndex = 0;
                    break;
            }

            var state = _states[_currentIndex];

            if (state == null)
            {
                if (_currentState != null) _currentState.Stop();
                _currentState = null;
                return;
            }
            else if (state == _currentState && state.IsPlaying)
            {
                if (state.Speed != _speed) state.Speed = _speed;
                return;
            }

            _currentState = state;
            _currentState.TimeSupplier = _timeSupplier;
            _currentState.Layer = _layer;
            float fade = (crossFadeOverride != null) ? crossFadeOverride.Value : _fadeLength;
            if (fade <= 0f)
            {
                _currentState.Play(_speed, queueMode, playMode);
            }
            else
            {
                _currentState.CrossFade(_speed, fade, queueMode, playMode);
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
                if (_currentState != null) _currentState.Speed = _speed;
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
                    _currentState.TimeSupplier = _timeSupplier;
            }
        }

        public bool IsPlaying
        {
            get { return _currentState != null && _currentState.IsPlaying; }
        }

        public float Time
        {
            get
            {
                return (_currentState != null) ? _currentState.Time : 0f;
            }
            set
            {
                if (_currentState != null) _currentState.Time = value;
            }
        }

        public float Duration
        {
            get
            {
                return (_currentState != null) ? _currentState.Duration : 0f;
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
                _currentState.Stop();
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

        public void Add(ISPAnim item)
        {
            if (item == null) throw new System.ArgumentNullException("item");
            _states.Add(item);
        }

        public bool Contains(ISPAnim state)
        {
            return _states.Contains(state);
        }

        public bool Remove(ISPAnim state)
        {
            if(_states.Remove(state))
            {
                if (this.IsPlaying) this.PlayAnimationAtCurrentPosition();
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
            _states.Clear();
        }

        public void CopyTo(ISPAnim[] array, int arrayIndex)
        {
            _states.CopyTo(array);
        }

        public int Count
        {
            get { return _states.Count; }
        }

        bool ICollection<ISPAnim>.IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<ISPAnim> GetEnumerator()
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
            if (_scheduler != null)
            {
                _scheduler.Dispose();
                _scheduler = null;
            }
        }

        #endregion

    }
}
