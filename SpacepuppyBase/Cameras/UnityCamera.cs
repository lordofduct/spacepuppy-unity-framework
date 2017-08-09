using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Cameras
{

    [RequireComponentInEntity(typeof(Camera))]
    [DisallowMultipleComponent()]
    public class UnityCamera : SPComponent, ICamera
    {

        #region Fields

        [SerializeField]
        private CameraCategory _type;

        [SerializeField]
        [DefaultFromSelf(UseEntity = true)]
        private Camera _camera;
        
        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            //_camera = this.GetComponent<Camera>();
            //if (_camera == null)
            //{
            //    ObjUtil.SmartDestroy(this);
            //}
            //else
            //{
            //    CameraManager.Register(this);
            //}


            if(_camera != null)
            {
                CameraPool.Register(this);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            CameraPool.Unregister(this);
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
            get { return _camera; }
        }
        Camera ICamera.camera
        {
            get { return _camera; }
        }

        public bool IsAlive { get { return _camera != null; } }

        public bool Contains(Camera cam)
        {
            return object.ReferenceEquals(_camera, cam);
        }

        #endregion
        
    }
}
