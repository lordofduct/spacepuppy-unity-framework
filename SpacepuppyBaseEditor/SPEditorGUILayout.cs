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

        #region DefaultPropertyField

        public static bool DefaultPropertyField(SerializedProperty property)
        {
            return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUILayout(property, GUIContent.none, false, null);
        }

        public static bool DefaultPropertyField(SerializedProperty property, GUIContent label)
        {
            return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUILayout(property, label, false, null);
        }
        
        public static object DefaultPropertyField(string label, object value, System.Type valueType)
        {
            return SPEditorGUI.DefaultPropertyField(EditorGUILayout.GetControlRect(true), EditorHelper.TempContent(label), value, valueType);
        }

        public static object DefaultPropertyField(GUIContent label, object value, System.Type valueType)
        {
            return SPEditorGUI.DefaultPropertyField(EditorGUILayout.GetControlRect(true), label, value, valueType);
        }

        #endregion

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

        public static bool PropertyField(SerializedObject obj, string prop, GUIContent label, bool includeChildren)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            var serial = obj.FindProperty(prop);
            if (serial != null)
            {
                EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(serial, label, includeChildren);
                SPEditorGUILayout.PropertyField(serial, label, includeChildren);
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

        #region EnumPopup Inspector

        public static System.Enum EnumPopupExcluding(System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            return SPEditorGUI.EnumPopupExcluding(EditorGUILayout.GetControlRect(false), enumValue, ignoredValues);
        }

        public static System.Enum EnumPopupExcluding(string label, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            return SPEditorGUI.EnumPopupExcluding(EditorGUILayout.GetControlRect(true), EditorHelper.TempContent(label), enumValue, ignoredValues);
        }

        public static System.Enum EnumPopupExcluding(GUIContent label, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            return SPEditorGUI.EnumPopupExcluding(EditorGUILayout.GetControlRect(label != null && label != GUIContent.none), label, enumValue, ignoredValues);
        }

        #endregion

        #region EnumFlag Inspector

        public static int EnumFlagField(System.Type enumType, GUIContent label, int value)
        {
            var names = (from e in EnumUtil.GetUniqueEnumFlags(enumType) select e.ToString()).ToArray();
            return EditorGUILayout.MaskField(label, value, names);
        }

        public static System.Enum EnumFlagField(GUIContent label, System.Enum value)
        {
            if (value == null) throw new System.ArgumentException("Enum value must be non-null.", "value");

            var enumType = value.GetType();
            int i = EnumFlagField(enumType, label, System.Convert.ToInt32(value));
            return System.Enum.ToObject(enumType, i) as System.Enum;
        }

        public static WrapMode WrapModeField(string label, WrapMode mode, bool allowDefault = false)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.WrapModeField(position, label, mode, allowDefault);
        }

        public static WrapMode WrapModeField(GUIContent label, WrapMode mode, bool allowDefault = false)
        {
            var position = EditorGUILayout.GetControlRect(label != null && label != GUIContent.none);
            return SPEditorGUI.WrapModeField(position, label, mode, allowDefault);
        }

        #endregion

        #region Type Dropdown

        public static System.Type TypeDropDown(GUIContent label, System.Type baseType, System.Type selectedType, bool allowAbstractTypes = false, bool allowInterfaces = false, System.Type defaultType = null, System.Type[] excludedTypes = null, TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.TypeDropDown(position, label, baseType, selectedType, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, listType);
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

        #region Component Selection From Source

        public static Component SelectComponentFromSourceField(string label, GameObject source, Component selectedComp, System.Predicate<Component> filter = null)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.SelectComponentFromSourceField(position, EditorHelper.TempContent(label), source, selectedComp, filter);
        }

        public static Component SelectComponentFromSourceField(GUIContent label, GameObject source, Component selectedComp, System.Predicate<Component> filter = null)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.SelectComponentFromSourceField(position, label, source, selectedComp, filter);
        }

        public static Component SelectComponentField(string label, Component[] components, Component selectedComp)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.SelectComponentField(position, EditorHelper.TempContent(label), components, selectedComp);
        }

        public static Component SelectComponentField(GUIContent label, Component[] components, Component selectedComp)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.SelectComponentField(position, label, components, selectedComp);
        }

        public static Component SelectComponentField(string label, Component[] components, string[] componentLabels, Component selectedComp)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.SelectComponentField(position, label, components, componentLabels, selectedComp);
        }

        public static Component SelectComponentField(GUIContent label, Component[] components, GUIContent[] componentLabels, Component selectedComp)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return SPEditorGUI.SelectComponentField(position, label, components, componentLabels, selectedComp);
        }

        #endregion


        #region ReflectedPropertyField

        /// <summary>
        /// Reflects the available properties and shows them in a dropdown
        /// </summary>
        public static string ReflectedPropertyField(GUIContent label, object targObj, string selectedMemberName, com.spacepuppy.Dynamic.DynamicMemberAccess access, out System.Reflection.MemberInfo selectedMember)
        {
            var position = EditorGUILayout.GetControlRect(label == GUIContent.none);
            return SPEditorGUI.ReflectedPropertyField(position, label, targObj, selectedMemberName, access, out selectedMember);
        }

        public static string ReflectedPropertyField(GUIContent label, object targObj, string selectedMemberName, com.spacepuppy.Dynamic.DynamicMemberAccess access)
        {
            var position = EditorGUILayout.GetControlRect(label == GUIContent.none);
            System.Reflection.MemberInfo selectedMember;
            return SPEditorGUI.ReflectedPropertyField(position, label, targObj, selectedMemberName, access, out selectedMember);
        }

        /// <summary>
        /// Reflects the available properties and shows them in a dropdown
        /// </summary>
        public static string ReflectedPropertyField(GUIContent label, System.Type targType, string selectedMemberName, out System.Reflection.MemberInfo selectedMember)
        {
            var position = EditorGUILayout.GetControlRect(label == GUIContent.none);
            return SPEditorGUI.ReflectedPropertyField(position, label, targType, selectedMemberName, out selectedMember);
        }

        public static string ReflectedPropertyField(GUIContent label, System.Type targType, string selectedMemberName)
        {
            var position = EditorGUILayout.GetControlRect(label == GUIContent.none);
            System.Reflection.MemberInfo selectedMember;
            return SPEditorGUI.ReflectedPropertyField(position, label, targType, selectedMemberName, out selectedMember);
        }

        #endregion




        #region SelectionTabs

        public static int SelectionTabs(int mode, string[] modes, int xCount)
        {
            int yCount = Mathf.CeilToInt((float)modes.Length / (float)xCount);
            int currentRow = Mathf.FloorToInt((float)mode / (float)xCount);

            var gridStyle = new GUIStyle(GUI.skin.window);
            gridStyle.padding.bottom = -20;
            Rect rect = GUILayoutUtility.GetRect(1, 16f * yCount);
            rect.x += 4;
            rect.width -= 7;

            if(currentRow == yCount - 1)
            {
                //if selected row is last row, don't bother remapping
                return GUI.SelectionGrid(rect, mode, modes, 2, gridStyle);
            }
            else
            {
                //remap so that selected row is the last row
                var altModes = modes.Clone() as string[];
                for(int i = 0; i < xCount; i++)
                {
                    altModes[(altModes.Length - xCount) + i] = modes[xCount * currentRow + i]; //move selected row to end
                    altModes[xCount * currentRow + i] = modes[(modes.Length - xCount) + i]; //move last row to selected row
                }
                int altMode = (modes.Length - xCount) + (mode % xCount);
                EditorGUI.BeginChangeCheck();
                altMode = GUI.SelectionGrid(rect, altMode, altModes, 2, gridStyle);
                if(EditorGUI.EndChangeCheck())
                {
                    var newRow = Mathf.FloorToInt((float)altMode / (float)xCount);
                    if (newRow == yCount - 1)
                    {
                        return (currentRow * xCount) + (altMode % xCount);
                    }
                    else if(newRow == currentRow)
                    {
                        return (modes.Length - xCount) + (altMode % xCount);
                    }
                    else
                    {
                        return altMode;
                    }
                }
                else
                {
                    return mode;
                }
            }
        }

        #endregion

    }

}
