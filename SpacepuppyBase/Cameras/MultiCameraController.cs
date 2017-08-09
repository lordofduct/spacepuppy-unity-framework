#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{

    /// <summary>
    /// A state machine of multiple cameras that are children of this. Only one is ever enabled, allowing multiple cameras to act as one. 
    /// It's easiest if you move just the GameObject this is attached to, rather than the child cameras individually. Zeroing out the local 
    /// position of the various cameras inside of this.
    /// </summary>
    public class MultiCameraController : SPComponent, IMultiCamera
    {

        #region Fields

        private TypedStateMachine<Camera> _stateMachine;

        [SerializeField]
        private CameraCategory _type;

        [SerializeField()]
        private Camera _defaultCamera;

        [SerializeField()]
        private Camera[] _ignoreCameras;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            CameraPool.Register(this);

            _stateMachine = TypedStateMachine<Camera>.CreateFromParentComponentSource(this.gameObject, false, false);
            _stateMachine.StateChanged += this.OnStateChanged;
        }

        protected override void Start()
        {
            base.Start();

            if(_stateMachine.Current == null)
            {
                _stateMachine.ChangeState(this.DefaultCamera);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            CameraPool.Unregister(this);
        }

        #endregion

        #region Properties

        public Camera CurrentCamera
        {
            get { return _stateMachine.Current; }
        }

        public IStateMachine<Camera> States
        {
            get { return _stateMachine; }
        }

        public Camera DefaultCamera
        {
            get { return _defaultCamera; }
            set
            {
                _defaultCamera = value;
            }
        }

        #endregion

        #region Handlers

        private void OnStateChanged(object sender, StateChangedEventArgs<Camera> e)
        {
            if (_ignoreCameras != null && _ignoreCameras.Length > 0)
            {
                foreach (var c in _stateMachine)
                {
                    if (c != e.ToState && _ignoreCameras.IndexOf(c) < 0) c.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (var c in _stateMachine)
                {
                    if (c != e.ToState) c.gameObject.SetActive(false);
                }
            }

            if (e.ToState != null)
            {
                if (!e.ToState.gameObject.activeSelf) e.ToState.gameObject.SetActive(true);
                //if (this.HasTag(SPConstants.TAG_MAINCAMERA))
                //{
                //    if(Camera.main == null || _stateMachine.Contains(Camera.main))
                //    {
                        
                //    }
                //}
            }
        }

        #endregion

        #region ICamera Interface

        public CameraCategory Category
        {
            get { return _type; }
            set { _type = value; }
        }
        
        public new Camera camera
        {
            get { return _stateMachine.Current; }
        }
        Camera ICamera.camera
        {
            get { return _stateMachine.Current; }
        }

        public bool IsAlive { get { return _stateMachine.Current != null; } }

        public bool Contains(Camera cam)
        {
            return _stateMachine.Contains(cam);
        }

        #endregion

        #region IEnumerable Interface
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _stateMachine.GetEnumerator();
        }

        public IEnumerator<Camera> GetEnumerator()
        {
            return _stateMachine.GetEnumerator();
        }

        #endregion
        

    }
}
