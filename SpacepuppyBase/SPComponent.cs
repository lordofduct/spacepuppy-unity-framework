using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public abstract class SPComponent : MonoBehaviour, IComponent
    {

        #region Events

        public event System.EventHandler OnEnabled;
        public event System.EventHandler OnDisabled;

        #endregion

        #region Fields

        [System.NonSerialized()]
        private GameObject _entityRoot;
        [System.NonSerialized()]
        private bool _started = false;

        #endregion

        #region CONSTRUCTOR

        protected virtual void Awake()
        {
            this.SyncEntityRoot();
        }

        protected virtual void Start()
        {
            _started = true;
            this.SyncEntityRoot();
            this.OnStartOrEnable();
        }

        /// <summary>
        /// On start or on enable if and only if start already occurred. This adjusts the order of 'OnEnable' so that it can be used in conjunction with 'OnDisable' to wire up handlers cleanly. 
        /// OnEnable occurs BEFORE Start sometimes, and other components aren't ready yet. This remedies that.
        /// </summary>
        protected virtual void OnStartOrEnable()
        {

        }

        protected virtual void OnEnable()
        {
            //this.SendMessage(SPConstants.MSG_ONSPCOMPONENTENABLED, this, SendMessageOptions.DontRequireReceiver);
            if (this.OnEnabled != null) this.OnEnabled(this, System.EventArgs.Empty);

            if (_started) this.OnStartOrEnable();
        }

        protected virtual void OnDisable()
        {
            //this.SendMessage(SPConstants.MSG_ONSPCOMPONENTDISABLED, this, SendMessageOptions.DontRequireReceiver);
            if (this.OnDisabled != null) this.OnDisabled(this, System.EventArgs.Empty);
        }

        protected virtual void OnDespawn()
        {
        }

        protected virtual void OnDestroy()
        {
            //InvokeUtil.CancelInvoke(this);
            if (this.ComponentDestroyed != null)
            {
                this.ComponentDestroyed(this, System.EventArgs.Empty);
            }
        }

        #endregion

        #region Properties

        public GameObject entityRoot { get { return _entityRoot; } }

        /// <summary>
        /// Start has been called on this component.
        /// </summary>
        public bool started { get { return _started; } }

        #endregion

        #region Methods

        /// <summary>
        /// Call this to resync the 'root' property incase the hierarchy of this object has changed. This needs to be performed since 
        /// unity doesn't have an event/message to signal a change in hierarchy.
        /// </summary>
        public virtual void SyncEntityRoot()
        {
            _entityRoot = this.FindRoot();
        }

        #endregion

        #region Radical Coroutine Methods

        public RadicalCoroutine StartRadicalCoroutine(System.Collections.IEnumerator routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            co.Start(this, disableMode);
            return co;
        }

        public RadicalCoroutine StartRadicalCoroutine(System.Collections.IEnumerable routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine.GetEnumerator());
            co.Start(this, disableMode);
            return co;
        }

        public RadicalCoroutine StartRadicalCoroutine(CoroutineMethod routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine().GetEnumerator());
            co.Start(this, disableMode);
            return co;
        }

        public RadicalCoroutine InvokeRadical(System.Action method, float delay, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (method == null) throw new System.ArgumentNullException("method");

            return this.StartRadicalCoroutine(CoroutineUtil.InvokeRedirect(method, delay), disableMode);
        }

        public RadicalCoroutine InvokeRepeatingRadical(System.Action method, float delay, float repeatRate, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (method == null) throw new System.ArgumentNullException("method");

            return this.StartRadicalCoroutine(CoroutineUtil.InvokeRedirect(method, delay), disableMode);
        }

        #endregion

        #region IComponent Interface

        public event System.EventHandler ComponentDestroyed;

        bool IComponent.enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        Component IComponent.component
        {
            get { return this; }
        }

        //implemented implicitly
        /*
        GameObject IComponent.gameObject { get { return this.gameObject; } }
        Transform IComponent.transform { get { return this.transform; } }
        */

        #endregion

    }

}
