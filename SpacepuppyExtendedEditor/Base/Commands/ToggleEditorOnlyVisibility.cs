using UnityEngine;
using UnityEditor;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    public class ToggleEditorOnlyVisibility : SPEditor
    {

        public const string MENU_NAME = SPMenu.MENU_NAME_ROOT + "/Toggle EditorOnly Visible";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_GROUP2;



        private static bool _active = true;

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        static void Init()
        {
            _active = !_active;

            foreach (GameObject obj in GameObjectUtil.FindGameObjectsWithMultiTag(com.spacepuppy.SPConstants.TAG_EDITORONLY))
            {
                var rb = obj.GetComponent<Renderer>();
                if (rb != null) rb.enabled = _active;
            }
        }
    }

}