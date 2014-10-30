using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public class Singleton : SPComponent
    {

        #region Static Interface

        public const string GAMEOBJECT_NAME = "Spacepuppy.SingletonSource";

        private static GameObject _gameObject;

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

            var single = Singleton.GameObjectSource.GetComponent(tp);
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent(tp) as Singleton;
            }
            return single as Singleton;
        }

        public static Singleton Instance(string stype)
        {
            var single = Singleton.GameObjectSource.GetComponent(stype);
            if (single == null)
            {
                single = Singleton.GameObjectSource.AddComponent(stype);
            }
            if (!(single is Singleton))
            {
                Object.Destroy(single);
                throw new System.ArgumentException("Type must inherit from Singleton.", "tp");
            }

            return single as Singleton;
        }

        #endregion

        #region Singleton Enforcement

        protected virtual void Awake()
        {
            var c = Singleton.GameObjectSource.GetComponent(this.GetType());
            if (!Object.ReferenceEquals(c, this))
            {
                Object.Destroy(this);
                throw new System.InvalidOperationException("Attempted to create an instance of a Singleton out of its appropriate operating bounds.");
            }
        }

        #endregion

    }

    public static class Singleton<T> where T : Singleton
    {

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Singleton.Instance<T>();

                return _instance;
            }
        }
    }

    public class SingletonManager : Singleton
    {

        public bool MaintainSingletonsOnLoad = true;

        protected override void Awake()
        {
            base.Awake();

            if (MaintainSingletonsOnLoad)
                GameObject.DontDestroyOnLoad(this.gameObject);
        }

    }

}
