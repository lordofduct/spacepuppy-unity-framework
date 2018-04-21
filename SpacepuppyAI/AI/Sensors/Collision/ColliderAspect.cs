using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors.Collision
{
    public class ColliderAspect : AbstractAspect
    {

        #region Fields

        private Collider _collider;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _collider = this.GetComponent<Collider>();
        }

        #endregion

        #region Properties

        public Collider Collider
        {
            get { return _collider; }
        }

        #endregion

        #region IAspect Interface

        public override bool IsActive
        {
            get { return _collider.IsActiveAndEnabled(); }
        }

        public override float Precedence
        {
            get { return 0f; }
            set { }
        }
        
        #endregion
        
        #region Static Interface

        public static ColliderAspect GetAspect(Collider coll)
        {
            if (coll == null) return null;

            return coll.AddOrGetComponent<ColliderAspect>();
        }

        #endregion

    }
}
