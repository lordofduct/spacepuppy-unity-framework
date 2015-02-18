using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    public static class SPEditorGUILayout
    {

        #region PropertyFields

        public static bool PropertyField(SerializedObject obj, string prop)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            var serial = obj.FindProperty(prop);
            if (serial != null)
            {
                EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(serial);
                SPEditorGUILayout.PropertyField(serial);
                return EditorGUI.EndChangeCheck();
            }

            return false;
        }

        public static bool PropertyField(SerializedObject obj, string prop, bool includeChildren)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            var serial = obj.FindProperty(prop);
            if (serial != null)
            {
                EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(serial, includeChildren);
                SPEditorGUILayout.PropertyField(serial, includeChildren);
                return EditorGUI.EndChangeCheck();
            }

            return false;
        }

        public static bool PropertyField(SerializedObject obj, string prop, string label, bool includeChildren)
        {
            return SPEditorGUILayout.PropertyField(obj, prop, EditorHelper.TempContent(label), includeChildren);
        }

        public static bool PropertyField(SerializedObject obj, string prop, GUIContent content, bool includeChildren)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            var serial = obj.FindProperty(prop);
            if (serial != null)
            {
                EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(serial, content, includeChildren);
                SPEditorGUILayout.PropertyField(serial, content, includeChildren);
                return EditorGUI.EndChangeCheck();
            }

            return false;
        }

        public static bool PropertyField(SerializedProperty property, params GUILayoutOption[] options)
        {
            return SPEditorGUILayout.PropertyField(property, (GUIContent)null, false, options);
        }

        public static bool PropertyField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            return SPEditorGUILayout.PropertyField(property, label, false, options);
        }

        public static bool PropertyField(SerializedProperty property, bool includeChildren, params GUILayoutOption[] options)
        {
            return SPEditorGUILayout.PropertyField(property, (GUIContent)null, includeChildren, options);
        }

        public static bool PropertyField(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
        {
            return com.spacepuppyeditor.Internal.ScriptAttributeUtility.GetHandler(property).OnGUILayout(property, label, includeChildren, options);
        }

        #endregion

        #region LayerMaskField

        public static LayerMask LayerMaskField(string label, int selectedMask)
        {
            return EditorGUILayout.MaskField(label, selectedMask, LayerUtil.GetAllLayerNames());
        }

        #endregion

        #region EnumFlag Inspector

        public static int EnumFlagField(System.Type enumType, GUIContent label, int value)
        {
            var names = (from e in ObjUtil.GetUniqueEnumFlags(enumType) select e.ToString()).ToArray();
            return EditorGUILayout.MaskField(label, value, names);
        }

        public static System.Enum EnumFlagField(GUIContent label, System.Enum value)
        {
            if (value == null) throw new System.ArgumentException("Enum value must be non-null.", "value");

            var enumType = value.GetType();
            int i = EnumFlagField(enumType, label, System.Convert.ToInt32(value));
            return System.Enum.ToObject(enumType, i) as System.Enum;
        }

        #endregion

        #region Type Dropdown

        public static System.Type TypeDropDown(GUIContent label, System.Type baseType, System.Type selectedType, bool allowAbstractTypes = false, bool allowInterfaces = false, System.Type defaultType = null, TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.TypeDropDown(position, label, baseType, selectedType, allowAbstractTypes, allowInterfaces, defaultType, listType);
        }

        #endregion

        #region Quaternion Field

        public static Quaternion QuaternionField(GUIContent label, Quaternion value, bool useRadians = false)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.QuaternionField(position, label, value, useRadians);
        }

        #endregion

        #region IComponentField

        public static Component ComponentField(GUIContent label, Component value, System.Type inheritsFromType, bool allowSceneObjects)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.ComponentField(position, label, value, inheritsFromType, allowSceneObjects);
        }

        #endregion

    }

}
