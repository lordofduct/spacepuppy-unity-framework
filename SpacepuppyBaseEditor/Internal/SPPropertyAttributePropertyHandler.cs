using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Modifiers;

namespace com.spacepuppyeditor.Internal
{
    internal class SPPropertyAttributePropertyHandler : IPropertyHandler
    {

        #region Fields

        private System.Reflection.FieldInfo _fieldInfo;
        private SPPropertyAttribute _attrib;
        private PropertyDrawer _visibleDrawer;

        #endregion

        #region CONSTRUCTOR

        public SPPropertyAttributePropertyHandler(System.Reflection.FieldInfo fieldInfo, SPPropertyAttribute attrib)
        {
            if (fieldInfo == null) throw new System.ArgumentNullException("fieldInfo");
            if (attrib == null) throw new System.ArgumentNullException("attrib");

            _fieldInfo = fieldInfo;
            _attrib = attrib;
        }

        #endregion

        #region Methods

        private void Init()
        {
            var dtp = ScriptAttributeUtility.GetDrawerTypeForType(_attrib.GetType());
            var drawer = PropertyDrawerActivator.Create(dtp, _attrib, _fieldInfo);
            if (drawer is PropertyModifier) (drawer as PropertyModifier).Init(true);
            _visibleDrawer = drawer;
        }

        #endregion

        #region IPropertyHandler Interface

        public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (_visibleDrawer == null) this.Init();

            property = property.Copy();
            if (label == null) label = EditorHelper.TempContent(property.displayName);

            if (_visibleDrawer is IArrayHandlingPropertyDrawer || !property.isArray)
            {
                return _visibleDrawer.GetPropertyHeight(property, label);
            }
            else
            {
                float h = SPEditorGUI.GetSinglePropertyHeight(property, label);
                if (!includeChildren || !property.isExpanded) return h;

                h += EditorGUIUtility.singleLineHeight + 2f;

                for(int i = 0; i < property.arraySize; i++)
                {
                    var pchild = property.GetArrayElementAtIndex(i);
                    h += _visibleDrawer.GetPropertyHeight(pchild, EditorHelper.TempContent(pchild.displayName)) + 2f;
                }
                return h;
            }
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (_visibleDrawer == null) this.Init();

            property = property.Copy();
            if (label == null) label = EditorHelper.TempContent(property.displayName);

            if (_visibleDrawer is IArrayHandlingPropertyDrawer || !property.isArray)
            {
                _visibleDrawer.OnGUI(position, property, label);
                PropertyHandlerValidationUtility.AddAsHandled(property, this);
                return !includeChildren && property.isExpanded;
            }
            else
            {
                if (includeChildren && property.isExpanded)
                {
                    var rect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                    property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

                    EditorGUI.indentLevel++;
                    rect = new Rect(rect.xMin, rect.yMax + 2f, rect.width, EditorGUIUtility.singleLineHeight);
                    property.arraySize = Mathf.Max(0, EditorGUI.IntField(rect, "Size", property.arraySize));

                    var lbl = EditorHelper.TempContent("");
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        var pchild = property.GetArrayElementAtIndex(i);
                        lbl.text = pchild.displayName;
                        var h = _visibleDrawer.GetPropertyHeight(pchild, lbl);
                        rect = new Rect(rect.xMin, rect.yMax + 2f, rect.width, h);
                        _visibleDrawer.OnGUI(rect, pchild, lbl);
                    }

                    EditorGUI.indentLevel--;
                    PropertyHandlerValidationUtility.AddAsHandled(property, this);
                    return true;
                }
                else
                {
                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
                    PropertyHandlerValidationUtility.AddAsHandled(property, this);
                    return false;
                }
            }
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            if (_visibleDrawer == null) this.Init();

            property = property.Copy();
            if (label == null) label = EditorHelper.TempContent(property.displayName);

            if (_visibleDrawer is IArrayHandlingPropertyDrawer || !property.isArray)
            {
                if (label == null) label = EditorHelper.TempContent(property.displayName);
                var rect = EditorGUILayout.GetControlRect(true, _visibleDrawer.GetPropertyHeight(property, label), options);
                _visibleDrawer.OnGUI(rect, property, label);
                PropertyHandlerValidationUtility.AddAsHandled(property, this);
                return !includeChildren && property.isExpanded;
            }
            else
            {
                if (includeChildren && property.isExpanded)
                {
                    var rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                    property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

                    EditorGUI.indentLevel++;
                    rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                    property.arraySize = Mathf.Max(0, EditorGUI.IntField(rect, "Size", property.arraySize));

                    var lbl = EditorHelper.TempContent("");
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        var pchild = property.GetArrayElementAtIndex(i);
                        lbl.text = pchild.displayName;
                        var h = _visibleDrawer.GetPropertyHeight(pchild, lbl);
                        rect = EditorGUILayout.GetControlRect(true, h);
                        _visibleDrawer.OnGUI(rect, pchild, lbl);
                    }

                    EditorGUI.indentLevel--;
                    PropertyHandlerValidationUtility.AddAsHandled(property, this);
                    return true;
                }
                else
                {
                    property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label);
                    PropertyHandlerValidationUtility.AddAsHandled(property, this);
                    return false;
                }
            }
        }

        public void OnValidate(SerializedProperty property)
        {
            if (_visibleDrawer is PropertyModifier)
            {
                property = property.Copy();

                var modifier = _visibleDrawer as PropertyModifier;
                if (_visibleDrawer is IArrayHandlingPropertyDrawer || !property.isArray)
                {
                    modifier.OnValidate(property);
                }
                else
                {
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        modifier.OnValidate(property.GetArrayElementAtIndex(i));
                    }
                }
            }
        }

        #endregion

    }
}
