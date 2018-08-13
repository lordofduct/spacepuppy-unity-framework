using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{

    public static class UIEventUtil
    {

        public static bool SetSelectedGameObject(GameObject selected)
        {
            var ev = EventSystem.current;
            if (ev == null) return false;

            if (selected != null && ev.currentSelectedGameObject == selected)
            {
                ev.SetSelectedGameObject(null);
                ev.SetSelectedGameObject(selected);
            }
            else
            {
                ev.SetSelectedGameObject(selected);
            }
            return true;
        }

        public static GameObject GetSelectedGameObjet()
        {
            var ev = EventSystem.current;
            return ev != null ? ev.currentSelectedGameObject : null;
        }
        
    }

}
