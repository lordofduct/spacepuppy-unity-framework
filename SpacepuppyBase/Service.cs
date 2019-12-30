#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using com.spacepuppy.Collections;
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
        /// <summary>
        /// Signals if a service has been disposed of in a manner that may not have allowed the proper Services channels to be called. 
        /// Such as if a Component is destroyed.
        /// </summary>
        event System.EventHandler Disposed;
        event System.EventHandler ServiceUnregistered;
        void SignalServiceUnregistered();
    }

    public interface IServiceDecorator<T> : IService where T : IService
    {
        void Consume(T service);
    }

    /// <summary>
    /// Access point for all registered Services.
    /// 
    /// Note - the reflected System.Type versions of these methods will not work on AOT systems.
    /// </summary>
    public static class Services
    {

        private static HashSet<IService> _services = new HashSet<IService>();
        private static Dictionary<System.Type, IService> _serviceTable = new Dictionary<Type, IService>();
        private static Dictionary<System.Type, System.Reflection.FieldInfo> _staticFieldTable = new Dictionary<Type, System.Reflection.FieldInfo>();

        #region Methods

        /// <summary>
        /// Returns true if the service is registered directly with any look-up type. 
        /// Note this may return false if the service is decorated. 
        /// Use the 'Exists' method if you want to know if a service is available.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static bool IsManaging(IService service)
        {
            return service != null && _services.Contains(service);
        }

        public static IService[] GetAll()
        {
            return _services.ToArray();
        }

        public static int GetAll(ICollection<IService> results)
        {
            foreach (var service in _services)
            {
                results.Add(service);
            }
            return _services.Count;
        }

        public static void ClearAll(bool destroyIfCan = false, bool donotSignalUnregister = false)
        {
            using (var types = TempCollection.GetSet<System.Type>(_serviceTable.Keys))
            using(var services = TempCollection.GetSet<IService>(_serviceTable.Values))
            {
                //Replace references
                var e2 = types.GetEnumerator();
                while (e2.MoveNext())
                {
                    var field = GetStaticEntryFieldInfo(e2.Current, false);
                    if (field != null) field.SetValue(null, null);
                }

                //Signal
                if (!donotSignalUnregister)
                {
                    foreach (var service in services)
                    {
                        if (service is UnityEngine.Object)
                            service.SignalServiceUnregistered();
                    }
                }

                if (destroyIfCan)
                {
                    foreach(var service in services)
                    {
                        if(service is UnityEngine.Object)
                            ObjUtil.SmartDestroy(service as UnityEngine.Object);
                    }
                }
            }
        }

        public static bool Exists<T>() where T : class, IService
        {
            return !Entry<T>.Instance.IsNullOrDestroyed();
        }

        public static bool Exists(System.Type tp)
        {
            IService serv;
            if (_serviceTable.TryGetValue(tp, out serv))
                return !serv.IsNullOrDestroyed();

            return false;
        }

        public static T Get<T>() where T : class, IService
        {
            //var result = Entry<T>.Instance;
            //if (!object.ReferenceEquals(result, null) && result.IsNullOrDestroyed())
            //{
            //    result = null;
            //    Entry<T>.Instance = null;
            //}
            //return result;
            return Entry<T>.Instance;
        }

        public static IService Get(Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(IService).IsAssignableFrom(tp)) throw new System.ArgumentException("Type is not a IService");
            
            IService result;
            if (_serviceTable.TryGetValue(tp, out result))
            {
                return result;
            }
            return null;
        }

        public static void Register<T>(T service) where T : class, IService
        {
            if (service.IsNullOrDestroyed()) throw new System.ArgumentNullException("service");
            var other = Entry<T>.Instance;
            if (!other.IsNullOrDestroyed() && !object.ReferenceEquals(other, service)) throw new System.InvalidOperationException("You must first unregister a service before registering a new one.");

            _services.Add(service);
            Entry<T>.Instance = service;
            _serviceTable[typeof(T)] = service;

            //make sure we don't add it multiple times
            service.Disposed -= OnServiceInadvertentlyDisposed;
            service.Disposed += OnServiceInadvertentlyDisposed;

            //register fieldinfo
            if(!_staticFieldTable.ContainsKey(typeof(T)))
                _staticFieldTable[typeof(T)] = typeof(Entry<T>).GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        }

        public static void Register(System.Type tp, IService service)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (service.IsNullOrDestroyed()) throw new System.ArgumentNullException("service");
            if (!typeof(IService).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must implement IService.", "tp");
            if (!tp.IsAssignableFrom(service.GetType())) throw new System.ArgumentException(string.Format("Service must implement the service type '{0}' it's being registered for.", tp.Name), "tp");

            System.Reflection.FieldInfo field = GetStaticEntryFieldInfo(tp, true);
            if (field == null) throw new System.InvalidOperationException("Failed to resolve type '" + tp.Name + "'.");

            var other = field.GetValue(null);
            if (!other.IsNullOrDestroyed() && !object.ReferenceEquals(other, service)) throw new System.InvalidOperationException("You must first unregister a service before registering a new one.");

            _services.Add(service);
            field.SetValue(null, service);
            _serviceTable[tp] = service;

            //make sure we don't add it multiple times
            service.Disposed -= OnServiceInadvertentlyDisposed;
            service.Disposed += OnServiceInadvertentlyDisposed;
        }


        /// <summary>
        /// Unregister the service associated with type 'T'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destroyIfCan">Forcefully destroy the service if possible.</param>
        /// <param name="donotSignalUnregister">Suppress the signaling to the service that it is being unregistered. This is useful if a service is unregistering itself.</param>
        public static void Unregister<T>(bool destroyIfCan = false, bool donotSignalUnregister = false) where T : class, IService
        {
            Unregister(Get<T>(), destroyIfCan, donotSignalUnregister);
        }

        /// <summary>
        /// Unregister a service associated with type 'tp'
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="destroyIfCan">Forcefully destroy the service if possible.</param>
        /// <param name="donotSignalUnregister">Suppress the signaling to the service that it is being unregistered. This is useful if a service is unregistering itself.</param>
        public static void Unregister(System.Type tp, bool destroyIfCan = false, bool donotSignalUnregister = false)
        {
            Unregister(Get(tp), destroyIfCan, donotSignalUnregister);
        }

        /// <summary>
        /// Unregister a service from all the types it's associated.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="destroyIfCan">Forcefully destroy the service if possible.</param>
        /// <param name="donotSignalUnregister">Suppress the signaling to the service that it is being unregistered. This is useful if a service is unregistering itself.</param>
        public static void Unregister(IService service, bool destroyIfCan = false, bool donotSignalUnregister = false)
        {
            if (service == null || !_services.Contains(service)) return;

            //Remove reflected references
            ReplaceReferences(service, null);

            //Signal
            if (!donotSignalUnregister)
                service.SignalServiceUnregistered();

            if (destroyIfCan && service is UnityEngine.Object)
                ObjUtil.SmartDestroy(service as UnityEngine.Object);
        }

        /// <summary>
        /// Replace all references to 'old' with 'replacement' as a service. 
        /// This is usually used when decorating an existing service.
        /// </summary>
        /// <param name="old">The service to replace</param>
        /// <param name="replacement">The service to replace with</param>
        /// <param name="keepOldIfCannotReplace">If the service look-up type does not match the replacement type, should 'old' be left in place or removed for that look-up type</param>
        private static void ReplaceReferences(IService old, IService replacement)
        {
            if (old != null)
            {
                _services.Remove(old);
                old.Disposed -= OnServiceInadvertentlyDisposed;
            }
            if (replacement != null)
            {
                _services.Add(replacement);

                //make sure we don't add it multiple times
                replacement.Disposed -= OnServiceInadvertentlyDisposed;
                replacement.Disposed += OnServiceInadvertentlyDisposed;
            }

            //Find all references
            using (var cache = TempCollection.GetSet<System.Type>())
            {
                var e1 = _serviceTable.GetEnumerator();
                while (e1.MoveNext())
                {
                    if (object.ReferenceEquals(e1.Current.Value, old))
                    {
                        cache.Add(e1.Current.Key);
                    }
                }

                //Replace references
                var rtp = replacement != null ? replacement.GetType() : null;
                var e2 = cache.GetEnumerator();
                while (e2.MoveNext())
                {
                    bool cannotReplace = (rtp != null && !e2.Current.IsAssignableFrom(rtp));

                    if (cannotReplace || replacement == null)
                        _serviceTable.Remove(e2.Current);
                    else
                        _serviceTable[e2.Current] = replacement;


                    var field = GetStaticEntryFieldInfo(e2.Current, false);
                    if (field != null) field.SetValue(null, cannotReplace ? null : replacement);
                }
            }
        }




        public static T CreateUnregisteredService<T>(bool persistent = false, string name = null) where T : class, IService
        {
            var tp = typeof(T);
            if (typeof(Component).IsAssignableFrom(tp))
            {
                return ServiceComponent<T>.Create(tp, persistent, name);
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(tp))
            {
                return ServiceScriptableObject<T>.Create(tp, persistent, name);
            }
            else
            {
                try
                {
                    return System.Activator.CreateInstance<T>();
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        public static TServiceType CreateUnregisteredService<TServiceType>(System.Type concreteType, bool persistent = false, string name = null) where TServiceType : class, IService
        {
            if (concreteType == null) throw new System.ArgumentNullException("concreteType");
            if (!typeof(TServiceType).IsAssignableFrom(concreteType)) throw new System.ArgumentException("Type must implement " + typeof(TServiceType).Name);

            if (typeof(Component).IsAssignableFrom(concreteType))
            {
                var obj = ServiceComponent<TServiceType>.Create(concreteType, persistent, name);
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(concreteType))
            {
                var obj = ServiceScriptableObject<TServiceType>.Create(concreteType, persistent, name);
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else
            {
                try
                {
                    var obj = System.Activator.CreateInstance(concreteType) as TServiceType;
                    Services.Register<TServiceType>(obj);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        public static TConcrete Create<TServiceType, TConcrete>(bool persistent = false, string name = null) where TServiceType : class, IService
                                                                                                                where TConcrete : class, TServiceType
        {
            var inst = Services.Get<TServiceType>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(TServiceType).Name));

            var serv = CreateUnregisteredService<TConcrete>(persistent, name);
            if (serv == null) return null;

            Register<TServiceType>(serv);
            return serv;
        }

        public static T Create<T>(bool persistent = false, string name = null) where T : class, IService
        {
            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            var serv = CreateUnregisteredService<T>(persistent, name);
            if (serv == null) return null;

            Register<T>(serv);
            return serv;
        }

        public static TServiceType Create<TServiceType>(System.Type concreteType, bool persistent = false, string name = null) where TServiceType : class, IService
        {
            var inst = Services.Get<TServiceType>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(TServiceType).Name));

            var serv = CreateUnregisteredService<TServiceType>(concreteType, persistent, name);
            if (serv == null) return null;

            Register<TServiceType>(serv);
            return serv;
        }

        public static TServiceType GetOrCreate<TServiceType, TConcrete>(bool persistent = false, string name = null) where TServiceType : class, IService
                                                                                                                     where TConcrete : class, TServiceType
        {
            var inst = Services.Get<TServiceType>();
            if (inst != null) return inst;

            var serv = CreateUnregisteredService<TConcrete>(persistent, name);
            if (serv == null) return null;

            Register<TServiceType>(serv);
            return serv;
        }

        public static T GetOrCreate<T>(bool persistent = false, string name = null) where T : class, IService
        {
            var inst = Services.Get<T>();
            if (inst != null) return inst;

            var serv = CreateUnregisteredService<T>(persistent, name);
            if (serv == null) return null;

            Register<T>(serv);
            return serv;
        }

        public static TServiceType GetOrCreate<TServiceType>(System.Type concreteType, bool persistent = false, string name = null) where TServiceType : class, IService
        {
            var inst = Services.Get<TServiceType>();
            if (inst != null) return inst;

            var serv = CreateUnregisteredService<TServiceType>(concreteType, persistent, name);
            if (serv == null) return null;

            Register<TServiceType>(serv);
            return serv;
        }




        public static TServiceType CreateDecorator<TServiceType, TDecoratorType>(bool persistent = false, string name = null) where TServiceType : class, IService
                                                                                                                              where TDecoratorType : class, TServiceType, IServiceDecorator<TServiceType>
        {
            var inst = Services.Get<TServiceType>();
            if (inst == null) throw new System.InvalidOperationException(string.Format("Must create a concreate service of type {0} before decorating it.", typeof(TServiceType).Name));
            
            var decorator = CreateUnregisteredService<TDecoratorType>(persistent, name);
            if (decorator == null) return null;
            
            ReplaceReferences(inst, decorator);
            Entry<TServiceType>.Instance = decorator;

            decorator.Consume(inst);
            return decorator;
        }

        public static TServiceType CreateDecorator<TServiceType>(System.Type decoratorType, bool persistent = false, string name = null) where TServiceType : class, IService
        {
            var inst = Services.Get<TServiceType>();
            if (inst == null) throw new System.InvalidOperationException(string.Format("Must create a concreate service of type {0} before decorating it.", typeof(TServiceType).Name));

            if (decoratorType == null) throw new System.ArgumentNullException("decoratorType");
            if (!typeof(TServiceType).IsAssignableFrom(decoratorType)) throw new System.ArgumentException("Decorator Type must implement " + typeof(TServiceType).Name);
            if (!typeof(IServiceDecorator<TServiceType>).IsAssignableFrom(decoratorType)) throw new System.ArgumentException("Decorator Type must implement IServiceDecorator");

            var replacement = CreateUnregisteredService<TServiceType>(decoratorType, persistent, name);
            var decorator = replacement as IServiceDecorator<TServiceType>;
            if (decorator == null) return null;

            Entry<TServiceType>.Instance = replacement;
            ReplaceReferences(inst, replacement);

            decorator.Consume(inst);
            return replacement;
        }

        public static void RegisterDecorator<TServiceType>(IServiceDecorator<TServiceType> decorator) where TServiceType : class, IService
        {
            if (decorator == null) throw new System.ArgumentNullException("decorator");
            if (!(decorator is TServiceType)) throw new System.ArgumentException(string.Format("Decorator must implement the service type it is decorating: {0}.", typeof(TServiceType).Name));

            var inst = Services.Get<TServiceType>();
            if (inst == null) throw new System.InvalidOperationException(string.Format("Must create a concreate service of type {0} before decorating it.", typeof(TServiceType).Name));

            Entry<TServiceType>.Instance = decorator as TServiceType;
            ReplaceReferences(inst, decorator);

            decorator.Consume(inst);
        }

        #endregion

        #region Dispose Handler

        private static System.EventHandler _onServiceInadvertentlyDisposed;
        private static System.EventHandler OnServiceInadvertentlyDisposed
        {
            get
            {
                if(_onServiceInadvertentlyDisposed == null)
                {
                    _onServiceInadvertentlyDisposed = (s, e) =>
                    {
                        if(s is IService)
                        {
                            Unregister(s as IService, false, true);
                        }
                    };
                }
                return _onServiceInadvertentlyDisposed;
            }
        }

        #endregion

        #region Special Types

        private static System.Reflection.FieldInfo GetStaticEntryFieldInfo(System.Type tp, bool createIfNotFound)
        {
            System.Reflection.FieldInfo result = null;
            if (_staticFieldTable.TryGetValue(tp, out result))
                return result;

            if(createIfNotFound)
            {
                try
                {
                    var klass = typeof(Entry<>);
                    klass = klass.MakeGenericType(tp);
                    result = klass.GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    _staticFieldTable[tp] = result;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Failed to resolve type '" + tp.Name + "'.", ex);
                }
            }

            return result;
        }

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
    public abstract class ServiceComponent<T> : SPComponent, IService where T : class, IService
    {

        #region Fields

        [SerializeField]
        private bool _autoRegisterService;
        [SerializeField]
        private bool _destroyIfMultiple;

        #endregion

        #region CONSTRUCTOR

        public ServiceComponent()
        {

        }

        public ServiceComponent(bool autoRegister, bool destroyIfMultiple)
        {
            _autoRegisterService = autoRegister;
            _destroyIfMultiple = destroyIfMultiple;
        }

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
                if (_autoRegisterService)
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
            //we unregister before signaling destroyed
            Services.Unregister(this, false, true);

            base.OnDestroy();
        }

        #endregion

        #region IService Interface

        event System.EventHandler IService.Disposed
        {
            add { this.ComponentDestroyed += value; }
            remove { this.ComponentDestroyed -= value; }
        }

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

        #region Static Factory

        public static T Get()
        {
            return Services.Get<T>();
        }

        public static TConcrete Create<TConcrete>(bool persistent = false, string name = null) where TConcrete : Component, T
        {
            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent<TConcrete>();
        }

        public static T Create(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(Component).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent(tp) as T;
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

        public static T GetOrCreate(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(Component).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent(tp) as T;
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
    public abstract class ServiceScriptableObject<T> : ScriptableObject, IService where T : class, IService
    {

        #region Fields

        [SerializeField]
        private bool _autoRegisterService;
        [SerializeField]
        private bool _destroyIfMultiple;
        [System.NonSerialized]
        private System.EventHandler _onDisposed;

        #endregion

        #region CONSTRUCTOR

        public ServiceScriptableObject()
        {

        }

        public ServiceScriptableObject(bool autoRegister, bool destroyIfMultiple)
        {
            _autoRegisterService = autoRegister;
            _destroyIfMultiple = destroyIfMultiple;
        }

        protected virtual void OnEnable() //NOTE - using OnEnable now since it appears Awake doesn't occur on SOs that are created as an asset and loaded that way.
        {
            if (!(this is T))
            {
                this.AutoDestruct();
                return;
            }

            var inst = Services.Get<T>();
            if (inst == null)
            {
                if(_autoRegisterService)
                    Services.Register<T>(this as T);
            }
            else if (object.ReferenceEquals(this, inst))
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
        
        protected virtual void OnDestroy()
        {
            Services.Unregister(this, false, true);

            var d = _onDisposed;
            if (d != null) d(this, System.EventArgs.Empty);
        }

        #endregion

        #region IService Interface

        event System.EventHandler IService.Disposed
        {
            add { _onDisposed += value; }
            remove { _onDisposed -= value; }
        }

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

        #region Static Factory

        public static T Get()
        {
            return Services.Get<T>();
        }

        public static TConcrete Create<TConcrete>(bool persistent = false, string name = null) where TConcrete : ScriptableObject, T
        {
            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj =  ScriptableObject.CreateInstance<TConcrete>();
            obj.name = name;
            return obj;
        }

        public static T Create(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(ScriptableObject).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj = ScriptableObject.CreateInstance(tp) as ScriptableObject;
            obj.name = name;
            return obj as T;
        }

        public static T GetOrCreate<TConcrete>(bool persistent = false, string name = null) where TConcrete : ScriptableObject, T
        {
            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj = ScriptableObject.CreateInstance<TConcrete>();
            obj.name = name;
            return obj;
        }

        public static T GetOrCreate(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(ScriptableObject).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj = ScriptableObject.CreateInstance(tp) as ScriptableObject;
            obj.name = name;
            return obj as T;
        }

        #endregion


    }


    /// <summary>
    /// Inherit from this class, and add a 'CreateAssetMenuAttribute' to create a asset that can be used as a proxy to a Service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceProxy<T> : ScriptableObject, IProxy where T : class, IService
    {

        bool IProxy.QueriesTarget
        {
            get { return false; }
        }

        public object GetTarget()
        {
            return Services.Get<T>();
        }

        public object GetTarget(object arg)
        {
            return Services.Get<T>();
        }

        public System.Type GetTargetType()
        {
            return typeof(T);
        }

    }
    
}
