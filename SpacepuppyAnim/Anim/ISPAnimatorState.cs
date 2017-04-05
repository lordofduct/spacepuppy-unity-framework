using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Anim
{
    public interface ISPAnimatorState : IComponent
    {

        bool IsActiveState { get; }

        void Activate();
        void Deactivate();
        void UpdateState();

    }


    public abstract class SPAnimatorState : SPNotifyingComponent, ISPAnimatorState
    {

        #region Fields

        [System.NonSerialized()]
        private SPAnimatorStateMachine _machine;
        [System.NonSerialized()]
        private bool _isActive;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _machine = this.GetComponent<SPAnimatorStateMachine>();
        }

        #endregion

        #region Fields

        public SPAnimatorStateMachine StateMachine { get { return _machine; } }

        #endregion

        #region ISPAnimatorState Interface

        public bool IsActiveState
        {
            get { return _isActive; }
        }

        protected virtual void Activate()
        {

        }
        void ISPAnimatorState.Activate()
        {
            _isActive = true;
            this.Activate();
        }

        protected virtual void Deactivate()
        {
        }
        void ISPAnimatorState.Deactivate()
        {
            this.Deactivate();
            _isActive = false;
        }

        protected virtual void UpdateState()
        {

        }
        void ISPAnimatorState.UpdateState()
        {
            if (_isActive) this.UpdateState();
        }

        #endregion
        
    }

}
