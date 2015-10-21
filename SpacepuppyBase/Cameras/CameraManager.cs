using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{

    [Singleton.Config(DefaultLifeCycle = SingletonLifeCycleRule.LivesForever, ExcludeFromSingletonManager = true, LifeCycleReadOnly = true)]
    public class CameraManager : Singleton
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
        }

        private void OnLevelWasLoaded(int index)
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
        
        #endregion

    }
}
