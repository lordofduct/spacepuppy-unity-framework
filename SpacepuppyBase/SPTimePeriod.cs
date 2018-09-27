using UnityEngine;

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

        public SPTimePeriod(float seconds, DeltaTimeType type, string timeSupplierName = null)
        {
            _seconds = seconds;
            _timeSupplierType = type;
            _timeSupplierName = timeSupplierName;
        }

        public SPTimePeriod(float seconds, ITimeSupplier supplier)
        {
            _seconds = seconds;
            _timeSupplierType = DeltaTimeType.Normal;
            _timeSupplierName = null;
            this.TimeSupplier = supplier;
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
                _timeSupplierType = SPTime.GetDeltaType(value);
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

        public static implicit operator SPTimePeriod(float seconds)
        {
            return new SPTimePeriod(seconds);
        }

        #endregion

    }

}
