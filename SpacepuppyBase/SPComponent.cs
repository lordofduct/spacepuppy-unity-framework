using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public abstract class SPComponent : MonoBehaviour, IEventfulComponent, ISPDisposable
    {

        #region Events

        public event System.EventHandler OnEnabled;
        public event System.EventHandler OnStarted;
        public event System.EventHandler OnDisabled;
        public event System.EventHandler ComponentDestroyed;

        #endregion

        #region Fields

        [System.NonSerialized()]
        private bool _started = false;

        #endregion

        #region CONSTRUCTOR

        protected virtual void Awake()
        {
            if (this is IMixin) MixinUtil.Initialize(this as IMixin);
            //this.SyncEntityRoot();
        }

        protected virtual void Start()
        {
            _started = true;
            //this.SyncEntityRoot();
            this.OnStartOrEnable();
            if (this.OnStarted != null) this.OnStarted(this, System.EventArgs.Empty);
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
            if (this.OnEnabled != null) this.OnEnabled(this, System.EventArgs.Empty);

            if (_started)
            {
                this.OnStartOrEnable();
            }
        }

        protected virtual void OnDisable()
        {
            if (this.OnDisabled != null) this.OnDisabled(this, System.EventArgs.Empty);
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

        /// <summary>
        /// Start has been called on this component.
        /// </summary>
        public bool started { get { return _started; } }

        //OBSOLETE - unity added this in latest version of unity
        //public bool isActiveAndEnabled { get { return this.gameObject.activeInHierarchy && this.enabled; } }

        #endregion
            
        #region Radical Coroutine Methods
        
        public new void StopAllCoroutines()
        {
            RadicalCoroutineManager manager = this.GetComponent<RadicalCoroutineManager>();
            if (manager != null)
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
#if SP_LIB
                return !ObjUtil.IsObjectAlive(this);
#else
                return this == null;
#endif
            }
        }

        void System.IDisposable.Dispose()
        {
#if SP_LIB
            ObjUtil.SmartDestroy(this);
#else
            Object.Destroy(this);
#endif
        }

        #endregion
        
    }

    /// <summary>
    /// Represents a component that should always exist as a member of an entity.
    /// 
    /// Such a component should not change parents frequently as it would be expensive.
    /// </summary>
    public class SPEntityComponent : SPComponent
    {

        #region Fields

        [System.NonSerialized]
        private SPEntity _entity;
        [System.NonSerialized]
        private GameObject _entityRoot;
        [System.NonSerialized]
        private bool _synced;

        #endregion

        #region Properties

        public SPEntity Entity
        {
            get
            {
                if (!_synced) this.SyncRoot();
                return _entity;
            }
        }

        public GameObject entityRoot
        {
            get
            {
                if (!_synced) this.SyncRoot();
                return _entityRoot;
            }
        }

        #endregion

        #region Methods

        protected virtual void OnTransformParentChanged()
        {
            _synced = false;
            _entity = null;
            _entityRoot = null;
        }

        protected void SyncRoot()
        {
            _synced = true;
            _entity = SPEntity.Pool.GetFromSource(this);
#if SP_LIB
            _entityRoot = (_entity != null) ? _entity.gameObject : this.FindRoot();
#else
            _entityRoot = (_entity != null) ? _entity.transform : this.gameObject;
#endif
        }

        #endregion

    }

}
