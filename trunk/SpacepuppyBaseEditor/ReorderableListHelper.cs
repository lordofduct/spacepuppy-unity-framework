using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{
    public static class ReorderableListHelper
    {

        public static bool IsRightClickingDraggableArea(this ReorderableList lst, Rect area)
        {
            if (!lst.draggable) return false;

            const float MARGIN = 20f; //this is the size of the draggable handle before the actual content is drawn. Use this area as the left click area.
            var clickArea = new Rect(area.xMin - MARGIN, area.yMin, MARGIN, area.height);
            return Event.current.type == EventType.mouseUp && Event.current.button == MouseUtil.BTN_RIGHT && clickArea.Contains(Event.current.mousePosition);
        }

        public static bool IsClickingArea(Rect area, int mouseButton = 0)
        {
            return Event.current.type == EventType.mouseUp && Event.current.button == mouseButton && area.Contains(Event.current.mousePosition);
        }

        public static void DrawDraggableElementDeleteContextMenu(this ReorderableList lst, Rect area, int index, bool isActive, bool isFocused)
        {
            if (IsRightClickingDraggableArea(lst, area))
            {
                Event.current.Use();

                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    lst.serializedProperty.DeleteArrayElementAtIndex(index);
                    lst.serializedProperty.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext();
            }
        }

    }
}
