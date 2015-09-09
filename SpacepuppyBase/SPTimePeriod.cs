using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    [System.Serializable()]
    public struct SPTimePeriod
    {

        public static SPTimePeriod Zero { get { return new SPTimePeriod(0f); } }

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

        public SPTimePeriod(float seconds)
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

        public bool IsCustom
        {
            get { return _timeSupplierType == DeltaTimeType.Custom; }
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
        public class Config : SPTime.Config
        {

        }

        #endregion

        #region Conversion

        public static implicit operator SPTime(SPTimePeriod period)
        {
            return new SPTime(period._timeSupplierType, period._timeSupplierName);
        }

        #endregion

    }

}
