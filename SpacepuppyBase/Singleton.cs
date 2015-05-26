using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public interface ISingleton : IComponent
    {

        bool MaintainOnLoad { get; set; }

    }

    public class Singleton : SPNotifyingComponent, ISingleton
    {

        #region Static Interface

        public const string GAMEOBJECT_NAME = "Spacepuppy.SingletonSource";

        private static GameObject _gameObject;
        private static Dictionary<System.Type, ISingleton> _singletonRefs = new Dictionary<System.Type, ISingleton>();

        public static GameObject GameObjectSource
        {
            get
            {
                if(Application.isPlaying)
                {
                    if (object.ReferenceEquals(_gameObject, null) && !GameLoopEntry.ApplicationClosing)
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

        public static T GetInstance<T>() where T : Component, ISingleton
        {
            if (_singletonRefs.ContainsKey(typeof(T))) return _singletonRefs[typeof(T)] as T;
            if (GameLoopEntry.ApplicationClosing) return null;

            var single = Singleton.GameObjectSource.GetComponent<T>();
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent<T>();
            }
            return single;
        }

        public static ISingleton GetInstance(System.Type tp)
        {
            if (!typeof(ISingleton).IsAssignableFrom(tp)) throw new TypeArgumentMismatchException(tp, typeof(ISingleton), "tp");
            if (_singletonRefs.ContainsKey(tp)) return _singletonRefs[tp];
            if (GameLoopEntry.ApplicationClosing) return null;

            var single = Singleton.GameObjectSource.GetComponent(tp) as ISingleton;
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent(tp) as ISingleton;
            }
            return single;
        }

        public static T CreateSpecialInstance<T>(string gameObjectName, bool maintainOnLoad = true) where T : Component, ISingleton
        {
            if (_singletonRefs.ContainsKey(typeof(T))) return _singletonRefs[typeof(T)] as T;
            if (GameLoopEntry.ApplicationClosing) return null;

            var go = new GameObject(gameObjectName);
            var single = go.AddComponent<T>();
            single.MaintainOnLoad = maintainOnLoad;
            return single;
        }

        public static ISingleton CreateSpecialInstance(System.Type tp, string gameObjectName, bool maintainOnLoad = true)
        {
            if (!typeof(ISingleton).IsAssignableFrom(tp)) throw new TypeArgumentMismatchException(tp, typeof(ISingleton), "tp");
            if (_singletonRefs.ContainsKey(tp)) return _singletonRefs[tp];
            if (GameLoopEntry.ApplicationClosing) return null;

            var go = new GameObject(gameObjectName);
            var single = go.AddComponent(tp) as ISingleton;
            single.MaintainOnLoad = maintainOnLoad;
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

        public static bool HasInstance(System.Type tp, out ISingleton instance)
        {
            return _singletonRefs.TryGetValue(tp, out instance);
        }

        public static IEnumerable<ISingleton> AllSingletons
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
            if (!_singletonRefs.ContainsKey(tp)) _singletonRefs[tp] = this;
        }

        protected override void Awake()
        {
            base.Awake();

            _maintainer.OnAwake(this);
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

        public bool MaintainOnLoad
        {
            get { return _maintainer.MaintainOnLoad; }
            set
            {
                _maintainer.MaintainOnLoad = value;
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
            private bool _maintainOnLoad;


            [System.NonSerialized()]
            private ISingleton _target;
            [System.NonSerialized()]
            private bool _flaggedSelfMaintaining;
            [System.NonSerialized()]
            private bool _started;

            #endregion

            #region CONSTRUCTOR

            public Maintainer()
            {

            }

            public Maintainer(bool maintainOnLoad)
            {
                _maintainOnLoad = maintainOnLoad;
            }

            #endregion

            #region Properties

            public ISingleton Target { get { return _target; } }

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

            public void OnAwake(ISingleton target)
            {
                if (object.ReferenceEquals(target, null)) throw new System.ArgumentNullException("target");
                _target = target;

                //first test if we have the appropriate configuration
                if (_target.component.GetLikeComponents<ISingleton>().Count() > 1 && !_target.component.HasComponent<SingletonManager>())
                {
                    Debug.LogWarning("Gameobject with multiple Singletons exists without a SingletonManager attached, adding a SingletonManager with default destroy settings.", target.component);
                    _target.component.AddComponent<SingletonManager>();
                }

                //now set up singleton reference
                if (_target.component.IsActiveAndEnabled())
                {
                    this.EnforceThisAsSingleton();
                    _target.ComponentDestroyed += this.OnComponentDestroyed;
                    GameLoopEntry.LevelWasLoaded += this.OnLevelWasLoaded;
                }
                else
                {
                    //remove self if it were added
                    this.RemoveThisAsSingleton();
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
                GameLoopEntry.LevelWasLoaded -= this.OnLevelWasLoaded;
                this.RemoveThisAsSingleton();

                //if this isn't on the GameObjectSource, we should check if we need to delete our GameObject.
                if (!GameLoopEntry.ApplicationClosing)
                {
                    if (_target.gameObject != null && _target.gameObject != Singleton.GameObjectSource)
                    {
                        var others = _target.gameObject.GetComponents<Singleton>();
                        if (!(others.Length > 1 && !(from s in others select s.MaintainOnLoad).Any()))
                        {
                            ObjUtil.SmartDestroy(_target.gameObject);
                        }
                    }
                }
            }

            private void OnLevelWasLoaded(object sender, GameLoopEntry.LevelWasLoadedEventArgs e)
            {
                if (_maintainOnLoad) return;
                if (_target.component == null) return;
                if (_target.component.HasComponent<SingletonManager>()) return; //let the manager take care of this

                //OnLevelWasLoaded gets called in between Awake and Start, we haven't started at that point, so ignore this
                if (_target.isActiveAndEnabled && !_started) return;

                Object.Destroy(_target.gameObject);
            }

            private void EnforceThisAsSingleton()
            {
                var c = (_singletonRefs.ContainsKey(_target.GetType())) ? _singletonRefs[_target.GetType()] : null;
                if (c.component == null || c.component == _target.component || !c.isActiveAndEnabled)
                {
                    _singletonRefs[_target.GetType()] = _target;
                }
                else if(_target.component != null)
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

                this.UpdateMaintainOnLoadStatus();
            }

            private void RemoveThisAsSingleton()
            {
                var tp = _target.GetType();
                if (_singletonRefs.ContainsKey(tp))
                {
                    if (_singletonRefs[tp].component == _target.component)
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
                if (!_flaggedSelfMaintaining && _maintainOnLoad)
                {
                    GameObject.DontDestroyOnLoad(_target.gameObject);
                    _flaggedSelfMaintaining = true;
                }
            }

            #endregion

        }

        public class ConfigAttribute : System.Attribute
        {
            public bool ExcludeFromSingletonManager;

            public ConfigAttribute(bool excludeFromSingletonManager)
            {
                this.ExcludeFromSingletonManager = excludeFromSingletonManager;
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

        public IEnumerable<ISingleton> GetSingletons()
        {
            return this.GetLikeComponents<ISingleton>();
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

        private void OnLevelWasLoaded(int level)
        {
            if (this.MaintainOnLoad) return;

            //OnLevelWasLoaded gets called on the objects in the scene being loaded, we haven't started at that point, so ignore this
            if (this.isActiveAndEnabled && !this.started) return;

            Object.Destroy(this.gameObject);
        }

        #endregion

    }

}
