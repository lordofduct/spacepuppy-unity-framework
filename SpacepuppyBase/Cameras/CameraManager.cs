using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;
using System;
using System.Collections;

namespace com.spacepuppy.Cameras
{

    public static class CameraPool
    {

        #region Static Interface

        private static HashSet<ICamera> _cameras = new HashSet<ICamera>();

        #endregion

        #region Methods

        /// <summary>
        /// Camera considered the main currently. This is the first camera found with 'MainCamera' tag, if not manually set.
        /// </summary>
        public static ICamera Main
        {
            get
            {
                var manager = Services.Get<ICameraManager>();
                if (manager != null)
                    return manager.Main;

                var cam = Camera.main;
                if (cam != null)
                    return cam.AddOrGetComponent<UnityCamera>();

                return null;
            }
            set
            {
                var manager = Services.Get<ICameraManager>();
                if (manager != null)
                    manager.Main = value;
            }
        }

        public static void Register(ICamera cam)
        {
            if (cam == null) throw new System.ArgumentNullException("cam");
            if (GameLoopEntry.ApplicationClosing) return;
            if(_cameras.Add(cam))
            {
                var manager = Services.Get<ICameraManager>();
                if (manager != null)
                    manager.OnRegistered(cam);
            }
        }


        public static void Unregister(ICamera cam)
        {
            if (cam == null) throw new System.ArgumentNullException("cam");
            if (GameLoopEntry.ApplicationClosing) return;
            if(_cameras.Remove(cam))
            {
                var manager = Services.Get<ICameraManager>();
                if (manager != null)
                    manager.OnUnregistered(cam);
            }
        }

        public static IEnumerable<ICamera> All
        {
            get
            {
                return _cameras;
            }
        }

        public static IEnumerable<ICamera> Group(string tag)
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.gameObject.HasTag(tag)) yield return e.Current;
            }
        }


        public static ICamera[] FindAllCameraControllers(System.Func<ICamera, bool> predicate = null)
        {
            using (var coll = com.spacepuppy.Collections.TempCollection.GetCollection<ICamera>())
            {
                FindAllCameraControllers(coll, predicate);
                return coll.ToArray();
            }
        }

        public static void FindAllCameraControllers(System.Collections.Generic.ICollection<ICamera> coll, System.Func<ICamera, bool> predicate = null)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            if (predicate == null)
            {
                var e = _cameras.GetEnumerator();
                while (e.MoveNext())
                {
                    coll.Add(e.Current);
                }

                var ucams = Camera.allCameras;
                foreach (var c in ucams)
                {
                    coll.Add(c.AddOrGetComponent<UnityCamera>());
                }
            }
            else
            {
                var e = _cameras.GetEnumerator();
                while (e.MoveNext())
                {
                    if (predicate(e.Current)) coll.Add(e.Current);
                }

                var ucams = Camera.allCameras;
                foreach (var c in ucams)
                {
                    var uc = c.AddOrGetComponent<UnityCamera>();
                    if (predicate(uc)) coll.Add(uc);
                }
            }
        }

        public static ICamera FindCameraController(System.Func<ICamera, bool> predicate)
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (predicate(e.Current)) return e.Current;
            }

            return null;
        }

        public static ICamera FindCameraController(Camera cam)
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Contains(cam)) return e.Current;
            }

            return cam.AddOrGetComponent<UnityCamera>();
        }

        public static ICamera FindTaggedMainCamera()
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.gameObject.HasTag(SPConstants.TAG_MAINCAMERA))
                {
                    return e.Current;
                }
            }

            var cams = Camera.allCameras;
            foreach (var cam in cams)
            {
                if (cam.HasTag(SPConstants.TAG_MAINCAMERA))
                {
                    return cam.AddOrGetComponent<UnityCamera>();
                }
            }

            return null;
        }


        public static IEnumerator<ICamera> GetEnumerator()
        {
            return new Enumerator(_cameras);
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<ICamera>
        {

            #region Fields

            private HashSet<ICamera>.Enumerator _e;

            #endregion

            #region CONSTRUCTOR

            internal Enumerator(HashSet<ICamera> set)
            {
                _e = set.GetEnumerator();
            }

            #endregion

            #region IEnumerator Interface

            public ICamera Current
            {
                get
                {
                    return _e.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _e.Current;
                }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                (_e as IEnumerator).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            #endregion

        }

        #endregion

    }

    public interface ICameraManager : IService
    {

        ICamera Main { get; set; }

        void OnRegistered(ICamera cam);
        void OnUnregistered(ICamera cam);

    }

    public class CameraManager : ServiceComponent<ICameraManager>, ICameraManager
    {
        
        #region Fields

        private ICamera _main;
        private bool _overrideAsNull;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            if (this.MainNeedsSyncing())
            {
                this.ForceSyncTaggedMainCamera();
            }

            SceneManager.sceneLoaded += this.OnSceneWasLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.sceneLoaded -= this.OnSceneWasLoaded;
        }

        private void OnSceneWasLoaded(Scene sc, LoadSceneMode mode)
        {
            if (this.MainNeedsSyncing())
            {
                this.ForceSyncTaggedMainCamera();
            }
        }

        #endregion

        #region Methods

        public void ForceSyncTaggedMainCamera()
        {
            _main = CameraPool.FindTaggedMainCamera();
            _overrideAsNull = false;
        }

        private bool MainNeedsSyncing()
        {
            if (_main == null)
                return !_overrideAsNull;
            else
                return !_main.IsAlive;
        }

        private bool AnyNonUnityCamerasContains(Camera c)
        {
            var e = CameraPool.GetEnumerator();
            while (e.MoveNext())
            {
                if (!(e.Current is UnityCamera) && e.Current.Contains(c)) return true;
            }
            return false;
        }

        #endregion

        #region ICameraManager Interface

        public ICamera Main
        {
            get
            {
                if (this.MainNeedsSyncing()) this.ForceSyncTaggedMainCamera();
                return _main;
            }
            set
            {
                _main = value;
                _overrideAsNull = (value == null);
            }
        }

        void ICameraManager.OnRegistered(ICamera cam)
        {
            if (this.started && this.MainNeedsSyncing())
            {
                this.ForceSyncTaggedMainCamera();
            }
        }

        void ICameraManager.OnUnregistered(ICamera cam)
        {
            if (_main == cam)
            {
                _main = null;
                if (this.started && !GameLoopEntry.ApplicationClosing)
                {
                    this.ForceSyncTaggedMainCamera();
                }
            }
        }

        #endregion

    }

    /*
    [Singleton.Config(SingletonLifeCycleRule.LivesForever, ExcludeFromSingletonManager = true, LifeCycleReadOnly = true)]
    public class CameraManager : Singleton, IEnumerable<ICamera>
    {

        #region Singleton Interface

        private static CameraManager _instance;
        internal static CameraManager Instance
        {
            get
            {
                if (object.ReferenceEquals(_instance, null)) _instance = Singleton.CreateSpecialInstance<CameraManager>("Spacepuppy.CameraManager", SingletonLifeCycleRule.LivesForever);
                return _instance;
            }
        }

        public static IEnumerable<ICamera> Cameras
        {
            get
            {
                return Instance;
            }
        }

        public static IEnumerable<ICamera> CameraGroup(string tag)
        {
            var e = Instance.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.gameObject.HasTag(tag)) yield return e.Current;
            }
        }

        #endregion

        #region Static Interface
        
        /// <summary>
        /// Camera considered the main currently. This is the first camera found with 'MainCamera' tag, if not manually set.
        /// </summary>
        public static ICamera Main
        {
            get
            {
                var inst = CameraManager.Instance;
                if (inst.MainNeedsSyncing()) CameraManager.ForceSyncTaggedMainCamera();
                return inst._main;
            }
            set
            {
                var inst = CameraManager.Instance;
                inst._main = value;
                inst._overrideAsNull = (value == null);
            }
        }

        public static ICamera[] FindAllCameraControllers(System.Func<ICamera, bool> predicate = null)
        {
            using (var coll = com.spacepuppy.Collections.TempCollection.GetCollection<ICamera>())
            {
                FindAllCameraControllers(coll, predicate);
                return coll.ToArray();
            }
        }

        public static void FindAllCameraControllers(System.Collections.Generic.ICollection<ICamera> coll, System.Func<ICamera, bool> predicate = null)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            if(predicate == null)
            {
                var e = CameraManager.Instance._cameras.GetEnumerator();
                while (e.MoveNext())
                {
                    coll.Add(e.Current);
                }

                var ucams = Camera.allCameras;
                foreach (var c in ucams)
                {
                    coll.Add(c.AddOrGetComponent<UnityCamera>());
                }
            }
            else
            {
                var e = CameraManager.Instance._cameras.GetEnumerator();
                while (e.MoveNext())
                {
                    if (predicate(e.Current)) coll.Add(e.Current);
                }

                var ucams = Camera.allCameras;
                foreach (var c in ucams)
                {
                    var uc = c.AddOrGetComponent<UnityCamera>();
                    if (predicate(uc)) coll.Add(uc);
                }
            }
        }

        public static ICamera FindCameraController(System.Func<ICamera, bool> predicate)
        {
            var e = CameraManager.Instance._cameras.GetEnumerator();
            while(e.MoveNext())
            {
                if (predicate(e.Current)) return e.Current;
            }

            return null;
        }

        public static ICamera FindCameraController(Camera cam)
        {
            var lst = CameraManager.Instance._cameras;
            var e = lst.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Contains(cam)) return e.Current;
            }

            return cam.AddOrGetComponent<UnityCamera>();
        }

        public static ICamera FindTaggedMainCamera()
        {
            var e = CameraManager.Instance._cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.gameObject.HasTag(SPConstants.TAG_MAINCAMERA))
                {
                    return e.Current;
                }
            }

            var cams = Camera.allCameras;
            foreach (var cam in cams)
            {
                if (cam.HasTag(SPConstants.TAG_MAINCAMERA))
                {
                    return cam.AddOrGetComponent<UnityCamera>();
                }
            }

            return null;
        }

        public static void ForceSyncTaggedMainCamera()
        {
            var inst = CameraManager.Instance;
            inst._main = CameraManager.FindTaggedMainCamera();
            inst._overrideAsNull = false;
        }

        #endregion




        #region Instance Interface

        #region Fields

        private HashSet<ICamera> _cameras = new HashSet<ICamera>();

        private ICamera _main;
        private bool _overrideAsNull;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            if(this.MainNeedsSyncing())
            {
                CameraManager.ForceSyncTaggedMainCamera();
            }

            SceneManager.sceneLoaded += this.OnSceneWasLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.sceneLoaded -= this.OnSceneWasLoaded;
        }
        
        private void OnSceneWasLoaded(Scene sc, LoadSceneMode mode)
        {
            if (this.MainNeedsSyncing())
            {
                CameraManager.ForceSyncTaggedMainCamera();
            }
        }

        #endregion

        #region Methods

        internal static void Register(ICamera cam)
        {
            if (GameLoopEntry.ApplicationClosing) return;
            CameraManager.Instance.Register_Imp(cam);
        }
        private void Register_Imp(ICamera cam)
        {
            if (_cameras.Contains(cam)) return;

            _cameras.Add(cam);
            if (this.started && this.MainNeedsSyncing())
            {
                CameraManager.ForceSyncTaggedMainCamera();
            }
        }

        internal static void UnRegister(ICamera cam)
        {
            if (GameLoopEntry.ApplicationClosing) return;
            CameraManager.Instance.UnRegister_Imp(cam);
        }
        private void UnRegister_Imp(ICamera cam)
        {
            if (cam == null) return;

            if (_cameras.Contains(cam)) _cameras.Remove(cam);
            if (_main == cam)
            {
                _main = null;
                if (this.started && !GameLoopEntry.ApplicationClosing)
                {
                    CameraManager.ForceSyncTaggedMainCamera();
                }
            }
        }
        


        private bool MainNeedsSyncing()
        {
            if (_main == null)
                return !_overrideAsNull;
            else
                return !_main.IsAlive;
        }

        private bool AnyNonUnityCamerasContains(Camera c)
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (!(e.Current is UnityCamera) && e.Current.Contains(c)) return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<ICamera> GetEnumerator()
        {
            return _cameras.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _cameras.GetEnumerator();
        }

        #endregion

        #endregion

    }
    */

}
