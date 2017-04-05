using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;
using System.Collections;

namespace com.spacepuppy.Anim
{

    /// <summary>
    /// This is a special class that gets attached to an Animation when any special relational data needs to be associated with that object.
    /// 
    /// TODO LIST -
    /// Add SPAnim disposal to this to allow auto-complete of animations.
    /// 
    /// Add more efficient Scaled Time Layer handling here rather than where it is.
    /// 
    /// Add a way to update all playing animations if 'speed' changes at runtime
    /// </summary>
    [DisallowMultipleComponent()]
    public class SPAnimationController : SPComponent, ISPAnimationSource
    {

        #region Fields

        [SerializeField()]
        private bool _animatePhysics;
        [SerializeField()]
        private AnimationCullingType _animCullingType = AnimationCullingType.BasedOnRenderers;

        [SerializeField()]
        private SPTime _timeSupplier;
        [SerializeField()]
        private float _speed = 1f;

        [SerializeField()]
        private SPAnimClipCollection _states = new SPAnimClipCollection();

        [SerializeField()]
        private string _animToPlayOnStart = null;




        [System.NonSerialized()]
        private Animation _animation;


        [System.NonSerialized()]
        private Dictionary<string, AnimationCallbackData> _animEventTable;

        [System.NonSerialized]
        private ScriptableAnimCollection _scriptableAnims;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _animation = this.AddOrGetComponent<Animation>();
            _animation.playAutomatically = false;

            _states.Init(this, null);
        }

        protected override void Start()
        {
            base.Start();

            //_animation.playAutomatically = false;
            _animation.animatePhysics = _animatePhysics;
            _animation.cullingType = _animCullingType;
            //this.enabled = false;

            if (this.States.ContainsKey(_animToPlayOnStart))
            {
                var state = this.States[_animToPlayOnStart];
                if (state != null) state.PlayDirectly(PlayMode.StopAll);
            }
        }

        #endregion

        #region Properties

        public new Animation animation { get { return _animation; } }

        public SPAnimClipCollection States { get { return _states; } }

        public ITimeSupplier TimeSupplier { get { return _timeSupplier.TimeSupplier; } }

        public float Speed { get { return _speed; } }

        public bool AnimatePhysics
        {
            get
            {
                return (_animation != null) ? _animation.animatePhysics : _animatePhysics;
            }
            set
            {
                _animatePhysics = value;
                if (_animation != null) _animation.animatePhysics = value;
            }
        }

        public AnimationCullingType CullingType
        {
            get { return (_animation != null) ? _animation.cullingType : _animCullingType; }
            set
            {
                _animCullingType = value;
                if (_animation != null) _animation.cullingType = value;
            }
        }

        #endregion

        #region Methods

        public ISPAnim Play(string clipId, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            var state = _states[clipId];
            //if (state == null) throw new UnknownStateException(clipId);
            if (state == null) return null;

            return state.Play(queueMode, playMode);
        }

        public ISPAnim CrossFade(string clipId, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            var state = _states[clipId];
            //if (state == null) throw new UnknownStateException(clipId);
            if (state == null) return null;
            
            return state.CrossFade(fadeLength, queueMode, playMode);
        }

        public ISPAnim Play(IScriptableAnimationClip clip, PlayMode mode = PlayMode.StopSameLayer)
        {
            var state = CreateScriptableAnimState(this, clip);
            state.Play(QueueMode.PlayNow, mode);
            return state;
        }




        internal void PlayInternal(string clipId, PlayMode mode, int layer)
        {
            _animation.Play(clipId, mode);

            if(_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (mode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear();
                }
                else
                {
                    _scriptableAnims.Remove(layer);
                }
            }
        }

        internal void CrossFadeInternal(string clipId, float fadeLength, PlayMode mode, int layer)
        {
            _animation.CrossFade(clipId, fadeLength, mode);

            if (_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (mode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear();
                }
                else
                {
                    _scriptableAnims.Remove(layer);
                }
            }
        }

        internal AnimationState PlayQueuedInternal(string clipId, QueueMode queueMode, PlayMode playMode, int layer)
        {
            var anim = _animation.PlayQueued(clipId, queueMode, playMode);

            if (_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (playMode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear();
                }
                else
                {
                    _scriptableAnims.Remove(layer);
                }
            }

            return anim;
        }

        internal AnimationState CrossFadeQueuedInternal(string clipId, float fadeLength, QueueMode queueMode, PlayMode playMode, int layer)
        {
            var anim = _animation.CrossFadeQueued(clipId, fadeLength, queueMode, playMode);

            if (_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (playMode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear();
                }
                else
                {
                    _scriptableAnims.Remove(layer);
                }
            }

            return anim;
        }


        private void StartScriptableAnim(ScriptableAnimationStateWrapper state, PlayMode mode)
        {
            if (_scriptableAnims == null)
                _scriptableAnims = new ScriptableAnimCollection(this);

            if (mode == PlayMode.StopAll)
                _scriptableAnims.Clear();
            else
                _scriptableAnims.Remove(state.Layer);

            _scriptableAnims.Add(state);
        }


        #endregion



        #region Internal Event Registering

        internal void CreateAnimationEvent(AnimationClip clip, float time, AnimationEventCallback callback, object token)
        {
            //TODO - just realized, this might actually create multiple events on the same clip if 2 different gameobjects call to add it... not sure if 'AnimationClip' is a shared asset or not. Need to test!

            var ev = new AnimationEvent();
            ev.time = time;
            ev.functionName = "SPAnimationEventHook33417";
            ev.stringParameter = ShortGuid.NewGuid().Value;

            if (_animEventTable == null) _animEventTable = new Dictionary<string, AnimationCallbackData>();
            _animEventTable.Add(ev.stringParameter, new AnimationCallbackData(callback, token));

            clip.AddEvent(ev);
        }

        private void SPAnimationEventHook33417(string id)
        {
            if (_animEventTable == null) return;

            AnimationCallbackData data;
            if(_animEventTable.TryGetValue(id, out data))
            {
                _animEventTable.Remove(id);
                data.Callback(data.Token);
            }
        }

        #endregion

        #region ISPAnimationSource Interface

        public virtual ISPAnim GetAnim(string name)
        {
            var state = _states[name];
            if (state != null) return state.CreateAnimatableState();
            else return null;
        }

        #endregion

        #region Special Types

        private class AnimationCallbackData
        {
            public AnimationEventCallback Callback;
            public object Token;

            public AnimationCallbackData(AnimationEventCallback callback, object token)
            {
                this.Callback = callback;
                this.Token = token;
            }
        }

        private struct InUpdateInfo
        {
            public AnimEventScheduler Scheduler;
            public bool Add;

            public InUpdateInfo(AnimEventScheduler s, bool add)
            {
                this.Scheduler = s;
                this.Add = add;
            }
        }



        private class ScriptableAnimCollection : IUpdateable, IEnumerable<ScriptableAnimationStateWrapper>
        {
            private SPAnimationController _controller;
            private HashSet<ScriptableAnimationStateWrapper> _set = new HashSet<ScriptableAnimationStateWrapper>();
            private HashSet<ScriptableAnimationStateWrapper> _toAdd = new HashSet<ScriptableAnimationStateWrapper>();
            private HashSet<ScriptableAnimationStateWrapper> _toRemove = new HashSet<ScriptableAnimationStateWrapper>();
            private bool _inUpdate;

            public ScriptableAnimCollection(SPAnimationController controller)
            {
                _controller = controller;
            }

            public int Count
            {
                get { return _set.Count; }
            }

            public void Add(ScriptableAnimationStateWrapper wrapper)
            {
                if(_inUpdate)
                {
                    _toAdd.Add(wrapper);
                }
                else
                {
                    _set.Add(wrapper);
                }
                if(!GameLoopEntry.LateUpdatePump.Contains(this))
                    GameLoopEntry.LateUpdatePump.Add(this);
            }
            
            public void Remove(ScriptableAnimationStateWrapper wrapper)
            {
                if (!_set.Contains(wrapper)) return;

                if(_inUpdate)
                {
                    _toRemove.Add(wrapper);
                }
                else
                {
                    if(_set.Remove(wrapper))
                    {
                        wrapper.Dispose();
                    }
                }
            }

            public void Remove(int layer)
            {
                var e = _set.GetEnumerator();
                while(e.MoveNext())
                {
                    if(e.Current.Layer == layer)
                    {
                        _toRemove.Add(e.Current);
                    }
                }

                if(!_inUpdate)
                {
                    e = _toRemove.GetEnumerator();
                    while(e.MoveNext())
                    {
                        e.Current.Dispose();
                        _set.Remove(e.Current);
                    }
                    _toRemove.Clear();
                }
            }

            public void Clear()
            {
                if(_inUpdate)
                {
                    var e = _set.GetEnumerator();
                    while(e.MoveNext())
                    {
                        _toRemove.Add(e.Current);
                    }
                }
                else
                {
                    var e = _set.GetEnumerator();
                    while(e.MoveNext())
                    {
                        e.Current.Dispose();
                    }
                    _set.Clear();
                }
            }

            public void Update()
            {
                _inUpdate = true;
                var e = _set.GetEnumerator();
                while(e.MoveNext())
                {
                    if (!e.Current.Tick(false))
                        _toRemove.Add(e.Current);
                }
                _inUpdate = false;

                if(_toAdd.Count > 0)
                {
                    e = _toAdd.GetEnumerator();
                    while(e.MoveNext())
                    {
                        _set.Add(e.Current);
                    }
                    _toAdd.Clear();
                }

                if (_toRemove.Count > 0)
                {
                    e = _toRemove.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (_set.Remove(e.Current)) e.Current.Dispose();
                    }
                    _toRemove.Clear();
                }

                if(_set.Count == 0)
                {
                    GameLoopEntry.LateUpdatePump.Remove(this);
                }
            }


            public HashSet<ScriptableAnimationStateWrapper>.Enumerator GetEnumerator()
            {
                return _set.GetEnumerator();
            }

            IEnumerator<ScriptableAnimationStateWrapper> IEnumerable<ScriptableAnimationStateWrapper>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class ScriptableAnimationStateWrapper : ISPAnim, System.IDisposable
        {

            private SPAnimationController _controller;
            private IScriptableAnimationState _state;

            private float _startTime;
            private bool _complete;
            private AnimEventScheduler _scheduler;

            public ScriptableAnimationStateWrapper()
            {

            }

            #region Methods

            public void Init(SPAnimationController controller, IScriptableAnimationState state)
            {
                _controller = controller;
                _state = state;
                _startTime = UnityEngine.Time.time;
            }

            public bool Tick(bool layerIsObscured)
            {
                if (_complete || _state == null) return false;
                _complete = !_state.Tick(layerIsObscured);
                return !_complete;
            }
            
            #endregion

            #region ISPAnim Interface

            public SPAnimationController Controller
            {
                get
                {
                    return _controller;
                }
            }

            public float Duration
            {
                get
                {
                    if (_state == null) throw new System.ObjectDisposedException("ISPAnim");
                    return _state.Length;
                }
            }

            public bool IsPlaying
            {
                get
                {
                    if(_controller == null) throw new System.ObjectDisposedException("ISPAnim");
                    if (_controller._scriptableAnims == null) return false;
                    return _controller._scriptableAnims.Contains(this);
                }
            }

            public int Layer
            {
                get
                {
                    if (_state == null) throw new System.ObjectDisposedException("ISPAnim");
                    return _state.Layer;
                }

                set
                {
                    if (_state == null) throw new System.ObjectDisposedException("ISPAnim");
                    _state.Layer = value;
                }
            }

            public float Speed
            {
                get
                {
                    return 1f;
                }

                set
                {
                    //do nothing
                }
            }

            public float Time
            {
                get
                {
                    return UnityEngine.Time.time - _startTime;
                }

                set
                {
                    //do nothing
                }
            }

            public ITimeSupplier TimeSupplier
            {
                get
                {
                    return SPTime.Normal;
                }

                set
                {
                    //do nothing
                }
            }

            public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
                if (_state == null) throw new System.ObjectDisposedException("ISPAnim");
                if (this.IsPlaying) return;

                _state.OnStart();
                _controller.StartScriptableAnim(this, playMode);
            }

            public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
                if (_state == null) throw new System.ObjectDisposedException("ISPAnim");
                if (this.IsPlaying) return;

                _state.OnStart();
                _controller.StartScriptableAnim(this, playMode);
            }

            public void Schedule(Action<ISPAnim> callback)
            {
                if (_state == null) throw new System.ObjectDisposedException("ISPAnim");
                if (_scheduler == null)
                    _scheduler = new AnimEventScheduler(this);
                _scheduler.Schedule(callback);
            }

            public void Schedule(Action<ISPAnim> callback, float timeout, ITimeSupplier time)
            {
                if (_state == null) throw new System.ObjectDisposedException("ISPAnim");
                if (_scheduler == null)
                    _scheduler = new AnimEventScheduler(this);
                _scheduler.Schedule(callback, timeout, time);
            }

            public void Stop()
            {
                _complete = true;
            }

            #endregion

            #region IRadicalWaitHandle Interface

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
                    return _complete;
                }
            }

            public void OnComplete(Action<IRadicalWaitHandle> callback)
            {
                this.Schedule((a) => callback(a));
            }


            public bool Tick(out object yieldObject)
            {
                yieldObject = null;
                return !_complete;
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
                if(_state != null)
                {
                    _state.OnStop();
                }
                _controller = null;
                _state = null;
            }

            #endregion

        }



        private class CoroutineWrapper : ISPAnim, System.IDisposable
        {

            private SPAnimationController _controller;
            private RadicalCoroutine _routine;
            private float _startTime;

            public CoroutineWrapper()
            {

            }

            public void Init(SPAnimationController controller, RadicalCoroutine routine)
            {
                _controller = controller;
                _routine = routine;
                _startTime = UnityEngine.Time.time;
            }

            #region ISPAnim Interface

            public SPAnimationController Controller
            {
                get
                {
                    return _controller; 
                }
            }

            public float Duration
            {
                get
                {
                    return float.PositiveInfinity;
                }
            }

            public bool IsPlaying
            {
                get
                {
                    return !this.IsComplete;
                }
            }

            public int Layer
            {
                get
                {
                    return 0;
                }

                set
                {
                    //do nothing
                }
            }

            public float Speed
            {
                get
                {
                    return 1f;
                }

                set
                {
                    //do nothing
                }
            }

            public float Time
            {
                get
                {
                    return UnityEngine.Time.time - _startTime;
                }

                set
                {
                    //do nothing
                }
            }

            public ITimeSupplier TimeSupplier
            {
                get
                {
                    return SPTime.Normal;
                }

                set
                {
                    //do nothing
                }
            }

            public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
                if (_routine == null) throw new System.ObjectDisposedException("ISPAnim");
                if (_routine.OperatingState == RadicalCoroutineOperatingState.Inactive)
                    _routine.Start(_controller, RadicalCoroutineDisableMode.Pauses);
            }

            public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
            {
                if (_routine == null) throw new System.ObjectDisposedException("ISPAnim");
                if (_routine.OperatingState == RadicalCoroutineOperatingState.Inactive)
                    _routine.Start(_controller, RadicalCoroutineDisableMode.Pauses);
            }

            public void Schedule(Action<ISPAnim> callback)
            {
                if (_routine == null) throw new System.ObjectDisposedException("ISPAnim");
                _routine.OnComplete += (s, e) => { callback(this); };
            }

            private AnimEventScheduler _scheduler;
            public void Schedule(Action<ISPAnim> callback, float timeout, ITimeSupplier time)
            {
                if (_routine == null) throw new System.ObjectDisposedException("ISPAnim");
                if (_scheduler == null)
                    _scheduler = new AnimEventScheduler(this);
                _scheduler.Schedule(callback, timeout, time);
            }

            public void Stop()
            {
                _routine.Cancel();
            }

            #endregion

            #region IRadicalWaitHandle Interface

            public bool Cancelled
            {
                get
                {
                    return _routine.Cancelled;
                }
            }

            public bool IsComplete
            {
                get
                {
                    if (_routine == null) return true;
                    return _routine.Finished;
                }
            }

            public void OnComplete(Action<IRadicalWaitHandle> callback)
            {
                if (_routine == null) throw new System.ObjectDisposedException("ISPAnim");
                (_routine as IRadicalWaitHandle).OnComplete(callback);
            }


            public bool Tick(out object yieldObject)
            {
                yieldObject = null;
                return !this.IsComplete;
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
                _controller = null;
                _routine = null;
            }

            #endregion

        }

        #endregion


        #region Static Factory

        internal static ISPAnim CreateScriptableAnimState(SPAnimationController controller, IScriptableAnimationClip clip)
        {
            if(controller == null) throw new System.ArgumentNullException("controller");
            if (clip == null) throw new System.ArgumentNullException("clip");

            var st = clip.GetState(controller);
            if (st == null) return SPAnim.Null;

            var state = new ScriptableAnimationStateWrapper();
            state.Init(controller, st);
            return state;
        }

        #endregion

    }
}
