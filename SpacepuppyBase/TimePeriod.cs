using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    [System.Serializable()]
    public struct TimePeriod
    {

        public static TimePeriod Zero { get { return new TimePeriod(0f); } }

        public enum Units
        {
            Seconds = 0,
            Minutes = 1,
            Hours = 2,
            Days = 3,
            Years = 4
        }

        public const float SECONDS_IN_MINUTE = 60f;
        public const float SECONDS_IN_HOUR = 3600f;
        public const float SECONDS_IN_DAY = 86400f;
        public const float SECONDS_IN_WEEK = 604800;



        #region Fields

        [SerializeField()]
        private float _seconds;

        [SerializeField()]
        private DeltaTimeType _timeSupplierType;
        [SerializeField()]
        private string _timeSupplierName;
        //[System.NonSerialized()]
        //private CustomTimeSupplier _customTime;

        #endregion

        #region CONSTRUCTOR

        public TimePeriod(float seconds)
        {
            _seconds = seconds;
            _timeSupplierType = DeltaTimeType.Normal;
            _timeSupplierName = null;
        }

        #endregion

        #region Properties

        public float Seconds
        {
            get { return _seconds; }
            set { _seconds = value; }
        }

        public DeltaTimeType TimeSupplierType
        {
            get { return _timeSupplierType; }
        }

        public ITimeSupplier TimeSupplier
        {
            get
            {
                return SPTime.GetTime(_timeSupplierType, _timeSupplierName);
            }
        }

        #endregion

        #region Methods

        public bool Elapsed(double startTime)
        {
            var time = this.TimeSupplier;
            if (time == null) return false;
            return (time.TotalPrecise - startTime) >= _seconds;
        }

        #endregion

        #region Special Types

        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple=false)]
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
