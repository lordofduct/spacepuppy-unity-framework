using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;

namespace com.spacepuppy.Cameras
{

    /// <summary>
    /// A state machine for moving a camera.
    /// </summary>
    public class CameraMovementController : SPComponent
    {

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("TargetCamera")]
        [DefaultFromSelf()]
        private Transform _targetCamera;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("TetherTarget")]
        [Tooltip("A target that can be used by a movement style when updating the camera position.")]
        private Transform _tetherTarget;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("UseUpdateSequence")]
        private UpdateSequence _useUpdateSequence = UpdateSequence.Update;

        [DisableOnPlay()]
        [SerializeField()]
        [Tooltip("Camera states can be attached to GameObject children of this. Note - this makes changing states slower.")]
        private bool _allowStatesAsChildren;
        
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("StartingCameraStyle")]
        private Component _startingCameraStyle;



        [System.NonSerialized()]
        private ITypedStateMachine<ICameraMovementControllerState> _stateMachine;

        [System.NonSerialized()]
        private Coroutine _updateRoutine;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if(_allowStatesAsChildren)
            {
                _stateMachine = TypedStateMachine<ICameraMovementControllerState>.CreateFromParentComponentSource(this.gameObject, true, false);
            }
            else
            {
                _stateMachine = TypedStateMachine<ICameraMovementControllerState>.CreateFromComponentSource(this.gameObject);
            }
            _stateMachine.StateChanged += this.OnStateChanged_Internal;
        }

        protected override void Start()
        {
            base.Start();

            if (_startingCameraStyle is ICameraMovementControllerState)
            {
                if (_stateMachine.Contains(_startingCameraStyle as ICameraMovementControllerState))
                {
                    _stateMachine.ChangeState(_startingCameraStyle as ICameraMovementControllerState);
                }
            }
        }

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            _updateRoutine = this.StartCoroutine(this.UpdateRoutine());
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!this.started) return;
            if (_stateMachine.Current != null) _stateMachine.Current.OnResumed();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_stateMachine.Current != null) _stateMachine.Current.OnPaused();

            if (_updateRoutine != null) this.StopCoroutine(_updateRoutine);
            _updateRoutine = null;
        }

        #endregion

        #region Properties

        public Transform TargetCamera
        {
            get { return _targetCamera; }
            set { _targetCamera = value; }
        }

        public Transform TetherTarget
        {
            get { return _tetherTarget; }
            set { _tetherTarget = value; }
        }

        public UpdateSequence UseUpdateSequence
        {
            get { return _useUpdateSequence; }
            set { _useUpdateSequence = value; }
        }

        public ICameraMovementControllerState StartingCameraStyle
        {
            get { return _startingCameraStyle as ICameraMovementControllerState; }
            set { _startingCameraStyle = value as Component; }
        }

        public ITypedStateMachine<ICameraMovementControllerState> States { get { return _stateMachine; } }

        #endregion

        #region Methods

        public void SnapToTarget()
        {
            if (_stateMachine.Current != null)
            {
                _stateMachine.Current.SnapToTarget();
            }
        }

        #endregion


        #region UpdateRoutine

        private System.Collections.IEnumerator UpdateRoutine()
        {
            var waitForFixed = new WaitForFixedUpdate();
            var waitForLate = new WaitForEndOfFrame();

            Restart:

            switch(_useUpdateSequence)
            {
                case UpdateSequence.None:
                case UpdateSequence.Update:
                    yield return null;
                    break;
                case UpdateSequence.FixedUpdate:
                    yield return waitForFixed;
                    break;
                case UpdateSequence.LateUpdate:
                    yield return waitForLate;
                    break;
            }

            if (_useUpdateSequence != UpdateSequence.None && _stateMachine.Current != null)
            {
                _stateMachine.Current.UpdateMovement();
            }

            goto Restart;
        }

        #endregion

        #region Handlers
        
        private void OnStateChanged_Internal(object sender, StateChangedEventArgs<ICameraMovementControllerState> e)
        {
            if (e.FromState != null)
            {
                e.FromState.Deactivate();
            }

            this.OnStateChanged(e);

            if (e.ToState != null)
            {
                e.ToState.Activate(this);
            }
        }

        protected virtual void OnStateChanged(StateChangedEventArgs<ICameraMovementControllerState> e)
        {

        }

        #endregion

    }
}
