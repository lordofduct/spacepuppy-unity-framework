using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.ClassicTimers
{

    /// <summary>
    /// Switch that swaps its 'Allow' property between true and false over random durations.
    /// </summary>
    public class RandomSwitch : Timer
    {

        #region Fields

        private float _t;
        private float _cap;

        #endregion

        #region CONSTRUCTOR

        public RandomSwitch()
        {

        }

        public RandomSwitch(float offMin, float offMax, float onMin, float onMax)
        {
            this.SetRanges(offMin, offMax, onMin, onMax);
        }

        #endregion

        #region Properties

        public bool Allow
        {
            get { return _t < 0; }
        }

        public float OffMinDuration { get; private set; }
        public float OffMaxDuration { get; private set; }

        public float OnMinDuration { get; private set; }
        public float OnMaxDuration { get; private set; }

        #endregion

        #region Methods

        public void SetRanges(float offMin, float offMax, float onMin, float onMax)
        {
            this.OffMinDuration = offMin;
            this.OffMaxDuration = offMax;
            this.OnMinDuration = onMin;
            this.OnMaxDuration = onMax;

            this.Reset();
        }

        public void Reset()
        {
            this.Stop();

            _t = 0;
            _cap = RandomUtil.Standard.Range(OffMaxDuration, OffMinDuration);
        }

        protected override void UpdateTimer(float dt)
        {
            if (!this.IsRunning) return;
            if (_cap <= 0) return;

            _t += dt;
            while (_t > _cap)
            {
                _t -= _cap;
                _t -= Random.Range(OnMinDuration, OnMinDuration);
                _cap = Random.Range(OffMinDuration, OffMaxDuration);
            }
        }

        #endregion

    }
}
