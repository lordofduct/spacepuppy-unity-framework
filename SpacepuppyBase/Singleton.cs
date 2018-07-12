using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// A contract that binds IManagedSingleton and IService together so that they are easily accessible from the Singletons.Get method.
    /// 
    /// If implementing this directly, ensure that there is a static property getter named 'Instance'.
    /// </summary>
    public interface ISingleton
    {

    }

    public static class Singletons
    {

        public static T Get<T>(bool createIfNone = false) where T : class, ISingleton
        {
            var tp = typeof(T);

            if (typeof(IManagedSingleton).IsAssignableFrom(tp))
                return Singleton.GetInstance(tp, !createIfNone) as T;
            else if (typeof(IService).IsAssignableFrom(tp))
                return Services.Get(tp) as T;
            else
            {
                var prop = tp.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop != null)
                    return prop.GetValue(null, null) as T;
                var field = tp.GetField("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (field != null)
                    return field.GetValue(null) as T;
            }

            return null;
        }

        public static ISingleton Get(System.Type tp, bool createIfNone = false)
        {
            if (typeof(IManagedSingleton).IsAssignableFrom(tp))
                return Singleton.GetInstance(tp, !createIfNone);
            else if (typeof(IService).IsAssignableFrom(tp))
                return Services.Get(tp) as ISingleton;
            else if(typeof(ISingleton).IsAssignableFrom(tp))
            {
                var prop = tp.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop != null)
                    return prop.GetValue(null, null) as ISingleton;
                var field = tp.GetField("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (field != null)
                    return field.GetValue(null) as ISingleton;
            }

            return null;
        }

    }



    [System.Flags()]
    public enum SingletonLifeCycleRule
    {

        LivesForDurationOfScene = 0, //when load is called, this singleton should die
        LivesForever = 1, //when load is called, this singleton will remain to the next scene
        AlwaysReplace = 2, //this singleton should replace any singleton that already exists
        LiveForeverAndAlwaysReplace = 3

    }
    
    public interface IManagedSingleton : ISingleton, IComponent, ISPDisposable
    {

        event System.EventHandler ComponentDestroyed;

        SingletonLifeCycleRule LifeCycle { get; set; }

    }
    
    /// <summary>
    /// A base class that any script that should act like a Singleton can inherit from.
    /// 
    /// Singletons are granted a 'LifeCycle' which controls if the Singleton gets destroyed 
    /// when a new level is loaded, as well what to do if a second Singleton is loaded 
    /// when a previous one already existed.
    /// 
    /// LivesForDurationOfScene - the singleton will be destroyed when the next level is loaded
    /// LivesForever - the singleton will exist indefinitely, even on load
    /// AlwaysReplace - if a singleton of this type already exists when this singleton starts, it will replace the older one
    /// LiveForeverAndAlwaysReplace - acts like LivesForever and AlwaysReplace
    /// </summary>
    public abstract class Singleton : SPNotifyingComponent, IManagedSingleton
    {

        #region Static Interface

        public const string GAMEOBJECT_NAME = "Spacepuppy.SingletonSource";

        private static GameObject _gameObject;
        private static Dictionary<System.Type, IManagedSingleton> _singletonRefs = new Dictionary<System.Type, IManagedSingleton>();

        public static GameObject GameObjectSource
        {
            get
            {
                if(Application.isPlaying)
                {
                    if (_gameObject == null && !GameLoopEntry.ApplicationClosing)
                    {
                        _gameObject = GameObject.Find(GAMEOBJECT_NAME);
                        if (_gameObject == null)
                        {
                            _gameObject = new GameObject(GAMEOBJECT_NAME);
                            _gameObject.AddComponent<SingletonManager>();
                        }
                    }
                    return _gameObject;
                }
                else
                {
                    _gameObject = null;
                    return GameObject.Find(GAMEOBJECT_NAME);
                }
            }
        }

        public static T GetInstance<T>(bool bDoNotCreateIfNotExist = false) where T : Component, IManagedSingleton
        {
            IManagedSingleton single;
            if (_singletonRefs.TryGetValue(typeof(T), out single)) return single as T;
            if (GameLoopEntry.ApplicationClosing) return null;

            if (bDoNotCreateIfNotExist)
                return Singleton.GameObjectSource.GetComponent<T>();
            else
            {
                var tp = typeof(T);
                var attrib = tp.GetCustomAttributes(typeof(ConfigAttribute), false).FirstOrDefault() as ConfigAttribute;
                if (attrib != null && attrib.ExcludeFromSingletonManager)
                    return CreateSpecialInstance<T>(tp.Name, attrib.DefaultLifeCycle);
                else
                    return Singleton.GameObjectSource.AddOrGetComponent<T>();
            }
        }

        public static IManagedSingleton GetInstance(System.Type tp, bool bDoNotCreateIfNotExist = false)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(IManagedSingleton).IsAssignableFrom(tp)) throw new TypeArgumentMismatchException(tp, typeof(IManagedSingleton), "tp");

            IManagedSingleton single;
            if (_singletonRefs.TryGetValue(tp, out single)) return single;
            if (GameLoopEntry.ApplicationClosing) return null;

            if (bDoNotCreateIfNotExist)
                return Singleton.GameObjectSource.GetComponent(tp) as IManagedSingleton;
            else
            {
                var attrib = tp.GetCustomAttributes(typeof(ConfigAttribute), false).FirstOrDefault() as ConfigAttribute;
                if (attrib != null && attrib.ExcludeFromSingletonManager)
                    return CreateSpecialInstance(tp, tp.Name, attrib.DefaultLifeCycle);
                else
                    return Singleton.GameObjectSource.AddOrGetComponent(tp) as IManagedSingleton;
            }
        }



        public static T CreateSpecialInstance<T>(string gameObjectName, SingletonLifeCycleRule lifeCycle) where T : Component, IManagedSingleton
        {
            IManagedSingleton single;
            if (_singletonRefs.TryGetValue(typeof(T), out single)) return single as T;
            if (GameLoopEntry.ApplicationClosing) return null;

            var go = new GameObject(gameObjectName);
            single = go.AddComponent<T>();
            single.LifeCycle = lifeCycle;
            return single as T;
        }

        public static IManagedSingleton CreateSpecialInstance(System.Type tp, string gameObjectName, SingletonLifeCycleRule lifeCycle)
        {
            if (!typeof(IManagedSingleton).IsAssignableFrom(tp)) throw new TypeArgumentMismatchException(tp, typeof(IManagedSingleton), "tp");

            IManagedSingleton single;
            if (_singletonRefs.TryGetValue(tp, out single)) return single;
            if (GameLoopEntry.ApplicationClosing) return null;

            var go = new GameObject(gameObjectName);
            single = go.AddComponent(tp) as IManagedSingleton;
            single.LifeCycle = lifeCycle;
            return single;
        }

        public static bool HasInstance<T>() where T : Singleton
        {
            return _singletonRefs.ContainsKey(typeof(T));
        }

        public static bool HasInstance(System.Type tp)
        {
            return _singletonRefs.ContainsKey(tp);
        }

        public static bool HasInstance(System.Type tp, out IManagedSingleton instance)
        {
            return _singletonRefs.TryGetValue(tp, out instance);
        }

        public static IEnumerable<IManagedSingleton> AllSingletons
        {
            get
            {
                return _singletonRefs.Values;
            }
        }

        #endregion

        #region Instance Level

        [SerializeField()]
        private Maintainer _maintainer = new Maintainer();

        #region CONSTRUCTOR

        public Singleton()
        {
            var tp = this.GetType();
            //if (Application.isEditor && !Application.isPlaying)
            //{
            var attrib = tp.GetCustomAttributes(typeof(Singleton.ConfigAttribute), false).FirstOrDefault() as Singleton.ConfigAttribute;
            if (attrib != null) _maintainer.LifeCycle = attrib.DefaultLifeCycle;
            //}

            //if (!_singletonRefs.ContainsKey(tp)) _singletonRefs[tp] = this;
        }

        protected override sealed void Awake()
        {
            base.Awake();

            if (_maintainer.OnAwake(this)) this.OnValidAwake();
        }

        /// <summary>
        /// Called if the singleton object properly validated during 'Awake'. 
        /// Override this to perform 'Awake' operations.
        /// </summary>
        protected virtual void OnValidAwake()
        {

        }

        protected override void Start()
        {
            base.Start();

            _maintainer.OnStart();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _maintainer.OnEnabled();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _maintainer.OnDisable();
        }

        #endregion

        public virtual SingletonLifeCycleRule LifeCycle
        {
            get { return _maintainer.LifeCycle; }
            set
            {
                _maintainer.LifeCycle = value;
            }
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class Maintainer
        {

            #region Fields

            [SerializeField()]
            [Tooltip("Should this Singleton be maintained when a new scene is loaded.")]
            private SingletonLifeCycleRule _lifeCycle;


            [System.NonSerialized()]
            private IManagedSingleton _target;
            [System.NonSerialized()]
            private bool _flaggedSelfMaintaining;
            [System.NonSerialized()]
            private bool _started;

            #endregion

            #region CONSTRUCTOR

            public Maintainer()
            {

            }

            public Maintainer(SingletonLifeCycleRule lifeCycle)
            {
                _lifeCycle = lifeCycle;
            }

            #endregion

            #region Properties

            public IManagedSingleton Target { get { return _target; } }

            public SingletonLifeCycleRule LifeCycle
            {
                get { return _lifeCycle; }
                set
                {
                    if (_lifeCycle == value) return;
                    _lifeCycle = value;

                    if (_target != null && !_flaggedSelfMaintaining && (_lifeCycle & SingletonLifeCycleRule.LivesForever) != 0) this.UpdateMaintainOnLoadStatus();
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// 
            /// </summary>
            /// <param name="target"></param>
            /// <returns>Returns true if the validation was a success.</returns>
            public bool OnAwake(IManagedSingleton target)
            {
                if (object.ReferenceEquals(target, null)) throw new System.ArgumentNullException("target");
                _target = target;

                //first test if we have the appropriate configuration
                if (_target.component.GetComponents<IManagedSingleton>().Count() > 1 && !_target.component.HasComponent<SingletonManager>())
                {
                    Debug.LogWarning("Gameobject with multiple Singletons exists without a SingletonManager attached, adding a SingletonManager with default destroy settings.", target.component);
                    _target.component.AddComponent<SingletonManager>();
                }

                //now set up singleton reference
                if (_target.component.IsActiveAndEnabled())
                {
                    this.EnforceThisAsSingleton();
                    _target.ComponentDestroyed += this.OnComponentDestroyed;
                    SceneManager.sceneLoaded += this.OnLevelWasLoaded;
                    return true;
                }
                else
                {
                    //remove self if it were added
                    this.RemoveThisAsSingleton();
                    return false;
                }

            }

            public void OnStart()
            {
                _started = true;
                this.EnforceThisAsSingleton();
            }

            public void OnEnabled()
            {
                if (!_started) return;
                this.EnforceThisAsSingleton();
            }

            public void OnDisable()
            {
                this.RemoveThisAsSingleton();
            }

            private void OnComponentDestroyed(object sender, System.EventArgs e)
            {
                SceneManager.sceneLoaded -= OnLevelWasLoaded;
                this.RemoveThisAsSingleton();

                //if this isn't on the GameObjectSource, we should check if we need to delete our GameObject.
                if (!GameLoopEntry.ApplicationClosing)
                {
                    if (_target.gameObject != null && _target.gameObject != Singleton.GameObjectSource)
                    {
                        var others = _target.gameObject.GetComponents<Singleton>();
                        if (!(others.Length > 1 && !(from s in others select (s.LifeCycle & SingletonLifeCycleRule.LivesForever) != 0).Any()))
                        {
                            ObjUtil.SmartDestroy(_target.gameObject);
                        }
                    }
                }
            }
            
            private void OnLevelWasLoaded(Scene sc, LoadSceneMode mode)
            {
                if ((_lifeCycle & SingletonLifeCycleRule.LivesForever) != 0) return;
                if (_target.component == null) return;
                if (_target.component.HasComponent<SingletonManager>()) return; //let the manager take care of this

                //OnLevelWasLoaded gets called in between Awake and Start, we haven't started at that point, so ignore this
                if (_target.isActiveAndEnabled && !_started) return;

                Object.Destroy(_target.gameObject);
            }

            private void EnforceThisAsSingleton()
            {
                var targTp = _target.GetType();
                IManagedSingleton c;
                if (!_singletonRefs.TryGetValue(targTp, out c)) c = null;

                if (object.ReferenceEquals(c, null) || c.component == null || c.component == _target.component || !c.isActiveAndEnabled)
                {
                    _singletonRefs[targTp] = _target;
                }
                else if(_target.component != null)
                {
                    if((_lifeCycle & SingletonLifeCycleRule.AlwaysReplace) != 0)
                    {
                        _singletonRefs[targTp] = _target;
                        if (_target.component.HasComponent<SingletonManager>())
                        {
                            Object.Destroy(c.component);
                        }
                        else
                        {
                            Object.Destroy(c.gameObject);
                        }
                    }
                    else
                    {
                        if (_target.component.HasComponent<SingletonManager>())
                        {
                            Object.Destroy(_target.component);
                        }
                        else
                        {
                            Object.Destroy(_target.gameObject);
                        }
                        throw new System.InvalidOperationException("Attempted to create an instance of a Singleton out of its appropriate operating bounds.");
                    }
                }

                this.UpdateMaintainOnLoadStatus();
            }

            private void RemoveThisAsSingleton()
            {
                if (_target == null) return;

                var tp = _target.GetType();
                IManagedSingleton sing;
                if(_singletonRefs.TryGetValue(tp, out sing))
                {
                    if(sing.component == _target.component)
                    {
                        _singletonRefs.Remove(tp);
                    }
                }
            }

            private void UpdateMaintainOnLoadStatus()
            {
                if (_target.component == null) return;
                if (_target.component.HasComponent<SingletonManager>()) return;

                //for singletons not on the primary singleton source
                if (!_flaggedSelfMaintaining && (_lifeCycle & SingletonLifeCycleRule.LivesForever) != 0)
                {
                    GameObject.DontDestroyOnLoad(_target.gameObject);
                    _flaggedSelfMaintaining = true;
                }
            }

            #endregion

        }

        public class ConfigAttribute : System.Attribute
        {

            public SingletonLifeCycleRule DefaultLifeCycle;
            public bool LifeCycleReadOnly;
            public bool ExcludeFromSingletonManager;

            public ConfigAttribute(SingletonLifeCycleRule defaultLifeCycle)
            {
                this.DefaultLifeCycle = defaultLifeCycle;
            }
        }

        #endregion

    }

    /// <summary>
    /// Attach to a component that has more than one Singleton on it, it handles group management of Singletons on the same GameObject.
    /// </summary>
    public sealed class SingletonManager : SPComponent
    {

        #region Fields

        [SerializeField()]
        private bool _maintainOnLoad = false;
        [System.NonSerialized()]
        private bool _flaggedSelfMaintaining;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            
            this.UpdateMaintainOnLoadStatus();
            SceneManager.sceneLoaded += this.OnSceneWasLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.sceneLoaded -= this.OnSceneWasLoaded;
        }

        #endregion

        #region Properties

        public bool MaintainOnLoad
        {
            get { return _maintainOnLoad; }
            set
            {
                if (_maintainOnLoad == value) return;
                _maintainOnLoad = value;

                if (!_flaggedSelfMaintaining && _maintainOnLoad) this.UpdateMaintainOnLoadStatus();
            }
        }

        #endregion

        #region Methods

        public IEnumerable<IManagedSingleton> GetSingletons()
        {
            return this.GetComponents<IManagedSingleton>();
        }

        private void UpdateMaintainOnLoadStatus()
        {
            //for singletons not on the primary singleton source
            if (Application.isPlaying && !_flaggedSelfMaintaining && _maintainOnLoad)
            {
                GameObject.DontDestroyOnLoad(this.gameObject);
                _flaggedSelfMaintaining = true;
            }
        }
        
        private void OnSceneWasLoaded(Scene sc, LoadSceneMode mode)
        {
            if (this.MaintainOnLoad) return;

            //OnLevelWasLoaded gets called on the objects in the scene being loaded, we haven't started at that point, so ignore this
            if (this.isActiveAndEnabled && !this.started) return;

            Object.Destroy(this.gameObject);
        }

        #endregion

    }

}
