using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public interface IIgnorableCollision
    {

        void IgnoreCollision(Collider coll, bool ignore);
        void IgnoreCollision(IIgnorableCollision coll, bool ignore);

    }

    [RequireComponent(typeof(Rigidbody))]
    public class IgnorableRigidbody : SPComponent, IIgnorableCollision
    {

        #region Fields

        [System.NonSerialized()]
        private Collider[] _colliders; //consider making this configurable

        [System.NonSerialized()]
        private Rigidbody _rigidbody;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = this.GetComponent<Rigidbody>();
            _colliders = this.GetComponentsInChildren<Collider>();
        }

        #endregion

        #region IIgnorableCollision Interface

        public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            if (coll == null) return;
            if (_colliders == null) return;

            for(int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null) coll.IgnoreCollision(_colliders[i], ignore);
            }
        }

        public void IgnoreCollision(Collider coll, bool ignore)
        {
            if (coll == null) return;
            if (_colliders == null) return;

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null) Physics.IgnoreCollision(_colliders[i], coll, ignore);
            }
        }

        #endregion

        #region Static Utils

        public static IgnorableRigidbody GetIgnorableCollision(Rigidbody rb)
        {
            if (rb == null) return null;

            return rb.AddOrGetComponent<IgnorableRigidbody>();
        }

        #endregion

    }

    public class IgnorableCollider : SPComponent, IIgnorableCollision
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

        public Collider Collider { get { return _collider; } }

        #endregion
        
        #region IIgnorableCollision Interface

        public void IgnoreCollision(Collider coll, bool ignore)
        {
            if (_collider == null || coll == null || _collider == coll) return;

            Physics.IgnoreCollision(_collider, coll, ignore);
        }

        public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            if (_collider == null || coll == null || object.ReferenceEquals(this, coll)) return;

            coll.IgnoreCollision(_collider, ignore);
        }

        #endregion

        #region Static Interface
        
        public static IgnorableCollider GetIgnorableCollision(Collider coll)
        {
            if (coll == null) return null;

            return coll.AddOrGetComponent<IgnorableCollider>();
        }

        #endregion

    }

}
