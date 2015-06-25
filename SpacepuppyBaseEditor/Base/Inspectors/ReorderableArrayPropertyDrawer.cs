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
        private GUIContent _label;
        private bool _disallowFoldout;

        #endregion

        #region CONSTRUCTOR

        private void StartOnGUI(SerializedProperty property, GUIContent label)
        {
            if(_lst == null)
            {
                _lst = new ReorderableList(null, property, true, true, true, true);
                _lst.drawHeaderCallback = this._maskList_DrawHeader;
                _lst.drawElementCallback = this._maskList_DrawElement;
                _lst.elementHeight = SPEditorGUI.GetDefaultPropertyHeight(property) + 1;
            }
            else
            {
                _lst.serializedProperty = property;
                _lst.elementHeight = SPEditorGUI.GetDefaultPropertyHeight(property) + 1;
            }
            if (_lst.serializedProperty != null && _lst.index >= _lst.count) _lst.index = -1;

            var attrib = this.attribute as ReorderableArrayAttribute;
            if (attrib != null) _disallowFoldout = attrib.DisallowFoldout;

            _label = label;
        }
        
        private void EndOnGUI(SerializedProperty property, GUIContent label)
        {
            _lst.serializedProperty = null;
            _label = null;
        }

        #endregion

        #region Properties

        

        #endregion

        #region OnGUI

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.isArray)
            {
                this.StartOnGUI(property, label);

                if (_disallowFoldout || property.isExpanded)
                {
                    return _lst.GetHeight();
                }
                else
                {
                    return EditorGUIUtility.singleLineHeight;
                }
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
                this.StartOnGUI(property, label);

                if(_disallowFoldout)
                {
                    _lst.DoList(position);
                }
                else
                {
                    const float WIDTH_FOLDOUT = 5f;
                    property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, WIDTH_FOLDOUT, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);
                    if (property.isExpanded)
                    {
                        _lst.DoList(position);
                    }
                    else
                    {
                        ReorderableListHelper.DrawRetractedHeader(position, label);
                    }
                }

                this.EndOnGUI(property, label);
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
            if(this.Label != null)
            {
                EditorGUI.LabelField(area, this.Label ?? _lst.serializedProperty.displayName);
            }
            else
            {
                EditorGUI.LabelField(area, _label ?? EditorHelper.TempContent(_lst.serializedProperty.displayName));
            }
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
