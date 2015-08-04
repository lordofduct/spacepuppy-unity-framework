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

        #region Static Fields

        private static bool _isActive;

        private static GameObject _currentGO;
        private static Vector2 _menuPos;

        #endregion

        #region STATIC CONSTRUCTOR

        static EditorHierarchyAlternateContextMenuEvents()
        {
            SetActive(EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, true));
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
            if (_currentGO == null)
            {
                var ev = Event.current;
                if (ev != null && selectionRect.Contains(ev.mousePosition)
                    && ev.button == 1 && ev.control && Event.current.type <= EventType.mouseUp)
                {
                    // Find what object this is
                    GameObject clickedObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                    if (clickedObject != null)
                    {
                        _currentGO = clickedObject;
                        _menuPos = ev.mousePosition;
                        ev.Use();
                    }
                }

            }


            if(_currentGO != null)
            {
                var rect = new Rect(_menuPos.x, _menuPos.y, 0f, 0f);
                EditorUtility.DisplayPopupMenu(rect, "Assets/", null);
            }
        }

        #endregion

    }
}
