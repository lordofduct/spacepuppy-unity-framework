using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

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
    /// </summary>
    public static class SPTime
    {

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

        /// <summary>
        /// Reverse lookup for DeltaTimeType from an object.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieve a CustomTimeSupplier by name. If no time exists of that name, one is created.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CustomTimeSupplier Custom(string id)
        {
            if (_customTimes == null)
            {
                _customTimes = new Dictionary<string, CustomTimeSupplier>();
                GameLoopEntry.RegisterInternalEarlyUpdate(SPTime.Update);
            }

            if (_customTimes.ContainsKey(id))
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

        public static CustomTimeSupplier Custom(string id, bool createIfNotExists)
        {
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

        public static bool HasCustom(string id)
        {
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
        public static float GetInverseScale(IScalableTimeSupplier supplier)
        {
            if (supplier is NormalTimeSupplier) return 1f;

            return supplier.Scale / Time.timeScale;
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
                    float result = 1f;
                    var e = _scales.Values.GetEnumerator();
                    while(e.MoveNext())
                    {
                        result *= e.Current;
                    }
                    return result;

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

            public double TotalPrecise
            {
                get { return (double)UnityEngine.Time.unscaledTime; }
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

            public double TotalPrecise
            {
                get { return (double)UnityEngine.Time.time; }
            }

            public float Delta
            {
                get { return UnityEngine.Time.smoothDeltaTime; }
            }

        }

        #endregion

    }

}
