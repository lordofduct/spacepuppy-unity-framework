using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;

namespace com.spacepuppy.Scenario
{

    public class t_OnClick : TriggerComponent
    {

        #region Fields

        [SerializeField()]
        [Tooltip("A duration of time that the click must be held down to register as a click.")]
        private Interval _buttonLapse = Interval.MinMax(float.NegativeInfinity, float.PositiveInfinity);

        [System.NonSerialized()]
        private float _downT;

        #endregion

        #region Messages

        void OnMouseDown()
        {
            _downT = Time.unscaledTime;
        }

        void OnMouseUpAsButton()
        {
            if (_buttonLapse.Intersects(Time.unscaledTime - _downT))
            {
                this.ActivateTrigger();
            }
        }

        #endregion

    }

}
