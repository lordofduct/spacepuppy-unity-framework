using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Cameras
{
    public abstract class CameraController : SPNotifyingComponent, ICamera
    {

        #region Fields
        
        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            CameraPool.Register(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            CameraPool.Unregister(this);
        }

        #endregion

        #region ICamera Interface

        public abstract CameraCategory Category
        {
            get;
            set;
        }

        public new abstract Camera camera { get; }

        Camera ICamera.camera
        {
            get { return this.camera; }
        }
        
        public virtual bool IsAlive { get { return this != null && this.gameObject != null; } }

        bool ICamera.IsAlive { get { return this.IsAlive; } }

        public abstract bool Contains(Camera cam);

        bool ICamera.Contains(Camera cam)
        {
            return this.Contains(cam);
        }

        #endregion

    }
}
