using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Movement
{
    public class MovementMotor2D : MovementMotor //, ISerializationCallbackReceiver
    {

        #region Fields

        [System.NonSerialized]
        private IPlanarSurface _surface;
        [SerializeField]
        [Tooltip("Leave blank to default to XY plane on z=0")]
        [TypeRestriction(typeof(IPlanarSurface))]
        [OnChangedInEditor("ValidateSurfaceData", OnlyAtRuntime =true)]
        private UnityEngine.Object _surfaceData;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.ValidateSurfaceData();
        }

        private void ValidateSurfaceData()
        {
            _surface = _surfaceData as IPlanarSurface;
            if (_surface == null)
                _surface = XYPlanarSurface.Default;
        }

        #endregion

        #region Properties

        public IPlanarSurface Surface
        {
            get { return _surface; }
            set
            {
                _surface = value ?? XYPlanarSurface.Default;
                _surfaceData = _surface as UnityEngine.Object;
            }
        }

        public Vector2 Position
        {
            get { return _surface.ProjectPosition2D(this.Controller.transform.position); }
        }

        public Vector2 Velocity
        {
            get { return _surface.ProjectVectorTo2D(this.Controller.transform.position, this.Controller.Velocity); }
        }

        #endregion

        #region Methods

        public void Move(Vector2 mv)
        {
            this.Controller.Move(_surface.ProjectVectorTo3D(this.Controller.transform.position, mv));
        }

        #endregion
        
    }
}
