using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{

    public class ColliderOverlapResolver : SPComponent
    {

        #region Fields

        private List<ColliderOverlapResult> _currentTests = new List<ColliderOverlapResult>();
        private System.Func<Collider, bool> _validColliderTest;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public System.Func<Collider, bool> ValidColliderTest
        {
            get { return _validColliderTest; }
            set { _validColliderTest = value; }
        }

        #endregion

        #region Methods

        public ColliderOverlapResult GatherColliders(float duration = 0f)
        {
            if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
            if (!this.enabled) this.enabled = true;

            var result = new ColliderOverlapResult();
            this.StartCoroutine(this.WaitToGatherColliders(duration, result));
            return result;
        }

        private System.Collections.IEnumerator WaitToGatherColliders(float duration, ColliderOverlapResult result)
        {
            _currentTests.Add(result);

            var rb = this.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.WakeUp();
            }

            yield return new WaitForSeconds(duration);

            _currentTests.Remove(result);
            if (_currentTests.Count == 0)
            {
                this.enabled = false;
            }

            result.Internal_Set();
        }

        #endregion

        #region OnTriggerEnter Message

        private void OnTriggerEnter(Collider c)
        {
            if (_currentTests.Count == 0) return;
            if (_validColliderTest != null && !_validColliderTest(c)) return;

            foreach(var r in _currentTests)
            {
                r.Internal_AddCollider(c);
            }
        }

        private void OnTriggerStay(Collider c)
        {
            if (_currentTests.Count == 0) return;
            if (_validColliderTest != null && !_validColliderTest(c)) return;

            foreach (var r in _currentTests)
            {
                r.Internal_AddCollider(c);
            }
        }

        #endregion


        #region Factory Methods

        public static ColliderOverlapResult GatherColliders(GameObject go, float duration = 0f)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            var c = go.AddOrGetComponent<ColliderOverlapResolver>();
            return c.GatherColliders(duration);
        }

        #endregion

    }

}
