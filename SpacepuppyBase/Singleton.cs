using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public class Singleton : SPNotifyingComponent
    {

        #region Static Interface

        public const string GAMEOBJECT_NAME = "Spacepuppy.SingletonSource";

        private static GameObject _gameObject;
        private static Dictionary<System.Type, Singleton> _singletonRefs = new Dictionary<System.Type, Singleton>();

        public static GameObject GameObjectSource
        {
            get
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
        }

        public static T GetInstance<T>() where T : Singleton
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

        public static Singleton GetInstance(System.Type tp)
        {
            if (!typeof(Singleton).IsAssignableFrom(tp)) throw new TypeArgumentMismatchException(tp, typeof(Singleton), "tp");
            if (_singletonRefs.ContainsKey(tp)) return _singletonRefs[tp];
            if (GameLoopEntry.ApplicationClosing) return null;

            var single = Singleton.GameObjectSource.GetComponent(tp) as Singleton;
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent(tp) as Singleton;
            }
            return single;
        }

        public static T CreateSpecialInstance<T>(string gameObjectName, bool maintainOnLoad = true) where T:Singleton
        {
            if (_singletonRefs.ContainsKey(typeof(T))) return _singletonRefs[typeof(T)] as T;
            if (GameLoopEntry.ApplicationClosing) return null;

            var go = new GameObject(gameObjectName);
            var single = go.AddComponent<T>();
            single._maintainOnLoad = maintainOnLoad;
            return single;
        }

        public static Singleton CreateSpecialInstance(System.Type tp, string gameObjectName, bool maintainOnLoad = true)
        {
            if (!typeof(Singleton).IsAssignableFrom(tp)) throw new TypeArgumentMismatchException(tp, typeof(Singleton), "tp");
            if (_singletonRefs.ContainsKey(tp)) return _singletonRefs[tp];
            if (GameLoopEntry.ApplicationClosing) return null;

            var go = new GameObject(gameObjectName);
            var single = go.AddComponent(tp) as Singleton;
            single._maintainOnLoad = maintainOnLoad;
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

        public static IEnumerable<Singleton> AllSingletons
        {
            get
            {
                return _singletonRefs.Values;
            }
        }

        #endregion

        #region CONSTRUCTOR

        public Singleton()
        {
            var tp = this.GetType();
            if (!_singletonRefs.ContainsKey(tp)) _singletonRefs[tp] = this;
        }

        protected override void Awake()
        {
            base.Awake();

            if(this.enabled)
            {
                this.EnforceThisAsSingleton();
            }
            else
            {
                //remove self if it were added
                this.RemoveThisAsSingleton();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this.EnforceThisAsSingleton();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.RemoveThisAsSingleton();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.RemoveThisAsSingleton();

            //if this isn't on the GameObjectSource, we should check if we need to delete our GameObject.
            if(!GameLoopEntry.ApplicationClosing)
            {
                if (this.gameObject != null && this.gameObject != Singleton.GameObjectSource)
                {
                    var others = this.GetComponents<Singleton>();
                    if (!(others.Length > 1 && !(from s in others select s.MaintainOnLoad).Any()))
                    {
                        ObjUtil.SmartDestroy(this.gameObject);
                    }
                }
            }
        }

        #endregion

        #region Properties

        [SerializeField()]
        [Tooltip("Should this Singleton be maintained when a new scene is loaded.")]
        private bool _maintainOnLoad = true;
        [System.NonSerialized()]
        private bool _flaggedSelfMaintaining;

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

        #region Singleton Enforcement

        private void EnforceThisAsSingleton()
        {
            var c = (_singletonRefs.ContainsKey(this.GetType())) ? _singletonRefs[this.GetType()] : null;
            if (c == null || c == this || !c.enabled)
            {
                _singletonRefs[this.GetType()] = this;
            }
            else
            {
                Object.Destroy(this);
                throw new System.InvalidOperationException("Attempted to create an instance of a Singleton out of its appropriate operating bounds.");
            }

            this.UpdateMaintainOnLoadStatus();
        }

        private void RemoveThisAsSingleton()
        {
            var tp = this.GetType();
            if (_singletonRefs.ContainsKey(tp))
            {
                if (_singletonRefs[tp] == this)
                {
                    _singletonRefs.Remove(tp);
                }
            }
        }

        protected virtual void OnLevelWasLoaded(int level)
        {
            if (this.MaintainOnLoad) return;

            //OnLevelWasLoaded gets called on the objects in the scene being loaded, we haven't started at that point, so ignore this
            if (this.enabled && !this.started) return;

            Object.Destroy(this);
        }


        private void UpdateMaintainOnLoadStatus()
        {
            //for singletons not on the primary singleton source
            if (!_flaggedSelfMaintaining && this.gameObject != Singleton.GameObjectSource && this.MaintainOnLoad)
            {
                GameObject.DontDestroyOnLoad(this.gameObject);
                _flaggedSelfMaintaining = true;
            }
        }

        #endregion

    }

    public class SingletonManager : Singleton
    {

        protected override void OnLevelWasLoaded(int level)
        {
            if (this.MaintainOnLoad) return;

            //OnLevelWasLoaded gets called on the objects in the scene being loaded, we haven't started at that point, so ignore this
            if (this.enabled && !this.started) return;

            Object.Destroy(this.gameObject);
        }

    }

}
