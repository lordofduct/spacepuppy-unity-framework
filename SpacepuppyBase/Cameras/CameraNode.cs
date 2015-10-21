using UnityEngine;

using com.spacepuppy.Geom;

namespace com.spacepuppy.Cameras
{
    public class CameraNode : SPComponent
    {

        #region Fields

        [SerializeField()]
        [VariantCollection.AsPropertyList(typeof(CameraToken))]
        private VariantCollection _cameraSettings = new VariantCollection();

        #endregion

        #region Properties

        public Vector3 LocalPosition {  get { return this.transform.localPosition; } }

        public Vector3 Position { get { return this.transform.position; } }

        public Quaternion LocalRotation { get { return this.transform.localRotation; } }

        public Quaternion Rotation { get { return this.transform.rotation; } }

        public Trans LocalTrans { get { return new Trans(this.transform.localPosition, this.transform.localRotation); } }

        public Trans Trans { get { return new Trans(this.transform.position, this.transform.rotation); } }

        public VariantCollection CameraSettings
        {
            get { return _cameraSettings; }
        }

        #endregion

        #region Methods

        public void ApplyToCamera(Camera camera)
        {
            camera.transform.position = this.transform.position;
            camera.transform.rotation = this.transform.rotation;
            _cameraSettings.DynamicallyCopyTo(camera);
        }

        public void TweenTo(com.spacepuppy.Tween.TweenHash hash, com.spacepuppy.Tween.Ease ease, float dur)
        {
            hash.To("*GlobalTrans", ease, this.Trans, dur);
            _cameraSettings.DynamicallyTweenTo(hash, ease, dur);
        }

        #endregion

    }
}
