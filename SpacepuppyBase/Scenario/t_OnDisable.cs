using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_OnDisable : TriggerComponent
    {

        #region Fields

        [SerializeField()]
        [TimeUnitsSelector()]
        private float _delay;

        #endregion

        #region Properties

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Messages

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.ActivateTrigger(this);
                }, _delay);
            }
            else
            {
                this.ActivateTrigger(this);
            }
        }

        #endregion


    }
}
