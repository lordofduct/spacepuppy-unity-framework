using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Hooks;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;
using com.spacepuppy.Serialization;

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
    public class MovementController : SPComponent, IIgnorableCollision, IForceReceiver, ISerializationCallbackReceiver
    {

        #region Events

        private System.EventHandler<MovementControllerHitEventArgs> _movementControllerHit;
        public event System.EventHandler<MovementControllerHitEventArgs> MovementControllerHit
        {
            add
            {
                if(_movementControllerHit == null)
                {
                    _movementControllerHit = value;
                    if (_mover != null) _mover.OnHasHitListenersChanged();
                }
                else
                {
                    _movementControllerHit += value;
                }
            }
            remove
            {
                if (_movementControllerHit == null) return;
                _movementControllerHit -= value;
                if(_movementControllerHit == null)
                {
                    if (_mover != null) _mover.OnHasHitListenersChanged();
                }
            }
        }

        //protected event System.Action<ControllerColliderHit> OnCharacterControllerHit;

        //protected delegate void OnCollisionHandler(Collision c, bool bStayed);
        //protected event OnCollisionHandler OnCollision;

        #endregion

        #region Fields

        [SerializeField()]
        private bool _resetVelocityOnNoMove = false;

        [System.NonSerialized()]
        private bool _bMoveCalled = false;

        [System.NonSerialized()]
        private IGameObjectMover _mover;
        [System.NonSerialized()]
        private System.Action _updateCache;
        [System.NonSerialized()]
        private bool _bInUpdateSequence;

        [System.NonSerialized()]
        private Vector3 _lastPos;
        [System.NonSerialized()]
        private Vector3 _lastVel;

        //[SerializeField()]
        //private byte[] _moverSerializedValue;
        [SerializeField()]
        private UnityData _moverData;

        [System.NonSerialized()]
        private IGameObjectMover _pausedMover;

        [System.NonSerialized()]
        private Transform _transform;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            //this.ChangeMoverType(_moverType);

            _transform = this.GetComponent<Transform>();
            _lastPos = _transform.position;
            _mover.Reinit(this);
        }

        protected virtual void OnSpawn()
        {
            _lastPos = this.transform.position;
            _mover.Reinit(this);
            if (this.enabled) _mover.OnEnable();
        }

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

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
            try
            {
                _mover = _moverData.Deserialize() as IGameObjectMover;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
                _mover = null;
            }
            _moverData.Clear();
        }

        public void OnBeforeSerialize()
        {
            if (_moverData == null) _moverData = new UnityData();
            try
            {
                _moverData.Serialize(_mover);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }

        #endregion

        #region Properties

        public new Transform transform
        {
            get
            { 
                if(object.ReferenceEquals(_transform, null)) _transform = this.GetComponent<Transform>();
                return _transform;
            }
        }

        public IGameObjectMover Mover { get { return _mover; } }

        //public bool IsRigidbodyMoverType { get { return _moverType == GameObjectMoverType.SimulatedRigidbody || _moverType == GameObjectMoverType.DirectRigidbody; } }

        public bool InUpdateSequence { get { return _bInUpdateSequence; } }

        public bool IsPaused
        {
            get { return _mover is PausedMover; }
        }

        /// <summary>
        /// If InUpdateSequence is true, this will return true if move was called yet this update sequence.
        /// </summary>
        public bool MoveCalled { get { return _bMoveCalled; } }

        public bool HasHitListeners { get { return _movementControllerHit != null; } }


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
            if (this.enabled && Application.isPlaying) _mover.OnEnable();
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
            if (this.enabled && Application.isPlaying) _mover.OnEnable();

            return oldMover;
        }

        /// <summary>
        /// Puts the MovementController into a special IGameObjectMover state, this state will allow returning back to the previous state 
        /// at any time. If the controller uses a rigidbody, it stores the state of that rigidbody.
        /// </summary>
        /// <param name="pause"></param>
        public void Pause(bool pause)
        {
            if(pause)
            {
                if(!(_mover is PausedMover))
                {
                    if (!(_pausedMover is PausedMover)) _pausedMover = new PausedMover();
                    _pausedMover = this.SwapMover(_pausedMover);
                }
            }
            else
            {
                if(_mover is PausedMover)
                {
                    _pausedMover = this.SwapMover(_pausedMover);
                }
            }
        }

        public void OnBeforeUpdate()
        {
            if (!this.enabled) return;
            if (_bInUpdateSequence) throw new System.InvalidOperationException("MovementController->OnBeforeUpdate must be called only once per frame.");
            _bInUpdateSequence = true;

            try
            {
                _mover.OnBeforeUpdate();
            }
            catch(System.Exception ex)
            {
                //Debug.LogException(ex);
                throw new System.InvalidOperationException("Current Mover failed to operate OnBeforeUpdate correctly.", ex);
            }
            finally
            {
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
        }

        public void OnUpdateComplete()
        {
            if (!this.enabled) return;
            if (!_bInUpdateSequence) throw new System.InvalidOperationException("MovementController->OnUpdateComplete must be called only once per frame.");

            try
            {
                _mover.OnUpdateComplete();
            }
            catch(System.Exception ex)
            {
                //Debug.LogException(ex);
                throw new System.InvalidOperationException("Current Mover failed to operate OnUpdateComplete correctly.", ex);
            }
            finally
            {
                if (_resetVelocityOnNoMove && !_bMoveCalled)
                {
                    _mover.Velocity = Vector3.zero;
                }
                _bMoveCalled = false;

                _bInUpdateSequence = false;
            }

        }

        public void SignalHit(MovementControllerHitEventArgs e)
        {
            if (_movementControllerHit != null) _movementControllerHit(this, e);
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
                //mono seems to create garbage even if the anonymous delegate is in an if/else, so moved to own function
                this.SetMoveCache(mv);
            }
        }
        private void SetMoveCache(Vector3 mv)
        {
            _updateCache += () =>
            {
                _bMoveCalled = true;
                _mover.Move(mv);
            };
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
                _updateCache += () =>
                {
                    _mover.AtypicalMove(mv);
                };
            }
        }

        public void MovePosition(Vector3 pos)
        {
            this.Move(pos - this.transform.position);
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
                    _mover.Velocity = (pos - this.transform.position) / Time.deltaTime;
                }
                this.transform.position = pos;
            }
            else
            {
                _updateCache += () =>
                {
                    if (bSetVelocityByChangeInPosition)
                    {
                        _mover.Velocity = (pos - this.transform.position) / Time.deltaTime;
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

        public void AddExplosionForce(float explosionForce, Vector3 explosionCenter, float explosionRadius, float upwardsModifier = 0f, ForceMode mode = ForceMode.Force)
        {
            if (!this.enabled) return;

            var geom = _mover.GetGeom(true);
            var v = geom.Center - explosionCenter;
            var force = v.normalized * Mathf.Clamp01(v.magnitude / explosionRadius) * explosionForce;
            //TODO - apply upwards modifier

            this.AddForce(force, mode);
        }

        public Capsule GetGeom(bool ignoreSkin)
        {
            return _mover.GetGeom(ignoreSkin);
        }

        #endregion

        #region Messages

        /*
         
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
         
         */

        #endregion

        #region IIgnorableCollision Interface

        public void IgnoreCollision(Collider coll, bool ignore)
        {
            if (_mover != null) _mover.IgnoreCollision(coll, ignore);
        }

        public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            if (_mover != null) _mover.IgnoreCollision(coll, ignore);
        }

        #endregion

        #region Special Mover Types

        public interface IGameObjectMover : IIgnorableCollision, System.IDisposable
        {

            bool PrefersFixedUpdate { get; }

            float Mass { get; set; }
            float StepOffset { get; set; }
            float SkinWidth { get; set; }
            bool CollisionEnabled { get; set; }
            Vector3 Velocity { get; set; }

            void Reinit(MovementController controller);
            void OnEnable();
            void OnDisable();
            void OnHasHitListenersChanged();

            /// <summary>
            /// Occurs before any Moves for this frame are called, and before the LastPosition and LastVelocity are set. This is useful for resolving anything that deals with 
            /// the FixedUpdate loop and Rigidbodies, since the actual effects caused by moving a rigidbody aren't accessible until after the fact.
            /// </summary>
            void OnBeforeUpdate();
            /// <summary>
            /// Occurs after all Moves for this frame have been called.
            /// </summary>
            void OnUpdateComplete();

            Capsule GetGeom(bool ignoreSkin);

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
            [System.NonSerialized()]
            private ControllerColliderHitEventHooks _hooks;

            [SerializeField()]
            private float _mass = 1.0f;
            [SerializeField()]
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

            public bool PrefersFixedUpdate { get { return false; } }

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

                this.OnHasHitListenersChanged();
            }

            public void OnDisable()
            {
                _controller.enabled = false;

                if(!object.ReferenceEquals(_hooks, null))
                {
                    ObjUtil.SmartDestroy(_hooks);
                    _hooks = null;
                }
            }

            public void OnHasHitListenersChanged()
            {
                if (_owner._movementControllerHit != null && object.ReferenceEquals(_hooks, null))
                {
                    _hooks = _controller.AddComponent<ControllerColliderHitEventHooks>();
                    _hooks.ControllerColliderHit += this.OnCharacterControllerHit;
                }
                else if(_owner._movementControllerHit == null && !object.ReferenceEquals(_hooks, null))
                {
                    ObjUtil.SmartDestroy(_hooks);
                    _hooks = null;
                }
            }

            public void OnBeforeUpdate()
            {
                _talliedVel = Vector3.zero;
            }

            public void OnUpdateComplete()
            {

            }

            public Capsule GetGeom(bool ignoreSkin)
            {
                var cap = Capsule.FromCollider(_controller);
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
                    _owner.transform.position += mv;
                    //update velocity
                    _talliedVel += mv / Time.deltaTime;
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
                    _owner.transform.position += mv;
                }
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        //force = mass*distance/time^2
                        //distance = force * time^2 / mass
                        this.Move(f * Time.deltaTime * Time.deltaTime / _mass);
                        break;
                    case ForceMode.Acceleration:
                        //acceleration = distance/time^2
                        //distance = acceleration * time^2
                        this.Move(f * (Time.deltaTime * Time.deltaTime));
                        break;
                    case ForceMode.Impulse:
                        //impulse = mass*distance/time
                        //distance = impulse * time / mass
                        this.Move(f * Time.deltaTime / _mass);
                        break;
                    case ForceMode.VelocityChange:
                        //velocity = distance/time
                        //distance = velocity * time
                        this.Move(f * Time.deltaTime);
                        break;
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                this.AddForce(f, mode);
            }

            #endregion

            #region Event Handlers

            private void OnCharacterControllerHit(object sender, ControllerColliderHit hit)
            {
                if(_owner.HasHitListeners)
                {
                    //var ev = new MovementControllerHitEventArgs(_owner, hit.collider, hit.normal, hit.point);
                    var ev = new MovementControllerHitEventArgs(_owner, hit.collider, new MovementControllerHitContact(hit.normal, hit.point));
                    _owner.SignalHit(ev);
                }
            }

            #endregion

            #region IIGnorableCollision Interface

            public void IgnoreCollision(Collider coll, bool ignore)
            {
                if (_controller != null && _controller.enabled) Physics.IgnoreCollision(_controller, coll, ignore);
            }

            public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
            {
                if (_controller != null && coll != null) coll.IgnoreCollision(_controller, ignore);
            }

            #endregion

            #region Disposable Interface

            private bool _isDisposed = false;

            void System.IDisposable.Dispose()
            {
                if (!_isDisposed)
                {
                    //if (_owner != null)
                    //{
                    //    _owner.OnCharacterControllerHit -= this.OnCharacterControllerHit;
                    //}
                    if(_hooks != null)
                    {
                        _hooks.ControllerColliderHit -= this.OnCharacterControllerHit;
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
            [System.NonSerialized()]
            private MovementControllerCollisionHook _hooks;

            [SerializeField()]
            private bool _bPrefersFixedUpdate = true;

            [SerializeField()]
            private float _stepOffset;
            [SerializeField()]
            private float _skinWidth;

            [System.NonSerialized()]
            private Vector3 _vel;

            //[System.NonSerialized()]
            //private System.Action _moves;
            [System.NonSerialized()]
            private bool _moveCalledLastFrame;
            [System.NonSerialized()]
            private Vector3 _talliedMove;
            [System.NonSerialized()]
            private float _lastDt;

            [System.NonSerialized()]
            private Vector3 _fullTalliedMove;

            //enabled cache
            [System.NonSerialized()]
            private bool _cacheIsKinematic;

            #endregion

            #region CONSTRUCTOR

            public SimulatedRigidbodyMover()
            {
            }

            #endregion

            #region IGameObjectMover Interface

            //public bool PrefersFixedUpdate { get { return (_rigidbody != null) ? !_rigidbody.isKinematic : false; } }
            public bool PrefersFixedUpdate { get { return _bPrefersFixedUpdate; } set { _bPrefersFixedUpdate = value; } }

            public float Mass
            {
                get { return (_rigidbody != null) ? _rigidbody.mass : 0f; }
                set { if(_rigidbody != null) _rigidbody.mass = value; }
            }
            public float StepOffset { get { return _stepOffset; } set { _stepOffset = value; } }
            public float SkinWidth { get { return _skinWidth; } set { _skinWidth = value; } }
            public bool CollisionEnabled
            {
                get
                {
                    return (_coll != null) ? _coll.enabled : false;
                }
                set
                {
                    if (_coll != null) _coll.enabled = value;
                }
            }
            public Vector3 Velocity
            {
                get { return _vel; }
                set { _vel = value; }
            }

            public void Reinit(MovementController controller)
            {
                if (Object.Equals(controller, null)) throw new System.ArgumentNullException("controller");
                _owner = controller;

                var rb = _owner.GetComponent<Rigidbody>();
                if (rb == null) throw new System.InvalidOperationException("MovementController requires Rigidbody component to operate in Rigidbody mode.");
                //if (!_owner.HasComponent<SphereCollider>() && !_owner.HasComponent<CapsuleCollider>()) throw new System.InvalidOperationException("MovementController on '" + _owner.gameObject.name + "' requires a supported Sphere or Capsule Collider component to operate in Rigidbody mode.");

                _rigidbody = rb;
                _coll = _owner.GetComponent<Collider>();
                _vel = Vector3.zero;
            }

            public void OnEnable()
            {
                _cacheIsKinematic = _rigidbody.isKinematic;
                _rigidbody.isKinematic = false;

                if (_owner.HasHitListeners) this.RegisterCollisionHooks();
            }

            public void OnDisable()
            {
                _rigidbody.isKinematic = _cacheIsKinematic;

                this.UnregisterCollisionHooks();
            }

            public void OnHasHitListenersChanged()
            {
                if (_owner != null && _owner.HasHitListeners)
                    this.RegisterCollisionHooks();
                else
                    this.UnregisterCollisionHooks();
            }

            public void OnBeforeUpdate()
            {
                if (_moveCalledLastFrame)
                {
                    _moveCalledLastFrame = false;

                    //we calculate velocity of LAST move in this move
                    if (_lastDt != 0f)
                    {
                        var actualMove = (_owner.transform.position - _owner.LastPosition);
                        
                        actualMove -= (_fullTalliedMove - _talliedMove);

                        //_vel = actualMove / _lastDt;

                        //var n = actualMove.normalized;
                        //_vel = n * MathUtil.Average(actualMove.magnitude, Vector3.Dot(_talliedMove, n)) / _lastDt;

                        var n = _talliedMove.normalized;
                        _vel = n * Mathf.Max(Vector3.Dot(actualMove, n), 0f) / _lastDt;
                    }
                }

                _fullTalliedMove = Vector3.zero;
                _talliedMove = Vector3.zero;
            }

            public void OnUpdateComplete()
            {
                //if (_moves != null)
                //{
                //    _moves();
                //    _moveCalledLastFrame = true;
                //    _moves = null;
                //}

                if(_fullTalliedMove != Vector3.zero)
                {
                    _rigidbody.MovePosition(_rigidbody.position + _fullTalliedMove);
                }

                _lastDt = Time.deltaTime;

                //zero out rigidbody velocity, as this should ONLY use MovePosition
                if(!_rigidbody.isKinematic)
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                }
            }

            public Capsule GetGeom(bool ignoreSkin)
            {
                if (_coll == null)
                {
                    var p = _owner.transform.position;
                    if (ignoreSkin)
                        return new Capsule(p, p, 0f);
                    else
                        return new Capsule(p, p, _skinWidth);
                }
                else
                {
                    var c = Capsule.FromCollider(_coll);
                    if (ignoreSkin) c.Radius -= _skinWidth;
                    return c;
                }
            }

            public void Move(Vector3 mv)
            {
                //_moves += () =>
                //{
                //    _rigidbody.MovePosition(_rigidbody.position + mv);

                //    _talliedMove += mv;
                //    _vel = _talliedMove / Time.deltaTime;
                //};

                _fullTalliedMove += mv;
                _talliedMove += mv;
                _vel = _talliedMove / Time.deltaTime;
            }

            public void AtypicalMove(Vector3 mv)
            {
                //_moves += () =>
                //{
                //    _rigidbody.MovePosition(_rigidbody.position + mv);
                //};

                _fullTalliedMove += mv;
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        //force = mass*distance/time^2
                        //distance = force * time^2 / mass
                        this.Move(f * Time.deltaTime * Time.deltaTime / this.Mass);
                        break;
                    case ForceMode.Acceleration:
                        //acceleration = distance/time^2
                        //distance = acceleration * time^2
                        this.Move(f * (Time.deltaTime * Time.deltaTime));
                        break;
                    case ForceMode.Impulse:
                        //impulse = mass*distance/time
                        //distance = impulse * time / mass
                        this.Move(f * Time.deltaTime / this.Mass);
                        break;
                    case ForceMode.VelocityChange:
                        //velocity = distance/time
                        //distance = velocity * time
                        this.Move(f * Time.deltaTime);
                        break;
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                this.AddForce(f, mode);
            }

            #endregion

            #region Event Handlers

            private void RegisterCollisionHooks()
            {
                if (!object.ReferenceEquals(_hooks, null)) return;

                _hooks = _owner.AddOrGetComponent<MovementControllerCollisionHook>();
                var callback = new OnCollisionCallback(this.OnCollision);
                _hooks.OnEnter -= callback;
                _hooks.OnStay -= callback;
                _hooks.OnExit -= callback;

                _hooks.OnEnter += callback;
                _hooks.OnStay += callback;
                _hooks.OnExit += callback;
            }

            private void UnregisterCollisionHooks()
            {
                if (_hooks != null)
                {
                    var callback = new OnCollisionCallback(this.OnCollision);
                    _hooks.OnEnter -= callback;
                    _hooks.OnStay -= callback;
                    _hooks.OnExit -= callback;
                    _hooks = null;
                }
            }

            private void OnCollision(object sender, Collision c)
            {
                if(_owner.HasHitListeners)
                {
                    //TODO - need to think of a better way to deal with more than 1 contact, currently just going to treat as the FIRST contact
                    if (c.contacts.Length >= 1)
                    {
                        var points = c.contacts;
                        using (var lst = TempCollection.GetList<MovementControllerHitContact>())
                        {
                            for (int i = 0; i < points.Length; i++)
                            {
                                lst.Add(new MovementControllerHitContact(points[i].normal, points[i].point));
                            }
                            var e = new MovementControllerHitEventArgs(_owner, c.collider, lst.ToArray());
                            _owner.SignalHit(e);
                        }

                    }
                }
                else
                {
                    this.UnregisterCollisionHooks();
                }
            }

            #endregion

            #region IIGnorableCollision Interface

            public void IgnoreCollision(Collider coll, bool ignore)
            {
                if (_coll != null && _coll.enabled) Physics.IgnoreCollision(_coll, coll, ignore);
            }

            public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
            {
                if (_coll != null && coll != null) coll.IgnoreCollision(_coll, ignore);
            }

            #endregion

            #region Disposable Interface

            private bool _isDisposed = false;

            void System.IDisposable.Dispose()
            {
                if (!_isDisposed)
                {
                    if(_hooks != null)
                    {
                        _hooks.OnEnter -= this.OnCollision;
                        _hooks.OnExit -= this.OnCollision;
                    }
                    
                    _owner = null;
                    _coll = null;
                    _hooks = null;
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
            [System.NonSerialized()]
            private MovementControllerCollisionHook _hooks;

            [SerializeField()]
            private float _stepOffset;
            [SerializeField()]
            private float _skinWidth;
            [SerializeField()]
            private bool _freeMovement;

            [System.NonSerialized()]
            private Vector3 _talliedMove;
            [System.NonSerialized()]
            private Vector3 _lastVel;


            //enabled cache
            [System.NonSerialized()]
            private bool _cacheIsKinematic;

            #endregion

            #region CONSTRUCTOR

            public DirectRigidBodyMover()
            {
            }

            #endregion

            #region Properties

            public bool FreeMovement
            {
                get { return _freeMovement; }
                set { _freeMovement = value; }
            }

            #endregion

            #region IGameObjectMover Interface

            public bool PrefersFixedUpdate { get { return true; } }

            public float Mass
            {
                get { return (_rigidbody != null) ? _rigidbody.mass : 0f; }
                set { if (_rigidbody != null) _rigidbody.mass = value; }
            }
            public float StepOffset { get { return _stepOffset; } set { _stepOffset = value; } }
            public float SkinWidth { get { return _skinWidth; } set { _skinWidth = value; } }
            public bool CollisionEnabled
            {
                get
                { 
                    return (_coll != null) ? _coll.enabled : false;
                } 
                set
                { 
                    if(_coll != null) _coll.enabled = value;
                }
            }
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

                var rb = _owner.GetComponent<Rigidbody>();
                if (rb == null) throw new System.InvalidOperationException("MovementController requires Rigidbody component to operate in Rigidbody mode.");
                //if (!_owner.HasComponent<SphereCollider>() && !_owner.HasComponent<CapsuleCollider>()) throw new System.InvalidOperationException("MovementController requires a supported Sphere or Capsule Collider component to operate in Rigidbody mode.");

                _rigidbody = rb;
                _coll = _owner.GetComponent<Collider>();
            }

            public void OnEnable()
            {
                _cacheIsKinematic = _rigidbody.isKinematic;
                _rigidbody.isKinematic = false;

                _lastVel = _rigidbody.velocity;

                if (_owner.HasHitListeners) this.RegisterCollisionHooks();
            }

            public void OnDisable()
            {
                _rigidbody.isKinematic = _cacheIsKinematic;

                this.UnregisterCollisionHooks();
            }

            public void OnHasHitListenersChanged()
            {
                if (_owner != null && _owner.HasHitListeners)
                    this.RegisterCollisionHooks();
                else
                    this.UnregisterCollisionHooks();
            }

            public void OnBeforeUpdate()
            {
                _talliedMove = Vector3.zero;
            }

            public void OnUpdateComplete()
            {
                if(_freeMovement)
                {
                    _lastVel = _rigidbody.velocity;
                }
                else
                {
                    if (!_owner.MoveCalled)
                    {
                        _rigidbody.velocity = _lastVel;
                    }
                    else
                    {
                        _lastVel = _rigidbody.velocity;
                    }
                }
            }

            public Capsule GetGeom(bool ignoreSkin)
            {
                if (_coll == null)
                {
                    var p = _owner.transform.position;
                    if (ignoreSkin)
                        return new Capsule(p, p, 0f);
                    else
                        return new Capsule(p, p, _skinWidth);
                }
                else
                {
                    var c = Capsule.FromCollider(_coll);
                    if (ignoreSkin) c.Radius -= _skinWidth;
                    return c;
                }
            }

            public void Move(Vector3 mv)
            {
                _talliedMove += mv;

                Vector3 v = _talliedMove / Time.deltaTime;
                //v -= _owner.LastVelocity; //remove the old velocity so it's setting to, not adding to
                //_rigidbody.AddForce(v, ForceMode.VelocityChange);
                _rigidbody.velocity = v;
            }

            public void AtypicalMove(Vector3 mv)
            {
                //_rigidbody.MovePosition(_rigidbody.position + mv);
                _owner.transform.position += mv; //for some reason moveposition doesn't work with moving platforms
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

            private void RegisterCollisionHooks()
            {
                if (!object.ReferenceEquals(_hooks, null)) return;

                _hooks = _owner.AddOrGetComponent<MovementControllerCollisionHook>();
                var callback = new OnCollisionCallback(this.OnCollision);
                _hooks.OnEnter -= callback;
                _hooks.OnStay -= callback;
                _hooks.OnExit -= callback;

                _hooks.OnEnter += callback;
                _hooks.OnStay += callback;
                _hooks.OnExit += callback;
            }

            private void UnregisterCollisionHooks()
            {
                if (_hooks != null)
                {
                    var callback = new OnCollisionCallback(this.OnCollision);
                    _hooks.OnEnter -= callback;
                    _hooks.OnStay -= callback;
                    _hooks.OnExit -= callback;
                    _hooks = null;
                }
            }

            private void OnCollision(object sender, Collision c)
            {
                if(_owner.HasHitListeners)
                {
                    //TODO - need to think of a better way to deal with more than 1 contact, currently just going to treat as the FIRST contact
                    if (c.contacts.Length >= 1)
                    {
                        var points = c.contacts;
                        using (var lst = TempCollection.GetList<MovementControllerHitContact>())
                        {
                            for (int i = 0; i < points.Length; i++)
                            {
                                lst.Add(new MovementControllerHitContact(points[i].normal, points[i].point));
                            }
                            var e = new MovementControllerHitEventArgs(_owner, c.collider, lst.ToArray());
                            _owner.SignalHit(e);
                        }

                    }
                }
                else
                {
                    this.UnregisterCollisionHooks();
                }
            }

            #endregion

            #region IIGnorableCollision Interface

            public void IgnoreCollision(Collider coll, bool ignore)
            {
                if (_coll != null && _coll.enabled) Physics.IgnoreCollision(_coll, coll, ignore);
            }

            public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
            {
                if (_coll != null && coll != null) coll.IgnoreCollision(_coll, ignore);
            }

            #endregion

            #region Disposable Interface

            private bool _isDisposed = false;

            void System.IDisposable.Dispose()
            {
                if (!_isDisposed)
                {
                    if (_hooks != null)
                    {
                        _hooks.OnEnter -= this.OnCollision;
                        _hooks.OnExit -= this.OnCollision;
                    }
                    
                    _owner = null;
                    _coll = null;
                    _hooks = null;
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

            public bool PrefersFixedUpdate { get { return true; } }

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
                    if (_armature != null)
                    {
                        for(int i = 0; i < _armature.JointCount; i++)
                        {
                            _armature.GetJoint(i).velocity = value;
                        }
                    }
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

            public void OnHasHitListenersChanged()
            {
                //do nothing
            }

            public void OnBeforeUpdate()
            {
            }

            public void OnUpdateComplete()
            {
                if (_armature.RootJoint != null && _armature.RootJoint.transform != _owner.transform)
                {
                    var pos = _armature.RootJoint.transform.position;
                    _owner.transform.position = pos;
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
                //if (_armature.RootJoint != null)
                //{
                //    var v = mv / GameTime.DeltaTime;
                //    _armature.RootJoint.AddForce(v, ForceMode.VelocityChange);


                //    var v = mv / GameTime.DeltaTime;
                //    var m = v.magnitude;
                //    var n = v / m;
                //    var d = Vector3.Dot(_armature.RootJoint.velocity, n);
                //    if (d > m)
                //    {
                //        _armature.RootJoint.velocity -= n * (d - m);
                //    }
                //}


                var v = mv / Time.deltaTime;
                var m = v.magnitude;
                var n = v / m;
                for (int i = 0; i < _armature.JointCount; i++)
                {
                    var j = _armature.GetJoint(i);
                    j.AddForce(v, ForceMode.VelocityChange);

                    var d = Vector3.Dot(j.velocity, n);
                    if (d > m)
                    {
                        j.velocity -= n * (d - m);
                    }
                }
            }

            public void AtypicalMove(Vector3 mv)
            {
                for (int i = 0; i < _armature.JointCount; i++)
                {
                    var j = _armature.GetJoint(i);
                    j.MovePosition(j.position + mv);
                }
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                for (int i = 0; i < _armature.JointCount; i++)
                {
                    _armature.GetJoint(i).AddForce(f, mode);
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                for (int i = 0; i < _armature.JointCount; i++)
                {
                    _armature.GetJoint(i).AddForceAtPosition(f, pos, mode);
                }
            }

            #endregion

            #region IIGnorableCollision Interface

            public void IgnoreCollision(Collider coll, bool ignore)
            {
                _armature.IgnoreCollision(coll, ignore);
            }

            public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
            {
                //this is only safe because ArmatureRig safely deals with the IIgnorableCollision and does call back into the passed in IIgnorableCollision
                if (_armature != null && coll != null) _armature.IgnoreCollision(coll, ignore);
            }

            #endregion

            #region IDisposable Interface

            public void Dispose()
            {
                _owner = null;
                _armature = null;
            }

            #endregion

        }

        [System.Serializable()]
        public class DumbMover : IGameObjectMover
        {

            #region Fields

            [System.NonSerialized()]
            private MovementController _controller;
            [System.NonSerialized()]
            private Vector3 _talliedMv;
            [System.NonSerialized()]
            private Vector3 _vel;

            #endregion


            #region IGameObjectMover Interface

            public bool PrefersFixedUpdate
            {
                get { return false; }
            }

            public float Mass
            {
                get
                {
                    return 0f;
                }
                set
                {
                }
            }

            public float StepOffset
            {
                get
                {
                    return 0f;
                }
                set
                {
                }
            }

            public float SkinWidth
            {
                get
                {
                    return 0f;
                }
                set
                {
                }
            }

            public bool CollisionEnabled
            {
                get
                {
                    return true;
                }
                set
                {
                }
            }

            public Vector3 Velocity
            {
                get
                {
                    return _vel;
                }
                set
                {
                    _vel = value;
                }
            }

            public void Reinit(MovementController controller)
            {
                _controller = controller;
                _vel = Vector2.zero;
            }

            public void OnEnable()
            {
                _vel = Vector2.zero;
            }

            public void OnDisable()
            {
                _vel = Vector2.zero;
            }

            public void OnHasHitListenersChanged()
            {
                //do nothing
            }

            public void OnBeforeUpdate()
            {
            }

            public void OnUpdateComplete()
            {
                _vel = _talliedMv / Time.deltaTime;
                _talliedMv = Vector3.zero;
            }

            public Capsule GetGeom(bool ignoreSkin)
            {
                return new Capsule(_controller.transform.position, _controller.transform.position, 0f);
            }

            public void Move(Vector3 mv)
            {
                _controller.transform.Translate(mv);
                _talliedMv += mv;
                _vel = _talliedMv / Time.deltaTime;
            }

            public void AtypicalMove(Vector3 mv)
            {
                _controller.transform.Translate(mv);
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        //force = mass*distance/time^2
                        //distance = force * time^2 / mass
                        this.Move(f * Time.deltaTime * Time.deltaTime / this.Mass);
                        break;
                    case ForceMode.Acceleration:
                        //acceleration = distance/time^2
                        //distance = acceleration * time^2
                        this.Move(f * (Time.deltaTime * Time.deltaTime));
                        break;
                    case ForceMode.Impulse:
                        //impulse = mass*distance/time
                        //distance = impulse * time / mass
                        this.Move(f * Time.deltaTime / this.Mass);
                        break;
                    case ForceMode.VelocityChange:
                        //velocity = distance/time
                        //distance = velocity * time
                        this.Move(f * Time.deltaTime);
                        break;
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                this.AddForce(f, mode);
            }

            public void IgnoreCollision(Collider coll, bool ignore)
            {
            }

            public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
            {
            }

            public void Dispose()
            {
                _controller = null;
            }

            #endregion

        }


        [System.Serializable()]
        public class PausedMover : IGameObjectMover
        {

            [System.NonSerialized()]
            private MovementController _owner;
            [System.NonSerialized()]
            private Vector3 _cachedVelocity;
            [System.NonSerialized()]
            private bool _cachedKinematic;

            public bool PrefersFixedUpdate
            {
                get { return false; }
            }

            public float Mass
            {
                get
                {
                    return 0;
                }
                set
                {
                }
            }

            public float StepOffset
            {
                get
                {
                    return 0f;
                }
                set
                {
                }
            }

            public float SkinWidth
            {
                get
                {
                    return 0f;
                }
                set
                {
                }
            }

            public bool CollisionEnabled
            {
                get
                {
                    return false;
                }
                set
                {
                }
            }

            public Vector3 Velocity
            {
                get
                {
                    return Vector3.zero;
                }
                set
                {
                }
            }

            public void Reinit(MovementController controller)
            {
                _owner = controller;
            }

            public void OnEnable()
            {
                var rb = _owner.GetComponent<Rigidbody>();
                if(rb != null)
                {
                    _cachedVelocity = rb.velocity;
                    _cachedKinematic = rb.isKinematic;
                    if(!rb.isKinematic) rb.velocity = Vector3.zero;
                    rb.isKinematic = true;
                }
            }

            public void OnDisable()
            {
                var rb = _owner.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = _cachedKinematic;
                    if (!rb.isKinematic) rb.velocity = _cachedVelocity;
                }
            }

            public void OnHasHitListenersChanged()
            {
                //do nothing
            }

            public void OnBeforeUpdate()
            {
            }

            public void OnUpdateComplete()
            {
            }

            public Capsule GetGeom(bool ignoreSkin)
            {
                var p = _owner.transform.position;
                return new Capsule(p, p, 0f);
            }

            public void Move(Vector3 mv)
            {
            }

            public void AtypicalMove(Vector3 mv)
            {
            }

            public void AddForce(Vector3 f, ForceMode mode)
            {
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
            }

            public void IgnoreCollision(Collider coll, bool ignore)
            {
            }

            public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
            {
            }

            public void Dispose()
            {
                this.OnDisable();
            }
        }






        private sealed class MovementControllerCollisionHook : MonoBehaviour
        {
            private OnCollisionCallback _onEnter;
            private OnCollisionCallback _onExit;
            private OnCollisionCallback _onStay;


            public event OnCollisionCallback OnEnter
            {
                add
                {
                    _onEnter += value;
                    if (!this.enabled) this.enabled = true;
                }
                remove
                {
                    _onEnter -= value;
                    if (_onEnter == null && _onExit == null) this.enabled = false;
                }
            }
            public event OnCollisionCallback OnExit
            {
                add
                {
                    _onExit += value;
                    if (!this.enabled) this.enabled = true;
                }
                remove
                {
                    _onExit -= value;
                    if (_onEnter == null && _onExit == null) this.enabled = false;
                }
            }

            public event OnCollisionCallback OnStay
            {
                add
                {
                    _onStay += value;
                    if (!this.enabled) this.enabled = true;
                }
                remove
                {
                    _onStay -= value;
                    if (_onStay == null) this.enabled = false;
                }
            }

            private void OnCollisionEnter(Collision collision)
            {
                if (!this.isActiveAndEnabled) return;

                if (_onEnter != null) _onEnter(this.gameObject, collision);
            }

            private void OnCollisionStay(Collision collision)
            {
                if (!this.isActiveAndEnabled) return;

                if (_onStay != null) _onStay(this.gameObject, collision);
            }

            private void OnCollisionExit(Collision collision)
            {
                if (!this.isActiveAndEnabled) return;

                if (_onExit != null) _onExit(this.gameObject, collision);
            }

            private void OnDestroy()
            {
                _onEnter = null;
                _onExit = null;
                _onStay = null;
            }
        }

        #endregion



        #region Event Arg Types

        public class MovementControllerHitEventArgs : System.EventArgs
        {

            #region Fields

            private MovementController _controller;
            private Collider _collider;
            //private Vector3 _normal;
            //private Vector3 _point;
            private MovementControllerHitContact[] _contacts;

            #endregion

            #region CONSTRUCTOR

            public MovementControllerHitEventArgs(MovementController controller, Collider otherCollider, params MovementControllerHitContact[] contacts) // Vector3 norm, Vector3 pnt)
            {
                _controller = controller;
                _collider = otherCollider;
                //_normal = norm;
                //_point = pnt;
                _contacts = contacts;
            }

            #endregion

            #region Properties

            public MovementController Controller { get { return _controller; } }
            public Collider Collider { get { return _collider; } }
            //public Vector3 Normal { get { return _normal; } }
            //public Vector3 Point { get { return _point; } }
            public MovementControllerHitContact[] Contacts { get { return _contacts; } }

            #endregion

        }

        public struct MovementControllerHitContact
        {
            public readonly Vector3 Normal;
            public readonly Vector3 Point;

            public MovementControllerHitContact(Vector3 n, Vector3 p)
            {
                this.Normal = n;
                this.Point = p;
            }
        }

        #endregion





        //EMULATE MOVER TYPE, STORED FOR POSSIBLE FUTURE IMP
        /// <summary>
        /// This attempts to emulate the way CharacterController operates by casting a capsule. Unlike CharacterController it attempts to be axis ambiguous, where as CC is locked to the +Y up axis.
        /// </summary>
        [System.Serializable()]
        public class EmulatedCharacterControllerBodyMover : IGameObjectMover
        {

            #region Fields

            [System.NonSerialized()]
            private MovementController _owner;

            [SerializeField()]
            private float _mass = 1.0f;
            [SerializeField()]
            private float _stepOffset;
            [SerializeField()]
            private float _skinWidth;
            [SerializeField()]
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

            public bool PrefersFixedUpdate { get { return false; } }

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

            public void OnHasHitListenersChanged()
            {

            }

            public void OnBeforeUpdate()
            {

            }

            public void OnUpdateComplete()
            {

            }

            public Capsule GetGeom(bool ignoreSkin)
            {
                Vector3 s = _owner.transform.TransformPoint(_geom.Start);
                Vector3 e = _owner.transform.TransformPoint(_geom.End);
                float r = VectorUtil.GetMaxScalar(_owner.transform.lossyScale) * ((ignoreSkin) ? _geom.Radius : _geom.Radius + _skinWidth);

                return new Capsule(s, e, r);
            }

            public void Move(Vector3 mv)
            {
                //TODO - need to make this actually work, but is NOT important at this current moment in time, don't forget to include the 'CollisionEnabled' flag

                var geom = this.GetGeom(true);
                int mask = PhysicsUtil.CalculateLayerMaskAgainst(_owner.gameObject.layer);
                RaycastHit hit;
                //bool bStayedGrounded = false;

                if (_onSurface)
                {
                    if (geom.Cast(-_currentSurfaceNormal, out hit, _skinWidth + 0.0001f, mask))
                    {
                        //update ground normal
                        RaycastHit simpleHit;
                        if (Physics.Raycast(hit.point + hit.normal, -hit.normal, out simpleHit, 1.01f, mask))
                        {
                            //bStayedGrounded = true;
                            _currentSurfaceNormal = simpleHit.normal;

                            var d = Vector3.Dot(mv, _currentSurfaceNormal);
                            if (d > 0f) mv -= _currentSurfaceNormal * d;

                            if (hit.distance < _skinWidth)
                            {
                                _owner.transform.position += hit.normal * (_skinWidth - hit.distance);
                            }
                        }
                        else
                        {
                            _onSurface = false;
                        }
                    }
                    else
                    {
                        _onSurface = false;
                    }

                }


                if (mv != Vector3.zero)
                {
                    Vector3 dir = mv.normalized;
                    float dist = mv.magnitude;
                    
                    if (geom.Cast(dir, out hit, dist, mask))
                    {
                        //if (!bStayedGrounded)
                        //{
                        //    _onSurface = true;
                        //    //update ground normal
                        //    RaycastHit simpleHit;
                        //    if (Physics.Raycast(hit.point + hit.normal, -hit.normal, out simpleHit, 1.01f, mask))
                        //    {
                        //        _currentSurfaceNormal = simpleHit.normal;
                        //    }
                        //}

                        //var d = hit.distance - _skinWidth;
                        //_owner.transform.position += dir * (d - _skinWidth * 0.5f);

                        var a = Vector3.Angle(hit.normal, dir) - 90f;
                        if(a < _stepOffset)
                        {
                            var d = Mathf.Max(hit.distance - _skinWidth * 0.5f, 0f);
                            var nmv = dir * d;
                            var adjust = Vector3.RotateTowards(hit.normal, dir, MathUtil.PI_2, 0f);
                            _owner.transform.position += nmv + adjust * (dist - d);
                        }
                        else
                        {
                            _owner.transform.position += dir * hit.distance;
                        }
                    }
                    else
                    {
                        _owner.transform.position += mv;
                    }
                }

                ////NOTE - temp code until above is fixed
                //_owner.transform.position += mv;

                //update velocity
                _vel = (_owner.transform.position - _owner.LastPosition) / Time.deltaTime;
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
                        this.Move(f * Time.deltaTime * Time.deltaTime / _mass);
                        break;
                    case ForceMode.Acceleration:
                        //acceleration = distance/time^2
                        //distance = acceleration * time^2
                        this.Move(f * (Time.deltaTime * Time.deltaTime));
                        break;
                    case ForceMode.Impulse:
                        //impulse = mass*distance/time
                        //distance = impulse * time / mass
                        this.Move(f * Time.deltaTime / _mass);
                        break;
                    case ForceMode.VelocityChange:
                        //velocity = distance/time
                        //distance = velocity * time
                        this.Move(f * Time.deltaTime);
                        break;
                }
            }

            public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
            {
                this.AddForce(f, mode);
            }

            #endregion

            #region IIGnorableCollision Interface

            public void IgnoreCollision(Collider coll, bool ignore)
            {
                //TODO - not sure how we'll be doing this ignore physics...
            }

            public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
            {
                //do nothing, we don't actually have any colliders
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
         
    }

}
