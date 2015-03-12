using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(ReorderableArrayAttribute))]
    public class ReorderableArrayPropertyDrawer : PropertyDrawer
    {

        #region Fields

        public string Label;
        private ReorderableList _lst;

        #endregion

        #region CONSTRUCTOR

        private void Init(SerializedProperty property)
        {
            if(_lst == null)
            {
                _lst = new ReorderableList(null, property, true, true, true, true);
                _lst.drawHeaderCallback = this._maskList_DrawHeader;
                _lst.drawElementCallback = this._maskList_DrawElement;
            }
            else
            {
                _lst.serializedProperty = property;
                _lst.elementHeight = SPEditorGUI.GetDefaultPropertyHeight(property) + 1;
            }
            if (_lst.serializedProperty != null && _lst.index >= _lst.count) _lst.index = -1;
        }

        #endregion

        #region OnGUI

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.isArray)
            {
                this.Init(property);

                return _lst.GetHeight();
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.isArray)
            {
                this.Init(property);

                _lst.DoList(position);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, false);
            }
        }

        #endregion

        #region Masks ReorderableList Handlers

        private void _maskList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, this.Label ?? _lst.serializedProperty.displayName);
        }

        private void _maskList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _lst.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;

            //EditorGUI.PropertyField(area, element, GUIContent.none, false);
            var attrib = this.attribute as ReorderableArrayAttribute;
            GUIContent label;
            if(attrib != null && attrib.ElementLabelFormatString != null)
            {
                label = EditorHelper.TempContent(string.Format(attrib.ElementLabelFormatString, index));
            }
            else
            {
                label = GUIContent.none;
            }
            SPEditorGUI.DefaultPropertyField(area, element, label);

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_lst, area, index, isActive, isFocused);
        }

        #endregion

    }
}
