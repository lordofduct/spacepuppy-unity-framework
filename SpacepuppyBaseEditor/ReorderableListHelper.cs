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

        #region Fields

        private static ReorderableList.Defaults s_Defaults;
        public static ReorderableList.Defaults DefaultBehaviours
        {
            get
            {
                if(ReorderableList.defaultBehaviours == null)
                {
                    if (s_Defaults == null) s_Defaults = new ReorderableList.Defaults();
                    return s_Defaults;
                }
                else
                {
                    return ReorderableList.defaultBehaviours;
                }
            }
        }

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Public Util Methods

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

        public static void DrawRetractedHeader(Rect position, GUIContent label)
        {
            DrawRetractedHeader(position, label, ReorderableListHelper.DefaultBehaviours.headerBackground);
        }

        public static void DrawRetractedHeader(Rect position, GUIContent label, GUIStyle backgroundStyle)
        {
            var rbg = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            if (Event.current.type == EventType.Repaint && backgroundStyle != null)
                backgroundStyle.Draw(rbg, false, false, false, false);

            var rlbl = rbg;
            rlbl.xMin += 6f;
            rlbl.xMax -= 6f;
            rlbl.y += 1f;
            EditorGUI.LabelField(rlbl, label);
        }

        #endregion

    }
}
