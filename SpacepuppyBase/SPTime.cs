using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

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

        public static IScalableTimeSupplier Normal { get { return _normalTime; } }

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

        public static CustomTimeSupplier CreateCustomTime(string id)
        {
            if (_customTimes == null)
            {
                _customTimes = new Dictionary<string, CustomTimeSupplier>();
                GameLoopEntry.RegisterInternalEarlyUpdate(SPTime.Update);
            }

            if(_customTimes.ContainsKey(id))
            {
                var ct = _customTimes[id];
                return ct;
            }
            else
            {
                var ct = new CustomTimeSupplier(id);
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

        private class NormalTimeSupplier : IScalableTimeSupplier
        {

            private bool _paused;
            private Dictionary<string, float> _scales = new Dictionary<string, float>();

            public float Total
            {
                get { return UnityEngine.Time.time; }
            }

            public float Delta
            {
                get { return UnityEngine.Time.deltaTime; }
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
                        UnityEngine.Time.timeScale = 0f;
                    }
                    else
                    {
                        UnityEngine.Time.timeScale = this.GetTimeScale();
                    }
                }
            }

            public float Scale
            {
                get
                {
                    return (_paused) ? this.GetTimeScale() : UnityEngine.Time.timeScale;
                }
            }

            public IEnumerable<string> ScaleIds
            {
                get { return _scales.Keys; }
            }

            public void SetScale(string id, float scale)
            {
                _scales[id] = scale;
                if(!_paused)
                {
                    Time.timeScale = this.GetTimeScale();
                }
            }

            public float GetScale(string id)
            {
                float result;
                if(_scales.TryGetValue(id, out result))
                {
                    return result;
                }
                else
                {
                    return float.NaN;
                }
            }

            public bool RemoveScale(string id)
            {
                if (_scales.Remove(id))
                {
                    if (!_paused)
                    {
                        Time.timeScale = this.GetTimeScale();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool HasScale(string id)
            {
                return _scales.ContainsKey(id);
            }

            private float GetTimeScale()
            {
                if(_scales.Count == 0)
                {
                    return 1f;
                }
                else
                {
                    return _scales.Values.Product();
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

        }

        #endregion

    }

}
