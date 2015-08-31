using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{
    [System.Serializable()]
    public struct TimePeriod
    {

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
                switch (_timeSupplierType)
                {
                    case DeltaTimeType.Normal:
                        return SPTime.Normal;
                    case DeltaTimeType.Real:
                        return SPTime.Real;
                    case DeltaTimeType.Smooth:
                        return SPTime.Smooth;
                    case DeltaTimeType.Custom:
                        {
                            //if (_customTime == null || !_customTime.Valid) _customTime = SPTime.Custom(_timeSupplierName, false);
                            //return _customTime;
                            return SPTime.Custom(_timeSupplierName, false);
                        }
                    default:
                        return null;
                }
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
