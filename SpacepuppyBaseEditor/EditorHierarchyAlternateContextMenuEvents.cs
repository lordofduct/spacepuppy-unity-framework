using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor
{

    [InitializeOnLoad()]
    public static class EditorHierarchyAlternateContextMenuEvents
    {

        public const string MENU_PREFIX_ALTCONTEXT = "CONTEXT/ALT/";

        #region Static Fields

        private static bool _isActive;

        #endregion

        #region STATIC CONSTRUCTOR

        static EditorHierarchyAlternateContextMenuEvents()
        {
            SetActive(SpacepuppySettings.UseHierarchyAlternateContextMenu);
        }

        #endregion

        #region Properties

        public static bool IsActive
        {
            get { return _isActive; }
        }

        #endregion

        #region Methods

        public static void SetActive(bool active)
        {
            if (_isActive == active) return;

            _isActive = active;
            if (_isActive)
            {
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;

                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
            }
            else
            {
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            }
        }

        #endregion

        
        #region HierarchWindowItemGUI

        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            var ev = Event.current;
            if (ev != null && selectionRect.Contains(ev.mousePosition)
                && ev.button == 1 && ev.control && Event.current.type <= EventType.MouseUp)
            {
                // Find what object this is
                GameObject clickedObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                if (clickedObject != null)
                {
                    var mpos = ev.mousePosition;
                    var rect = new Rect(mpos.x, mpos.y, 0f, 0f);
                    var cmnd = new MenuCommand(clickedObject, instanceID);
                    Selection.activeGameObject = clickedObject;
                    EditorUtility.DisplayPopupMenu(rect, MENU_PREFIX_ALTCONTEXT, cmnd);
                    ev.Use();
                }
            }
        }

        #endregion
        
    }
}
