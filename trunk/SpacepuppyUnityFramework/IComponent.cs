using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Base contract for any interface contract that should be considered a Component
    /// </summary>
    public interface IComponent
    {
        bool enabled { get; set; }
        Component component { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
    }

    public abstract class SPComponent : MonoBehaviour, IComponent
    {

        #region Fields

        [System.NonSerialized]
        private GameObject _root;
        private bool _started = false;

        #endregion

        #region CONSTRUCTOR

        protected virtual void Awake()
        {
            if (com.spacepuppy.Utils.Assertions.AssertRequireLikeComponentAttrib(this))
            {
                Object.Destroy(this);
            }
            if (com.spacepuppy.Utils.Assertions.AssertUniqueToEntityAttrib(this))
            {
                Object.Destroy(this);
            }

            this.SyncRoot();
        }

        protected virtual void Start()
        {
            this.SyncRoot();
            _started = true;
        }

        protected virtual void OnDestroy()
        {
            //InvokeUtil.CancelInvoke(this);
        }

        protected virtual void OnEnable()
        {
            this.SendMessage(SPConstants.MSG_ONSPCOMPONENTENABLED, this, SendMessageOptions.DontRequireReceiver);
        }

        protected virtual void OnDisable()
        {
            this.SendMessage(SPConstants.MSG_ONSPCOMPONENTDISABLED, this, SendMessageOptions.DontRequireReceiver);
        }

        #endregion

        #region Properties

        public GameObject root { get { return _root; } }

        public Transform rootTransform { get { return _root.transform; } }

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
        public void SyncRoot()
        {
            _root = this.FindRoot();
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

    }

}
