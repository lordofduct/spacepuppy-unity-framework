using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace com.spacepuppy.Scenario
{
    public class i_SetActiveUIElement : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        private GameObject _element;

        #endregion

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;


            if(_element != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(_element);
                return true;
            }

            return false;
        }

    }
}
