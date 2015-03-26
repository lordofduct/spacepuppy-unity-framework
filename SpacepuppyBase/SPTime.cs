using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public static class SPTime
    {

        #region Fields

        private static Dictionary<string, CustomTime> _customTimes;
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

        public static CustomTime CreateCustomTime(string id, float scale = 1f)
        {
            if (_customTimes == null)
            {
                _customTimes = new Dictionary<string, CustomTime>();
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
                var ct = new CustomTime(id, scale);
                _customTimes[ct.Id] = ct;
                return ct;
            }
        }

        public static bool RemoveCustomTime(string id)
        {
            if (_customTimes != null) return _customTimes.Remove(id);
            else return false;
        }

        public static CustomTime Custom(string id)
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

        public class CustomTime : ITimeSupplier
        {

            #region Fields

            private string _id;
            private double _t;
            private double _ft;
            private double _dt;
            private double _scale;
            private bool _paused;

            #endregion

            #region CONSTRUCTOR

            internal CustomTime(string id, float scale)
            {
                _id = id;
                _scale = (double)scale;
            }

            #endregion

            #region Properties

            public string Id { get { return _id; } }

            /// <summary>
            /// The total time passed since thie CustomTime was created. Value is dependent on the UpdateSequence being accessed from.
            /// </summary>
            public float Total { get { return (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate) ? (float)_ft : (float)_t; } }

            /// <summary>
            /// The total time passed since the CustomTime was created. Value is relative to the Update sequence.
            /// </summary>
            public float UpdateTotal { get { return (float)_t; } }

            /// <summary>
            /// The total time passed since the CustomTime was created. Value is relative to the FixedUpdate sequence;
            /// </summary>
            public float FixedTotal { get { return (float)_ft; } }

            /// <summary>
            /// The delta time since the last call to update/fixedupdate, relative to in which update/fixedupdate you call.
            /// </summary>
            public float Delta { get { return (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate) ? (float)(_scale) * Time.fixedDeltaTime : (float)_dt; } }
            
            /// <summary>
            /// The delta time since the call to standard update. This will always return the delta since last update, regardless of if you call it in update/fixedupdate.
            /// </summary>
            public float UpdateDelta { get { return (float)_dt; } }

            /// <summary>
            /// The delta time since the call to fixed update. This will always return the delta since last fixedupdate, regardless of if you call it in update/fixedupdate.
            /// </summary>
            public float FixedDelta { get { return Time.fixedDeltaTime * (float)_scale; } }

            public float Scale
            {
                get { return (float)_scale; }
                set { _scale = (double)_scale; }
            }

            public bool Paused
            {
                get { return _paused; }
                set { _paused = value; }
            }

            #endregion

            #region Methods

            internal void Update(bool isFixed)
            {
                if (_paused) return;

                if(isFixed)
                {
                    _ft += Time.fixedDeltaTime * _scale;
                }
                else
                {
                    _dt = Time.unscaledDeltaTime * _scale;
                    _t += _dt;
                }
            }

            public bool Destroy()
            {
                return SPTime.RemoveCustomTime(_id);
            }

            #endregion

        }

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
