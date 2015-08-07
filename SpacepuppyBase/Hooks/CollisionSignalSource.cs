using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Hooks
{

    [System.Serializable()]
    public class CollisionSignalSource : IGameObjectSource
    {

        #region Events

        public event OnTriggerCallback OnTriggerEnter
        {
            add
            {
                if (_signalSource == null) return;
                if (_triggerEnterExitHook == null) _triggerEnterExitHook = _signalSource.AddOrGetComponent<TriggerEnterExitHook>();
                _triggerEnterExitHook.OnEnter += value;
            }
            remove
            {
                if (_triggerEnterExitHook != null) _triggerEnterExitHook.OnEnter -= value;
            }
        }
        public event OnTriggerCallback OnTriggerStay
        {
            add
            {
                if (_signalSource == null) return;
                if (_triggerStayHook == null) _triggerStayHook = _signalSource.AddOrGetComponent<TriggerStayHook>();
                _triggerStayHook.OnStay += value;
            }
            remove
            {
                if (_triggerEnterExitHook != null) _triggerEnterExitHook.OnEnter -= value;
            }
        }
        public event OnTriggerCallback OnTriggerExit
        {
            add
            {
                if (_signalSource == null) return;
                if (_triggerEnterExitHook == null) _triggerEnterExitHook = _signalSource.AddOrGetComponent<TriggerEnterExitHook>();
                _triggerEnterExitHook.OnExit += value;
            }
            remove
            {
                if (_triggerEnterExitHook != null) _triggerEnterExitHook.OnExit -= value;
            }
        }
        public event OnCollisionCallback OnCollisionEnter
        {
            add
            {
                if (_signalSource == null) return;
                if (_collisionEnterExitHook == null) _collisionEnterExitHook = _signalSource.AddOrGetComponent<CollisionEnterExitHook>();
                _collisionEnterExitHook.OnEnter += value;
            }
            remove
            {
                if (_collisionEnterExitHook != null) _collisionEnterExitHook.OnEnter -= value;
            }
        }
        public event OnCollisionCallback OnCollisionStay
        {
            add
            {
                if (_signalSource == null) return;
                if (_collisionStayHook == null) _collisionStayHook = _signalSource.AddOrGetComponent<CollisionStayHook>();
                _collisionStayHook.OnStay += value;
            }
            remove
            {
                if (_collisionStayHook != null) _collisionStayHook.OnStay -= value;
            }
        }
        public event OnCollisionCallback OnCollisionExit
        {
            add
            {
                if (_signalSource == null) return;
                if (_collisionEnterExitHook == null) _collisionEnterExitHook = _signalSource.AddOrGetComponent<CollisionEnterExitHook>();
                _collisionEnterExitHook.OnExit += value;
            }
            remove
            {
                if (_collisionEnterExitHook != null) _collisionEnterExitHook.OnExit -= value;
            }
        }

        private OnStrikeCallback _onStrike;
        public event OnStrikeCallback OnStrike
        {
            add
            {
                if(_onStrike != null)
                {
                    if (_triggerEnterExitHook == null) _triggerEnterExitHook = _signalSource.AddOrGetComponent<TriggerEnterExitHook>();
                    if (_collisionEnterExitHook == null) _collisionEnterExitHook = _signalSource.AddOrGetComponent<CollisionEnterExitHook>();
                    _triggerEnterExitHook.OnEnter -= this.OnTriggerStrike;
                    _collisionEnterExitHook.OnEnter -= this.OnCollisionStrike;
                    _triggerEnterExitHook.OnEnter += this.OnTriggerStrike;
                    _collisionEnterExitHook.OnEnter += this.OnCollisionStrike;
                }
                _onStrike += value;
            }
            remove
            {
                if (_onStrike == null) return;
                _onStrike -= value;
                if(_onStrike == null)
                {
                    if (_triggerEnterExitHook != null) _triggerEnterExitHook.OnEnter -= this.OnTriggerStrike;
                    if (_collisionEnterExitHook != null) _collisionEnterExitHook.OnEnter -= this.OnCollisionStrike;
                }
            }
        }

        #endregion

        #region Fields

        [SerializeField()]
        [DisableOnPlay()]
        private GameObject _signalSource;

        [System.NonSerialized()]
        private TriggerEnterExitHook _triggerEnterExitHook;
        [System.NonSerialized()]
        private TriggerStayHook _triggerStayHook;
        [System.NonSerialized()]
        private CollisionEnterExitHook _collisionEnterExitHook;
        [System.NonSerialized()]
        private CollisionStayHook _collisionStayHook;

        #endregion

        #region CONSTRUCTOR

        protected CollisionSignalSource()
        {
            //simple constructor for unity serializer
        }

        public CollisionSignalSource(GameObject signalSource)
        {
            _signalSource = signalSource;
        }

        #endregion

        #region Properties

        public GameObject SignalSource
        {
            get { return _signalSource; }
        }

        #endregion

        #region Event Handlers

        private void OnCollisionStrike(GameObject sender, Collision c)
        {
            if (_onStrike != null) _onStrike(sender, c.collider);
        }

        private void OnTriggerStrike(GameObject sender, Collider c)
        {
            if (_onStrike != null) _onStrike(sender, c);
        }

        #endregion

        #region IGameObjectSource Interface

        GameObject IGameObjectSource.gameObject
        {
            get { return _signalSource; }
        }

        Transform IGameObjectSource.transform
        {
            get { return (_signalSource != null) ? _signalSource.transform : null; }
        }

        #endregion

    }

}
