using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{

    public class i_ChangeCursorProperties : AutoTriggerableMechanism
    {

        public enum CursorVisibilityOption
        {
            None,
            SetVisible = 1,
            SetInvisible = 2
        }

        public enum CursorLockOptions
        {
            None = 0,
            SetCursorUnlocked = 1,
            SetCursorLocked = 2,
            SetCursorConfined = 3
        }

        [SerializeField]
        public CursorVisibilityOption VisibilityOption;
        [SerializeField]
        public CursorLockOptions LockOption;


        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;


            switch(this.VisibilityOption)
            {
                case CursorVisibilityOption.SetVisible:
                    Cursor.visible = true;
                    break;
                case CursorVisibilityOption.SetInvisible:
                    Cursor.visible = false;
                    break;
            }
            Cursor.lockState = CursorLockMode.Confined;
            
            switch(this.LockOption)
            {
                case CursorLockOptions.SetCursorUnlocked:
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case CursorLockOptions.SetCursorLocked:
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case CursorLockOptions.SetCursorConfined:
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
            }

            return true;
        }
    }

}
