﻿using UnityEngine;
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

            var prop = editor.serializedObject.GetIterator();
            for (bool enterChildren = true; prop.NextVisible(enterChildren); enterChildren = false)
            {
                if (propsNotToDraw == null || !propsNotToDraw.Contains(prop.name))
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }
        }

        #endregion

        #region SerializedProperty Helpers

        public static System.Type GetScriptTypeFromProperty(SerializedProperty prop)
        {
            SerializedProperty scriptProp = prop.serializedObject.FindProperty(PROP_SCRIPT);
            if (scriptProp == null)
                return null;
            MonoScript monoScript = scriptProp.objectReferenceValue as MonoScript;
            if ((UnityEngine.Object)monoScript == (UnityEngine.Object)null)
                return null;
            else
                return monoScript.GetClass();
        }

        /// <summary>
        /// Returns the fieldInfo of the property. If the property is an Array/List, the fieldInfo for the Array is returned.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            var scriptType = GetScriptTypeFromProperty(prop);
            var elements = path.Split('.');

            FieldInfo result = null;
            System.Type tp = scriptType;
            foreach (var element in elements)
            {
                if (element.Contains('['))
                {
                    var name = element.Substring(0, element.IndexOf('['));
                    FieldInfo info = null;
                    for (var tp2 = tp; info == null && tp2 != null; tp2 = tp2.BaseType)
                        info = tp2.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (info == null)
                    {
                        return null;
                    }
                    else
                    {
                        result = info;
                        tp = info.FieldType;
                    }

                    if (ObjUtil.IsListType(tp))
                    {
                        tp = ObjUtil.GetElementTypeOfListType(tp);
                    }
                }
                else
                {
                    FieldInfo info = null;
                    for (var tp2 = tp; info == null && tp2 != null; tp2 = tp2.BaseType)
                        info = tp2.GetField(element, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (info == null)
                    {
                        return null;
                    }
                    else
                    {
                        result = info;
                        tp = info.FieldType;
                    }
                }
            }

            return result;
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
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
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
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (Object.ReferenceEquals(obj, null)) return;

            try
            {
                var element = elements.Last();

                if (element.Contains("["))
                {
                    //var tp = obj.GetType();
                    //var elementName = element.Substring(0, element.IndexOf("["));
                    //var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    //var field = tp.GetField(elementName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    //var arr = field.GetValue(obj) as System.Collections.IList;
                    //arr[index] = value;
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    var arr = ObjUtil.GetValue(element, elementName) as System.Collections.IList;
                    if (arr != null) arr[index] = value;
                }
                else
                {
                    //var tp = obj.GetType();
                    //var field = tp.GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    //if (field != null)
                    //{
                    //    field.SetValue(obj, value);
                    //}
                    ObjUtil.SetValue(obj, element, value);
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
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }



        public static void SetPropertyValue(SerializedProperty prop, object value)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");

            switch (prop.propertyType)
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

        public static object GetPropertyValue(SerializedProperty prop)
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