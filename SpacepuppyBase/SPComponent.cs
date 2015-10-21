using UnityEngine;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy
{

    public abstract class SPComponent : MonoBehaviour, IComponent, ISPDisposable
    {

        #region Events

        public event System.EventHandler OnEnabled;
        public event System.EventHandler OnDisabled;
        public event System.EventHandler ComponentDestroyed;

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
            //this.SyncEntityRoot();
        }

        protected virtual void Start()
        {
            _started = true;
            //this.SyncEntityRoot();
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

        /// <summary>
        /// Occurs if this gameobject or one of its parents is moved in the hierarchy using 'GameObjUtil.AddChild' or 'GameObjUtil.RemoveFromParent'
        /// </summary>
        protected virtual void OnTransformHierarchyChanged()
        {
            _entityRoot = null;
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

        public GameObject entityRoot
        {
            get
            {
                if (object.ReferenceEquals(_entityRoot, null)) _entityRoot = this.FindRoot();
                return _entityRoot;
            }
        }

        /// <summary>
        /// Start has been called on this component.
        /// </summary>
        public bool started { get { return _started; } }

        //OBSOLETE - unity added this in latest version of unity
        //public bool isActiveAndEnabled { get { return this.gameObject.activeInHierarchy && this.enabled; } }

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

        #region Component Overrides

        //public new T GetComponent<T>() where T : class
        //{
        //    return ComponentUtil.GetComponentAlt<T>(this);
        //}

        //public new T[] GetComponents<T>() where T : class
        //{
        //    return ComponentUtil.GetComponentsAlt<T>(this);
        //}

        //public new T GetComponentInChildren<T>() where T : class
        //{
        //    return ComponentUtil.GetComponentInChildrenAlt<T>(this);
        //}

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

        public RadicalCoroutine InvokeRadical(System.Action method, float delay, ITimeSupplier time = null, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (method == null) throw new System.ArgumentNullException("method");

            return this.StartRadicalCoroutine(CoroutineUtil.RadicalInvokeRedirect(method, delay, -1f, time), disableMode);
        }

        public RadicalCoroutine InvokeAfterYield(System.Action method, object yieldInstruction, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.CancelOnDisable)
        {
            if (method == null) throw new System.ArgumentNullException("method");

            return this.StartRadicalCoroutine(CoroutineUtil.InvokeAfterYieldRedirect(method, yieldInstruction), disableMode);
        }

        public RadicalCoroutine InvokeRepeatingRadical(System.Action method, float delay, float repeatRate, ITimeSupplier time = null, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (method == null) throw new System.ArgumentNullException("method");

            return this.StartRadicalCoroutine(CoroutineUtil.RadicalInvokeRedirect(method, delay, repeatRate, time), disableMode);
        }

        public new void StopAllCoroutines()
        {
            RadicalCoroutineManager manager;
            if(this.GetComponent<RadicalCoroutineManager>(out manager))
            {
                manager.PurgeCoroutines(this);
            }
            base.StopAllCoroutines();
        }

        #endregion

        #region IComponent Interface

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


        #region ISPDisposable Interface

        bool ISPDisposable.IsDisposed
        {
            get
            {
                return !ObjUtil.IsObjectAlive(this);
            }
        }

        void IDisposable.Dispose()
        {
            ObjUtil.SmartDestroy(this);
        }

        #endregion

    }

}
