using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim.Blend
{

    /// <summary>
    /// Select an animation across a linear gradient and blends them together.
    /// 
    /// Assumes that all anims in the collection operate on the same TimeSupplier.
    /// </summary>
    public class LinearAnimationBlend : ICollection<LinearAnimationBlend.StateData>, ISPAnim, System.IDisposable
    {

        #region Fields

        private List<StateData> _states = new List<StateData>();

        private int _layer;
        private float _speed = 1f;
        private ITimeSupplier _timeSupplier;
        private float _currentT;
        private StateData _currentStateL;
        private StateData _currentStateH;

        private SPAnimationController _controller;
        private AnimEventScheduler _scheduler;

        #endregion

        #region CONSTRUCTOR

        public LinearAnimationBlend(SPAnimationController controller, int layer = 0)
        {
            _controller = controller;
            _layer = layer;
        }

        #endregion

        #region Properties

        public float Position
        {
            get { return _currentT; }
            set
            {
                if(_states.Count == 0)
                {
                    _currentT = 0;
                }
                else
                {
                    _currentT = Mathf.Clamp(value, 0f, _states.Last().Position);
                    if (_currentStateL != null) this.PlayAnimationAtCurrentPosition();
                }
            }
        }

        #endregion

        #region Methods

        public StateData Add(ISPAnim state, float pos)
        {
            if (state == null) throw new System.ArgumentNullException("state");

            var data = new StateData(this, state, pos);
            _states.Add(data);
            _states.Sort(SortSubStates);

            if (_currentStateL != null) this.PlayAnimationAtCurrentPosition();

            return data;
        }

        private void PlayAnimationAtCurrentPosition(QueueMode queueMode = QueueMode.CompleteOthers, PlayMode mode = PlayMode.StopSameLayer)
        {
            if(_states.Count == 0)
            {
                this.Stop();
                return;
            }

            StateData stateL;
            StateData stateH;
            for (int i = 0; i < _states.Count; i++)
            {
                if (_currentT <= _states[i].Position)
                {
                    stateL = _states[i];
                    stateH = (i + 1 < _states.Count) ? _states[i + 1] : null;
                    break;
                }
            }

            //TODO - implement blend
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
                if (_currentStateL != null) _currentStateL.Anim.Speed = _speed;
                if (_currentStateH != null) _currentStateH.Anim.Speed = _speed;
            }
        }

        public ITimeSupplier TimeSupplier
        {
            get { return _timeSupplier ?? SPTime.Normal; }
            set
            {
                if (_timeSupplier == value) return;

                _timeSupplier = value;
                if (_currentStateL != null)
                    _currentStateL.Anim.TimeSupplier = _timeSupplier;
                if (_currentStateH != null)
                    _currentStateH.Anim.TimeSupplier = _timeSupplier;
            }
        }

        public bool IsPlaying
        {
            get { return _currentStateL != null; }
        }

        public float Time
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public float Duration
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            this.PlayAnimationAtCurrentPosition(queueMode, playMode);
        }
        
        public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            this.PlayAnimationAtCurrentPosition(queueMode, playMode);
        }
        
        public void Stop()
        {
            if (_currentStateL != null)
            {
                _currentStateL.Anim.Stop();
                _currentStateL = null;
            }
            if (_currentStateH != null)
            {
                _currentStateH.Anim.Stop();
                _currentStateH = null;
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
                return _states.Remove(state);
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
            if (_scheduler != null)
            {
                _scheduler.Dispose();
                _scheduler = null;
            }
        }

        #endregion




        #region Static Utils

        private static int SortSubStates(StateData a, StateData b)
        {
            if (a.Position < b.Position) return -1;
            else if (a.Position > b.Position) return 1;
            else return 0;
        }

        #endregion

        #region Special Types

        public class StateData
        {
            private LinearAnimationBlend _owner;
            private ISPAnim _anim;
            private float _pos;


            internal StateData(LinearAnimationBlend owner, ISPAnim anim, float pos)
            {
                _owner = owner;
                _anim = anim;
                _pos = pos;
            }

            public ISPAnim Anim
            {
                get { return _anim; }
                set
                {
                    if (value == null) throw new System.ArgumentNullException("value");
                    _anim = value;
                    if(_owner != null && _owner.IsPlaying && (_owner._currentStateL == this || _owner._currentStateH == this))
                    {
                        _owner.PlayAnimationAtCurrentPosition();
                    }
                }
            }

            public float Position
            {
                get { return _pos; }
                set
                {
                    _pos = value;
                    if(_owner != null && _owner.IsPlaying) _owner.PlayAnimationAtCurrentPosition();
                }
            }


            internal void Purge()
            {
                _owner = null;
            }

        }

        #endregion

    }
}
