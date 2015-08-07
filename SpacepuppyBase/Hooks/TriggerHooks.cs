using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Hooks
{

    public sealed class TriggerEnterExitHook : MonoBehaviour
    {

        private OnTriggerCallback _onEnter;
        private OnTriggerCallback _onExit;

        public event OnTriggerCallback OnEnter
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
        public event OnTriggerCallback OnExit
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

        private void OnTriggerEnter(Collider otherCollider)
        {
            if (_onEnter != null) _onEnter(this.gameObject, otherCollider);
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            if (_onExit != null) _onExit(this.gameObject, otherCollider);
        }

        private void OnDestroy()
        {
            _onEnter = null;
            _onExit = null;
        }

    }

    public sealed class TriggerStayHook : MonoBehaviour
    {

        private OnTriggerCallback _onStay;

        public event OnTriggerCallback OnStay
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

        private void OnTriggerStay(Collider otherCollider)
        {
            if (_onStay != null) _onStay(this.gameObject, otherCollider);
        }

        private void OnDestroy()
        {
            _onStay = null;
        }

    }

}
