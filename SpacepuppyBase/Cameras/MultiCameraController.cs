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
    public class MultiCameraController : CameraController
    {

        #region Fields

        private ITypedStateMachine<Camera> _stateMachine;

        [SerializeField()]
        private Camera _defaultCamera;

        [SerializeField()]
        private Camera[] _ignoreCameras;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

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

        public override Camera camera
        {
            get { return _stateMachine.Current; }
        }

        public override bool Contains(Camera cam)
        {
            return _stateMachine.Contains(cam);
        }

        #endregion

    }
}
