using UnityEngine;

namespace com.spacepuppy.Utils
{
    public static class MouseUtil
    {

        public const int BTN_LEFT = 0;
        public const int BTN_RIGHT = 1;

        public static bool GuiClicked(Event ev, int btn, Rect area)
        {
            return (ev.type == EventType.mouseDown && ev.button == btn && area.Contains(ev.mousePosition));
        }

    }
}
