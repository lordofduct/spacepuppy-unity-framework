using UnityEngine;

namespace com.spacepuppy.Geom
{
    public struct RaycastInfo
    {
        private Vector3 _origin;
        private Vector3 _dir;
        private float _dist;

        #region CONSTRUCTOR

        public RaycastInfo(Ray r, float dist)
        {
            _origin = r.origin;
            _dir = r.direction;
            _dist = dist;
        }

        public RaycastInfo(Vector3 origin, Vector3 dir, float dist)
        {
            _origin = origin;
            _dir = dir.normalized;
            _dist = dist;
        }

        #endregion

        public Ray Ray
        {
            get { return new Ray(_origin, _dir); }
            set
            {
                _origin = value.origin;
                _dir = value.direction.normalized;
            }
        }

        public Vector3 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public Vector3 Direction
        {
            get { return _dir; }
            set { _dir = value.normalized; }
        }

        public float Distance
        {
            get { return _dist; }
            set { _dist = value; }
        }
    }
}
