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

        public static T Instance<T>() where T : Singleton
        {
            if (_singletonRefs.ContainsKey(typeof(T))) return _singletonRefs[typeof(T)] as T;

            var single = Singleton.GameObjectSource.GetComponent<T>();
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent<T>();
            }
            return single;
        }

        public static Singleton Instance(System.Type tp)
        {
            if (!typeof(Singleton).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must inherit from Singleton.", "tp");
            if (_singletonRefs.ContainsKey(tp)) return _singletonRefs[tp];

            var single = Singleton.GameObjectSource.GetComponent(tp) as Singleton;
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent(tp) as Singleton;
            }
            return single as Singleton;
        }

        #endregion

        #region Properties

        [SerializeField()]
        [Tooltip("Should this Singleton be maintained when a new scene is loaded.")]
        private bool _maintainOnLoad = true;
        public virtual bool MaintainOnLoad
        {
            get { return _maintainOnLoad; }
            set { _maintainOnLoad = value; }
        }

        #endregion

        #region Singleton Enforcement

        protected virtual void Awake()
        {
            var c = (_singletonRefs.ContainsKey(this.GetType())) ? _singletonRefs[this.GetType()] : null;
            if (!Object.ReferenceEquals(c, null) && !Object.ReferenceEquals(c, this))
            {
                Object.Destroy(this);
                throw new System.InvalidOperationException("Attempted to create an instance of a Singleton out of its appropriate operating bounds.");
            }
            else
            {
                _singletonRefs[this.GetType()] = this;
            }


            //for singletons not on the primary singleton source
            if (!this.gameObject == Singleton.GameObjectSource && this.MaintainOnLoad)
            {
                GameObject.DontDestroyOnLoad(this.gameObject);
            }
        }

        protected virtual void OnLevelWasLoaded(int level)
        {
            if (this.MaintainOnLoad) return;

            //for singletons not on the primary singleton source
            if (this.enabled && !this.started) return;
            Object.Destroy(this);
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
