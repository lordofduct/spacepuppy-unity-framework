using System;
using UnityEngine;

namespace com.spacepuppy.Anim
{

    /// <summary>
    /// A specialized animation controller that plays specific animation sequences.
    /// 
    /// Example, you may have a PlayWalkAnimator, which calls Idle/Walk/Run in the correct order depending on the state of the player.
    /// </summary>
    public interface ISPAnimator
    {

        SPAnimationController Controller { get; }
        bool IsInitialized { get; }

        void Configure(SPAnimationController controller);
        void Play(string id, QueueMode queuMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);

    }


    /// <summary>
    /// A specialized animation controller that plays specific animation sequences.
    /// 
    /// Example, you may have a PlayWalkAnimator, which calls Idle/Walk/Run in the correct order depending on the state of the player.
    /// </summary>
    public abstract class SPAnimator : SPComponent, ISPAnimator //, IEntityAwakeHandler
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf(UseEntity = true)]
        private SPAnimationController _controller;

        [System.NonSerialized]
        private bool _initialized;

        #endregion

        #region CONSTRUCTOR

        //void IEntityAwakeHandler.OnEntityAwake(SPEntity entity)
        //{
        //    if (_initialized) return;

        //    _initialized = true;
        //    this.Init(entity, _controller);
        //}

        protected override void Start()
        {
            base.Start();

            var entity = SPEntity.Pool.GetFromSource<SPEntity>(this);
            if (!_initialized && entity != null && entity.IsAwake)
            {
                _initialized = true;
                this.Init(entity, _controller);
            }
        }

        protected abstract void Init(SPEntity entity, SPAnimationController controller);

        #endregion
        
        #region ISPAnimator Interface

        public SPAnimationController Controller
        {
            get
            {
                return _controller;
            }
        }

        public bool IsInitialized
        {
            get { return _initialized; }
        }

        public void Configure(SPAnimationController controller)
        {
            if (_initialized) throw new System.InvalidOperationException("Can not change the Controller of an SPAnimator once it's been initialized.");
            _controller = controller;
        }

        public abstract void Play(string id, QueueMode queuMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);
        
        #endregion

    }

}
