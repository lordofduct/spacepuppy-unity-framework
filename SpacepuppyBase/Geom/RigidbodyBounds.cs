using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Geom
{
    public class RigidbodyBounds : IEnumerable<Collider>
    {

        #region Fields

        private Rigidbody _rb;
        private bool _includeTriggers;
        private BoundingSphereAlgorithm _algorithm;

        private Collider[] _colliders;
        private Sphere _sphereBounds;
        private Bounds _rectBounds;

        #endregion

        #region CONSTRUCTOR

        public RigidbodyBounds(Rigidbody body)
        {
            _rb = body;
            _includeTriggers = false;
            _algorithm = GeomUtil.DefaultBoundingSphereAlgorithm;
        }

        public RigidbodyBounds(Rigidbody body, bool includeTriggers)
        {
            _rb = body;
            _includeTriggers = includeTriggers;
            _algorithm = GeomUtil.DefaultBoundingSphereAlgorithm;
        }

        public RigidbodyBounds(Rigidbody body, BoundingSphereAlgorithm algorithm)
        {
            _rb = body;
            _includeTriggers = false;
            _algorithm = algorithm;
        }

        public RigidbodyBounds(Rigidbody body, bool includeTriggers, BoundingSphereAlgorithm algorithm)
        {
            _rb = body;
            _includeTriggers = includeTriggers;
            _algorithm = algorithm;
        }

        #endregion

        #region Properties

        public Rigidbody Rigidbody
        {
            get { return _rb; }
            set
            {
                if (_rb == value) return;
                _rb = value;
                _colliders = null;
            }
        }

        public bool IncludeTriggers
        {
            get { return _includeTriggers; }
            set
            {
                if (_includeTriggers == value) return;
                _includeTriggers = value;
                _colliders = null;
            }
        }

        public BoundingSphereAlgorithm BoundingSphereAlgorithm
        {
            get { return _algorithm; }
            set
            {
                if (_algorithm == value) return;
                _algorithm = value;
                _colliders = null;
            }
        }

        public Sphere BoundingSphere
        {
            get
            {
                if (_colliders == null) this.Recalculate();
                return _sphereBounds;
            }
        }

        public Bounds Bounds
        {
            get
            {
                if (_colliders == null) this.Recalculate();
                return _rectBounds;
            }
        }

        #endregion

        #region Methods

        public void Recalculate()
        {
            if(_rb == null)
            {
                _colliders = new Collider[] { };
                _sphereBounds = new Sphere();
                _rectBounds = new Bounds();
                return;
            }

            _colliders = (from c in _rb.GetComponentsInChildren<Collider>(false)
                          where (_includeTriggers || !c.isTrigger) //TODO allow excluding colliders based on a list
                          select c).ToArray();

            if (_colliders.Length > 0)
            {
                _sphereBounds = Sphere.FromCollider(_colliders[0], _algorithm, false);
                _rectBounds = _colliders[0].bounds;
                for (int i = 1; i < _colliders.Length; i++)
                {
                    _sphereBounds.Encapsulate(Sphere.FromCollider(_colliders[i], _algorithm, false));
                    _rectBounds.Encapsulate(_colliders[i].bounds);
                }
            }
            else
            {
                _sphereBounds = new Sphere();
                _rectBounds = new Bounds();
            }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<Collider> GetEnumerator()
        {
            if (_colliders == null) this.Recalculate();
            return (_colliders as IEnumerable<Collider>).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }
}
