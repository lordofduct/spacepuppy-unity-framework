using UnityEngine;

namespace com.spacepuppy.Utils.Diminish
{

    [System.Serializable()]
    public class DiminishingWeightOverDuration
    {

        #region Fields

        [SerializeField()]
        private float _weight = 1f;
        [SerializeField()]
        [Tooltip("Number of times that can be selected before weight is forced to zero no matter the diminish rate.")]
        private DiscreteFloat _maxCount = DiscreteFloat.PositiveInfinity;
        [SerializeField()]
        [Tooltip("Every time the option is selected the weight is multipled by this rate.")]
        private float _diminishRate = 1.0f;
        [SerializeField()]
        [Tooltip("Amount of time each compounded diminish lasts against the weight.")]
        private float _diminishPeriodDuration = 1f;

        [System.NonSerialized()]
        private ITimeSupplier _timeSupplier;
        [System.NonSerialized()]
        private int _count;
        [System.NonSerialized()]
        private float _lastTime;

        #endregion

        #region CONSTRUCTOR

        public DiminishingWeightOverDuration()
        {

        }

        public DiminishingWeightOverDuration(float weight)
        {
            _weight = weight;
        }

        #endregion

        #region Properties

        public ITimeSupplier TimeSupplier
        {
            get
            {
                if (_timeSupplier == null) _timeSupplier = SPTime.Normal;
                return _timeSupplier;
            }
            set { _timeSupplier = value ?? SPTime.Normal; }
        }

        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public float DiminishRate
        {
            get { return _diminishRate; }
            set { _diminishRate = Mathf.Max(0f, value); }
        }

        public float DiminishPeriodDuration
        {
            get { return _diminishPeriodDuration; }
            set { _diminishPeriodDuration = Mathf.Max(0f, value); }
        }

        #endregion

        #region Methods

        public void Signal()
        {
            var dt = this.TimeSupplier.Total - _lastTime;
            int cnt = Mathf.FloorToInt(dt / _diminishPeriodDuration);
            if (cnt > 0)
            {
                _count = Mathf.Max(0, _count - cnt);
                _lastTime += cnt * _diminishPeriodDuration;
            }

            if(_count == 0)
            {
                _lastTime = this.TimeSupplier.Total;
                _count = 1;
            }
            else
            {
                _count++;
            }
        }

        public float GetAdjustedWeight()
        {
            if (_count >= _maxCount) return 0f;
            if (_count == 0 || MathUtil.FuzzyEqual(_diminishRate, 1f)) return _weight;

            var dt = this.TimeSupplier.Total - _lastTime;
            int cnt = Mathf.FloorToInt(dt / _diminishPeriodDuration);
            if (cnt > 0)
            {
                _count = Mathf.Max(0, _count - cnt);
                _lastTime += cnt * _diminishPeriodDuration;
            }

            float w = _weight;
            for (int i = 0; i < _count; i++)
            {
                w *= _diminishRate;
            }
            return w;
        }

        #endregion

    }

}
