using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_OnDisable : TriggerComponent
    {

        #region Fields
        
        #endregion

        #region Properties
        
        #endregion

        #region Messages

        protected override void OnDisable()
        {
            base.OnDisable();

            this.ActivateTrigger(this);
        }

        #endregion


    }
}
