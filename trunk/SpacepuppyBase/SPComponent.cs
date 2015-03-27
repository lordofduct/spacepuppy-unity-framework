using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public abstract class SPComponent : MonoBehaviour, IComponent
    {

        #region Fields

        [System.NonSerialized()]
        private GameObject _entityRoot;
        [System.NonSerialized()]
        private bool _started = false;
        [System.NonSerialized()]
        private List<RadicalCoroutine> _managedRoutines;

        #endregion

        #region CONSTRUCTOR

        protected virtual void Awake()
        {
            //if (Application.isEditor)
            //{
            //    com.spacepuppy.Utils.Assertions.AssertRequireLikeComponentAttrib(this);
            //    com.spacepuppy.Utils.Assertions.AssertUniqueToEntityAttrib(this);
            //}

            this.SyncEntityRoot();
        }

        protected virtual void Start()
        {
            _started = true;
            this.SyncEntityRoot();
            this.OnStartOrEnable();
        }

        protected virtual void OnDestroy()
        {
            //InvokeUtil.CancelInvoke(this);
            if (this.ComponentDestroyed != null)
            {
                this.ComponentDestroyed(this, System.EventArgs.Empty);
            }
        }

        protected virtual void OnEnable()
        {
            this.SendMessage(SPConstants.MSG_ONSPCOMPONENTENABLED, this, SendMessageOptions.DontRequireReceiver);

            if (_started) this.OnStartOrEnable();

            if(_managedRoutines != null && _managedRoutines.Count > 0)
            {
                for(int i = 0; i < _managedRoutines.Count; i++)
                {
                    var routine = _managedRoutines[i];
                    if(!routine.Active && routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
                    {
                        routine.Resume(this);
                    }
                }
            }
        }

        /// <summary>
        /// On start or on enable if and only if start already occurred. This adjusts the order of 'OnEnable' so that it can be used in conjunction with 'OnDisable' to wire up handlers cleanly. 
        /// OnEnable occurs BEFORE Start sometimes, and other components aren't ready yet. This remedies that.
        /// </summary>
        protected virtual void OnStartOrEnable()
        {

        }

        protected virtual void OnDespawn()
        {
        }

        protected virtual void OnDisable()
        {
            this.SendMessage(SPConstants.MSG_ONSPCOMPONENTDISABLED, this, SendMessageOptions.DontRequireReceiver);
            

            //Clean up routines
            if(_managedRoutines != null && _managedRoutines.Count > 0)
            {
                var arr = _managedRoutines.ToArray();
                var cancellableMode = (this.gameObject.activeInHierarchy) ? RadicalCoroutineDisableMode.CancelOnDisable : RadicalCoroutineDisableMode.CancelOnDeactivate;
                var stoppableMode = (this.gameObject.activeInHierarchy) ? RadicalCoroutineDisableMode.StopOnDisable : RadicalCoroutineDisableMode.StopOnDeactivate;
                RadicalCoroutine routine;
                for (int i = 0; i < arr.Length; i++)
                {
                    routine = _managedRoutines[i];
                    if (routine.DisableMode.HasFlag(cancellableMode))
                    {
                        routine.Cancel();
                        routine.OnFinished -= this.OnRoutineFinished;
                        _managedRoutines.Remove(routine);
                    }
                    else
                    {
                        if (routine.DisableMode.HasFlag(stoppableMode))
                        {
                            routine.Stop();
                        }
                        if (!routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
                        {
                            routine.OnFinished -= this.OnRoutineFinished;
                            _managedRoutines.Remove(routine);
                        }
                    }
                }
            }
        }

        #endregion

        #region Properties

        public GameObject entityRoot { get { return _entityRoot; } }

        /// <summary>
        /// Start has been called on this component.
        /// </summary>
        public bool started { get { return _started; } }

        public bool isActiveAndEnabled
        {
            get { return this.enabled && this.gameObject.activeInHierarchy; }
        }

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

        internal void RegisterCoroutine(RadicalCoroutine routine, RadicalCoroutineDisableMode disableMode)
        {
            if (disableMode == RadicalCoroutineDisableMode.Default) return;

            if (_managedRoutines == null) _managedRoutines = new List<RadicalCoroutine>();
            if (_managedRoutines.Contains(routine)) return;

            routine.OnFinished -= this.OnRoutineFinished;
            routine.OnFinished += this.OnRoutineFinished;
            _managedRoutines.Add(routine);
        }

        private void OnRoutineFinished(object sender, System.EventArgs e)
        {
            var routine = sender as RadicalCoroutine;
            if (routine == null) return;

            routine.OnComplete -= this.OnRoutineFinished;
            if (_managedRoutines != null) _managedRoutines.Remove(routine);
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
