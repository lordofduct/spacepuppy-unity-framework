using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public static class SPTime
    {

        #region Fields

        private static Dictionary<string, CustomTimeSupplier> _customTimes;
        private static NormalTimeSupplier _normalTime = new NormalTimeSupplier();
        private static RealTimeSupplier _realTime = new RealTimeSupplier();
        private static SmoothTimeSupplier _smoothTime = new SmoothTimeSupplier();

        #endregion

        #region Properties

        public static ITimeSupplier Normal { get { return _normalTime; } }

        public static ITimeSupplier Real { get { return _realTime; } }

        public static ITimeSupplier Smooth { get { return _smoothTime; } }

        #endregion

        #region Methods

        public static ITimeSupplier GetTime(DeltaTimeType etp)
        {
            switch(etp)
            {
                case DeltaTimeType.Normal:
                    return _normalTime;
                case DeltaTimeType.Real:
                    return _realTime;
                case DeltaTimeType.Smooth:
                    return _smoothTime;
                default:
                    return _customTimes.Values.FirstOrDefault();
            }
        }

        public static DeltaTimeType GetDeltaType(ITimeSupplier time)
        {
            if (time == _normalTime)
                return DeltaTimeType.Normal;
            else if (time == _realTime)
                return DeltaTimeType.Real;
            else if (time == _smoothTime)
                return DeltaTimeType.Smooth;
            else
                return DeltaTimeType.Custom;
        }

        public static CustomTimeSupplier CreateCustomTime(string id, float scale = 1f)
        {
            if (_customTimes == null)
            {
                _customTimes = new Dictionary<string, CustomTimeSupplier>();
                GameLoopEntry.RegisterInternalEarlyUpdate(SPTime.Update);
            }

            if(_customTimes.ContainsKey(id))
            {
                var ct = _customTimes[id];
                ct.Scale = scale;
                return ct;
            }
            else
            {
                var ct = new CustomTimeSupplier(id, scale);
                _customTimes[ct.Id] = ct;
                return ct;
            }
        }

        public static bool RemoveCustomTime(string id)
        {
            if (_customTimes != null) return _customTimes.Remove(id);
            else return false;
        }

        public static bool RemoveCustomTime(CustomTimeSupplier time)
        {
            if(_customTimes != null)
            {
                if(_customTimes.ContainsKey(time.Id) && _customTimes[time.Id] == time)
                {
                    return _customTimes.Remove(time.Id);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static CustomTimeSupplier Custom(string id)
        {
            if (_customTimes == null) return null;
            return _customTimes[id];
        }

        private static void Update(bool isFixed)
        {
            if(_customTimes.Count > 0)
            {
                foreach (var ct in _customTimes.Values)
                {
                    ct.Update(isFixed);
                }
            }
        }

        #endregion

        #region Special Types

        private class NormalTimeSupplier : ITimeSupplier
        {

            private bool _paused;
            private float _cachedTimeScale;

            public float Total
            {
                get { return UnityEngine.Time.time; }
            }

            public float Delta
            {
                get { return UnityEngine.Time.deltaTime; }
            }

            public float Scale
            {
                get
                {
                    return (_paused) ? _cachedTimeScale : UnityEngine.Time.timeScale;
                }
                set
                {
                    if (_paused)
                        _cachedTimeScale = value;
                    else
                        UnityEngine.Time.timeScale = value;
                }
            }

            public bool Paused
            {
                get { return _paused; }
                set
                {
                    if (_paused == value) return;
                    _paused = value;
                    if (_paused)
                    {
                        _cachedTimeScale = UnityEngine.Time.timeScale;
                        UnityEngine.Time.timeScale = 0f;
                    }
                    else
                    {
                        UnityEngine.Time.timeScale = _cachedTimeScale;
                    }
                }
            }
        }

        private class RealTimeSupplier : ITimeSupplier
        {

            public float Total
            {
                get { return UnityEngine.Time.unscaledTime; }
            }

            public float Delta
            {
                get { return UnityEngine.Time.unscaledDeltaTime; }
            }

            public float Scale
            {
                get
                {
                    return 1.0f;
                }
                set
                {
                    //Real Time can not be scaled
                }
            }

            public bool Paused
            {
                get { return false; } 
                set {
                    //Real Time can not be paused
                }
            }

        }

        private class SmoothTimeSupplier : ITimeSupplier
        {

            public float Total
            {
                get { return UnityEngine.Time.time; }
            }

            public float Delta
            {
                get { return UnityEngine.Time.smoothDeltaTime; }
            }

            public float Scale
            {
                get
                {
                    return UnityEngine.Time.timeScale;
                }
                set
                {
                    UnityEngine.Time.timeScale = value;
                }
            }

            public bool Paused
            {
                get { return SPTime.Normal.Paused; }
                set { SPTime.Normal.Paused = value; }
            }

        }

        #endregion

    }

}
