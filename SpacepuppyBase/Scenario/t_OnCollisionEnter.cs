using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class t_OnCollisionEnter : TriggerComponent
    {

        #region Fields

        [SerializeField()]
        private ScenarioActivatorMask _mask;
        public float CooldownInterval = 1.0f;
        public bool IncludeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public ScenarioActivatorMask Mask { get { return _mask; } }

        #endregion

        #region Methods

        private void OnCollisionEnter(Collision c)
        {
            if (_coolingDown) return;

            if (Mask == null || Mask.Intersects(c.collider))
            {
                if (this.IncludeColliderAsTriggerArg)
                {
                    this.ActivateTrigger(c.collider);
                }
                else
                {
                    this.ActivateTrigger();
                }

                _coolingDown = true;
                this.Invoke(() =>
                {
                    _coolingDown = false;
                }, this.CooldownInterval);
            }
        }

        #endregion

    }

}
