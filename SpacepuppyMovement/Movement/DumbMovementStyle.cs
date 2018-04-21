using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Movement;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Movement
{

    /// <summary>
    /// Represents a MovementStyle that operates regardless of if a MovementMotor is present or not, or even a MovementController. 
    /// Most MovementStyles rely on the MovementMotor state machine to manage when it is enabled or not, and a MovementController 
    /// to handle appropriately calculate translation on the call to 'Move'.
    /// 
    /// Some MovementStyles though may only need
    /// </summary>
    public abstract class DumbMovementStyle : SPEntityComponent, IMovementStyle, IIgnorableCollision
    {

        public enum UpdateMode
        {
            Inactive = 0,
            Motor = 1,
            MovementControllerOnly = 2,
            CharacterController = 3,
            DumbRigidbody = 4,
            DumbTransformOnly = 5
        }
        
        protected virtual void OnHitSomething(Collider c)
        {
        }
        protected virtual void OnHitSomething(Collision c)
        {
        }

        #region Fields

        [SerializeField]
        private bool _activateOnStart;

        [System.NonSerialized()]
        private UpdateMode _mode;

        [System.NonSerialized()]
        private MovementMotor _motor;
        [System.NonSerialized()]
        private MovementController _controller;
        [System.NonSerialized()]
        private CharacterController _charController;
        [System.NonSerialized()]
        private Rigidbody _rigidbody;

        [System.NonSerialized()]
        private RadicalCoroutine _routine;

        [System.NonSerialized()]
        private Vector3 _lastPos;
        [System.NonSerialized()]
        private Vector3 _dumbVel;


        [System.NonSerialized()]
        private bool _activeStatus;
        [System.NonSerialized()]
        private bool _paused;
        [System.NonSerialized]
        private bool _rigidbodyKinematicCache;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.DetermineUpdateMode();
        }

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();
            
            if (_activateOnStart && _mode > UpdateMode.Motor)
            {
                this.MakeActiveStyle();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (GameLoopEntry.ApplicationClosing) return;

            if (_activeStatus)
            {
                this.DeactivateStyle();
            }
        }

        #endregion

        #region Properties

        public bool ActivateOnStart
        {
            get { return _activateOnStart; }
            set { _activateOnStart = value; }
        }

        public MovementMotor Motor { get { return _motor; } }

        public MovementController Controller { get { return _controller; } }

        public CharacterController CharacterController { get { return _charController; } }

        public Rigidbody Rigidbody { get { return _rigidbody; } }

        public UpdateMode Mode { get { return _mode; } }

        public bool PrefersFixedUpdate
        {
            get
            {
                switch(_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        return _controller.Mover.PrefersFixedUpdate;
                    case UpdateMode.CharacterController:
                        return false;
                    case UpdateMode.DumbRigidbody:
                        return true;
                    case UpdateMode.DumbTransformOnly:
                    default:
                        return false;
                }
            }
        }

        public Vector3 Position
        {
            get
            {
                switch (_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        return _controller.transform.position;
                    case UpdateMode.CharacterController:
                        return _charController.transform.position;
                    case UpdateMode.DumbRigidbody:
                        return _rigidbody.position;
                    case UpdateMode.DumbTransformOnly:
                    default:
                        return this.entityRoot.transform.position;
                }
            }
            set
            {
                switch(_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        _controller.transform.position = value;
                        break;
                    case UpdateMode.CharacterController:
                        _charController.transform.position = value;
                        break;
                    case UpdateMode.DumbRigidbody:
                        _rigidbody.position = value;
                        break;
                    case UpdateMode.DumbTransformOnly:
                    default:
                        this.entityRoot.transform.position = value;
                        break;
                }
            }
        }

        public Vector3 Velocity
        {
            get
            {
                switch (_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        return _controller.Velocity;
                    case UpdateMode.CharacterController:
                        return _charController.velocity;
                    case UpdateMode.DumbRigidbody:
                        if (_rigidbody.isKinematic)
                            return _dumbVel;
                        else
                            return _rigidbody.velocity;
                    case UpdateMode.DumbTransformOnly:
                        return _dumbVel;
                    default:
                        return Vector3.zero;
                }
            }
            set
            {
                switch(_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        _controller.Velocity = value;
                        break;
                    case UpdateMode.CharacterController:
                        //do nothing
                        break;
                    case UpdateMode.DumbRigidbody:
                        if (_rigidbody.isKinematic)
                            _dumbVel = value;
                        else
                            _rigidbody.velocity = value;
                        break;
                    case UpdateMode.DumbTransformOnly:
                        _dumbVel = value;
                        break;
                }
            }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        #endregion

        #region Methods

        public void MakeActiveStyle(bool stackState = false)
        {
            if (!this.isActiveAndEnabled) return;

            if (_mode == UpdateMode.Motor)
            {
                if (stackState)
                    _motor.States.StackState(this);
                else
                    _motor.States.ChangeState(this);
            }
            else
            {
                _activeStatus = true;
                if (_routine == null || _routine.Finished)
                    _routine = this.StartRadicalCoroutine(this.SelfUpdateRoutine(), RadicalCoroutineDisableMode.Pauses);
                else if (_routine.OperatingState == RadicalCoroutineOperatingState.Inactive)
                    _routine.Start(this, RadicalCoroutineDisableMode.Pauses);
            }
        }

        public void DeactivateStyle(MovementMotor.ReleaseMode mode = MovementMotor.ReleaseMode.PopAllOrDefault, float precedence = 0f)
        {
            if (!_activeStatus) return;

            if (_mode == UpdateMode.Motor)
            {
                _motor.States.ReleaseCurrentState(mode, precedence);
            }
            else
            {
                if (_routine != null)
                {
                    _routine.Stop();
                }
                if (_paused) this.Pause(false);
                this.OnDeactivate(null, ActivationReason.Standard);
                _activeStatus = false;
            }
            _mode = UpdateMode.Inactive;
        }

        /// <summary>
        /// Puts the movement style into a state that it no longer moves the entity, if there is a non-kinematic rigidbody to the component, it is set to kinematic. 
        /// This will only work if this style is currently active, and will automatically be unpaused on deactivate.
        /// </summary>
        public void Pause(bool pause)
        {
            if (pause)
            {
                if (!_paused)
                {
                    if (!this.IsActiveStyle) return;

                    switch (_mode)
                    {
                        case UpdateMode.Inactive:
                            //do nothing
                            break;
                        case UpdateMode.Motor:
                        case UpdateMode.MovementControllerOnly:
                            _paused = true;
                            //_controller.Pause(true);
                            break;
                        case UpdateMode.CharacterController:
                            //do nothing
                            break;
                        case UpdateMode.DumbRigidbody:
                            if (!_rigidbody.isKinematic)
                            {
                                _paused = true;
                                _rigidbodyKinematicCache = _rigidbody.isKinematic;
                                _rigidbody.isKinematic = true;
                            }
                            break;
                        case UpdateMode.DumbTransformOnly:
                            //do nothing
                            break;
                    }
                }
            }
            else
            {
                if (_paused)
                {
                    _paused = false;
                    switch (_mode)
                    {
                        case UpdateMode.Inactive:
                            //do nothing
                            break;
                        case UpdateMode.Motor:
                        case UpdateMode.MovementControllerOnly:
                            //_controller.Pause(false);
                            break;
                        case UpdateMode.CharacterController:
                            //do nothing
                            break;
                        case UpdateMode.DumbRigidbody:
                            _rigidbody.isKinematic = _rigidbodyKinematicCache;
                            break;
                        case UpdateMode.DumbTransformOnly:
                            //do nothing
                            break;
                    }
                }
            }
        }
        
        protected UpdateMode DetermineUpdateMode()
        {
            _motor = this.GetComponent<MovementMotor>();
            _controller = this.entityRoot.GetComponent<MovementController>();
            _charController = this.entityRoot.GetComponent<CharacterController>();
            _rigidbody = this.entityRoot.GetComponent<Rigidbody>();

            if (_motor != null)
            {
                _mode = UpdateMode.Motor;
            }
            else if (_controller != null)
            {
                _mode = UpdateMode.MovementControllerOnly;
            }
            else if(_charController != null)
            {
                _mode = UpdateMode.CharacterController;
            }
            else if (_rigidbody != null)
            {
                _mode = UpdateMode.DumbRigidbody;
            }
            else
            {
                _mode = UpdateMode.DumbTransformOnly;
            }

            return _mode;
        }

        private System.Collections.IEnumerator SelfUpdateRoutine()
        {
            var fixedYieldInstruct = new WaitForFixedUpdate();
            this.OnActivate(null, ActivationReason.Standard);

        Loop:
            switch (_mode)
            {
                case UpdateMode.MovementControllerOnly:
                    try
                    {
                        _controller.OnBeforeUpdate();

                        this.UpdateMovement();

                        this.OnUpdateMovementComplete();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex, this);
                    }
                    finally
                    {
                        _controller.OnUpdateComplete();
                    }
                    yield return (_controller.Mover.PrefersFixedUpdate) ? fixedYieldInstruct : null;
                    break;

                case UpdateMode.DumbRigidbody:
                    if (_rigidbody.isKinematic)
                    {
                        yield return fixedYieldInstruct;
                        _lastPos = _rigidbody.position;
                        this.UpdateMovement();
                        _dumbVel = (_rigidbody.position - _lastPos) / Time.deltaTime;
                        this.OnUpdateMovementComplete();
                    }
                    else
                    {
                        yield return fixedYieldInstruct;
                        this.UpdateMovement();
                        this.OnUpdateMovementComplete();
                    }
                    break;

                case UpdateMode.DumbTransformOnly:
                    _lastPos = this.entityRoot.transform.position;
                    this.UpdateMovement();
                    _dumbVel = (this.entityRoot.transform.position - _lastPos) / Time.deltaTime;
                    this.OnUpdateMovementComplete();
                    yield return null;
                    break;
            }

            goto Loop;
        }

        protected abstract void UpdateMovement();

        protected virtual void OnUpdateMovementComplete()
        {

        }

        protected virtual void OnActivate(IMovementStyle lastStyle, ActivationReason reason)
        {

        }

        protected virtual void OnDeactivate(IMovementStyle nextStyle, ActivationReason reason)
        {

        }

        protected virtual void OnPurgedFromStack()
        {

        }

        #endregion
        
        #region IMovementStyle Interface

        void IMovementStyle.OnActivate(IMovementStyle lastStyle, ActivationReason reason)
        {
            if (_mode == UpdateMode.Inactive) this.DetermineUpdateMode();
            _activeStatus = true;
            this.OnActivate(lastStyle, reason);
        }

        void IMovementStyle.OnDeactivate(IMovementStyle nextStyle, ActivationReason reason)
        {
            if (_paused) this.Pause(false);
            this.OnDeactivate(nextStyle, reason);
            _activeStatus = false;
        }

        void IMovementStyle.OnPurgedFromStack()
        {
            this.OnPurgedFromStack();
        }

        void IMovementStyle.UpdateMovement()
        {
            this.UpdateMovement();
        }

        void IMovementStyle.OnUpdateMovementComplete()
        {
            this.OnUpdateMovementComplete();
        }

        #endregion

        #region Movement Interface

        public bool IsActiveStyle { get { return _activeStatus; } }

        public void Move(Vector3 mv)
        {
            if (_paused) return;

            switch (this.Mode)
            {
                case UpdateMode.Motor:
                    _motor.Controller.Move(mv);
                    break;

                case UpdateMode.MovementControllerOnly:
                    _controller.Move(mv);
                    break;
                case UpdateMode.CharacterController:
                    if(_charController.Move(mv) != CollisionFlags.None)
                    {
                        this.OnHitSomething((Collider)null);
                    }
                    break;
                case UpdateMode.DumbRigidbody:
                    if (_rigidbody.isKinematic)
                    {
                        _rigidbody.MovePosition(_rigidbody.position + mv);
                    }
                    else
                    {
                        _rigidbody.velocity = mv / Time.deltaTime;
                    }
                    break;
                case UpdateMode.DumbTransformOnly:
                    this.entityRoot.transform.Translate(mv);

                    break;
            }
        }

        public void MovePosition(Vector3 pos)
        {
            if (_paused) return;

            switch (this.Mode)
            {
                case UpdateMode.Motor:
                    _motor.Controller.Move(pos - _motor.Controller.transform.position);
                    break;

                case UpdateMode.MovementControllerOnly:
                    _controller.Move(pos - _controller.transform.position);
                    break;
                case UpdateMode.CharacterController:
                    if (_charController.Move(pos - _charController.transform.position) != CollisionFlags.None)
                    {
                        this.OnHitSomething((Collider)null);
                    }
                    break;
                case UpdateMode.DumbRigidbody:
                    _rigidbody.MovePosition(pos);
                    break;
                case UpdateMode.DumbTransformOnly:
                    this.entityRoot.transform.position = pos;

                    break;
            }
        }

        #endregion

        #region IIgnorableCollision Interface

        public void IgnoreCollision(Collider coll, bool ignore)
        {
            switch(_mode)
            {
                case UpdateMode.Motor:
                case UpdateMode.MovementControllerOnly:
                    _controller.IgnoreCollision(coll, ignore);
                    break;
                case UpdateMode.CharacterController:
                    Physics.IgnoreCollision(_charController, coll, ignore);
                    break;
                case UpdateMode.DumbRigidbody:
                    IgnorableRigidbody.GetIgnorableCollision(_rigidbody).IgnoreCollision(coll, ignore);
                    break;
            }
        }

        public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            switch (_mode)
            {
                case UpdateMode.Motor:
                case UpdateMode.MovementControllerOnly:
                    _controller.IgnoreCollision(coll, ignore);
                    break;
                case UpdateMode.CharacterController:
                    coll.IgnoreCollision(_charController, ignore);
                    break;
                case UpdateMode.DumbRigidbody:
                    IgnorableRigidbody.GetIgnorableCollision(_rigidbody).IgnoreCollision(coll, ignore);
                    break;
            }
        }

        #endregion
        
    }











































    /// <summary>
    /// Represents a MovementStyle that operates regardless of if a MovementMotor is present or not, or even a MovementController. 
    /// Most MovementStyles rely on the MovementMotor state machine to manage when it is enabled or not, and a MovementController 
    /// to handle appropriately calculate translation on the call to 'Move'.
    /// 
    /// Some MovementStyles though may only need
    /// </summary>
    internal abstract class DumbMovementStyle_Old : SPEntityComponent, IMovementStyle, IIgnorableCollision
    {

        public enum UpdateMode
        {
            Inactive = 0,
            Motor = 1,
            MovementControllerOnly = 2,
            CharacterController = 3,
            DumbRigidbody = 4,
            DumbTransformOnly = 5
        }

        public class HitEventArgs : System.EventArgs
        {
            private Collider _collider;

            public HitEventArgs(Collider collider)
            {
                _collider = collider;
            }

            public Collider Collider { get { return _collider; } }
        }
        public event System.EventHandler<HitEventArgs> HitSomething;
        protected virtual void OnHitSomething(Collider c)
        {
            if (this.HitSomething != null) this.HitSomething(this, new HitEventArgs(c));
        }
        protected virtual void OnHitSomething(Collision c)
        {
            if (this.HitSomething != null) this.HitSomething(this, new HitEventArgs(c.collider));
        }

        #region Fields

        [SerializeField]
        private bool _activateOnStart;

        private UpdateMode _mode;

        [System.NonSerialized()]
        private MovementMotor _motor;
        [System.NonSerialized()]
        private MovementController _controller;
        [System.NonSerialized()]
        private CharacterController _charController;
        [System.NonSerialized()]
        private Rigidbody _rigidbody;

        [System.NonSerialized()]
        private RadicalCoroutine _routine;

        [System.NonSerialized()]
        private Vector3 _lastPos;
        [System.NonSerialized()]
        private Vector3 _dumbVel;


        [System.NonSerialized()]
        private bool _activeStatus;
        [System.NonSerialized()]
        private bool _paused;
        [System.NonSerialized]
        private bool _rigidbodyKinematicCache;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.DetermineUpdateMode();
        }

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if (_activateOnStart && _mode > UpdateMode.Motor)
            {
                this.MakeActiveStyle();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (GameLoopEntry.ApplicationClosing) return;

            if (_activeStatus)
            {
                this.DeactivateStyle();
            }
        }

        #endregion

        #region Properties

        public bool ActivateOnStart
        {
            get { return _activateOnStart; }
            set { _activateOnStart = value; }
        }

        public MovementMotor Motor { get { return _motor; } }

        public MovementController Controller { get { return _controller; } }

        public CharacterController CharacterController { get { return _charController; } }

        public Rigidbody Rigidbody { get { return _rigidbody; } }

        public UpdateMode Mode { get { return _mode; } }

        public bool PrefersFixedUpdate
        {
            get
            {
                switch (_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        return _controller.Mover.PrefersFixedUpdate;
                    case UpdateMode.CharacterController:
                        return false;
                    case UpdateMode.DumbRigidbody:
                        return true;
                    case UpdateMode.DumbTransformOnly:
                    default:
                        return false;
                }
            }
        }

        public Vector3 Position
        {
            get
            {
                switch (_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        return _controller.transform.position;
                    case UpdateMode.CharacterController:
                        return _charController.transform.position;
                    case UpdateMode.DumbRigidbody:
                        return _rigidbody.position;
                    case UpdateMode.DumbTransformOnly:
                    default:
                        return this.entityRoot.transform.position;
                }
            }
            set
            {
                switch (_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        _controller.transform.position = value;
                        break;
                    case UpdateMode.CharacterController:
                        _charController.transform.position = value;
                        break;
                    case UpdateMode.DumbRigidbody:
                        _rigidbody.position = value;
                        break;
                    case UpdateMode.DumbTransformOnly:
                    default:
                        this.entityRoot.transform.position = value;
                        break;
                }
            }
        }

        public Vector3 Velocity
        {
            get
            {
                switch (_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        return _controller.Velocity;
                    case UpdateMode.CharacterController:
                        return _charController.velocity;
                    case UpdateMode.DumbRigidbody:
                        if (_rigidbody.isKinematic)
                            return _dumbVel;
                        else
                            return _rigidbody.velocity;
                    case UpdateMode.DumbTransformOnly:
                        return _dumbVel;
                    default:
                        return Vector3.zero;
                }
            }
            set
            {
                switch (_mode)
                {
                    case UpdateMode.Motor:
                    case UpdateMode.MovementControllerOnly:
                        _controller.Velocity = value;
                        break;
                    case UpdateMode.CharacterController:
                        //do nothing
                        break;
                    case UpdateMode.DumbRigidbody:
                        if (_rigidbody.isKinematic)
                            _dumbVel = value;
                        else
                            _rigidbody.velocity = value;
                        break;
                    case UpdateMode.DumbTransformOnly:
                        _dumbVel = value;
                        break;
                }
            }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        #endregion

        #region Methods

        public void MakeActiveStyle(bool stackState = false)
        {
            if (!this.isActiveAndEnabled) return;

            if (_mode == UpdateMode.Motor)
            {
                if (stackState)
                    _motor.States.StackState(this);
                else
                    _motor.States.ChangeState(this);
            }
            else
            {
                _activeStatus = true;
                if (_routine == null || _routine.Finished)
                    _routine = this.StartRadicalCoroutine(this.SelfUpdateRoutine(), RadicalCoroutineDisableMode.Pauses);
                else if (_routine.OperatingState == RadicalCoroutineOperatingState.Inactive)
                    _routine.Start(this, RadicalCoroutineDisableMode.Pauses);
            }
        }

        public void DeactivateStyle(MovementMotor.ReleaseMode mode = MovementMotor.ReleaseMode.PopAllOrDefault, float precedence = 0f)
        {
            if (!_activeStatus) return;

            if (_mode == UpdateMode.Motor)
            {
                _motor.States.ReleaseCurrentState(mode, precedence);
            }
            else
            {
                if (_routine != null)
                {
                    _routine.Stop();
                }
                if (_paused) this.Pause(false);
                this.OnDeactivate(null, ActivationReason.Standard);
                _activeStatus = false;
            }
            _mode = UpdateMode.Inactive;
        }

        /// <summary>
        /// Puts the movement style into a state that it no longer moves the entity, if there is a non-kinematic rigidbody to the component, it is set to kinematic. 
        /// This will only work if this style is currently active, and will automatically be unpaused on deactivate.
        /// </summary>
        public void Pause(bool pause)
        {
            if (pause)
            {
                if (!_paused)
                {
                    if (!this.IsActiveStyle) return;

                    switch (_mode)
                    {
                        case UpdateMode.Inactive:
                            //do nothing
                            break;
                        case UpdateMode.Motor:
                        case UpdateMode.MovementControllerOnly:
                            _paused = true;
                            //_controller.Pause(true);
                            break;
                        case UpdateMode.CharacterController:
                            //do nothing
                            break;
                        case UpdateMode.DumbRigidbody:
                            if (!_rigidbody.isKinematic)
                            {
                                _paused = true;
                                _rigidbodyKinematicCache = _rigidbody.isKinematic;
                                _rigidbody.isKinematic = true;
                            }
                            break;
                        case UpdateMode.DumbTransformOnly:
                            //do nothing
                            break;
                    }
                }
            }
            else
            {
                if (_paused)
                {
                    _paused = false;
                    switch (_mode)
                    {
                        case UpdateMode.Inactive:
                            //do nothing
                            break;
                        case UpdateMode.Motor:
                        case UpdateMode.MovementControllerOnly:
                            //_controller.Pause(false);
                            break;
                        case UpdateMode.CharacterController:
                            //do nothing
                            break;
                        case UpdateMode.DumbRigidbody:
                            _rigidbody.isKinematic = _rigidbodyKinematicCache;
                            break;
                        case UpdateMode.DumbTransformOnly:
                            //do nothing
                            break;
                    }
                }
            }
        }

        protected UpdateMode DetermineUpdateMode()
        {
            if (_controller != null) _controller.MovementControllerHit -= this.OnControllerHitHandler;

            _motor = this.GetComponent<MovementMotor>();
            _controller = this.entityRoot.GetComponent<MovementController>();
            _charController = this.entityRoot.GetComponent<CharacterController>();
            _rigidbody = this.entityRoot.GetComponent<Rigidbody>();

            if (_motor != null)
            {
                _mode = UpdateMode.Motor;
                _controller.MovementControllerHit -= this.OnControllerHitHandler;
                _controller.MovementControllerHit += this.OnControllerHitHandler;
            }
            else if (_controller != null)
            {
                _mode = UpdateMode.MovementControllerOnly;
                _controller.MovementControllerHit -= this.OnControllerHitHandler;
                _controller.MovementControllerHit += this.OnControllerHitHandler;
            }
            else if (_charController != null)
            {
                _mode = UpdateMode.CharacterController;
            }
            else if (_rigidbody != null)
            {
                _mode = UpdateMode.DumbRigidbody;
            }
            else
            {
                _mode = UpdateMode.DumbTransformOnly;
            }

            return _mode;
        }

        private System.Collections.IEnumerator SelfUpdateRoutine()
        {
            var fixedYieldInstruct = new WaitForFixedUpdate();
            this.OnActivate(null, ActivationReason.Standard);

            Loop:
            switch (_mode)
            {
                case UpdateMode.MovementControllerOnly:
                    try
                    {
                        _controller.OnBeforeUpdate();

                        this.UpdateMovement();

                        this.OnUpdateMovementComplete();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex, this);
                    }
                    finally
                    {
                        _controller.OnUpdateComplete();
                    }
                    yield return (_controller.Mover.PrefersFixedUpdate) ? fixedYieldInstruct : null;
                    break;

                case UpdateMode.DumbRigidbody:
                    if (_rigidbody.isKinematic)
                    {
                        yield return fixedYieldInstruct;
                        _lastPos = _rigidbody.position;
                        this.UpdateMovement();
                        _dumbVel = (_rigidbody.position - _lastPos) / Time.deltaTime;
                        this.OnUpdateMovementComplete();
                    }
                    else
                    {
                        yield return fixedYieldInstruct;
                        this.UpdateMovement();
                        this.OnUpdateMovementComplete();
                    }
                    break;

                case UpdateMode.DumbTransformOnly:
                    _lastPos = this.entityRoot.transform.position;
                    this.UpdateMovement();
                    _dumbVel = (this.entityRoot.transform.position - _lastPos) / Time.deltaTime;
                    this.OnUpdateMovementComplete();
                    yield return null;
                    break;
            }

            goto Loop;
        }

        protected abstract void UpdateMovement();

        protected virtual void OnUpdateMovementComplete()
        {

        }

        protected virtual void OnActivate(IMovementStyle lastStyle, ActivationReason reason)
        {

        }

        protected virtual void OnDeactivate(IMovementStyle nextStyle, ActivationReason reason)
        {

        }

        protected virtual void OnPurgedFromStack()
        {

        }

        #endregion

        #region Event Handlers

        private System.EventHandler<MovementController.MovementControllerHitEventArgs> _onControllerHitHandler;
        private System.EventHandler<MovementController.MovementControllerHitEventArgs> OnControllerHitHandler
        {
            get
            {
                if (_onControllerHitHandler == null) _onControllerHitHandler = new System.EventHandler<MovementController.MovementControllerHitEventArgs>(this.OnControllerHit);
                return _onControllerHitHandler;
            }
        }
        private void OnControllerHit(object sender, MovementController.MovementControllerHitEventArgs e)
        {
            if (_activeStatus) return;

            this.OnHitSomething(e.Collider);
        }

        private void OnCollisionEnter(Collision c)
        {
            if (!_activeStatus) return;
            if (_mode != UpdateMode.DumbRigidbody) return;

            this.OnHitSomething(c);
        }

        private void OnCollisionStay(Collision c)
        {
            if (!_activeStatus) return;
            if (_mode != UpdateMode.DumbRigidbody) return;

            this.OnHitSomething(c);
        }

        #endregion

        #region IMovementStyle Interface

        void IMovementStyle.OnActivate(IMovementStyle lastStyle, ActivationReason reason)
        {
            if (_mode == UpdateMode.Inactive) this.DetermineUpdateMode();
            _activeStatus = true;
            this.OnActivate(lastStyle, reason);
        }

        void IMovementStyle.OnDeactivate(IMovementStyle nextStyle, ActivationReason reason)
        {
            if (_paused) this.Pause(false);
            this.OnDeactivate(nextStyle, reason);
            _activeStatus = false;
            if (_controller != null) _controller.MovementControllerHit -= this.OnControllerHitHandler;
        }

        void IMovementStyle.OnPurgedFromStack()
        {
            this.OnPurgedFromStack();
        }

        void IMovementStyle.UpdateMovement()
        {
            this.UpdateMovement();
        }

        void IMovementStyle.OnUpdateMovementComplete()
        {
        }

        #endregion

        #region Movement Interface

        public bool IsActiveStyle { get { return _activeStatus; } }

        public void Move(Vector3 mv)
        {
            switch (this.Mode)
            {
                case UpdateMode.Motor:
                    _motor.Controller.Move(mv);
                    break;

                case UpdateMode.MovementControllerOnly:
                    _controller.Move(mv);
                    break;
                case UpdateMode.CharacterController:
                    if (_charController.Move(mv) != CollisionFlags.None)
                    {
                        this.OnHitSomething((Collider)null);
                    }
                    break;
                case UpdateMode.DumbRigidbody:
                    if (_rigidbody.isKinematic)
                    {
                        _rigidbody.MovePosition(_rigidbody.position + mv);
                    }
                    else
                    {
                        _rigidbody.velocity = mv / Time.deltaTime;
                    }
                    break;
                case UpdateMode.DumbTransformOnly:
                    this.entityRoot.transform.Translate(mv);

                    break;
            }
        }

        public void MovePosition(Vector3 pos)
        {
            switch (this.Mode)
            {
                case UpdateMode.Motor:
                    _motor.Controller.Move(pos - _motor.Controller.transform.position);
                    break;

                case UpdateMode.MovementControllerOnly:
                    _controller.Move(pos - _controller.transform.position);
                    break;
                case UpdateMode.CharacterController:
                    if (_charController.Move(pos - _charController.transform.position) != CollisionFlags.None)
                    {
                        this.OnHitSomething((Collider)null);
                    }
                    break;
                case UpdateMode.DumbRigidbody:
                    _rigidbody.MovePosition(pos);
                    break;
                case UpdateMode.DumbTransformOnly:
                    this.entityRoot.transform.position = pos;

                    break;
            }
        }

        #endregion

        #region IIgnorableCollision Interface

        public void IgnoreCollision(Collider coll, bool ignore)
        {
            switch (_mode)
            {
                case UpdateMode.Motor:
                case UpdateMode.MovementControllerOnly:
                    _controller.IgnoreCollision(coll, ignore);
                    break;
                case UpdateMode.CharacterController:
                    Physics.IgnoreCollision(_charController, coll, ignore);
                    break;
                case UpdateMode.DumbRigidbody:
                    IgnorableRigidbody.GetIgnorableCollision(_rigidbody).IgnoreCollision(coll, ignore);
                    break;
            }
        }

        public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            switch (_mode)
            {
                case UpdateMode.Motor:
                case UpdateMode.MovementControllerOnly:
                    _controller.IgnoreCollision(coll, ignore);
                    break;
                case UpdateMode.CharacterController:
                    coll.IgnoreCollision(_charController, ignore);
                    break;
                case UpdateMode.DumbRigidbody:
                    IgnorableRigidbody.GetIgnorableCollision(_rigidbody).IgnoreCollision(coll, ignore);
                    break;
            }
        }

        #endregion

    }

}
