using UnityEngine;

namespace com.spacepuppy.Cameras
{

    /// <summary>
    /// Stores the state of a Camera.
    /// 
    /// This type is a class rather than a struct, you should create an object for the token and recycle as needed.
    /// </summary>
    [System.Serializable()]
    public class CameraToken
    {

        #region Fields

        public CameraClearFlags clearFlags;
        public Color backgroundColor;
        public LayerMask cullingMask;
        public bool orthographic;
        public float orthographicSize;
        public float fieldOfView;
        public float nearClipPlane;
        public float farClipPlane;
        public Rect rect;
        public float depth;
        public RenderingPath renderingPath;
        public RenderTexture targetTexture;
        public bool useOcclusionCulling;
        public bool hdr;

        #endregion

        #region CONSTRUCTOR

        public CameraToken()
        {

        }

        public CameraToken(Camera camera)
        {
            this.CopyFrom(camera);
        }

        #endregion
        
        #region Methods

        public void CopyTo(Camera camera)
        {
            camera.clearFlags = this.clearFlags;
            camera.backgroundColor = this.backgroundColor;
            camera.cullingMask = this.cullingMask;
            camera.orthographic = this.orthographic;
            camera.orthographicSize = this.orthographicSize;
            camera.fieldOfView = this.fieldOfView;
            camera.nearClipPlane = this.nearClipPlane;
            camera.farClipPlane = this.farClipPlane;
            camera.rect = this.rect;
            camera.depth = this.depth;
            camera.renderingPath = this.renderingPath;
            camera.targetTexture = this.targetTexture;
            camera.useOcclusionCulling = this.useOcclusionCulling;
            camera.hdr = this.hdr;
        }

        public void CopyFrom(Camera camera)
        {
            this.clearFlags = camera.clearFlags;
            this.backgroundColor = camera.backgroundColor;
            this.cullingMask = camera.cullingMask;
            this.orthographic = camera.orthographic;
            this.orthographicSize = camera.orthographicSize;
            this.fieldOfView = camera.fieldOfView;
            this.nearClipPlane = camera.nearClipPlane;
            this.farClipPlane = camera.farClipPlane;
            this.rect = camera.rect;
            this.depth = camera.depth;
            this.renderingPath = camera.renderingPath;
            this.targetTexture = camera.targetTexture;
            this.useOcclusionCulling = camera.useOcclusionCulling;
            this.hdr = camera.hdr;
        }

        #endregion
        
    }
}
