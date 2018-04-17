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

    }


    /// <summary>
    /// A specialized animation controller that plays specific animation sequences.
    /// 
    /// Example, you may have a PlayWalkAnimator, which calls Idle/Walk/Run in the correct order depending on the state of the player.
    /// </summary>
    public abstract class SPAnimatorComponent : SPComponent, ISPAnimator //, IEntityAwakeHandler
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf(UseEntity = true)]
        private SPAnimationController _controller;

        [System.NonSerialized]
        private bool _initialized;

        #endregion

        #region CONSTRUCTOR
        
        protected override void Start()
        {
            if (!_initialized)
            {
                _initialized = true;
                this.Init(_controller);
            }

            base.Start();
        }

        protected abstract void Init(SPAnimationController controller);

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
        
        #endregion

    }

    public class SPAnimatorMethodAttribute : System.Attribute
    {

    }

}
