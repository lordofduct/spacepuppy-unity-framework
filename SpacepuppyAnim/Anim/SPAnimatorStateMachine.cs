using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{
    public class SPAnimatorStateMachine : SPAnimationController, ISPAnimationSource
    {

        #region Fields

        [System.NonSerialized()]
        private ITypedStateMachine<ISPAnimatorState> _states;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.animation.playAutomatically = false;

            _states = TypedStateMachine<ISPAnimatorState>.CreateFromComponentSource(this.gameObject); //new ComponentStateMachine<ISPAnimatorState>(this.gameObject);
            _states.StateChanged += this.OnStateChanged;
        }

        #endregion

        #region Properties

        public ITypedStateMachine<ISPAnimatorState> AnimatorStates { get { return _states; } }

        #endregion

        #region Methods

        public void Stop(string name)
        {
            this.animation.Stop(name);
        }

        public void StopAll()
        {
            this.animation.Stop();
        }

        public bool IsPlaying(string name)
        {
            return this.animation.IsPlaying(name);
        }


        private void OnStateChanged(object sender, StateChangedEventArgs<ISPAnimatorState> e)
        {
            if (e.FromState != null) e.FromState.Deactivate();
            if (e.ToState != null) e.ToState.Activate();
        }

        protected virtual void Update()
        {
            if (_states.Current != null) _states.Current.UpdateState();
        }

        #endregion

        #region ISPAnimationSource Interface

        public override ISPAnim GetAnim(string name)
        {
            var cur = this.AnimatorStates.Current;
            if (cur != null && cur is ISPAnimationSource) return (cur as ISPAnimationSource).GetAnim(name);
            else return base.GetAnim(name);
        }

        #endregion

    }
}
