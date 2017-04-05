using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Anim;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Anim
{

    [CustomPropertyDrawer(typeof(MaskCollection))]
    public class MaskCollectionPropertyDrawer : PropertyDrawer
    {


        #region Fields

        public string Label;

        private ReorderableList _maskList;
        private GUIContent _currentLabel;

        #endregion

        #region CONSTRUCTOR

        private void Init(SerializedProperty property, GUIContent label)
        {
            _currentLabel = label;
            _maskList = CachedReorderableList.GetListDrawer(property.FindPropertyRelative("_masks"), _maskList_DrawHeader, _maskList_DrawElement);

            if (_maskList.index >= _maskList.count) _maskList.index = -1;
        }

        #endregion

        #region OnGUI

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            this.Init(property, label);

            return _maskList.GetHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;

            this.Init(property, label);
            
            var cache = SPGUI.DisableIfPlaying();

            _maskList.DoList(position);

            cache.Reset();
        }

        #endregion

        #region Masks ReorderableList Handlers

        private void _maskList_DrawHeader(Rect area)
        {
            if (this.Label != null)
                EditorGUI.LabelField(area, this.Label);
            else
                EditorGUI.LabelField(area, _currentLabel);
        }

        private void _maskList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _maskList.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;

            const float RECURS_WIDTH = 80f;
            const float RECURS_LABEL_WIDTH = 60f;

            //DRAW TRANSFORM
            var transProp = element.FindPropertyRelative("Transform");
            var transRect = new Rect(area.xMin, area.yMin, area.width - RECURS_WIDTH, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(transRect, transProp, GUIContent.none);

            //DRAW RECURSIVE TOGGLE
            var recursProp = element.FindPropertyRelative("Recursive");
            var recursTotalRect = new Rect(transRect.xMax + 2f, area.yMin, RECURS_WIDTH - 2f, EditorGUIUtility.singleLineHeight);
            var recursLabelRect = new Rect(recursTotalRect.xMin, recursTotalRect.yMin, RECURS_LABEL_WIDTH, recursTotalRect.height);
            var recursToggleRect = new Rect(recursLabelRect.xMax, recursTotalRect.yMin, recursTotalRect.xMax - recursLabelRect.xMax, recursTotalRect.height);
            var recursLabel = new GUIContent("Recurses");
            EditorGUI.BeginProperty(recursTotalRect, recursLabel, recursProp);
            EditorGUI.LabelField(recursLabelRect, recursLabel);
            recursProp.boolValue = EditorGUI.Toggle(recursToggleRect, GUIContent.none, recursProp.boolValue);
            EditorGUI.EndProperty();

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_maskList, area, index, isActive, isFocused);
        }

        #endregion

    }
}
