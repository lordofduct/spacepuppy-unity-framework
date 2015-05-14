using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public interface IForceReceiver : IGameObjectSource
    {

        void Move(Vector3 mv);
        void AddForce(Vector3 force, ForceMode mode);
        void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode);
        void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode);

    }

    public class RigidbodyForceReceiverWrapper : IForceReceiver
    {

        private Rigidbody _body;

        public GameObject gameObject
        {
            get { return _body.gameObject; }
        }

        public Transform transform
        {
            get { return _body.transform; }
        }

        public RigidbodyForceReceiverWrapper(Rigidbody body)
        {
            if (body == null) throw new System.ArgumentNullException("body");
            _body = body;
        }

        public void Move(Vector3 mv)
        {
            _body.MovePosition(_body.position + mv);
        }

        public void AddForce(Vector3 force, ForceMode mode)
        {
            _body.AddForce(force, mode);
        }

        public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode)
        {
            _body.AddForceAtPosition(force, position, mode);
        }

        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode)
        {
            _body.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, mode);
        }

    }

}
