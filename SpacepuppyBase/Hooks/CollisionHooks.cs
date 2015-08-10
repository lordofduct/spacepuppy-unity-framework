using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace com.spacepuppy.Hooks
{

    public sealed class CollisionEnterExitHook : MonoBehaviour
    {

        private OnCollisionCallback _onEnter;
        private OnCollisionCallback _onExit;

        public event OnCollisionCallback OnEnter
        {
            add
            {
                _onEnter += value;
                if (!this.enabled) this.enabled = true;
            }
            remove
            {
                _onEnter -= value;
                if (_onEnter == null && _onExit == null) this.enabled = false;
            }
        }
        public event OnCollisionCallback OnExit
        {
            add
            {
                _onExit += value;
                if (!this.enabled) this.enabled = true;
            }
            remove
            {
                _onExit -= value;
                if (_onEnter == null && _onExit == null) this.enabled = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!this.isActiveAndEnabled) return;

            if (_onEnter != null) _onEnter(this.gameObject, collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!this.isActiveAndEnabled) return;

            if (_onExit != null) _onExit(this.gameObject, collision);
        }

        private void OnDestroy()
        {
            _onEnter = null;
            _onExit = null;
        }

    }

    public sealed class CollisionStayHook : MonoBehaviour
    {

        private OnCollisionCallback _onStay;

        public event OnCollisionCallback OnStay
        {
            add
            {
                _onStay += value;
                if (!this.enabled) this.enabled = true;
            }
            remove
            {
                _onStay -= value;
                if (_onStay == null) this.enabled = false;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!this.isActiveAndEnabled) return;

            if (_onStay != null) _onStay(this.gameObject, collision);
        }

        private void OnDestroy()
        {
            _onStay = null;
        }

    }

}
