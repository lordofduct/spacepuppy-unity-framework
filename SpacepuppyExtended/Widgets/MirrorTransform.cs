using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Widgets
{
    public class MirrorTransform : SPComponent
    {

        #region Fields

        [Tooltip("GameObject we should mirror.")]
        public Transform TargetTransform;

        public bool MirrorPosition = true;
        public bool MirrorRotation = true;
        public bool MirrorScale = false;

        #endregion

        #region Game Messages

        void LateUpdate()
        {
            if (TargetTransform == null) return;

            var t = Trans.GetGlobal(this.TargetTransform);
            if (!MirrorPosition) t.Position = this.transform.position;
            if (!MirrorRotation) t.Rotation = this.transform.rotation;
            t.SetToGlobal(this.transform, MirrorScale);
        }

        #endregion

    }
}
