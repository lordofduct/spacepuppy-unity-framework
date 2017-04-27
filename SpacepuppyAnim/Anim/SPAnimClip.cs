using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{

    /// <summary>
    /// Represents an AnimationClip reference that can be used by Spacepuppy Animation.
    /// </summary>
    /// <remarks>
    /// We attempted to implement the ability to define which frames of the AnimationClip to use so that you could use code to break apart a large animation into a segmented animation. 
    /// Unfortunately in Unity 4, about the time Mecanim was released, the method Animation.AddClip(name,clip,firstFrame,lastFrame) was broken. It adds an animation with the correct frames, 
    /// but the length of the animation is made to be 1 second, even though it's not. Unity does not appear to have any plans to fix this... since Mecanim is available. So I removed the support 
    /// but left it in comments just in case unity decides to get off their ass and fix it.
    /// 
    /// 
    /// Furthermore, in regards of scaled time layers. When calling to play an animation 'directly' on this clip, it will only respect the timescale of the 'normal' time. The value exists solely 
    /// for when calling 'PlayQueued', as this is the preferred way of playing animations.
    /// </remarks>
    [System.Serializable()]
    public class SPAnimClip : ISPDisposable
    {

        public const string PROP_WEIGHT = "_weight";
        public const string PROP_SPEED = "_speed";
        public const string PROP_LAYER = "_layer";
        public const string PROP_WRAPMODE = "_wrapMode";
        public const string PROP_BLENDMODE = "_blendMode";
        public const string PROP_TIMESUPPLIER = "_timeSupplier";
        public const string PROP_SCALEDDURATION = "ScaledDuration";
        public const string PROP_MASKS = "_masks";

        #region Fields
        
        [SerializeField()]
        private string _name;
        [SerializeField()]
        private UnityEngine.Object _clip;
        [SerializeField()]
        private float _weight = 1.0f;
        [SerializeField()]
        private float _speed = 1.0f;
        [SerializeField()]
        [AnimLayer()]
        private int _layer;
        [SerializeField()]
        private WrapMode _wrapMode;
        [SerializeField()]
        private AnimationBlendMode _blendMode = AnimationBlendMode.Blend;
        [SerializeField()]
        private MaskCollection _masks;
        [SerializeField()]
        private SPTime _timeSupplier;

        [System.NonSerialized()]
        private string _id;

        //***SEE NOTES IN CLASS DESCRIPTION

        //[System.NonSerialized()]
        //private int _firstFrame = 0;
        //[System.NonSerialized()]
        //private int _lastFrame = -1;



        [System.NonSerialized()]
        private SPAnimationController _controller;
        [System.NonSerialized()]
        private AnimationState _state;

        #endregion

        #region CONSTRUCTOR


        /// <summary>
        /// For deserializer ONLY
        /// </summary>
        private SPAnimClip()
        {
            //_name = null;
            //_clip = null;
            //_masks = new MaskCollection();
            //_firstFrame = 0;
            //_lastFrame = -1;
        }

        public SPAnimClip(string name)
        {
            _name = name;
            _clip = null;
            _masks = new MaskCollection();
            _timeSupplier = new SPTime();
            //_firstFrame = 0;
            //_lastFrame = -1;
        }

        public SPAnimClip(string name, AnimationClip clip)
        {
            _name = name;
            _clip = clip;
            _masks = new MaskCollection();
            _timeSupplier = new SPTime();
            //_firstFrame = 0;
            //_lastFrame = -1;
        }

        public SPAnimClip(string name, AnimationClip clip, ITimeSupplier timeSupplier)
        {
            _name = name;
            _clip = clip;
            _masks = new MaskCollection();
            _timeSupplier = new SPTime(timeSupplier);
            //_firstFrame = 0;
            //_lastFrame = -1;
        }
        
        public void Init(SPAnimationController controller, string uniqueHash = null)
        {
            if (_controller != null) throw new System.InvalidOperationException("Cannot initialize a clip that has already been initialized.");
            if (controller == null) throw new System.ArgumentNullException("controller");
            //if (_clip == null) return;
            //if (_clip == null) _clip = SPAnimClip.EmptyClip;

            _controller = controller;
            _id = (string.IsNullOrEmpty(uniqueHash)) ? _name : _name + uniqueHash;
            _controller.States.AddToMasterList(_id, this);
        }

        /// <summary>
        /// This is only called from the Master List after the AnimationClip has been added and initialized.
        /// Do not call for any other reason.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        internal void SetAnimState(SPAnimationController controller, string id, AnimationState state)
        {
            //we reset the controller and id to what the SPAnimationController decided it should be
            _controller = controller;
            _id = id;
            _state = state;
            if (_state != null)
            {
                _state.weight = _weight;
                _state.speed = _speed;
                _state.layer = _layer;
                _state.wrapMode = _wrapMode;
                _state.blendMode = _blendMode;
                _masks.SetState(_state);
            }
        }

        #endregion

        #region Properties

        #region Serialized Properties

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string ClipID
        {
            get
            {
                return _id;
            }
        }

        public UnityEngine.Object Clip { get { return _clip; } }

        public AnimationClip AnimationClip { get { return _clip as AnimationClip; } }

        public AnimationState State { get { return _state; } }

        public float Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                if (_state != null) _state.weight = value;
            }
        }

        public float Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                if (_state != null) _state.speed = value;
            }
        }

        public int Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;
                if (_state != null) _state.layer = value;
            }
        }

        public WrapMode WrapMode
        {
            get { return _wrapMode; }
            set
            {
                _wrapMode = value;
                if (_state != null) _state.wrapMode = value;
            }
        }

        public AnimationBlendMode BlendMode
        {
            get { return _blendMode; }
            set
            {
                _blendMode = value;
                if (_state != null) _state.blendMode = value;
            }
        }

        public ITimeSupplier TimeSupplier
        {
            get { return _timeSupplier.TimeSupplier; }
        }

        /// <summary>
        /// The duration of the clip at the current speed. When set modifies the speed property.
        /// </summary>
        public float ScaledDuration
        {
            get
            {
                if (this.Speed == 0f)
                    return float.PositiveInfinity;
                else
                    return Mathf.Abs(this.Duration / this.Speed);
            }
            set
            {
                if (value <= 0f)
                    this.Speed = 0f;
                else
                    this.Speed = this.Duration / value;
            }
        }

        public MaskCollection Masks
        {
            get { return _masks; }
        }

        //***SEE NOTES IN CLASS DESCRIPTION

        //public int FirstFrame
        //{
        //    get { return _firstFrame; }
        //}

        //public int LastFrame
        //{
        //    get { return _lastFrame; }
        //}

        #endregion

        public bool IsPlaying { get { return (_controller != null) ? _controller.animation.IsPlaying(_id) : false; } }

        public float Time
        {
            get { return (_state != null) ? _state.time : 0f; }
            set
            {
                if (_state != null) _state.time = value;
            }
        }

        public float Duration
        {
            get
            {
                if (_state != null)
                    return _state.length;
                if(_clip != null)
                {
                    if (_clip is AnimationClip)
                        return (_clip as AnimationClip).length;
                    else if (_clip is IScriptableAnimationClip)
                        return (_clip as IScriptableAnimationClip).Length;
                }

                return 0f;
                //return (_state != null) ? _state.length : ((_clip != null) ? _clip.length : 0f);
            }
        }

        public bool Enabled
        {
            get { return (_state != null) ? _state.enabled : false; }
            set
            {
                if (_state != null) _state.enabled = value;
            }
        }

        public SPAnimationController Controller { get { return _controller; } }

        //public Animation Container { get { return _container; } }

        public bool Initialized { get { return !object.ReferenceEquals(_controller, null); } }

        #endregion

        #region Methods

        //***SEE NOTES IN CLASS DESCRIPTION

        //public void SetFrameClamp(int firstFrame, int lastFrame)
        //{
        //    if (_container != null) throw new System.InvalidOperationException("The frame clamp can only be set on a clip that has not been initialized.");

        //    _firstFrame = firstFrame;
        //    _lastFrame = lastFrame;
        //    if (_firstFrame < 0) _firstFrame = 0;
        //    if (_lastFrame < 0) _lastFrame = -1;
        //    else if (_lastFrame < _firstFrame) _lastFrame = _firstFrame + 1;
        //}

        //public void ResetFrameClamp()
        //{
        //    _firstFrame = 0;
        //    _lastFrame = -1;
        //}

        /// <summary>
        /// Creates a state for use in animating.
        /// </summary>
        /// <returns></returns>
        public ISPAnim CreateAnimatableState()
        {
            if (_controller == null) throw new System.InvalidOperationException("This clip has not been initialized.");
            
            if(_clip is AnimationClip)
            {
                if (_state == null)
                {
                    //this.Dispose();
                    //throw new System.InvalidOperationException("This clip was unexpectedly destroyed, make sure the animation hasn't been destroyed, or another clip was added with the same name.");
                    return null;
                }

                var a = SPAnim.Create(_controller, _id);
                a.Weight = _weight;
                a.Speed = _speed;
                a.Layer = _layer;
                a.WrapMode = _wrapMode;
                a.BlendMode = _blendMode;
                if (_masks.Count > 0) a.Masks.Copy(_masks);
                if (_timeSupplier.IsCustom) a.TimeSupplier = _timeSupplier.TimeSupplier as ITimeSupplier;
                return a;
            }
            else if(_clip is IScriptableAnimationClip)
            {
                var a = (_clip as IScriptableAnimationClip).CreateState(_controller) ?? SPAnim.Null;
                a.Speed = _speed;
                a.Layer = _layer;
                if (_timeSupplier.IsCustom) a.TimeSupplier = _timeSupplier.TimeSupplier as ITimeSupplier;
                return a;
            }

            return null;
        }

        #region Playback

        public void PlayDirectly(PlayMode mode = PlayMode.StopSameLayer)
        {
            if (_controller == null) return;

            if(_clip is AnimationClip)
            {
                if (_state == null)
                {
                    //this.Dispose();
                    //throw new System.InvalidOperationException("This clip was unexpectedly destroyed, make sure the animation hasn't been destroyed, or another clip was added with the same name.");
                    return;
                }

                _state.weight = _weight;
                _state.speed = _speed * _controller.Speed;
                _state.layer = _layer;
                _state.wrapMode = _wrapMode;
                _state.blendMode = _blendMode;
                _controller.PlayInternal(_state.name, mode, _layer);
            }
            else if(_clip is IScriptableAnimationClip)
            {
                var state = (_clip as IScriptableAnimationClip).CreateState(_controller) ?? SPAnim.Null;
                state.Speed = _speed;
                state.Layer = _layer;
                if (_timeSupplier.IsCustom) state.TimeSupplier = _timeSupplier.TimeSupplier as ITimeSupplier;
                state.Play(QueueMode.PlayNow, mode);
            }
        }

        public ISPAnim Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            var a = this.CreateAnimatableState();
            if(a != null) a.Play(queueMode, playMode);
            return a;
        }

        public ISPAnim Play(float speed, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            var a = this.CreateAnimatableState();
            if (a != null) a.Play(speed, queueMode, playMode);
            return a;
        }

        public void CrossFadeDirectly(float fadeLength, PlayMode mode = PlayMode.StopSameLayer)
        {
            if (_controller == null) return;

            if (_clip is AnimationClip)
            {
                if (_state == null)
                {
                    //this.Dispose();
                    //throw new System.InvalidOperationException("This clip was unexpectedly destroyed, make sure the animation hasn't been destroyed, or another clip was added with the same name.");
                    return;
                }

                _state.weight = _weight;
                _state.speed = _speed * _controller.Speed;
                _state.layer = _layer;
                _state.wrapMode = _wrapMode;
                _state.blendMode = _blendMode;
                _controller.CrossFadeInternal(_state.name, fadeLength, mode, _layer);
            }
            else
            {
                var state = (_clip as IScriptableAnimationClip).CreateState(_controller) ?? SPAnim.Null;
                state.Speed = _speed;
                state.Layer = _layer;
                if (_timeSupplier.IsCustom) state.TimeSupplier = _timeSupplier.TimeSupplier as ITimeSupplier;
                state.CrossFade(fadeLength, QueueMode.PlayNow, mode);
            }
        }

        public ISPAnim CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            var a = this.CreateAnimatableState();
            if (a != null) a.CrossFade(fadeLength, queueMode, playMode);
            return a;
        }

        public ISPAnim CrossFade(float speed, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            var a = this.CreateAnimatableState();
            if (a != null) a.CrossFade(speed, fadeLength, queueMode, playMode);
            return a;
        }

        public void Rewind()
        {
            if (_controller == null) throw new System.InvalidOperationException("This clip has not been initialized.");
            if (_state == null)
            {
                //this.Dispose();
                //throw new System.InvalidOperationException("This clip was unexpectedly destroyed, make sure the animation hasn't been destroyed, or another clip was added with the same name.");
                return;
            }
            _controller.animation.Rewind(_id);
        }

        public void Stop()
        {
            if (_controller == null) throw new System.InvalidOperationException("This clip has not been initialized.");
            if (_state == null)
            {
                //this.Dispose();
                //throw new System.InvalidOperationException("This clip was unexpectedly destroyed, make sure the animation hasn't been destroyed, or another clip was added with the same name.");
                return;
            }
            _controller.animation.Stop(_id);
        }

        #endregion

        #region Events

        public void AddEvent(float time, AnimationEventCallback callback, object token)
        {
            if (_controller == null) throw new System.InvalidOperationException("This clip has not been initialized.");
            if (callback == null) throw new System.ArgumentNullException("callback");

            if (_clip is AnimationClip)
                _controller.CreateAnimationEvent(_clip as AnimationClip, time, callback, token);
            else
                throw new System.InvalidOperationException("ISPAnimationClip does not support AddEvent.");
        }

        #endregion

        #endregion

        #region IDisposable Interface

        public bool IsDisposed
        {
            get
            {
                return _clip == null;
            }
        }

        public void Dispose()
        {
            if (_controller != null && _state != null && _controller.animation[_state.name] == _state)
            {
                _controller.animation.RemoveClip(_state.name);
            }
            _controller = null;
            _state = null;
            _masks.SetState(null);
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {
            public bool HideDetailRegion;
            public string[] VisibleProps;

            public ConfigAttribute(bool hideDetailRegion)
            {
                this.HideDetailRegion = hideDetailRegion;
                this.VisibleProps = null;
            }

            public ConfigAttribute(params string[] visibleProps)
            {
                this.HideDetailRegion = false;
                this.VisibleProps = visibleProps;
            }
        }

        public class ReadOnlyNameAttribute : System.Attribute
        {
            private string _name;

            public ReadOnlyNameAttribute(string name)
            {
                _name = name;
            }

            public string Name { get { return _name; } }
        }

        #endregion

    }

}
