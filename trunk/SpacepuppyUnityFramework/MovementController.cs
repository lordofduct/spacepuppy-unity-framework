using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Hooks;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// A custom controller similar to CharacterController that supports multiple physics techniques for collision testing, while keeping a constant interface to the controller no matter the mode. 
    /// Moving this controller requires 3 steps as opposed to just one with CharacterController. CharacterController only allows 1 move per Update cycle, and is Update dependent for its state. 
    /// This on the other hand allows you to control when the state opens for move, is moved, and closes again. You should call 'OnBeforeUpdate' to ready the controller for this update, then perform all 
    /// calls to 'Move', then call 'OnUpdateComplete' to close up that frame's session. If a call to 'Move' is made outside of this session, then it is cached to be applied the next time 'OnBeforeUpdate' 
    /// is called.
    /// </summary>
    [AddComponentMenu("SpacePuppy/Motors/Movement Controller")]
    public class MovementController : SPComponent, ISerializationCallbackReceiver
    {

        #region Events

        protected event System.Action<ControllerColliderHit> OnCharacterControllerHit;

        protected delegate void OnCollisionHandler(Collision c, bool bStayed);
        protected event OnCollisionHandler OnCollision;

        #endregion

        #region Fields

        [SerializeField()]
        private bool _resetVelocityOnNoMove = false;
        private bool _bMoveCalled = false;

        private IGameObjectMover _mover;
        private System.Action _updateCache;
        private bool _bInUpdateSequence;

        private Vector3 _lastPos;
        private Vector3 _lastVel;

        //[SerializeField()]
        //private string _moverSerializedValue;
        [SerializeField()]
        private byte[] _moverSerializedValue;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            //this.ChangeMoverType(_moverType);

            _lastPos = this.transform.position;
            _mover.Reinit(this);
        }

        protected virtual void OnSpawn()
        {
            _lastPos = this.transform.position;
            _mover.Reinit(this);
            if (this.enabled) _mover.OnEnable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _lastVel = Vector3.zero;
            if (_mover != null) _mover.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_mover != null) _mover.OnDisable();
        }

        #endregion

        #region ISerializationNotificationCallback Interface

        public void OnAfterDeserialize()
        {
            //if (_mover != null)
            //{
            //    //destroy old mover
            //    _mover.Dispose();
            //    _mover = null;
            //}

            //_mover = BuildMover(this, _moverType);
            //if (_mover != null)
            //{
            //    _mover.Deserialize(_moverSerializedValue);
            //}

            _mover = SerializedDataTranslator.Deserialize(_moverSerializedValue) as IGameObjectMover;
            _moverSerializedValue = null;
        }

        public void OnBeforeSerialize()
        {
            //if (_mover == null)
            //{
            //    _moverSerializedValue = null;
            //}
            //else
            //{
            //    _moverSerializedValue = _mover.Serialize();
            //}

            _moverSerializedValue = SerializedDataTranslator.Serialize(_mover);
        }

        #endregion

        #region Properties

        public IGameObjectMover Mover { get { return _mover; } }

        //public bool IsRigidbodyMoverType { get { return _moverType == GameObjectMoverType.SimulatedRigidbody || _moverType == GameObjectMoverType.DirectRigidbody; } }

        public bool InUpdateSequence { get { return _bInUpdateSequence; } }



        public float Mass { get { return _mover.Mass; } set { _mover.Mass = value; } }

        public float StepOffset { get { return _mover.StepOffset; } set { _mover.StepOffset = value; } }

        public float SkinWidth { get { return _mover.SkinWidth; } set { _mover.SkinWidth = value; } }

        public bool CollisionEnabled { get { return _mover.CollisionEnabled; } set { _mover.CollisionEnabled = value; } }

        public Vector3 Velocity { get { return _mover.Velocity; } set { _mover.Velocity = value; } }



        public Vector3 LastPosition { get { return _lastPos; } }

        public Vector3 LastVelocity { get { return _lastVel; } }

        public bool ResetVelocityOnNoMove { get { return _resetVelocityOnNoMove; } set { _resetVelocityOnNoMove = value; } }

        #endregion

        #region Public Methods

        public void ChangeMover(IGameObjectMover mover)
        {
            if (mover == null) throw new System.ArgumentNullException("mover");
            if (mover == _mover) return;

            if (_mover != null)
            {
                //destroy old mover
                _mover.Dispose();
                _mover = null;
            }

            _mover = mover;
            _mover.Reinit(this);
            if (this.enabled) _mover.OnEnable();
        }

        /// <summary>
        /// Sets the current mover to the new object and returns the mover being replaced. Later that old mover can be swapped back in to allow for temporary changing of mode.
        /// </summary>
        /// <param name="mover"></param>
        /// <returns></returns>
        public IGameObjectMover SwapMover(IGameObjectMover mover)
        {
            if (mover == null) throw new System.ArgumentNullException("mover");
            if (mover == _mover) return _mover;

            var oldMover = _mover;
            if (oldMover != null) oldMover.OnDisable();

            _mover = mover;
            _mover.Reinit(this);
            if (this.enabled) _mover.OnEnable();

            return oldMover;
        }

        public void OnBeforeUpdate()
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence) throw new System.InvalidOperationException("MovementController->OnBeforeUpdate must be called only once per frame.");
            _bInUpdateSequence = true;

            _mover.OnBeforeUpdate();

            //set last pos and last vel, note this happens AFTER OnBeforeUpdate of the mover, 
            //this is because the physical reactions of last move didn't complete until now
            _lastPos = this.transform.position;
            _lastVel = _mover.Velocity;

            if (_updateCache != null)
            {
                _updateCache();
                _updateCache = null;
            }
        }

        public void OnUpdateComplete()
        {
            if (!this.enabled) return;
            if (!_bInUpdateSequence) throw new System.InvalidOperationException("MovementController->OnUpdateComplete must be called only once per frame.");

            _mover.OnUpdateComplete();

            if (_resetVelocityOnNoMove && !_bMoveCalled)
            {
                _mover.Velocity = Vector3.zero;
            }
            _bMoveCalled = false;

            _bInUpdateSequence = false;
        }

        /// <summary>
        /// Moves the character by some delta.
        /// </summary>
        /// <param name="mv"></param>
        public void Move(Vector3 mv)
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence)
            {
                _bMoveCalled = true;
                _mover.Move(mv);
            }
            else
            {
                _updateCache += () =>
                {
                    _bMoveCalled = true;
                    _mover.Move(mv);
                };
            }
        }

        /// <summary>
        /// Moves the character by some delta, but does not effect the velocity. This is useful for things like moving platforms.
        /// </summary>
        /// <param name="mv"></param>
        public void AtypicalMove(Vector3 mv)
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence)
            {
                _mover.AtypicalMove(mv);
            }
            else
            {
                _updateCache += () => _mover.AtypicalMove(mv);
            }
        }

        /// <summary>
        /// Move controller to a specific location with no collision.
        /// </summary>
        /// <param name="pos"></param>
        public void TeleportTo(Vector3 pos)
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence)
            {
                this.transform.position = pos;
            }
            else
            {
                _updateCache += () =>
                {
                    this.transform.position = pos;
                };
            }
        }

        /// <summary>
        /// Move controller to a specific location with no collision.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="bSetVelocityByChangeInPosition">If true velocity is updated to match the total change. If done, this should only be called ONCE per update cycle.</param>
        public void TeleportTo(Vector3 pos, bool bSetVelocityByChangeInPosition)
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence)
            {
                if (bSetVelocityByChangeInPosition)
                {
                    _mover.Velocity = (pos - this.transform.position) / GameTime.DeltaTime;
                }
                this.transform.position = pos;
            }
            else
            {
                _updateCache += () =>
                {
                    if (bSetVelocityByChangeInPosition)
                    {
                        _mover.Velocity = (pos - this.transform.position) / GameTime.DeltaTime;
                    }
                    this.transform.position = pos;
                };
            }
        }

        public void AddForce(Vector3 f, ForceMode mode)
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence)
            {
                _bMoveCalled = true;
                _mover.AddForce(f, mode);
            }
            else
            {
                _updateCache += () =>
                {
                    _bMoveCalled = true;
                    _mover.AddForce(f, mode);
                };
            }
        }

        public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence)
            {
                _bMoveCalled = true;
                _mover.AddForceAtPosition(f, pos, mode);
            }
            else
            {
                _updateCache += () =>
                {
                    _bMoveCalled = true;
                    _mover.AddForceAtPosition(f, pos, mode);
                };
            }
        }

        public com.spacepuppy.Geom.Capsule GetGeom(bool ignoreSkin)
        {
            return _mover.GetGeom(ignoreSkin);
        }

        #endregion

        #region Messages

        protected void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (this.OnCharacterControllerHit != null) this.OnCharacterControllerHit(hit);
        }

        protected void OnCollisionEnter(Collision c)
        {
            if (this.OnCollision != null) this.OnCollision(c, false);
        }

        protected void OnCollisionStay(Collision c)
        {
            if (this.OnCollision != null) this.OnCollision(c, true);
        }

        #endregion

        #region EditorOnlyMethods

#if UNITY_EDITOR
        public void ChangeMoverInEditor(IGameObjectMover mover)
        {
            if (mover == null) throw new System.ArgumentNullException("mover");
            if (mover == _mover) return;

            if (_mover != null)
            {
                //destroy old mover
                _mover.Dispose();
                _mover = null;
            }

            _mover = mover;
            _mover.Reinit(this);
        }
#endif

        #endregion

        #region Special Types

        public interface IGameObjectMover : System.IDisposable
        {

            float Mass { get; set; }
            float StepOffset { get; set; }
            float SkinWidth { get; set; }
            bool CollisionEnabled { get; set; }
            Vector3 Velocity { get; set; }

            void Reinit(MovementController controller);
            void OnEnable();
            void OnDisable();

            /// <summary>
            /// Occurs before any Moves for this frame are called, and before the LastPosition and LastVelocity are set. This is useful for resolving anything that deals with 
            /// the FixedUpdate loop and Rigidbodies, since the actual effects caused by moving a rigidbody aren't accessible until after the fact.
            /// </summary>
            void OnBeforeUpdate();
            /// <summary>
            /// Occurs after all Moves for this frame have been called.
            /// </summary>
            void OnUpdateComplete();

            com.spacepuppy.Geom.Capsule GetGeom(bool ignoreSkin);

            void Move(Vector3 mv);
            void AtypicalMove(Vector3 mv);
            void AddForce(Vector3 f, ForceMode mode);
            void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode);

        }

        /// <summary>
        /// This operates directly on a CharacterController and acts pretty much like a CharacterController with some added featues (AddForce, AddForceAt, Can be called more than once per update and remain accurate).
        /// </summary>
        [System.Serializable()]
        public class CharacterControllerMover : IGameObjectMover
        {

            #region Fields

            [System.NonSerialized()]
            private MovementController _owner;
            [System.NonSerialized()]
            private CharacterController _controller;

            private float _mass;
            private float _skinWidth;

            [System.NonSerialized()]
            private Vector3 _vel;
            [System.NonSerialized()]
            private Vector3 _talliedVel;

            #endregion

            #region CONSTRUCTOR

            public CharacterControllerMover()
            {
            }

            #endregion

            #region IGameObjectMover Interface

            public float Mass { get { return _mass; } set { _mass = value; } }
            public float StepOffset
            {
                get { return (_controller != null) ? _controller.stepOffset : 0f; }
                set { if (_controller != null) _controller.stepOffset = value; }
            }
            public float SkinWidth { get { return _skinWidth; } set { _skinWidth = value; } }
            public bool CollisionEnabled { get { return _controller.detectCollisions; } set { _controller.detectCollisions = value; } }
            public Vector3 Velocity
            {
                get { return _vel; }
                set
                {
                    _vel = value;
                    _talliedVel = _vel;
                }
            }

            public void Reinit(MovementController controller)
            {
                if (Object.Equals(controller, null)) throw new System.ArgumentNullException("controller");
                _owner = controller;

                if (!_owner.HasComponent<CharacterController>()) throw new System.InvalidOperationException("MovementController requires CharacterController component to operate in CharacterController mode.");

                _controller = _owner.GetComponent<CharacterController>();
                _vel = Vector3.zero;
            }

            public void OnEnable()
            {
                _controller.enabled = true;
                _owner.OnCharacterControllerHit -= this.OnCharacterControllerHit;
                _owner.OnCharacterControllerHit += this.OnCharacterControllerHit;
            }

            public void OnDisable()
            {
                _controller.enabled = false;
                _owner.OnCharacterControllerHit -= this.OnCharacterControllerHit;
            }

            public void OnBeforeUpdate()
            {
                _talliedVel = Vector3.zero;
            }

            public void OnUpdateComplete()
            {

            }

            public Geom.Capsule GetGeom(bool ignoreSkin)
            {
                var cap = com.spacepuppy.Geom.Capsule.FromCollider(_controller);
                if (!ignoreSkin) cap.Radius += _skinWidth;
                return cap;
            }

            public void Move(Vector3 mv)
            {
                if (_controller.detectCollisions)
                {
                    _controller.Move(mv);
                    //update velocity
                    _talliedVel += _controller.velocity;
                    _vel = _talliedVel;
                }
                else
                {
                    _controller.transform.position += mv;
                    //update velocity
                    _talliedVel += mv / GameTime.DeltaTime;
                    _vel = _talliedVel;
                }
            }

            public void AtypicalMove(Vector3 mv)
            {
                if (_controller.detectCollisions)
                {
                    _controller.Move(mv);
                }
                else
                {
                    _controller.transform.position += mv;
                }
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        //force = mass*distance/time^2
                        //distance = force * time^2 / mass
                        this.Move(f * GameTime.DeltaTime * GameTime.DeltaTime / _mass);
                        break;
                    case ForceMode.Acceleration:
                        //acceleration = distance/time^2
                        //distance = acceleration * time^2
                        this.Move(f * (GameTime.DeltaTime * GameTime.DeltaTime));
                        break;
                    case ForceMode.Impulse:
                        //impulse = mass*distance/time
                        //distance = impulse * time / mass
                        this.Move(f * GameTime.DeltaTime / _mass);
                        break;
                    case ForceMode.VelocityChange:
                        //velocity = distance/time
                        //distance = velocity * time
                        this.Move(f * GameTime.DeltaTime);
                        break;
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                this.AddForce(f, mode);
            }

            #endregion

            #region Event Handlers

            private void OnCharacterControllerHit(ControllerColliderHit hit)
            {
                if (Notification.HasObserver<MovementControllerHitNotification>(_owner, true))
                {
                    var n = new MovementControllerHitNotification(_owner, hit.collider, hit.moveDirection, hit.moveLength, hit.normal, hit.point);
                    Notification.PostNotification<MovementControllerHitNotification>(_owner, n, true);
                }
            }

            #endregion

            #region Disposable Interface

            private bool _isDisposed = false;

            void System.IDisposable.Dispose()
            {
                if (!_isDisposed)
                {
                    if (_owner != null)
                    {
                        _owner.OnCharacterControllerHit -= this.OnCharacterControllerHit;
                    }

                    _owner = null;
                    _controller = null;
                    _isDisposed = true;
                }
            }

            #endregion

        }

        /// <summary>
        /// This is a rigidbody mover that attempts to simulate CharacterController like functionality.
        /// </summary>
        [System.Serializable()]
        public class SimulatedRigidbodyMover : IGameObjectMover
        {

            #region Fields

            [System.NonSerialized()]
            private MovementController _owner;
            [System.NonSerialized()]
            private Rigidbody _rigidbody;
            [System.NonSerialized()]
            private Collider _coll;

            private float _stepOffset;
            private float _skinWidth;

            [System.NonSerialized()]
            private Vector3 _vel;

            [System.NonSerialized()]
            private System.Action _moves;
            [System.NonSerialized()]
            private bool _moveCalledLastFrame;
            [System.NonSerialized()]
            private Vector3 _talliedMove;
            [System.NonSerialized()]
            private float _lastDt;

            #endregion

            #region CONSTRUCTOR

            public SimulatedRigidbodyMover()
            {
            }

            #endregion

            #region IGameObjectMover Interface

            public float Mass
            {
                get { return (_rigidbody != null) ? _rigidbody.mass : 0f; }
                set { if (_rigidbody != null) _rigidbody.mass = value; }
            }
            public float StepOffset { get { return _stepOffset; } set { _stepOffset = value; } }
            public float SkinWidth { get { return _skinWidth; } set { _skinWidth = value; } }
            public bool CollisionEnabled { get { return _coll.enabled; } set { _coll.enabled = value; } }
            public Vector3 Velocity
            {
                get { return _vel; }
                set { _vel = value; }
            }

            public void Reinit(MovementController controller)
            {
                if (Object.Equals(controller, null)) throw new System.ArgumentNullException("controller");
                _owner = controller;

                if (_owner.rigidbody == null) throw new System.InvalidOperationException("MovementController requires Rigidbody component to operate in Rigidbody mode.");
                if (!_owner.HasComponent<SphereCollider>() && !_owner.HasComponent<CapsuleCollider>()) throw new System.InvalidOperationException("MovementController on '" + _owner.gameObject.name + "' requires a supported Sphere or Capsule Collider component to operate in Rigidbody mode.");

                _rigidbody = _owner.rigidbody;
                _coll = _owner.GetFirstLikeComponent<Collider>();
                _vel = Vector3.zero;
            }

            public void OnEnable()
            {
                _coll.enabled = true;
                _rigidbody.isKinematic = false;
                _owner.OnCollision -= this.OnCollision;
                _owner.OnCollision += this.OnCollision;
            }

            public void OnDisable()
            {
                _coll.enabled = false;
                _rigidbody.isKinematic = true;
                _owner.OnCollision -= this.OnCollision;
            }

            public void OnBeforeUpdate()
            {
                if (_moveCalledLastFrame)
                {
                    _moveCalledLastFrame = false;

                    //_vel = (_owner.transform.position - _owner.LastPosition) / _lastDt;

                    //we calculate velocity of LAST move in this move
                    var actualMove = (_owner.transform.position - _owner.LastPosition);
                    var n = actualMove.normalized;
                    _vel = n * Mathf.Min(actualMove.magnitude, Vector3.Dot(_talliedMove, n)) / _lastDt;
                }

                _talliedMove = Vector3.zero;
            }

            public void OnUpdateComplete()
            {
                if (_moves != null)
                {
                    _moves();
                    _moveCalledLastFrame = true;
                    _moves = null;
                }

                _lastDt = GameTime.DeltaTime;
            }

            public Geom.Capsule GetGeom(bool ignoreSkin)
            {
                if (_coll is SphereCollider)
                {
                    var c = com.spacepuppy.Geom.Capsule.FromCollider(_coll as SphereCollider);
                    if (!ignoreSkin) c.Radius -= _skinWidth;
                    return c;
                }
                else if (_coll is CapsuleCollider)
                {
                    var c = com.spacepuppy.Geom.Capsule.FromCollider(_coll as CapsuleCollider);
                    if (!ignoreSkin) c.Radius -= _skinWidth;
                    return c;
                }
                else
                {
                    return new com.spacepuppy.Geom.Capsule(_owner.transform.position, _owner.transform.position, 0f);
                }
            }

            public void Move(Vector3 mv)
            {
                _moves += () =>
                {
                    _rigidbody.MovePosition(_rigidbody.position + mv);

                    _talliedMove += mv;
                    _vel = _talliedMove / GameTime.DeltaTime;
                };
            }

            public void AtypicalMove(Vector3 mv)
            {
                _moves += () =>
                {
                    _rigidbody.MovePosition(_rigidbody.position + mv);
                };
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        //force = mass*distance/time^2
                        //distance = force * time^2 / mass
                        this.Move(f * GameTime.DeltaTime * GameTime.DeltaTime / this.Mass);
                        break;
                    case ForceMode.Acceleration:
                        //acceleration = distance/time^2
                        //distance = acceleration * time^2
                        this.Move(f * (GameTime.DeltaTime * GameTime.DeltaTime));
                        break;
                    case ForceMode.Impulse:
                        //impulse = mass*distance/time
                        //distance = impulse * time / mass
                        this.Move(f * GameTime.DeltaTime / this.Mass);
                        break;
                    case ForceMode.VelocityChange:
                        //velocity = distance/time
                        //distance = velocity * time
                        this.Move(f * GameTime.DeltaTime);
                        break;
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                this.AddForce(f, mode);
            }

            #endregion

            #region Event Handlers

            private void OnCollision(Collision c, bool bStayed)
            {
                if (Notification.HasObserver<MovementControllerHitNotification>(_owner, true))
                {
                    //TODO - need to think of a better way to deal with more than 1 contact, currently just going to treat as the FIRST contact
                    if (c.contacts.Length >= 1)
                    {
                        var contact = c.contacts[0];
                        var n = new MovementControllerHitNotification(_owner, c.collider, _talliedMove.normalized, _talliedMove.magnitude, contact.normal, contact.point);
                        Notification.PostNotification<MovementControllerHitNotification>(_owner, n, true);
                    }
                }
            }

            #endregion

            #region Disposable Interface

            private bool _isDisposed = false;

            void System.IDisposable.Dispose()
            {
                if (!_isDisposed)
                {
                    if (_owner != null)
                    {
                        _owner.OnCollision -= this.OnCollision;
                    }

                    _owner = null;
                    _coll = null;
                    _isDisposed = true;
                }
            }

            #endregion

        }


        /// <summary>
        /// This is a rigidbody controller that manipulates the velocity directly.
        /// </summary>
        [System.Serializable()]
        public class DirectRigidBodyMover : IGameObjectMover
        {

            #region Fields

            [System.NonSerialized()]
            private MovementController _owner;
            [System.NonSerialized()]
            private Rigidbody _rigidbody;
            [System.NonSerialized()]
            private Collider _coll;

            private float _stepOffset;
            private float _skinWidth;

            [System.NonSerialized()]
            private Vector3 _talliedMove;

            #endregion

            #region CONSTRUCTOR

            public DirectRigidBodyMover()
            {
            }

            #endregion

            #region IGameObjectMover Interface

            public float Mass
            {
                get { return (_rigidbody != null) ? _rigidbody.mass : 0f; }
                set { if (_rigidbody != null) _rigidbody.mass = value; }
            }
            public float StepOffset { get { return _stepOffset; } set { _stepOffset = value; } }
            public float SkinWidth { get { return _skinWidth; } set { _skinWidth = value; } }
            public bool CollisionEnabled { get { return _coll.enabled; } set { _coll.enabled = value; } }
            public Vector3 Velocity
            {
                get { return _rigidbody.velocity; }
                set
                {
                    _rigidbody.velocity = value;
                    _talliedMove = value;
                }
            }

            public void Reinit(MovementController controller)
            {
                if (Object.Equals(controller, null)) throw new System.ArgumentNullException("controller");
                _owner = controller;

                if (_owner.rigidbody == null) throw new System.InvalidOperationException("MovementController requires Rigidbody component to operate in Rigidbody mode.");
                if (!_owner.HasComponent<SphereCollider>() && !_owner.HasComponent<CapsuleCollider>()) throw new System.InvalidOperationException("MovementController requires a supported Sphere or Capsule Collider component to operate in Rigidbody mode.");

                _rigidbody = _owner.rigidbody;
                _coll = _owner.GetFirstLikeComponent<Collider>();
            }

            public void OnEnable()
            {
                _coll.enabled = true;
                _rigidbody.isKinematic = false;
                _owner.OnCollision -= this.OnCollision;
                _owner.OnCollision += this.OnCollision;
            }

            public void OnDisable()
            {
                _coll.enabled = false;
                _rigidbody.isKinematic = true;
                _owner.OnCollision -= this.OnCollision;
            }

            public void OnBeforeUpdate()
            {
                _talliedMove = Vector3.zero;
            }

            public void OnUpdateComplete()
            {

            }

            public Geom.Capsule GetGeom(bool ignoreSkin)
            {
                if (_coll is SphereCollider)
                {
                    var c = com.spacepuppy.Geom.Capsule.FromCollider(_coll as SphereCollider);
                    if (!ignoreSkin) c.Radius -= _skinWidth;
                    return c;
                }
                else if (_coll is CapsuleCollider)
                {
                    var c = com.spacepuppy.Geom.Capsule.FromCollider(_coll as CapsuleCollider);
                    if (!ignoreSkin) c.Radius -= _skinWidth;
                    return c;
                }
                else
                {
                    return new com.spacepuppy.Geom.Capsule(_owner.transform.position, _owner.transform.position, 0f);
                }
            }

            public void Move(Vector3 mv)
            {
                _talliedMove += mv;

                Vector3 v = _talliedMove / GameTime.DeltaTime;
                //v -= _owner.LastVelocity; //remove the old velocity so it's setting to, not adding to
                //_rigidbody.AddForce(v, ForceMode.VelocityChange);
                _rigidbody.velocity = v;
            }

            public void AtypicalMove(Vector3 mv)
            {
                _rigidbody.MovePosition(_rigidbody.position + mv);
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                _rigidbody.AddForce(f, mode);
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                _rigidbody.AddForceAtPosition(f, pos, mode);
            }

            #endregion

            #region Event Handlers

            private void OnCollision(Collision c, bool bStayed)
            {
                if (Notification.HasObserver<MovementControllerHitNotification>(_owner, true))
                {
                    //TODO - need to think of a better way to deal with more than 1 contact, currently just going to treat as the FIRST contact
                    if (c.contacts.Length >= 1)
                    {
                        var contact = c.contacts[0];
                        var n = new MovementControllerHitNotification(_owner, c.collider, _talliedMove.normalized, _talliedMove.magnitude, contact.normal, contact.point);
                        Notification.PostNotification<MovementControllerHitNotification>(_owner, n, true);
                    }
                }
            }

            #endregion

            #region Disposable Interface

            private bool _isDisposed = false;

            void System.IDisposable.Dispose()
            {
                if (!_isDisposed)
                {
                    if (_owner != null)
                    {
                        _owner.OnCollision -= this.OnCollision;
                    }

                    _owner = null;
                    _coll = null;
                    _isDisposed = true;
                }
            }

            #endregion

        }

        /// <summary>
        /// This attempts to emulate the way CharacterController operates by casting a capsule. Unlike CharacterController it attempts to be axis ambiguous, where as CC is locked to the +Y up axis.
        /// </summary>
        [System.Serializable()]
        public class EmulatedCharacterControllerBodyMover : IGameObjectMover
        {

            #region Fields

            [System.NonSerialized()]
            private MovementController _owner;

            private float _mass;
            private float _stepOffset;
            private float _skinWidth;
            private Capsule _geom;

            [System.NonSerialized()]
            private bool _collisionEnabled;

            [System.NonSerialized()]
            private Vector3 _vel;

            [System.NonSerialized()]
            private bool _onSurface;
            [System.NonSerialized()]
            private Vector3 _currentSurfaceNormal;

            #endregion

            #region CONSTRUCTOR

            public EmulatedCharacterControllerBodyMover()
            {
            }

            #endregion

            #region Properties

            public Capsule Geom
            {
                get { return _geom; }
                set { _geom = value; }
            }

            #endregion

            #region IGameObjectMover Interface

            public float Mass { get { return _mass; } set { _mass = value; } }
            public float StepOffset { get { return _stepOffset; } set { _stepOffset = value; } }
            public float SkinWidth { get { return _skinWidth; } set { _skinWidth = value; } }
            public bool CollisionEnabled { get { return _collisionEnabled; } set { _collisionEnabled = value; } }
            public Vector3 Velocity
            {
                get { return _vel; }
                set { _vel = value; }
            }

            public void Reinit(MovementController controller)
            {
                if (Object.Equals(controller, null)) throw new System.ArgumentNullException("controller");
                _owner = controller;

                _vel = Vector3.zero;
            }

            public void OnEnable()
            {
            }

            public void OnDisable()
            {
            }

            public void OnBeforeUpdate()
            {

            }

            public void OnUpdateComplete()
            {

            }

            public Geom.Capsule GetGeom(bool ignoreSkin)
            {
                Vector3 s = _owner.transform.TransformPoint(_geom.Start);
                Vector3 e = _owner.transform.TransformPoint(_geom.End);
                float r = VectorUtil.GetMaxScalar(_owner.transform.lossyScale) * ((ignoreSkin) ? _geom.Radius : _geom.Radius + _skinWidth);

                return new Geom.Capsule(s, e, r);
            }

            public void Move(Vector3 mv)
            {
                //TODO - need to make this actually work, but is NOT important at this current moment in time, don't forget to include the 'CollisionEnabled' flag

                //var geom = this.GetGeom(true);
                //int mask = PhysicsUtil.CalculateLayerMaskAgainst(_owner.gameObject.layer);
                //RaycastHit hit;
                //bool bStayedGrounded = false;

                //if (_onSurface)
                //{
                //    if (geom.Cast(-_currentSurfaceNormal, out hit, _skinWidth + 0.0001f, mask))
                //    {
                //        //update ground normal
                //        RaycastHit simpleHit;
                //        if (Physics.Raycast(hit.point + hit.normal, -hit.normal, out simpleHit, 1.01f, mask))
                //        {
                //            bStayedGrounded = true;
                //            _currentSurfaceNormal = simpleHit.normal;

                //            var d = Vector3.Dot(mv, _currentSurfaceNormal);
                //            if (d > 0f) mv -= _currentSurfaceNormal * d;

                //            if (hit.distance < _skinWidth)
                //            {
                //                _owner.transform.position += hit.normal * (_skinWidth - hit.distance);
                //            }
                //        }
                //        else
                //        {
                //            _onSurface = false;
                //        }
                //    }
                //    else
                //    {
                //        _onSurface = false;
                //    }

                //}



                //Vector3 dir = mv.normalized;
                //float dist = mv.magnitude - _skinWidth;
                //if (geom.Cast(dir, out hit, dist, mask))
                //{
                //    if (!bStayedGrounded)
                //    {
                //        _onSurface = true;
                //        //update ground normal
                //        RaycastHit simpleHit;
                //        if (Physics.Raycast(hit.point + hit.normal, -hit.normal, out simpleHit, 1.01f, mask))
                //        {
                //            _currentSurfaceNormal = simpleHit.normal;
                //        }
                //    }

                //    var d = hit.distance - _skinWidth;
                //    _owner.transform.position += dir * d;
                //}
                //else
                //{
                //    _owner.transform.position += mv;
                //}

                //NOTE - temp code until above is fixed
                _owner.transform.position += mv;

                //update velocity
                _vel = (_owner.transform.position - _owner.LastPosition) / GameTime.DeltaTime;
            }

            public void AtypicalMove(Vector3 mv)
            {

            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        //force = mass*distance/time^2
                        //distance = force * time^2 / mass
                        this.Move(f * GameTime.DeltaTime * GameTime.DeltaTime / _mass);
                        break;
                    case ForceMode.Acceleration:
                        //acceleration = distance/time^2
                        //distance = acceleration * time^2
                        this.Move(f * (GameTime.DeltaTime * GameTime.DeltaTime));
                        break;
                    case ForceMode.Impulse:
                        //impulse = mass*distance/time
                        //distance = impulse * time / mass
                        this.Move(f * GameTime.DeltaTime / _mass);
                        break;
                    case ForceMode.VelocityChange:
                        //velocity = distance/time
                        //distance = velocity * time
                        this.Move(f * GameTime.DeltaTime);
                        break;
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                this.AddForce(f, mode);
            }

            #endregion

            #region Disposable Interface

            private bool _isDisposed = false;

            void System.IDisposable.Dispose()
            {
                if (!_isDisposed)
                {
                    _owner = null;
                    _isDisposed = true;
                }
            }

            #endregion

        }

        [System.Serializable()]
        public class RagdollBodyMover : IGameObjectMover
        {

            #region Fields

            [System.NonSerialized()]
            private MovementController _owner;
            [System.NonSerialized()]
            private ArmatureRig _armature;

            [System.NonSerialized()]
            private float _mass;

            #endregion

            #region CONSTRUCTOR

            public RagdollBodyMover()
            {
            }

            #endregion

            #region IGameObjectMover Interface

            public float Mass { get { return _mass; } set { } }

            public float StepOffset { get { return 0f; } set { } }

            public float SkinWidth { get { return 0f; } set { } }

            public bool CollisionEnabled
            {
                get
                {
                    return (_armature != null && _armature.RootJoint != null) ? !_armature.RootJoint.isKinematic : false;
                }
                set
                {
                    if (_armature != null)
                    {
                        _armature.SetCollidersEnabled(value);
                    }
                }
            }

            public Vector3 Velocity
            {
                get
                {
                    return (_armature != null && _armature.RootJoint != null) ? _armature.RootJoint.velocity : Vector3.zero;
                }
                set
                {
                    if (_armature != null && _armature.RootJoint != null)
                        _armature.RootJoint.velocity = value;
                }
            }



            public void Reinit(MovementController controller)
            {
                if (Object.Equals(controller, null)) throw new System.ArgumentNullException("controller");
                _owner = controller;

                _armature = _owner.FindComponent<ArmatureRig>();
                if (_armature == null) throw new System.InvalidOperationException("MovementController requires an ArmatureRig to operate in Ragdoll mode.");

                _mass = _armature.CalculateMass();
            }

            public void OnEnable()
            {
                _armature.SetJointsKinematic(false);
                _armature.SetCollidersEnabled(true);
            }

            public void OnDisable()
            {
            }

            public void OnBeforeUpdate()
            {
                //do nothing
            }

            public void OnUpdateComplete()
            {
                if (_armature.RootJoint != null)
                {
                    var pos = _armature.RootJoint.transform.position;
                    _owner.rootTransform.position = pos;
                    _armature.RootJoint.transform.position = pos;
                }
            }

            public Capsule GetGeom(bool ignoreSkin)
            {
                var s = _armature.CalculateBoundingSphere();
                return new Capsule(s.Center, s.Center, s.Radius);
            }

            public void Move(Vector3 mv)
            {
                if (_armature.RootJoint != null)
                {
                    var v = mv / GameTime.DeltaTime;
                    _armature.RootJoint.AddForce(v, ForceMode.VelocityChange);

                    var m = v.magnitude;
                    var n = v / m;
                    var d = Vector3.Dot(_armature.RootJoint.velocity, n);
                    if (d > m)
                    {
                        _armature.RootJoint.velocity -= n * (d - m);
                    }
                }
            }

            public void AtypicalMove(Vector3 mv)
            {
                this.Move(mv);
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                if (_armature.RootJoint != null)
                {
                    _armature.RootJoint.AddForce(f, mode);
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                if (_armature.RootJoint != null)
                {
                    _armature.RootJoint.AddForceAtPosition(f, pos, mode);
                }
            }




            public string Serialize()
            {
                return null;
            }

            public void Deserialize(string value)
            {
                //do nothing
            }

            public void Dispose()
            {
                _owner = null;
                _armature = null;
            }

            #endregion

        }

        #endregion




        #region Notification Types

        public class MovementControllerHitNotification : Notification
        {

            #region Fields

            private MovementController _controller;
            private Collider _collider;
            private Vector3 _moveDir;
            private float _moveLen;
            private Vector3 _normal;
            private Vector3 _point;

            #endregion

            #region CONSTRUCTOR

            public MovementControllerHitNotification(MovementController controller, Collider otherCollider,
                                                     Vector3 moveDir, float moveLen, Vector3 norm, Vector3 pnt)
            {
                _controller = controller;
                _collider = otherCollider;
                _moveDir = moveDir;
                _moveLen = moveLen;
                _normal = norm;
                _point = pnt;
            }

            #endregion

            #region Properties

            public MovementController Controller { get { return _controller; } }
            public Collider Collider { get { return _collider; } }
            public Vector3 MoveDirection { get { return _moveDir; } }
            public float MoveLength { get { return _moveLen; } }
            public Vector3 Normal { get { return _normal; } }
            public Vector3 Point { get { return _point; } }

            #endregion

        }

        #endregion

    }

}
