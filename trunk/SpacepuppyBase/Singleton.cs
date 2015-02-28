using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
                if (_gameObject == null)
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

            var single = Singleton.GameObjectSource.GetComponent(tp) as Singleton;
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent(tp) as Singleton;
            }
            return single;
        }

        public static T CreateSpecialInstance<T>(string gameObjectName) where T:Singleton
        {
            if (_singletonRefs.ContainsKey(typeof(T))) return _singletonRefs[typeof(T)] as T;

            var go = new GameObject(gameObjectName);
            var single = go.AddComponent<T>();
            return single;
        }

        public static Singleton CreateSpecialInstance(System.Type tp, string gameObjectName)
        {
            if (!typeof(Singleton).IsAssignableFrom(tp)) throw new TypeArgumentMismatchException(tp, typeof(Singleton), "tp");
            if (_singletonRefs.ContainsKey(tp)) return _singletonRefs[tp];

            var go = new GameObject(gameObjectName);
            var single = go.AddComponent(tp) as Singleton;
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

        public static IEnumerable<Singleton> Instances
        {
            get
            {
                return _singletonRefs.Values;
            }
        }

        #endregion

        #region Properties

        [SerializeField()]
        [Tooltip("Should this Singleton be maintained when a new scene is loaded.")]
        private bool _maintainOnLoad = true;
        [System.NonSerialized()]
        private bool _flaggedSelfMaintaining;

        public virtual bool MaintainOnLoad
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

        public Singleton()
        {
            var tp = this.GetType();
            if (!_singletonRefs.ContainsKey(tp)) _singletonRefs[tp] = this;
        }

        protected override void Awake()
        {
            base.Awake();

            var c = (_singletonRefs.ContainsKey(this.GetType())) ? _singletonRefs[this.GetType()] : null;
            //if (!System.Object.ReferenceEquals(c, null) && !System.Object.ReferenceEquals(c, this))
            //{
            //    Object.Destroy(this);
            //    throw new System.InvalidOperationException("Attempted to create an instance of a Singleton out of its appropriate operating bounds.");
            //}
            //else
            //{
            //    _singletonRefs[this.GetType()] = this;
            //}
            if(c == null || c == this)
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var tp = this.GetType();
            if(_singletonRefs.ContainsKey(tp))
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

            //for singletons not on the primary singleton source
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

        [SerializeField()]
        private bool _maintainAllSingletonsOnLoad = true;

        public bool MaintainAllSingletonsOnLoad { get { return _maintainAllSingletonsOnLoad; } }

        public override bool MaintainOnLoad
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (_maintainAllSingletonsOnLoad)
                GameObject.DontDestroyOnLoad(this.gameObject);
        }

        protected override void OnLevelWasLoaded(int level)
        {
            //block for SingletonManager
        }

    }

}
