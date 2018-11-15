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
        [SerializeField]
        private AnimControllerMaskSerializedRef _mask;

        [SerializeField()]
        private SPAnimClipCollection _states;

        [SerializeField()]
        private string _animToPlayOnStart = null;




        [System.NonSerialized()]
        private Animation _animation;

        [System.NonSerialized]
        private bool _initialized;

        [System.NonSerialized()]
        private Dictionary<string, AnimationCallbackData> _animEventTable;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if (_mask == null) _mask = new AnimControllerMaskSerializedRef();
            if (_states == null) _states = new SPAnimClipCollection();

            _animation = this.AddOrGetComponent<Animation>();
            _animation.playAutomatically = false;

            _states.InitMasterCollection(this);
            //_states.Init(this, null);
        }

        protected override void Start()
        {
            if (!_initialized) this.Init();
            
            base.Start();

            if (this.States.ContainsKey(_animToPlayOnStart))
            {
                var state = this.States[_animToPlayOnStart];
                if (state != null) state.PlayDirectly(PlayMode.StopAll);
            }
        }

        private void Init()
        {
            _states.SyncMasterAnims();
            _initialized = true;

            //_animation.playAutomatically = false;
            _animation.animatePhysics = _animatePhysics;
            _animation.cullingType = _animCullingType;
            //this.enabled = false;
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

        public IAnimControllerMask ControllerMask
        {
            get { return _mask.Value; }
            set { _mask.Value = value; }
        }

        #endregion

        #region Methods

        public ISPAnim Play(string clipId, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

            var state = _states[clipId];
            //if (state == null) throw new UnknownStateException(clipId);
            if (state == null) return null;
            if (this.ControllerMask != null && !this.ControllerMask.CanPlay(state)) return null;

            return state.Play(queueMode, playMode);
        }

        public ISPAnim CrossFade(string clipId, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

            var state = _states[clipId];
            //if (state == null) throw new UnknownStateException(clipId);
            if (state == null) return null;
            if (this.ControllerMask != null && !this.ControllerMask.CanPlay(state)) return null;

            return state.CrossFade(fadeLength, queueMode, playMode);
        }

        public ISPAnim Play(IScriptableAnimationClip clip, PlayMode mode = PlayMode.StopSameLayer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();
            if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip)) return SPAnim.Null;

            var state = clip.CreateState(this) ?? SPAnim.Null;
            state.Play(QueueMode.PlayNow, mode);
            return state;
        }

        public void Stop(string id)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();
            
            _animation.Stop(id);
        }

        public void Stop(int layer)
        {
            this.StopInternal(layer, PlayMode.StopSameLayer);
        }

        public void StopAll()
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

            _animation.Stop();
        }


        /// <summary>
        /// Play an AnimationClip on the AnimationController that doesn't already exist on it. If the same clip is called multiple times, it will not be readded.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="queueMode"></param>
        /// <param name="playMode"></param>
        /// <returns></returns>
        public ISPAnim PlayAuxiliary(SPAnimClip clip, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();

            if (clip.Clip is AnimationClip)
            {
                if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip)) return null;

                var id = this.AddAuxiliaryClip(clip.Clip as AnimationClip, auxId);
                var anim = SPAnim.Create(_animation, id);
                anim.Weight = clip.Weight;
                anim.Speed = clip.Speed;
                anim.Layer = clip.Layer;
                anim.WrapMode = clip.WrapMode;
                anim.BlendMode = clip.BlendMode;
                anim.TimeSupplier = (clip.TimeSupplier != SPTime.Normal) ? anim.TimeSupplier : null;
                anim.Play(queueMode, playMode);
                return anim;
            }
            else if(clip.Clip is IScriptableAnimationClip)
            {
                return this.Play(clip.Clip as IScriptableAnimationClip, playMode);
            }
            else
            {
                return null;
            }
        }

        public ISPAnim CrossFadeAuxiliary(SPAnimClip clip, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();

            if (clip.Clip is AnimationClip)
            {
                if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip)) return null;

                var id = this.AddAuxiliaryClip(clip.Clip as AnimationClip, auxId);
                var anim = SPAnim.Create(_animation, id);
                anim.Weight = clip.Weight;
                anim.Speed = clip.Speed;
                anim.Layer = clip.Layer;
                anim.WrapMode = clip.WrapMode;
                anim.BlendMode = clip.BlendMode;
                anim.TimeSupplier = (clip.TimeSupplier != SPTime.Normal) ? anim.TimeSupplier : null;
                anim.CrossFade(fadeLength, queueMode, playMode);
                return anim;
            }
            else if (clip.Clip is IScriptableAnimationClip)
            {
                return this.Play(clip.Clip as IScriptableAnimationClip, playMode);
            }
            else
            {
                return null;
            }
        }

        public SPAnim PlayAuxiliary(AnimationClip clip, AnimSettings settings, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();
            if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip, settings)) return null;

            var id = this.AddAuxiliaryClip(clip, auxId);
            var anim = SPAnim.Create(_animation, id);
            settings.Apply(anim);
            anim.Play(queueMode, playMode);
            return anim;
        }

        public SPAnim CrossFadeAuxiliary(AnimationClip clip, AnimSettings settings, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();
            if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip, settings)) return null;

            var id = this.AddAuxiliaryClip(clip, auxId);
            var anim = SPAnim.Create(_animation, id);
            settings.Apply(anim);
            anim.CrossFade(fadeLength, queueMode, playMode);
            return anim;
        }



        public string PlayAuxiliaryDirectly(SPAnimClip clip, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();

            if (clip.Clip is AnimationClip)
            {
                if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip)) return null;

                var id = this.AddAuxiliaryClip(clip.Clip as AnimationClip, auxId);
                var anim = _animation[id];
                anim.weight = clip.Weight;
                anim.speed = clip.Speed * this.Speed;
                anim.layer = clip.Layer;
                anim.wrapMode = clip.WrapMode;
                anim.blendMode = clip.BlendMode;
                this.PlayInternal(anim.name, playMode, clip.Layer);
                return id;
            }
            else if (clip.Clip is IScriptableAnimationClip)
            {
                this.Play(clip.Clip as IScriptableAnimationClip, playMode);
            }

            return null;
        }

        public string CrossFadeAuxiliaryDirectly(SPAnimClip clip, float fadeLength, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();

            if (clip.Clip is AnimationClip)
            {
                if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip)) return null;

                var id = this.AddAuxiliaryClip(clip.Clip as AnimationClip, auxId);
                var anim = _animation[id];
                anim.weight = clip.Weight;
                anim.speed = clip.Speed * this.Speed;
                anim.layer = clip.Layer;
                anim.wrapMode = clip.WrapMode;
                anim.blendMode = clip.BlendMode;
                this.CrossFadeInternal(id, fadeLength, playMode, clip.Layer);
                return id;
            }
            else if (clip.Clip is IScriptableAnimationClip)
            {
                this.Play(clip.Clip as IScriptableAnimationClip, playMode);
            }

            return null;
        }

        public string PlayAuxiliaryDirectly(AnimationClip clip, AnimSettings settings, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();
            if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip, settings)) return null;

            var id = this.AddAuxiliaryClip(clip, auxId);
            var anim = _animation[id];
            settings.Apply(anim);
            this.PlayInternal(id, playMode, anim.layer);
            return id;
        }

        public string CrossFadeAuxiliaryDirectly(AnimationClip clip, AnimSettings settings, float fadeLength, PlayMode playMode = PlayMode.StopSameLayer, string auxId = null)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (clip == null) throw new System.ArgumentNullException("clip");
            if (!_initialized) this.Init();
            if (this.ControllerMask != null && !this.ControllerMask.CanPlay(clip, settings)) return null;

            var id = this.AddAuxiliaryClip(clip, auxId);
            var anim = _animation[id];
            settings.Apply(anim);
            this.CrossFadeInternal(id, fadeLength, playMode, anim.layer);
            return id;
        }




        internal void PlayInternal(string clipId, PlayMode mode, int layer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();
            
            _animation.Play(clipId, mode);
            if(_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (mode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear(true);
                }
                else
                {
                    _scriptableAnims.Remove(layer, true);
                }
            }
        }

        internal void CrossFadeInternal(string clipId, float fadeLength, PlayMode mode, int layer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

            _animation.CrossFade(clipId, fadeLength, mode);

            if (_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (mode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear(true);
                }
                else
                {
                    _scriptableAnims.Remove(layer, true);
                }
            }
        }

        internal AnimationState PlayQueuedInternal(string clipId, QueueMode queueMode, PlayMode playMode, int layer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

            //HACK - StopSameLayer respects the base state's layer...
            AnimationState result;
            if(playMode == PlayMode.StopSameLayer)
            {
                var st = _animation[clipId];
                if (!object.ReferenceEquals(st, null) && st.layer != layer)
                {
                    int cache = st.layer;
                    st.layer = layer;
                    result = _animation.PlayQueued(clipId, queueMode, playMode);
                    st.layer = cache;
                }
                else
                {
                    result = _animation.PlayQueued(clipId, queueMode, playMode);
                }
            }
            else
            {
                result = _animation.PlayQueued(clipId, queueMode, playMode);
            }


            if (_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (playMode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear(true);
                }
                else
                {
                    _scriptableAnims.Remove(layer, true);
                }
            }

            return result;
        }

        internal AnimationState CrossFadeQueuedInternal(string clipId, float fadeLength, QueueMode queueMode, PlayMode playMode, int layer)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

            var anim = _animation.CrossFadeQueued(clipId, fadeLength, queueMode, playMode);

            if (_scriptableAnims != null && _scriptableAnims.Count > 0)
            {
                if (playMode == PlayMode.StopAll)
                {
                    _scriptableAnims.Clear(true);
                }
                else
                {
                    _scriptableAnims.Remove(layer, true);
                }
            }

            return anim;
        }

        internal void StopInternal(int layer, PlayMode mode)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

            if (mode == PlayMode.StopAll)
            {
                _animation.Stop();
            }
            else
            {
                _animation.Stop(layer);
            }
        }


        private string AddAuxiliaryClip(AnimationClip clip, string auxId)
        {
            string id = string.IsNullOrEmpty(auxId) ? "aux*" + clip.GetInstanceID() : auxId;
            var a = _animation[id];
            if (a == null || a.clip != clip)
            {
                _animation.AddClip(clip, id);
            }

            return id;
        }

        #endregion



        #region Internal Event Registering

        internal void CreateAnimationEvent(AnimationClip clip, float time, AnimationEventCallback callback, object token)
        {
            //TODO - just realized, this might actually create multiple events on the same clip if 2 different gameobjects call to add it... not sure if 'AnimationClip' is a shared asset or not. Need to test!

            var ev = new AnimationEvent();
            ev.time = time;
            ev.functionName = "SPAnimationEventHook33417";
            ev.stringParameter = ShortUid.NewId().ToString();

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

        public bool CanPlayAnim
        {
            get { return this != null && _animation != null && this.isActiveAndEnabled; }
        }

        public virtual ISPAnim GetAnim(string name)
        {
            if (_animation == null) throw new AnimationInvalidAccessException();
            if (!_initialized) this.Init();

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
        
        #endregion



        #region Scriptable Animation Interface

        [System.NonSerialized]
        private ScriptableAnimCollection _scriptableAnims;
        
        /// <summary>
        /// Starts an IScriptableAnimationCallback stopping animations according to PlayMode, this acts like 'Animation.Play'.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="mode"></param>
        public void StartScriptableAnim(IScriptableAnimationCallback state, PlayMode mode)
        {
            if (state == null) throw new System.ArgumentNullException("state");

            if (_scriptableAnims == null)
                _scriptableAnims = new ScriptableAnimCollection(this);

            this.StopInternal(state.Layer, mode);
            if (mode == PlayMode.StopAll)
                _scriptableAnims.Clear(true);
            else
                _scriptableAnims.Remove(state.Layer, true);

            _scriptableAnims.Add(state);
        }

        /// <summary>
        /// Starts an IScriptableAnimationCallback, but doesn't stop any other animations. This is similar to enabling an AnimationState, or calling Animation.Blend.
        /// </summary>
        /// <param name="state"></param>
        public void EnableScriptableAnim(IScriptableAnimationCallback state)
        {
            if (state == null) throw new System.ArgumentNullException("state");

            if (_scriptableAnims == null)
                _scriptableAnims = new ScriptableAnimCollection(this);

            _scriptableAnims.Add(state);
        }

        /// <summary>
        /// Stops the IScriptableAnimationCallback, usually we do not dispose as this should be called from within the IScriptableAnimationCallback when 'Stop' is called. 
        /// But an overload is allowed if needed.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="dispose"></param>
        public void StopScriptableAnim(IScriptableAnimationCallback state, bool dispose = false)
        {
            if (state == null) throw new System.ArgumentNullException("state");

            if(_scriptableAnims != null)
            {
                _scriptableAnims.Remove(state, dispose);
            }
        }

        private class ScriptableAnimCollection : IUpdateable, IEnumerable<IScriptableAnimationCallback>
        {
            private SPAnimationController _controller;
            private HashSet<IScriptableAnimationCallback> _set = new HashSet<IScriptableAnimationCallback>();
            private HashSet<IScriptableAnimationCallback> _toAdd = new HashSet<IScriptableAnimationCallback>();
            private HashSet<IScriptableAnimationCallback> _toRemove = new HashSet<IScriptableAnimationCallback>();
            private HashSet<IScriptableAnimationCallback> _toRemoveAndDispose = new HashSet<IScriptableAnimationCallback>();
            private bool _inUpdate;

            public ScriptableAnimCollection(SPAnimationController controller)
            {
                _controller = controller;
            }

            public int Count
            {
                get { return _set.Count; }
            }

            public void Add(IScriptableAnimationCallback wrapper)
            {
                if (_inUpdate)
                {
                    _toAdd.Add(wrapper);
                }
                else
                {
                    _set.Add(wrapper);
                }
                if (!GameLoopEntry.LateUpdatePump.Contains(this))
                    GameLoopEntry.LateUpdatePump.Add(this);
            }
            
            public void Remove(IScriptableAnimationCallback callback, bool dispose)
            {
                if (!_set.Contains(callback)) return;

                if (_inUpdate)
                {
                    if (dispose)
                        _toRemoveAndDispose.Add(callback);
                    else
                        _toRemove.Add(callback);
                }
                else
                {
                    if (_set.Remove(callback) && dispose)
                    {
                        callback.Dispose();
                    }
                }
            }

            public void Remove(int layer, bool dispose)
            {
                var e = _set.GetEnumerator();
                var rmSet = dispose ? _toRemoveAndDispose : _toRemove;
                while (e.MoveNext())
                {
                    if (e.Current.Layer == layer)
                    {
                        rmSet.Add(e.Current);
                    }
                }

                if (!_inUpdate)
                {
                    e = rmSet.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if(dispose)
                            e.Current.Dispose();
                        _set.Remove(e.Current);
                    }
                    rmSet.Clear();
                }
            }

            public void Clear(bool dispose)
            {
                var rmSet = dispose ? _toRemoveAndDispose : _toRemove;
                if (_inUpdate)
                {
                    var e = _set.GetEnumerator();
                    while (e.MoveNext())
                    {
                        rmSet.Add(e.Current);
                    }
                }
                else
                {
                    if(dispose)
                    {
                        using (var lst = TempCollection.GetList(_set))
                        {
                            _set.Clear();
                            GameLoopEntry.LateUpdatePump.Remove(this);

                            var e = lst.GetEnumerator();
                            while(e.MoveNext())
                            {
                                e.Current.Dispose();
                            }
                        }
                    }
                    else
                    {
                        _set.Clear();
                        GameLoopEntry.LateUpdatePump.Remove(this);
                    }
                }
            }

            public void Update()
            {
                _inUpdate = true;
                var e = _set.GetEnumerator();
                while (e.MoveNext())
                {
                    if (!e.Current.Tick(false))
                        _toRemove.Add(e.Current);
                }
                _inUpdate = false;

                if (_toAdd.Count > 0)
                {
                    e = _toAdd.GetEnumerator();
                    while (e.MoveNext())
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

                if (_set.Count == 0)
                {
                    GameLoopEntry.LateUpdatePump.Remove(this);
                }
            }


            public HashSet<IScriptableAnimationCallback>.Enumerator GetEnumerator()
            {
                return _set.GetEnumerator();
            }

            IEnumerator<IScriptableAnimationCallback> IEnumerable<IScriptableAnimationCallback>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        #endregion

    }
}
