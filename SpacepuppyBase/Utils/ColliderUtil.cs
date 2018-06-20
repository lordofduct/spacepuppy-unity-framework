using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{

    public static class ColliderUtil
    {

        public delegate void TriggerEventCallback(Collider sender, Collider other);
        public delegate void CollisionEventCallback(Collider sender, Collision collision);

        #region Trigger Event Handlers

        public static void AddOnTriggerEnterCallback(this Rigidbody rb, TriggerEventCallback callback)
        {
            if (rb == null || callback == null) return;

            var handle = rb.AddOrGetComponent<OnTriggerEnterCallbackHandle>();
            handle.TriggerEntered += callback;
        }

        public static void AddOnTriggerExitCallback(this Rigidbody rb, TriggerEventCallback callback)
        {
            if (rb == null || callback == null) return;

            var handle = rb.AddOrGetComponent<OnTriggerExitCallbackHandle>();
            handle.TriggerExited += callback;
        }

        public static void AddOnTriggerEnterCallback(this Collider collider, TriggerEventCallback callback)
        {
            if (collider == null || callback == null) return;

            var handle = collider.AddOrGetComponent<OnTriggerEnterCallbackHandle>();
            handle.TriggerEntered += callback;
        }

        public static void AddOnTriggerExitCallback(this Collider collider, TriggerEventCallback callback)
        {
            if (collider == null || callback == null) return;

            var handle = collider.AddOrGetComponent<OnTriggerExitCallbackHandle>();
            handle.TriggerExited += callback;
        }

        public static void RemoveOnTriggerEnterCallback(this Rigidbody rb, TriggerEventCallback callback)
        {
            if (rb == null || callback == null) return;

            var handle = rb.GetComponent<OnTriggerEnterCallbackHandle>();
            if (handle != null)
            {
                handle.TriggerEntered -= callback;
            }
        }

        public static void RemoveOnTriggerExitCallback(this Rigidbody rb, TriggerEventCallback callback)
        {
            if (rb == null || callback == null) return;

            var handle = rb.GetComponent<OnTriggerExitCallbackHandle>();
            if (handle != null)
            {
                handle.TriggerExited -= callback;
            }
        }

        public static void RemoveOnTriggerEnterCallback(this Collider collider, TriggerEventCallback callback)
        {
            if (collider == null || callback == null) return;

            var handle = collider.GetComponent<OnTriggerEnterCallbackHandle>();
            if(handle != null)
            {
                handle.TriggerEntered -= callback;
            }
        }

        public static void RemoveOnTriggerExitCallback(this Collider collider, TriggerEventCallback callback)
        {
            if (collider == null || callback == null) return;

            var handle = collider.GetComponent<OnTriggerExitCallbackHandle>();
            if (handle != null)
            {
                handle.TriggerExited -= callback;
            }
        }

        #endregion

        #region Special Types

        private sealed class OnTriggerEnterCallbackHandle : MonoBehaviour
        {

            public event TriggerEventCallback TriggerEntered
            {
                add
                {
                    _triggerEntered += value;
                    this.enabled = (_triggerEntered != null);
                }
                remove
                {
                    _triggerEntered -= value;
                    this.enabled = (_triggerEntered != null);
                }
            }

            private TriggerEventCallback _triggerEntered;
            private Collider _collider;
            
            void Awake()
            {
                _collider = this.GetComponent<Collider>();
            }

            private void OnTriggerEnter(Collider other)
            {
                var d = _triggerEntered;
                if (d != null) d(_collider, other);
            }

        }

        private sealed class OnTriggerExitCallbackHandle : MonoBehaviour
        {

            public event TriggerEventCallback TriggerExited
            {
                add
                {
                    _triggerExited += value;
                    this.enabled = (_triggerExited != null);
                }
                remove
                {
                    _triggerExited -= value;
                    this.enabled = (_triggerExited != null);
                }
            }

            private TriggerEventCallback _triggerExited;
            private Collider _collider;

            void Awake()
            {
                _collider = this.GetComponent<Collider>();
            }

            private void OnTriggerEnter(Collider other)
            {
                var d = _triggerExited;
                if (d != null) d(_collider, other);
            }

        }

        #endregion

    }

}
