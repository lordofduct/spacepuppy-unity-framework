using UnityEngine;

namespace com.spacepuppy.Movement
{

    public interface IMotorConstraint : IComponent
    {

        bool AutoConstrain { get; set; }

        void ConstrainSelf();

    }

    public interface IMotor2DSurfaceConstraint : IMotorConstraint
    {

        Vector3 SurfaceNormal { get; }

        Vector2 ProjectVectorTo2D(Vector3 v);
        Vector3 ProjectVectorTo3D(Vector2 v);
        Vector2 ProjectPosition2D(Vector3 v);
        Vector3 ProjectPosition3D(Vector2 v);

    }

    public abstract class AbstractMotor2DSurfaceConstraint : SPComponent, IMotor2DSurfaceConstraint
    {

        [SerializeField()]
        private bool _autoConstrain = true;
        [DefaultFromSelf()]
        [SerializeField()]
        private MovementMotor _motor;

        protected override void Awake()
        {
            base.Awake();

            if (_motor == null)
            {
                _motor = this.GetComponent<MovementMotor>();
            }
        }

        public MovementMotor Motor { get { return _motor; } }

        public bool AutoConstrain
        {
            get { return _autoConstrain; }
            set { _autoConstrain = value; }
        }

        public abstract Vector3 SurfaceNormal { get; }
        
        public abstract void ConstrainSelf();

        public abstract Vector2 ProjectVectorTo2D(Vector3 v);

        public abstract Vector3 ProjectVectorTo3D(Vector2 v);

        public abstract Vector2 ProjectPosition2D(Vector3 v);

        public abstract Vector3 ProjectPosition3D(Vector2 v);

        #region Game Messages

        void Update()
        {
            if (!AutoConstrain) return;

            this.ConstrainSelf();
        }

        #endregion

    }

}