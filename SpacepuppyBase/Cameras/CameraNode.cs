using UnityEngine;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Cameras
{
    public class CameraNode : SPComponent, IStateModifier
    {
        
        #region Fields

        [SerializeField()]
        [VariantCollection.AsPropertyList(typeof(CameraToken))]
        private VariantCollection _cameraSettings = new VariantCollection();

        #endregion

        #region Properties
        
        public VariantCollection CameraSettings
        {
            get { return _cameraSettings; }
        }

        #endregion
        
        #region IModifier Interface

        void IStateModifier.CopyTo(object targ)
        {
            _cameraSettings.CopyTo(targ);
        }

        void IStateModifier.LerpTo(object targ, float t)
        {
            _cameraSettings.LerpTo(targ, t);
        }

        void IStateModifier.Modify(object targ)
        {
            var cam = ComponentUtil.GetComponentFromSource<Camera>(targ);
            if (cam == null) return;

            _cameraSettings.CopyTo(cam);
        }

        void IStateModifier.ModifyWith(object targ, object source)
        {
            var cam = ComponentUtil.GetComponentFromSource<Camera>(targ);
            if (cam == null) return;

            DynamicUtil.CopyState(cam, source);
        }

        #endregion

    }
}
