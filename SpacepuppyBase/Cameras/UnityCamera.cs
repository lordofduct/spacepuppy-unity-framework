using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{

    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent()]
    public sealed class UnityCamera : MonoBehaviour, ICamera
    {

        #region Fields

        private Camera _camera;

        #endregion

        #region CONSTRUCTOR

        void Awake()
        {
            _camera = this.GetComponent<Camera>();
            if (_camera == null)
            {
                ObjUtil.SmartDestroy(this);
            }
            else
            {
                CameraManager.Register(this);
            }
        }

        void OnDestroy()
        {
            CameraManager.UnRegister(this);
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
