using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    public static class EditorHelper
    {

        public const string PROP_SCRIPT = "m_Script";



        #region DrawDefaultInspector

        public static void DrawDefaultInspector(this Editor editor, string prop)
        {
            if (editor == null) throw new System.ArgumentNullException("editor");

            var serial = editor.serializedObject.FindProperty(prop);
            if (serial != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serial);
                if (EditorGUI.EndChangeCheck())
                    editor.serializedObject.ApplyModifiedProperties();
            }
        }

        public static void DrawDefaultInspector(this Editor editor, string prop, bool includeChildren)
        {
            if (editor == null) throw new System.ArgumentNullException("editor");

            var serial = editor.serializedObject.FindProperty(prop);
            if (serial != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serial, includeChildren);
                if (EditorGUI.EndChangeCheck())
                    editor.serializedObject.ApplyModifiedProperties();
            }
        }

        public static void DrawDefaultInspector(this Editor editor, string prop, string label, bool includeChildren)
        {
            DrawDefaultInspector(editor, prop, new GUIContent(label), includeChildren);
        }

        public static void DrawDefaultInspector(this Editor editor, string prop, GUIContent content, bool includeChildren)
        {
            if (editor == null) throw new System.ArgumentNullException("editor");

            var serial = editor.serializedObject.FindProperty(prop);
            if (serial != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serial, content, includeChildren);
                if (EditorGUI.EndChangeCheck())
                    editor.serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion

        #region DrawDefaultInspectorExcept

        public static void DrawDefaultInspectorExcept(this Editor editor, params string[] propsNotToDraw)
        {
            if (editor == null) throw new System.ArgumentNullException("editor");

            string[] DEFAULT_IGNORES = new string[] { "m_PrefabParentObject", "m_PrefabInternal", "m_GameObject", "m_Enabled", "m_EditorHideFlags", "m_Name", "m_EditorClassIdentifier" };

            EditorGUI.BeginChangeCheck();
            var prop = editor.serializedObject.GetIterator();
            prop.Next(true); //get first element
            while (prop.Next(false))
            {
                if (!DEFAULT_IGNORES.Contains(prop.name) && (propsNotToDraw == null || !propsNotToDraw.Contains(prop.name)))
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }

            if (EditorGUI.EndChangeCheck())
                editor.serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region List Inspector

        public static void ListField<T>(string label, IList<T> lst)
        {

        }

        #endregion

        #region EnumFlag Inspector

        public static int EnumFlagField(Rect position, System.Type enumType, GUIContent label, int value)
        {
            var names = (from e in ObjUtil.GetUniqueEnumFlags(enumType) select e.ToString()).ToArray();
            return EditorGUI.MaskField(position, label, value, names);
        }

        public static int EnumFlagField(System.Type enumType, GUIContent label, int value)
        {
            var names = (from e in ObjUtil.GetUniqueEnumFlags(enumType) select e.ToString()).ToArray();
            return EditorGUILayout.MaskField(label, value, names);
        }

        public static System.Enum EnumFlagField(Rect position, GUIContent label, System.Enum value)
        {
            if (value == null) throw new System.ArgumentException("Enum value must be non-null.", "value");

            var enumType = value.GetType();
            int i = EnumFlagField(position, enumType, label, System.Convert.ToInt32(value));
            return System.Enum.ToObject(enumType, i) as System.Enum;
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

        public static System.Type TypeDropDown(Rect position, GUIContent label, System.Type baseType, System.Type selectedType, bool allowAbstractTypes = false, bool allowInterfaces = false, System.Type defaultType = null, TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            if (!ObjUtil.IsType(selectedType, baseType)) selectedType = null;

            var knownTypes = (from ass in System.AppDomain.CurrentDomain.GetAssemblies()
                              from tp in ass.GetTypes()
                              where ObjUtil.IsType(tp, baseType) && (allowAbstractTypes || !tp.IsAbstract) && (allowInterfaces || !tp.IsInterface)
                              orderby tp.FullName.Substring(tp.FullName.LastIndexOf(".") + 1) ascending
                              select tp).ToArray();
            GUIContent[] knownTypeNames = null;
            switch(listType)
            {
                case TypeDropDownListingStyle.Namespace:
                    knownTypeNames = knownTypes.Select((tp) =>
                    {
                        return new GUIContent(tp.FullName.Replace(".", "/"));
                    }).ToArray();
                    break;
                case TypeDropDownListingStyle.Flat:
                    knownTypeNames = (from tp in knownTypes select new GUIContent(tp.Name)).ToArray();
                    break;
                case TypeDropDownListingStyle.ComponentMenu:
                    knownTypeNames = knownTypes.Select((tp) =>
                    {
                        var menuAttrib = tp.GetCustomAttributes(typeof(AddComponentMenu), false).FirstOrDefault() as AddComponentMenu;
                        if (menuAttrib != null && !string.IsNullOrEmpty(menuAttrib.componentMenu))
                        {
                            return new GUIContent(menuAttrib.componentMenu);
                        }
                        else if (tp.FullName == tp.Name)
                        {
                            return new GUIContent("Scripts/" + tp.Name);
                        }
                        else
                        {
                            if(tp.FullName.StartsWith("UnityEngine."))
                            {
                                return new GUIContent(tp.FullName.Replace(".", "/"));
                            }
                            else
                            {
                                return new GUIContent("Scripts/" + tp.FullName.Replace(".", "/"));
                            }
                        }
                    }).ToArray();
                    break;
                default:
                    knownTypeNames = new GUIContent[0];
                    break;
            }

            if (defaultType == null)
            {
                knownTypes = knownTypes.Prepend(null).ToArray();
                knownTypeNames = knownTypeNames.Prepend(new GUIContent("Nothing")).ToArray();
            }

            int index = knownTypes.IndexOf(selectedType);
            index = EditorGUI.Popup(position, label, index, knownTypeNames);
            return (index >= 0) ? knownTypes[index] : defaultType;
        }

        public static System.Type TypeDropDown(GUIContent label, System.Type baseType, System.Type selectedType, bool allowAbstractTypes = false, bool allowInterfaces = false, System.Type defaultType = null, TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return TypeDropDown(position, label, baseType, selectedType, allowAbstractTypes, allowInterfaces, defaultType, listType);
        }

        #endregion

        #region Quaternion Field

        public static Quaternion QuaternionField(Rect position, GUIContent label, Quaternion value, bool useRadians = false)
        {
            Vector3 vRot = value.eulerAngles;
            if (useRadians)
            {
                vRot.x = vRot.x * Mathf.Deg2Rad;
                vRot.y = vRot.y * Mathf.Deg2Rad;
                vRot.z = vRot.z * Mathf.Deg2Rad;
            }

            vRot.x = MathUtil.NormalizeAngle(vRot.x, false);
            vRot.y = MathUtil.NormalizeAngle(vRot.y, false);
            vRot.z = MathUtil.NormalizeAngle(vRot.z, false);

            var vNewRot = EditorGUI.Vector3Field(position, label, vRot);

            vNewRot.x = MathUtil.NormalizeAngle(vNewRot.x, false);
            vNewRot.y = MathUtil.NormalizeAngle(vNewRot.y, false);
            vNewRot.z = MathUtil.NormalizeAngle(vNewRot.z, false);

            if (vRot != vNewRot)
            {
                if (useRadians)
                {
                    vNewRot.x = vNewRot.x * Mathf.Rad2Deg;
                    vNewRot.y = vNewRot.y * Mathf.Rad2Deg;
                    vNewRot.z = vNewRot.z * Mathf.Rad2Deg;
                }
                return Quaternion.Euler(vNewRot);
            }
            else
            {
                return value;
            }
        }

        public static Quaternion QuaternionField(GUIContent label, Quaternion value, bool useRadians = false)
        {
            var position = EditorGUILayout.GetControlRect(true);
            return QuaternionField(position, label, value, useRadians);
        }

        #endregion

        #region SerializedProperty Helpers

        /// <summary>
        /// Returns the fieldInfo of the property. If the property is an Array/List, the fieldInfo for the Array is returned.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach(var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            if (Object.ReferenceEquals(obj, null)) return null;

            try
            {
                var element = elements.Last();
                var tp = obj.GetType();
                if (elements.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    return tp.GetField(elementName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else
                {
                    return tp.GetField(elements.Last(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach(var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public static void SetTargetObjectOfProperty(SerializedProperty prop, object value)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            if (Object.ReferenceEquals(obj, null)) return;

            try
            {
                var element = elements.Last();
                var tp = obj.GetType();
                if (elements.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    var field = tp.GetField(elementName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var arr = field.GetValue(obj) as System.Collections.IList;
                    arr[index] = value;
                }
                else
                {
                    var field = tp.GetField(elements.Last(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    field.SetValue(obj, value);
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Gets the object that the property is a member of
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetTargetObjectWithProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        private static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        private static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for(int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }



        public static void SetValue(SerializedProperty prop, object value)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");

            switch(prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = ConvertUtil.ToInt(value);
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = ConvertUtil.ToBool(value);
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = ConvertUtil.ToSingle(value);
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = ConvertUtil.ToString(value);
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = value as Object;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = (value is LayerMask) ? ((LayerMask)value).value : ConvertUtil.ToInt(value);
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = ConvertUtil.ToInt(value);
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.arraySize = ConvertUtil.ToInt(value);
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = ConvertUtil.ToInt(value);
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = value as AnimationCurve;
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Gradient:
                    throw new System.InvalidOperationException("Can not handle Gradient types.");
            }
        }

        public static object GetValue(SerializedProperty prop)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue;
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return (LayerMask)prop.intValue;
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                case SerializedPropertyType.Rect:
                    return prop.rectValue;
                case SerializedPropertyType.ArraySize:
                    return prop.arraySize;
                case SerializedPropertyType.Character:
                    return (char)prop.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return prop.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue;
                case SerializedPropertyType.Gradient:
                    throw new System.InvalidOperationException("Can not handle Gradient types.");
            }

            return null;
        }

        #endregion

    }

}