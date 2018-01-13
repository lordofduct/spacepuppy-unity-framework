using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras.LegacyRender
{

    /*
    [RequireComponent(typeof(Camera))]
    internal sealed class GlobalPostProcessorHook : SPComponent
    {

        #region Fields

        private ICamera _camera;

        #endregion

        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if (_camera.IsNullOrDestroyed())
            {
                ObjUtil.SmartDestroy(this);
            }
        }
        
        #endregion

        #region Methods

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(_camera == null)
            {
                Graphics.Blit(source, destination);
                ObjUtil.SmartDestroy(this);
                return;
            }

            var manager = Services.Get<IPostProcessingManager>();
            if(manager == null)
            {
                Graphics.Blit(source, destination);
                this.enabled = false;
                return;
            }

            if(!manager.ApplyGlobalPostProcessing(_camera, source, destination))
            {
                manager.SetDirty();
                Graphics.Blit(source, destination);
                this.enabled = false;
                return;
            }
        }
        
        #endregion

        #region Static Interface
        
        public static void EnableHook(ICamera camera)
        {
            if (camera == null) throw new System.ArgumentNullException("camera");

            if(camera is IMultiCamera)
            {
                foreach(var c in (camera as IMultiCamera))
                {
                    var hook = c.AddOrGetComponent<GlobalPostProcessorHook>();
                    hook._camera = camera;
                    hook.enabled = true;
                }
            }
            else
            {
                var hook = camera.camera.AddOrGetComponent<GlobalPostProcessorHook>();
                hook._camera = camera;
                hook.enabled = true;
            }
        }

        public static void DisableHook(ICamera camera)
        {
            if (camera == null) throw new System.ArgumentNullException("camera");

            if (camera is IMultiCamera)
            {
                foreach (var c in (camera as IMultiCamera))
                {
                    var hook = c.GetComponent<GlobalPostProcessorHook>();
                    if (hook != null) hook.enabled = false;
                }
            }
            else
            {
                var hook = camera.camera.GetComponent<GlobalPostProcessorHook>();
                if (hook != null) hook.enabled = false;
            }
        }

        #endregion

    }
    */

}
