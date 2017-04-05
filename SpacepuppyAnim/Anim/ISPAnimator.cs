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

        void Play(string id, QueueMode queuMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);

    }


    /// <summary>
    /// A specialized animation controller that plays specific animation sequences.
    /// 
    /// Example, you may have a PlayWalkAnimator, which calls Idle/Walk/Run in the correct order depending on the state of the player.
    /// </summary>
    public abstract class SPAnimator : SPComponent, ISPAnimator, IEntityAwakeHandler
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf(UseEntity = true)]
        private SPAnimationController _controller;

        [System.NonSerialized]
        private bool _initialized;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            var entity = SPEntity.Pool.GetFromSource<SPEntity>(this);
            if (!_initialized && entity != null && entity.IsAwake)
            {
                _initialized = true;
                this.Init(entity, _controller);
            }
        }

        void IEntityAwakeHandler.OnEntityAwake(SPEntity entity)
        {
            if (_initialized) return;

            _initialized = true;
            this.Init(entity, _controller);
        }

        protected abstract void Init(SPEntity entity, SPAnimationController controller);

        #endregion

        #region Properties

        public bool IsInitialized
        {
            get { return _initialized; }
        }

        #endregion

        #region ISPAnimator Interface

        public SPAnimationController Controller
        {
            get
            {
                return _controller;
            }
        }

        public abstract void Play(string id, QueueMode queuMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);

        #endregion

    }

}
