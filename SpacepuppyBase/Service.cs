using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using com.spacepuppy.Utils;
using Type = System.Type;

namespace com.spacepuppy
{

    /// <summary>
    /// A special type of Singleton accessible by the Services static interface.
    /// 
    /// A service should implement some interface and is registered with Services as that interface. 
    /// This allows for a service to be accessed like a singleton, but implemented as an interface. 
    /// 
    /// See IGameInputManager, ICameraManager, and ISceneManager for examples.
    /// </summary>
    public interface IService : ISingleton
    {
        event System.EventHandler ServiceUnregistered;
        void SignalServiceUnregistered();
    }

    /// <summary>
    /// Access point for all registered Services.
    /// </summary>
    public static class Services
    {
        
        #region Methods

        public static T Get<T>() where T : class, IService
        {
            return Entry<T>.Instance;
        }

        public static object Get(Type tp)
        {
            if (tp == null || !tp.IsClass || typeof(IService).IsAssignableFrom(tp)) throw new System.ArgumentException("Type is not a IService");

            try
            {
                var klass = typeof(Entry<>);
                klass.MakeGenericType(tp);
                var field = klass.GetField("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                return field.GetValue(null);
            }
            catch(System.Exception ex)
            {
                throw new System.InvalidOperationException("Was unable to locate entry for type " + tp.Name, ex);
            }
        }
        
        public static void Register<T>(T service) where T : class, IService
        {
            Entry<T>.Instance = service;
        }

        public static void Unregister<T>() where T : class, IService
        {
            var inst = Entry<T>.Instance;
            if(!object.ReferenceEquals(inst, null))
            {
                Entry<T>.Instance = null;
                inst.SignalServiceUnregistered();
            }
        }

        #endregion

        #region Special Types

        private static class Entry<T> where T : class, IService
        {

            public static T Instance;

        }

        #endregion

    }

    /// <summary>
    /// Abstract component for implementing a Service Component. 
    /// 
    /// When implementing pass in the interface as the generic T parameter to denote as what type this service 
    /// should be accessed when calling Service.Get&ltT&gt.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Service<T> : SPComponent, IService where T : class, IService
    {

        #region Fields

        [SerializeField]
        private bool _destroyIfMultiple;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if(!(this is T))
            {
                this.AutoDestruct();
                return;
            }

            var inst = Services.Get<T>();
            if(inst == null)
            {
                Services.Register<T>(this as T);
            }
            else if(object.ReferenceEquals(this, inst))
            {
                //do nothing
            }
            else
            {
                this.AutoDestruct();
                return;
            }

            this.OnValidAwake();
        }

        private void AutoDestruct()
        {
            if (_destroyIfMultiple)
            {
                ObjUtil.SmartDestroy(this);
            }
        }
        
        protected virtual void OnValidAwake()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var inst = Services.Get<T>();
            if(object.ReferenceEquals(this, inst))
            {
                Services.Unregister<T>();
            }
        }

        #endregion

        #region Static Factory

        public static T Get()
        {
            return Services.Get<T>();
        }

        public static T Create<TConcrete>(bool persistent = false, string name = null) where TConcrete : Component, T
        {
            var inst = Services.Get<T>();
            if(inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if(name == null)
                name = "Service." + typeof (T).Name;
            var go = new GameObject(name);
            if(persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent<TConcrete>();
        }

        public static T GetOrCreate<TConcrete>(bool persistent = false, string name = null) where TConcrete : Component, T
        {
            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent<TConcrete>();
        }

        #endregion

        #region IService Interface

        public event System.EventHandler ServiceUnregistered;

        void IService.SignalServiceUnregistered()
        {
            this.OnServiceUnregistered();
            if (this.ServiceUnregistered != null) this.ServiceUnregistered(this, System.EventArgs.Empty);
        }

        protected virtual void OnServiceUnregistered()
        {
        }

        #endregion

    }

}
