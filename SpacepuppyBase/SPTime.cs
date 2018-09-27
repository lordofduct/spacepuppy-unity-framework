using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy
{

    /// <summary>
    /// A static entry point to the various ITimeSuppliers in existance. An ITimeSupplier gives object identity 
    /// to the varous kinds of time out there: normal (UnityEngine.Time.time), real (UnityEngine.Time.unscaledTime), 
    /// smooth (Time.smoothDeltaTime), custom (no unity parrallel). 
    /// 
    /// With the objects in hand you can than swap out what time is used when updating objects.
    /// 
    /// For example the com.spacepuppy.Tween namespace containers tweeners that can update on any of the existing 
    /// times in unity. These objects can be passed in to allow updating at any one of those times. Lets say for 
    /// instance you've set the time scale of Normal time to 0 (to pause the game), but you still need the menu to 
    /// tween and update appropriately. Well, you'd use the 'RealTime' or a 'Custom' time object to update the 
    /// tween with instead of 'Normal'.
    /// 
    /// An added feature includes stacking TimeScales. The Normal and Custom TimeSuppliers allow naming your time 
    /// scales so that you can compound them together. This way time scales don't overlap. Lets say you have an 
    /// effect where when swinging the sword that halves the time speed to look cool, but you also have a slomo effect 
    /// when an item is picked up for a duration of time. You expect the swords half speed to be half of the slomo 
    /// effect, not half of normal time, so that the sword looks right either way. This allows you to combine those 
    /// 2 with out having to test if one or the other is currently playing (or the scale of it playing). Which is 
    /// SUPER extra handy when tweening the time scale.
    /// 
    /// 
    /// Instances of the class can be used as an inspector configurable reference to a ITimeSupplier. These configurable 
    /// instances are intended to be immutable once deserialized at runtime. Else the ITimeSupplier interface pass through 
    /// could add event callbacks to one time supplier, but later remove it from another.
    /// </summary>
    [System.Serializable()]
    public struct SPTime : ITimeSupplier
    {

        #region Fields

        [SerializeField()]
        private DeltaTimeType _timeSupplierType;
        [SerializeField()]
        private string _timeSupplierName;

        #endregion

        #region CONSTRUCTOR

        //public SPTime()
        //{
        //    //exists only for unity serialization
        //}

        public SPTime(DeltaTimeType etp)
        {
            if (etp == DeltaTimeType.Custom) throw new System.ArgumentException("For custom time suppliers, you must specify it directly.");
            _timeSupplierType = etp;
            _timeSupplierName = null;
        }

        public SPTime(ITimeSupplier ts)
        {
            if (ts == null) throw new System.ArgumentNullException("ts");
            _timeSupplierType = SPTime.GetDeltaType(ts);
            if (ts is CustomTimeSupplier) _timeSupplierName = (ts as CustomTimeSupplier).Id;
            else _timeSupplierName = null;
        }

        public SPTime(DeltaTimeType etp, string timeSupplierName)
        {
            if(etp == DeltaTimeType.Custom)
            {
                _timeSupplierType = etp;
                _timeSupplierName = timeSupplierName;
            }
            else
            {
                _timeSupplierType = etp;
                _timeSupplierName = null;
            }
        }

        #endregion

        #region Properties

        public DeltaTimeType TimeSupplierType
        {
            get { return _timeSupplierType; }
        }

        public string CustomTimeSupplierName
        {
            get { return _timeSupplierName; }
        }

        public ITimeSupplier TimeSupplier
        {
            get
            {
                return SPTime.GetTime(_timeSupplierType, _timeSupplierName);
            }
            set
            {
                _timeSupplierType = GetDeltaType(value);
                if (_timeSupplierType == DeltaTimeType.Custom)
                {
                    var cts = value as CustomTimeSupplier;
                    _timeSupplierName = (cts != null) ? cts.Id : null;
                }
                else
                {
                    _timeSupplierName = null;
                }
            }
        }
        
        public bool IsCustom
        {
            get { return _timeSupplierType == DeltaTimeType.Custom; }
        }

        public float Total
        {
            get
            {
                var ts = this.TimeSupplier;
                if (ts != null)
                    return ts.Total;
                else
                    return 0f;
            }
        }

        public float Delta
        {
            get
            {
                var ts = this.TimeSupplier;
                if (ts != null)
                    return ts.Delta;
                else
                    return 0f;
            }
        }

        public float Scale
        {
            get
            {
                var ts = this.TimeSupplier;
                if (ts != null)
                    return ts.Scale;
                else
                    return 0f;
            }
        }

        public double TotalPrecise
        {
            get
            {
                var ts = this.TimeSupplier;
                if (ts != null)
                    return ts.TotalPrecise;
                else
                    return 0d;
            }
        }

        #endregion




        #region Static Interface

        #region Fields

        private static Dictionary<string, CustomTimeSupplier> _customTimes;
        private static NormalTimeSupplier _normalTime = new NormalTimeSupplier();
        private static RealTimeSupplier _realTime = new RealTimeSupplier();
        private static SmoothTimeSupplier _smoothTime = new SmoothTimeSupplier();

        #endregion

        #region Properties

        /// <summary>
        /// Represents the normal update time, ala Time.time & Time.deltaTime.
        /// </summary>
        public static IScalableTimeSupplier Normal { get { return _normalTime; } }

        /// <summary>
        /// Represents the real update time, ala Time.unscaledTime.
        /// </summary>
        public static ITimeSupplier Real { get { return _realTime; } }

        /// <summary>
        /// Represents smooth delta time, ala Time.smoothDeltaTime.
        /// </summary>
        public static ITimeSupplier Smooth { get { return _smoothTime; } }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieve the ITimeSupplier for a specific DeltaTimeType, see com.spacepuppy.DeltaTimeType.
        /// </summary>
        /// <param name="etp"></param>
        /// <returns></returns>
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

        public static ITimeSupplier GetTime(DeltaTimeType etp, string id)
        {
            switch (etp)
            {
                case DeltaTimeType.Normal:
                    return _normalTime;
                case DeltaTimeType.Real:
                    return _realTime;
                case DeltaTimeType.Smooth:
                    return _smoothTime;
                default:
                    {
                        if (id == null) return null;
                        CustomTimeSupplier ct;
                        if (_customTimes.TryGetValue(id, out ct))
                        {
                            return ct;
                        }
                        return null;
                    }
            }
        }

        /// <summary>
        /// Reverse lookup for DeltaTimeType from an object.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DeltaTimeType GetDeltaType(ITimeSupplier time)
        {
            if (time == _normalTime || time == null)
                return DeltaTimeType.Normal;
            else if (time == _realTime)
                return DeltaTimeType.Real;
            else if (time == _smoothTime)
                return DeltaTimeType.Smooth;
            else
                return DeltaTimeType.Custom;
        }

        /// <summary>
        /// Retrieve a CustomTimeSupplier by name.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        public static CustomTimeSupplier Custom(string id, bool createIfNotExists = false)
        {
            if (id == null) return null;
            if (_customTimes == null)
            {
                if (createIfNotExists)
                {
                    _customTimes = new Dictionary<string, CustomTimeSupplier>();
                    GameLoopEntry.RegisterInternalEarlyUpdate(SPTime.Update);
                    var ct = new CustomTimeSupplier(id);
                    _customTimes[ct.Id] = ct;
                    return ct;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                CustomTimeSupplier ct;
                if (_customTimes.TryGetValue(id, out ct))
                {
                    return ct;
                }
                else if (createIfNotExists)
                {
                    ct = new CustomTimeSupplier(id);
                    _customTimes[ct.Id] = ct;
                    return ct;
                }
                else
                {
                    return null;
                }
            }
        }

        public static CustomTimeSupplier[] GetAllCustom()
        {
            if (_customTimes == null) return ArrayUtil.Empty<CustomTimeSupplier>();
            return _customTimes.Values.ToArray();
        }

        public static string GetCustomName(ITimeSupplier supplier)
        {
            if (_customTimes == null || supplier == null) return null;

            var e = _customTimes.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value == supplier) return e.Current.Key;
            }
            return null;
        }

        public static bool HasCustom(string id)
        {
            if (id == null) return false;
            if (_customTimes == null) return false;
            return _customTimes.ContainsKey(id);
        }

        /// <summary>
        /// Removes a CustomTimeSupplier from the update pool by name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool RemoveCustomTime(string id)
        {
            if (id == null) return false;
            if (_customTimes != null) return _customTimes.Remove(id);
            else return false;
        }

        /// <summary>
        /// Removes a CustomTimeSupplier from the update pool by reference.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool RemoveCustomTime(CustomTimeSupplier time)
        {
            if (_customTimes != null)
            {
                if (_customTimes.ContainsKey(time.Id) && _customTimes[time.Id] == time)
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

        /// <summary>
        /// Returns the scale relative to NormalTime that would cause something updating by normal time appear at the scale of 'supplier'.
        /// Basically if you have an Animation/Animator, which animates relative to Time.timeScale, and you want to set the 'speed' property of it 
        /// to a value so that it appeared at the speed that is defined in 'supplier', you'd set it to this value.
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static float GetInverseScale(ITimeSupplier supplier)
        {
            if (supplier == null) return 1f;
            if (supplier is NormalTimeSupplier) return 1f;

            return supplier.Scale / Time.timeScale;
        }

        private static void Update(bool isFixed)
        {
            if(_customTimes.Count > 0)
            {
                var e = _customTimes.GetEnumerator();
                while(e.MoveNext())
                {
                    e.Current.Value.Update(isFixed);
                }
            }
        }

        #endregion

        #endregion

        #region Special Types

        private class NormalTimeSupplier : IScalableTimeSupplier
        {

            private bool _paused;
            private Dictionary<string, float> _scales = new Dictionary<string, float>();

            public event System.EventHandler TimeScaleChanged;

            public float Total
            {
                get { return UnityEngine.Time.time; }
            }

            public double TotalPrecise
            {
                get { return (double)UnityEngine.Time.time; }
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
                        this.SyncTimeScale();
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
                    this.SyncTimeScale();
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
                        this.SyncTimeScale();
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

            private void SyncTimeScale()
            {
                float result = this.GetTimeScale();
                
                if (MathUtil.FuzzyEqual(result, UnityEngine.Time.timeScale))
                {
                    UnityEngine.Time.timeScale = result;
                }
                else
                {
                    UnityEngine.Time.timeScale = result;
                    if (this.TimeScaleChanged != null) this.TimeScaleChanged(this, System.EventArgs.Empty);
                }

                if(Mathf.Approximately(result, UnityEngine.Time.timeScale))
                {
                    UnityEngine.Time.timeScale = result;
                }
                else
                {
                    UnityEngine.Time.timeScale = result;
                    if (this.TimeScaleChanged != null) this.TimeScaleChanged(this, System.EventArgs.Empty);
                }
            }

            private float GetTimeScale()
            {
                float result = 1f;
                if (_scales.Count > 0)
                {
                    var e = _scales.GetEnumerator();
                    while (e.MoveNext())
                    {
                        result *= e.Current.Value;
                    }
                }
                return result;
            }

        }

        private class RealTimeSupplier : ITimeSupplier
        {

            public float Total
            {
                get { return UnityEngine.Time.unscaledTime; }
            }

            public double TotalPrecise
            {
                get { return (double)UnityEngine.Time.unscaledTime; }
            }

            public float Delta
            {
                get { return UnityEngine.Time.unscaledDeltaTime; }
            }

            public float Scale
            {
                get
                {
                    return 1f;
                }
            }

        }

        private class SmoothTimeSupplier : IScalableTimeSupplier
        {

            public float Total
            {
                get { return UnityEngine.Time.time; }
            }

            public double TotalPrecise
            {
                get { return (double)UnityEngine.Time.time; }
            }

            public float Delta
            {
                get { return UnityEngine.Time.smoothDeltaTime; }
            }

            public float Scale
            {
                get
                {
                    return SPTime.Normal.Scale;
                }
            }

            bool IScalableTimeSupplier.Paused
            {
                get
                {
                    return SPTime.Normal.Paused;
                }
                set
                {
                    throw new System.NotSupportedException();
                }
            }

            IEnumerable<string> IScalableTimeSupplier.ScaleIds
            {
                get
                {
                    return SPTime.Normal.ScaleIds;
                }
            }

            event EventHandler IScalableTimeSupplier.TimeScaleChanged
            {
                add
                {
                    SPTime.Normal.TimeScaleChanged += value;
                }

                remove
                {
                    SPTime.Normal.TimeScaleChanged -= value;
                }
            }

            void IScalableTimeSupplier.SetScale(string id, float scale)
            {
                throw new System.NotSupportedException();
            }

            float IScalableTimeSupplier.GetScale(string id)
            {
                return SPTime.Normal.GetScale(id);
            }

            bool IScalableTimeSupplier.RemoveScale(string id)
            {
                throw new System.NotSupportedException();
            }

            bool IScalableTimeSupplier.HasScale(string id)
            {
                return SPTime.Normal.HasScale(id);
            }
        }

        #endregion

        #region Special Config Types

        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
        public class Config : System.Attribute
        {

#region Fields

            public string[] AvailableCustomTimeNames;

#endregion

            public Config(params string[] availableCustomTimeNames)
            {
                this.AvailableCustomTimeNames = availableCustomTimeNames;
            }

        }

#endregion

    }

}
