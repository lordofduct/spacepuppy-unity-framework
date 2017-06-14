using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{

    [RequireComponentInEntity(typeof(Camera))]
    [DisallowMultipleComponent()]
    public class UnityCamera : SPComponent, ICamera
    {

        #region Fields

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

        #region Properties

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
