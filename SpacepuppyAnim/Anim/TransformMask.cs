using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{

    [System.Obsolete("Use SPAnimationMask Intead")]
    [System.Serializable()]
    public struct TransformMask
    {

        #region Fields

        [SerializeField()]
        public Transform Transform;
        [SerializeField()]
        public bool Recursive;

        #endregion

        #region CONSTRUCTOR

        public TransformMask(Transform t, bool recursive)
        {
            this.Transform = t;
            this.Recursive = recursive;
        }

        #endregion

    }
}
