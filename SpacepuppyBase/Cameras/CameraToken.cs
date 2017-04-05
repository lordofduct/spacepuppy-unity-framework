using UnityEngine;

namespace com.spacepuppy.Cameras
{

    /// <summary>
    /// Stores the state of a Camera.
    /// 
    /// This type is a class rather than a struct, you should create an object for the token and recycle as needed.
    /// </summary>
    [System.Serializable()]
    public struct CameraToken
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
        
        #region Methods
        
        public void Apply(Camera camera)
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

        public static CameraToken FromCamera(Camera camera)
        {
            return new CameraToken() {
                clearFlags = camera.clearFlags,
                backgroundColor = camera.backgroundColor,
                cullingMask = camera.cullingMask,
                orthographic = camera.orthographic,
                orthographicSize = camera.orthographicSize,
                fieldOfView = camera.fieldOfView,
                nearClipPlane = camera.nearClipPlane,
                farClipPlane = camera.farClipPlane,
                rect = camera.rect,
                depth = camera.depth,
                renderingPath = camera.renderingPath,
                targetTexture = camera.targetTexture,
                useOcclusionCulling = camera.useOcclusionCulling,
                hdr = camera.hdr
            };
        }

        #endregion
        
    }
}
