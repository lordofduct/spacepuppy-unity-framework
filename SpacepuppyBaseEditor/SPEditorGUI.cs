using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;
using com.spacepuppy.Dynamic;

using com.spacepuppyeditor.Base;
using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor
{
    public static class SPEditorGUI
    {

        #region Fields

        private static TypeAccessWrapper _accessWrapper;

        private static int s_FoldoutHash = "Foldout".GetHashCode();

        private static System.Func<Rect, int, GUIContent, int, Rect> _imp_MultiFieldPrefixLabel;
        //private static System.Action<Rect, GUIContent[], float[], float> _imp_MultiFloatField_01;
        private static System.Func<SerializedProperty, GUIContent, float> _imp_GetSinglePropertyHeight;
        private static System.Func<GUIContent, Rect, Gradient, Gradient> _imp_GradientField;
        private static System.Func<Rect, string, string> _imp_SearchField;

        #endregion

        #region CONSTRUCTOR

        static SPEditorGUI()
        {
            var klass = InternalTypeUtil.UnityEditorAssembly.GetType("UnityEditor.EditorGUI");
            _accessWrapper = new TypeAccessWrapper(klass, true);
        }

        #endregion
        
        #region Internal EditorGUI Methods

        public static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_imp_GetSinglePropertyHeight == null) _imp_GetSinglePropertyHeight = _accessWrapper.GetStaticMethod("GetSinglePropertyHeight", typeof(System.Func<SerializedProperty, GUIContent, float>)) as System.Func<SerializedProperty, GUIContent, float>;
            return _imp_GetSinglePropertyHeight(property, label);
        }

        public static bool HasVisibleChildFields(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.Bounds:
                    return false;
                default:
                    return property.hasVisibleChildren;
            }
        }

        internal static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns)
        {
            if (_imp_MultiFieldPrefixLabel == null) _imp_MultiFieldPrefixLabel = _accessWrapper.GetStaticMethod("MultiFieldPrefixLabel", typeof(System.Func<Rect, int, GUIContent, int, Rect>)) as System.Func<Rect, int, GUIContent, int, Rect>;
            return _imp_MultiFieldPrefixLabel(totalPosition, id, label, columns);
        }

        public static void MultiFloatField(Rect position, GUIContent[] subLabels, float[] values)
        {
            //EditorGUI.MultiFloatField(position, subLabels, values);
            SPEditorGUI.MultiFloatField(position, subLabels, values, 13f);
        }

        public static void MultiFloatField(Rect position, GUIContent[] subLabels, float[] values, float labelWidth)
        {
            //if (_imp_MultiFloatField_01 == null) _imp_MultiFloatField_01 = _accessWrapper.GetStaticMethod("MultiFloatField", typeof(System.Action<Rect, GUIContent[], float[], float>)) as System.Action<Rect, GUIContent[], float[], float>;
            //_imp_MultiFloatField_01(position, subLabels, values, labelWidth);

            int length = values.Length;
            float num = (position.width - (float)(length - 1) * 2f) / (float)length;
            Rect position1 = new Rect(position);
            position1.width = num;
            float labelWidthCache = EditorGUIUtility.labelWidth;
            int indentLevelCache = EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = 0;
            for (int index = 0; index < values.Length; ++index)
            {
                values[index] = EditorGUI.FloatField(position1, subLabels[index], values[index]);
                position1.x += num + 2f;
            }
            EditorGUIUtility.labelWidth = labelWidthCache;
            EditorGUI.indentLevel = indentLevelCache;
        }

        public static void MultiFloatField(Rect position, GUIContent label, GUIContent[] subLabels, float[] values)
        {
            //EditorGUI.MultiFloatField(position, label, subLabels, values);

            int controlId = GUIUtility.GetControlID(SPEditorGUI.s_FoldoutHash, FocusType.Passive, position);
            position = SPEditorGUI.MultiFieldPrefixLabel(position, controlId, label, subLabels.Length);
            position.height = EditorGUIUtility.singleLineHeight;
            SPEditorGUI.MultiFloatField(position, subLabels, values);
        }

        public static void MultiFloatField(Rect position, GUIContent label, GUIContent[] subLabels, float[] values, float labelWidth)
        {
            int controlId = GUIUtility.GetControlID(SPEditorGUI.s_FoldoutHash, FocusType.Passive, position);
            position = SPEditorGUI.MultiFieldPrefixLabel(position, controlId, label, subLabels.Length);
            position.height = EditorGUIUtility.singleLineHeight;
            SPEditorGUI.MultiFloatField(position, subLabels, values, labelWidth);
        }



        public static void DelayedMultiFloatField(Rect position, GUIContent[] subLabels, float[] values)
        {
            SPEditorGUI.DelayedMultiFloatField(position, subLabels, values, 13f);
        }

        public static void DelayedMultiFloatField(Rect position, GUIContent[] subLabels, float[] values, float labelWidth)
        {
            //if (_imp_MultiFloatField_01 == null) _imp_MultiFloatField_01 = _accessWrapper.GetStaticMethod("MultiFloatField", typeof(System.Action<Rect, GUIContent[], float[], float>)) as System.Action<Rect, GUIContent[], float[], float>;
            //_imp_MultiFloatField_01(position, subLabels, values, labelWidth);

            int length = values.Length;
            float num = (position.width - (float)(length - 1) * 2f) / (float)length;
            Rect position1 = new Rect(position);
            position1.width = num;
            float labelWidthCache = EditorGUIUtility.labelWidth;
            int indentLevelCache = EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = 0;
            for (int index = 0; index < values.Length; ++index)
            {
                values[index] = EditorGUI.DelayedFloatField(position1, subLabels[index], values[index]);
                position1.x += num + 2f;
            }
            EditorGUIUtility.labelWidth = labelWidthCache;
            EditorGUI.indentLevel = indentLevelCache;
        }

        public static void DelayedMultiFloatField(Rect position, GUIContent label, GUIContent[] subLabels, float[] values)
        {
            //EditorGUI.MultiFloatField(position, label, subLabels, values);

            int controlId = GUIUtility.GetControlID(SPEditorGUI.s_FoldoutHash, FocusType.Passive, position);
            position = SPEditorGUI.MultiFieldPrefixLabel(position, controlId, label, subLabels.Length);
            position.height = EditorGUIUtility.singleLineHeight;
            SPEditorGUI.DelayedMultiFloatField(position, subLabels, values);
        }

        public static void DelayedMultiFloatField(Rect position, GUIContent label, GUIContent[] subLabels, float[] values, float labelWidth)
        {
            int controlId = GUIUtility.GetControlID(SPEditorGUI.s_FoldoutHash, FocusType.Passive, position);
            position = SPEditorGUI.MultiFieldPrefixLabel(position, controlId, label, subLabels.Length);
            position.height = EditorGUIUtility.singleLineHeight;
            SPEditorGUI.DelayedMultiFloatField(position, subLabels, values, labelWidth);
        }



        public static string SearchField(Rect position, string search)
        {
            if (_imp_SearchField == null) _imp_SearchField = _accessWrapper.GetStaticMethod("SearchField", typeof(System.Func<Rect, string, string>)) as System.Func<Rect, string, string>;
            return _imp_SearchField(position, search);
        }

        #endregion


        #region Prefix

        public static bool PrefixFoldoutLabel(Rect position, bool foldout, GUIContent label)
        {
            //EditorGUI.PrefixLabel(position, label);

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, EditorGUIUtility.labelWidth), EditorGUIUtility.singleLineHeight);
            return EditorGUI.Foldout(r, foldout, label);
        }

        public static bool PrefixFoldoutLabel(ref Rect position, bool foldout, GUIContent label)
        {
            //EditorGUI.PrefixLabel(position, label);
            
            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, EditorGUIUtility.labelWidth), EditorGUIUtility.singleLineHeight);
            position = new Rect(position.xMin + r.width, position.yMin, position.width - r.width, position.height);
            return EditorGUI.Foldout(r, foldout, label);
        }

        #endregion

        #region DefaultPropertyField

        public static float GetDefaultPropertyHeight(SerializedProperty property)
        {
            //return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.GetHeight(property, GUIContent.none, true);
            return ScriptAttributeUtility.SharedNullPropertyHandler.GetHeight(property, GUIContent.none, false);
        }

        public static float GetDefaultPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.GetHeight(property, label, true);
            return ScriptAttributeUtility.SharedNullPropertyHandler.GetHeight(property, label, false);
        }

        public static float GetDefaultPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            //return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.GetHeight(property, label, includeChildren);
            return ScriptAttributeUtility.SharedNullPropertyHandler.GetHeight(property, label, includeChildren);
        }

        public static bool DefaultPropertyField(Rect position, SerializedProperty property)
        {
            //return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUI(position, property, GUIContent.none, true);
            return ScriptAttributeUtility.SharedNullPropertyHandler.OnGUI(position, property, GUIContent.none, false);
        }

        public static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            //return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUI(position, property, label, true);
            return ScriptAttributeUtility.SharedNullPropertyHandler.OnGUI(position, property, label, false);
        }

        public static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            //return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUI(position, property, label, includeChildren);
            return ScriptAttributeUtility.SharedNullPropertyHandler.OnGUI(position, property, label, includeChildren);
        }
        
        public static object DefaultPropertyField(Rect position, string label, object value, System.Type valueType)
        {
            return DefaultPropertyField(position, EditorHelper.TempContent(label), value, valueType);
        }

        public static object DefaultPropertyField(Rect position, GUIContent label, object value, System.Type valueType)
        {
            SerializedPropertyType propertyType = SerializedPropertyType.Generic;
            if (valueType != null) propertyType = (valueType.IsInterface) ? SerializedPropertyType.ObjectReference : EditorHelper.GetPropertyType(valueType);

            switch (propertyType)
            {
                case SerializedPropertyType.Integer:
                    EditorGUI.BeginChangeCheck();
                    int num1 = EditorGUI.IntField(position, label, ConvertUtil.ToInt(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return num1;
                    }
                    else
                        break;
                case SerializedPropertyType.Boolean:
                    EditorGUI.BeginChangeCheck();
                    bool flag2 = EditorGUI.Toggle(position, label, ConvertUtil.ToBool(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return flag2;
                    }
                    else
                        break;
                case SerializedPropertyType.Float:
                    EditorGUI.BeginChangeCheck();
                    float num2 = EditorGUI.FloatField(position, label, ConvertUtil.ToSingle(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return num2;
                    }
                    else
                        break;
                case SerializedPropertyType.String:
                    EditorGUI.BeginChangeCheck();
                    string str1 = EditorGUI.TextField(position, label, ConvertUtil.ToString(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return str1;
                    }
                    else
                        break;
                case SerializedPropertyType.Color:
                    EditorGUI.BeginChangeCheck();
                    Color color = EditorGUI.ColorField(position, label, ConvertUtil.ToColor(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return color;
                    }
                    else
                        break;
                case SerializedPropertyType.ObjectReference:
                    EditorGUI.BeginChangeCheck();
                    object obj = EditorGUI.ObjectField(position, label, value as UnityEngine.Object, valueType, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        return obj;
                    }
                    break;
                case SerializedPropertyType.LayerMask:
                    EditorGUI.BeginChangeCheck();
                    LayerMask mask = (value is LayerMask) ? (LayerMask)value : (LayerMask)ConvertUtil.ToInt(value);
                    mask = SPEditorGUI.LayerMaskField(position, label, mask);
                    if (EditorGUI.EndChangeCheck())
                    {
                        return mask;
                    }
                    break;
                case SerializedPropertyType.Enum:
                    if (valueType.GetCustomAttributes(typeof(System.FlagsAttribute), false).Any())
                    {
                        EditorGUI.BeginChangeCheck();
                        var e = SPEditorGUI.EnumFlagField(position, label, ConvertUtil.ToEnumOfType(valueType, value));
                        if (EditorGUI.EndChangeCheck())
                        {
                            return e;
                        }
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        var e = SPEditorGUI.EnumPopupExcluding(position, label, ConvertUtil.ToEnumOfType(valueType, value));
                        if (EditorGUI.EndChangeCheck())
                        {
                            return e;
                        }
                    }
                    break;
                case SerializedPropertyType.Vector2:
                    EditorGUI.BeginChangeCheck();
                    var v2 = EditorGUI.Vector2Field(position, label, ConvertUtil.ToVector2(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return v2;
                    }
                    break;
                case SerializedPropertyType.Vector3:
                    EditorGUI.BeginChangeCheck();
                    var v3 = EditorGUI.Vector3Field(position, label, ConvertUtil.ToVector3(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return v3;
                    }
                    break;
                case SerializedPropertyType.Vector4:
                    EditorGUI.BeginChangeCheck();
                    var v4 = EditorGUI.Vector4Field(position, label.text, ConvertUtil.ToVector4(value));
                    if (EditorGUI.EndChangeCheck())
                    {
                        return v4;
                    }
                    break;
                case SerializedPropertyType.Rect:
                    EditorGUI.BeginChangeCheck();
                    Rect rect = (value is Rect) ? (Rect)value : new Rect();
                    rect = EditorGUI.RectField(position, label, rect);
                    if (EditorGUI.EndChangeCheck())
                    {
                        return rect;
                    }
                    break;
                case SerializedPropertyType.ArraySize:
                    EditorGUI.BeginChangeCheck();
                    int num3 = EditorGUI.IntField(position, label, ConvertUtil.ToInt(value), EditorStyles.numberField);
                    if (EditorGUI.EndChangeCheck())
                    {
                        return num3;
                    }
                    break;
                case SerializedPropertyType.Character:
                    bool changed = GUI.changed;
                    GUI.changed = false;
                    string str2 = EditorGUI.TextField(position, label, new string(ConvertUtil.ToChar(value), 1));
                    if (GUI.changed)
                    {
                        if (str2.Length == 1)
                            return str2[0];
                        else
                            GUI.changed = false;
                    }
                    GUI.changed = GUI.changed | changed;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    EditorGUI.BeginChangeCheck();
                    AnimationCurve curve = value as AnimationCurve;
                    curve = EditorGUI.CurveField(position, label, curve);
                    if (EditorGUI.EndChangeCheck())
                    {
                        return curve;
                    }
                    break;
                case SerializedPropertyType.Bounds:
                    EditorGUI.BeginChangeCheck();
                    Bounds bnds = (value is Bounds) ? (Bounds)value : new Bounds();
                    bnds = EditorGUI.BoundsField(position, label, bnds);
                    if (EditorGUI.EndChangeCheck())
                    {
                        return bnds;
                    }
                    break;
                case SerializedPropertyType.Gradient:
                    EditorGUI.BeginChangeCheck();
                    Gradient grad = value as Gradient;
                    grad = SPEditorGUI.GradientField(position, label, grad);
                    if (EditorGUI.EndChangeCheck())
                    {
                        return grad;
                    }
                    break;
                default:
                    EditorGUI.PrefixLabel(position, label);
                    break;
            }

            return value;
        }

        #endregion

        #region FlatPropertyField

        /// <summary>
        /// Draws all children of a property
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool FlatChildPropertyField(Rect position, SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException("property");

            EditorGUI.BeginChangeCheck();
            var iterator = property.Copy();
            var end = property.GetEndProperty();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (SerializedProperty.EqualContents(iterator, end))
                    break;

                var h = GetPropertyHeight(iterator);
                var rect = new Rect(position.xMin, position.yMin, position.width, h);
                position = new Rect(position.xMin, rect.yMax, position.width, position.height - h);
                PropertyField(rect, iterator, EditorHelper.TempContent(iterator.displayName), true);
            }
            return EditorGUI.EndChangeCheck();
        }

        #endregion

        #region PropertyFields

        public static float GetPropertyHeight(SerializedProperty property)
        {
            return com.spacepuppyeditor.Internal.ScriptAttributeUtility.GetHandler(property).GetHeight(property, GUIContent.none, false);
        }

        public static float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return com.spacepuppyeditor.Internal.ScriptAttributeUtility.GetHandler(property).GetHeight(property, label, false);
        }

        public static float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return com.spacepuppyeditor.Internal.ScriptAttributeUtility.GetHandler(property).GetHeight(property, label, includeChildren);
        }

        public static bool PropertyField(Rect position, SerializedProperty property, bool includeChildren = false)
        {
            return PropertyField(position, property, (GUIContent)null, includeChildren);
        }

        public static bool PropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren = false)
        {
            return com.spacepuppyeditor.Internal.ScriptAttributeUtility.GetHandler(property).OnGUI(position, property, label, includeChildren);
        }

        #endregion

        #region ObjectField w/ X-btn

        public static void ObjectFieldX(Rect position, SerializedProperty property, GUIContent label)
        {
            if (SPEditorGUI.XButton(ref position, "Clear Selected Object", true))
            {
                property.objectReferenceValue = null;
            }
            EditorGUI.ObjectField(position, property, label);
        }

        public static UnityEngine.Object ObjectFieldX(Rect position, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects)
        {
            if (SPEditorGUI.XButton(ref position, "Clear Selected Object", true))
            {
                obj = null;
            }
            obj = EditorGUI.ObjectField(position, obj, objType, allowSceneObjects);
            return obj;
        }

        public static void ObjectFieldX(Rect position, SerializedProperty property, System.Type objType, GUIContent label)
        {
            if (SPEditorGUI.XButton(ref position, "Clear Selected Object", true))
            {
                property.objectReferenceValue = null;
            }
            EditorGUI.ObjectField(position, property, objType, label);
        }

        public static UnityEngine.Object ObjectFieldX(Rect position, GUIContent label, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects)
        {
            if (SPEditorGUI.XButton(ref position, "Clear Selected Object", true))
            {
                obj = null;
            }
            obj = EditorGUI.ObjectField(position, label, obj, objType, allowSceneObjects);
            return obj;
        }

        public static UnityEngine.Object ObjectFieldX(Rect position, string label, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects)
        {
            if (SPEditorGUI.XButton(ref position, "Clear Selected Object", true))
            {
                obj = null;
            }
            obj = EditorGUI.ObjectField(position, label, obj, objType, allowSceneObjects);
            return obj;
        }

        #endregion

        #region LayerMaskField

        public static LayerMask LayerMaskField(Rect position, string label, int selectedMask)
        {
            return EditorGUI.MaskField(position, label, selectedMask, LayerUtil.GetAllLayerNames());
        }

        public static LayerMask LayerMaskField(Rect position, GUIContent label, int selectedMask)
        {
            return EditorGUI.MaskField(position, label, selectedMask, LayerUtil.GetAllLayerNames());
        }

        #endregion

        #region GradientField

        public static Gradient GradientField(Rect position, string label, Gradient gradient)
        {
            return GradientField(position, EditorHelper.TempContent(label), gradient);
        }

        public static Gradient GradientField(Rect position, GUIContent label, Gradient gradient)
        {
            if (_imp_GradientField == null) _imp_GradientField = _accessWrapper.GetStaticMethod("GradientField", typeof(System.Func<GUIContent, Rect, Gradient, Gradient>)) as System.Func<GUIContent, Rect, Gradient, Gradient>;
            return _imp_GradientField(label, position, gradient);
        }

        #endregion

        #region EnumPopup Inspector

        public static System.Enum EnumPopupExcluding(Rect position, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            return EnumPopupExcluding(position, GUIContent.none, enumValue, ignoredValues);
        }

        public static System.Enum EnumPopupExcluding(Rect position, string label, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            return EnumPopupExcluding(position, EditorHelper.TempContent(label), enumValue, ignoredValues);
        }

        public static System.Enum EnumPopupExcluding(Rect position, GUIContent label, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            var etp = enumValue.GetType();
            var evalues = System.Enum.GetValues(etp).Cast<System.Enum>().Except(ignoredValues).ToArray();
            if (evalues.Length == 0) throw new System.ArgumentException("Excluded all possible values, not a valid popup.");
            var names = (from e in evalues select EditorHelper.TempContent(e.ToString())).ToArray();
            var index = EditorGUI.Popup(position, label, evalues.IndexOf(enumValue), names);
            return (index >= 0) ? evalues[index] : evalues.First();
        }

        #endregion

        #region EnumFlag Inspector

        public static int EnumFlagField(Rect position, System.Type enumType, GUIContent label, int value)
        {
            //var names = (from e in EnumUtil.GetUniqueEnumFlags(enumType) select e.ToString()).ToArray();
            //return EditorGUI.MaskField(position, label, value, names);

            var code = System.Type.GetTypeCode(enumType);
            switch(code)
            {
                case System.TypeCode.SByte:
                case System.TypeCode.Int16:
                case System.TypeCode.Int32:
                    {
                        int[] acceptedValues = (from e in EnumUtil.GetUniqueEnumFlags(enumType) select System.Convert.ToInt32(e)).ToArray();
                        return EnumFlagField(position, enumType, acceptedValues, label, value);
                    }
                case System.TypeCode.Byte:
                case System.TypeCode.UInt16:
                case System.TypeCode.UInt32:
                    {
                        int[] acceptedValues = (from e in EnumUtil.GetUniqueEnumFlags(enumType) select (int)System.Convert.ToUInt32(e)).ToArray();
                        return EnumFlagField(position, enumType, acceptedValues, label, value);
                    }
                case System.TypeCode.Int64:
                    {
                        //unity MaskField only supports 'int', so we redact all values that are too large
                        int[] acceptedValues = (from e in EnumUtil.GetUniqueEnumFlags(enumType) let i = System.Convert.ToInt64(e) where i <= int.MaxValue && i >= int.MinValue select (int)i).ToArray();
                        return EnumFlagField(position, enumType, acceptedValues, label, value);
                    }
                case System.TypeCode.UInt64:
                    {
                        //unity MaskField only supports 'int', so we redact all values that are too large
                        int[] acceptedValues = (from e in EnumUtil.GetUniqueEnumFlags(enumType) let i = (long)System.Convert.ToUInt64(e) where i <= int.MaxValue && i >= int.MinValue select (int)i).ToArray();
                        return EnumFlagField(position, enumType, acceptedValues, label, value);
                    }
                default:
                    EditorGUI.LabelField(position, label, EditorHelper.TempContent("64-bit enum not supported..."));
                    return value;
                    
            }
        }

        public static System.Enum EnumFlagField(Rect position, GUIContent label, System.Enum value)
        {
            if (value == null) throw new System.ArgumentException("Enum value must be non-null.", "value");

            var enumType = value.GetType();
            int i = EnumFlagField(position, enumType, label, System.Convert.ToInt32(value));
            return System.Enum.ToObject(enumType, i) as System.Enum;
        }

        public static int EnumFlagField(Rect position, System.Type enumType, int[] acceptedFlags, GUIContent label, int value)
        {
            if (enumType == null) throw new System.ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new System.ArgumentException("Must be an enum type.", "enumType");

            using (var lst = TempCollection.GetList<int>())
            {
                foreach (var e in acceptedFlags)
                {
                    if ((e == (1 << 31) || (e > 0 && MathUtil.IsPowerOfTwo((ulong)e)))
                       && EnumUtil.EnumValueIsDefined(e, enumType))
                        lst.Add(e);
                }

                var evalue = ConvertUtil.ToEnumOfType(enumType, value);
                int normalizedValue = 0;
                for (int i = 0; i < lst.Count; i++)
                {
                    if (EnumUtil.HasFlag(evalue, lst[i]))
                    {
                        normalizedValue |= (1 << i);
                    }
                }

                normalizedValue = EditorGUI.MaskField(position, label, normalizedValue, (from e in lst select System.Enum.GetName(enumType, e)).ToArray());

                value = 0;
                for (int i = 0; i < lst.Count; i++)
                {
                    int j = (1 << i);
                    if ((normalizedValue & j) == j)
                        value |= lst[i];
                }

                return value;
            }
        }

        public static WrapMode WrapModeField(Rect position, string label, WrapMode mode, bool allowDefault = false)
        {
            return WrapModeField(position, EditorHelper.TempContent(label), mode, allowDefault);
        }

        public static WrapMode WrapModeField(Rect position, GUIContent label, WrapMode mode, bool allowDefault = false)
        {
            if(allowDefault)
            {
                int i = 0;
                switch(mode)
                {
                    case WrapMode.Default:
                        i = 0;
                        break;
                    case WrapMode.Once:
                    //case WrapMode.Clamp: //same as once
                        i = 1;
                        break;
                    case WrapMode.Loop:
                        i = 2;
                        break;
                    case WrapMode.PingPong:
                        i = 3;
                        break;
                    case WrapMode.ClampForever:
                        i = 4;
                        break;
                }
                i = EditorGUI.Popup(position, label, i, new GUIContent[] { EditorHelper.TempContent("Default"), EditorHelper.TempContent("Once|Clamp"), EditorHelper.TempContent("Loop"), EditorHelper.TempContent("PingPong"), EditorHelper.TempContent("ClampForever") });
                switch(i)
                {
                    case 0:
                        return WrapMode.Default;
                    case 1:
                        return WrapMode.Once;
                    case 2:
                        return WrapMode.Loop;
                    case 3:
                        return WrapMode.PingPong;
                    case 4:
                        return WrapMode.ClampForever;
                    default:
                        return WrapMode.Default;
                }
            }
            else
            {
                int i = 0;
                switch (mode)
                {
                    case WrapMode.Default:
                    case WrapMode.Once:
                        //case WrapMode.Clamp: //same as once
                        i = 0;
                        break;
                    case WrapMode.Loop:
                        i = 1;
                        break;
                    case WrapMode.PingPong:
                        i = 2;
                        break;
                    case WrapMode.ClampForever:
                        i = 3;
                        break;
                }
                i = EditorGUI.Popup(position, label, i, new GUIContent[] {EditorHelper.TempContent("Once|Clamp"), EditorHelper.TempContent("Loop"), EditorHelper.TempContent("PingPong"), EditorHelper.TempContent("ClampForever") });
                switch (i)
                {
                    case 0:
                        return WrapMode.Once;
                    case 1:
                        return WrapMode.Loop;
                    case 2:
                        return WrapMode.PingPong;
                    case 3:
                        return WrapMode.ClampForever;
                    default:
                        return WrapMode.Default;
                }
            }
        }

        #endregion

        #region Type Dropdown

        public static System.Type TypeDropDown(Rect position, GUIContent label, 
                                               System.Type baseType, System.Type selectedType, 
                                               bool allowAbstractTypes = false, bool allowInterfaces = false, 
                                               System.Type defaultType = null, System.Type[] excludedTypes = null,
                                               TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace,
                                               System.Predicate<System.Type> searchPredicate = null)
        {
            /*

            if (!TypeUtil.IsType(selectedType, baseType)) selectedType = null;

            //var knownTypes = (from ass in System.AppDomain.CurrentDomain.GetAssemblies()
            //                  from tp in ass.GetTypes()
            //                  where TypeUtil.IsType(tp, baseType) && (allowAbstractTypes || !tp.IsAbstract) && (allowInterfaces || !tp.IsInterface)
            //                  orderby tp.FullName.Substring(tp.FullName.LastIndexOf(".") + 1) ascending
            //                  select tp).ToArray();
            var knownTypes = (from tp in TypeUtil.GetTypesAssignableFrom(baseType)
                              where (allowAbstractTypes || !tp.IsAbstract) && (allowInterfaces || !tp.IsInterface) && (excludedTypes == null || System.Array.IndexOf(excludedTypes, tp) < 0)
                              orderby tp.FullName.Substring(tp.FullName.LastIndexOf(".") + 1) ascending
                              select tp).ToArray();
            GUIContent[] knownTypeNames = null;
            switch (listType)
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
                            if (tp.FullName.StartsWith("UnityEngine."))
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

            */

            return TypeSelectionDropDownWindow.Popup(position, label, baseType, selectedType, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, listType, searchPredicate);
        }

        #endregion

        #region Vector Field

        public static Vector3 DelayedVector3Field(Rect position, GUIContent label, Vector3 value)
        {
            position = EditorGUI.PrefixLabel(position, label);

            const float LBL_WIDTH = 13f;
            float w = position.width / 3f;

            EditorGUI.LabelField(new Rect(position.xMin, position.yMin, LBL_WIDTH, EditorGUIUtility.singleLineHeight), "X");
            value.x = EditorGUI.DelayedFloatField(new Rect(position.xMin + LBL_WIDTH, position.yMin, w - LBL_WIDTH - 1f, EditorGUIUtility.singleLineHeight), value.x);
            position = new Rect(position.xMin + w, position.yMin, position.width - w, position.height);

            EditorGUI.LabelField(new Rect(position.xMin, position.yMin, LBL_WIDTH, EditorGUIUtility.singleLineHeight), "Y");
            value.y = EditorGUI.DelayedFloatField(new Rect(position.xMin + LBL_WIDTH, position.yMin, w - LBL_WIDTH - 1f, EditorGUIUtility.singleLineHeight), value.y);
            position = new Rect(position.xMin + w, position.yMin, position.width - w, position.height);

            EditorGUI.LabelField(new Rect(position.xMin, position.yMin, LBL_WIDTH, EditorGUIUtility.singleLineHeight), "Z");
            value.z = EditorGUI.DelayedFloatField(new Rect(position.xMin + LBL_WIDTH, position.yMin, w - LBL_WIDTH - 1f, EditorGUIUtility.singleLineHeight), value.z);
            
            return value;
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
            
            EditorGUI.BeginChangeCheck();
            var vNewRot = DelayedVector3Field(position, label, vRot);
            if(EditorGUI.EndChangeCheck())
            {
                //vNewRot.x = MathUtil.NormalizeAngle(vNewRot.x, useRadians);
                //vNewRot.y = MathUtil.NormalizeAngle(vNewRot.y, useRadians);
                //vNewRot.z = MathUtil.NormalizeAngle(vNewRot.z, useRadians);
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

        #endregion

        #region IComponentField

        public static Component ComponentField(Rect position, GUIContent label, Component value, System.Type inheritsFromType, bool allowSceneObjects)
        {
            //if (inheritsFromType == null) inheritsFromType = typeof(Component);
            //else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !typeof(IComponent).IsAssignableFrom(inheritsFromType)) throw new TypeArgumentMismatchException(inheritsFromType, typeof(IComponent), "Type must inherit from IComponent or Component.", "inheritsFromType");
            if (inheritsFromType == null) inheritsFromType = typeof(Component);
            else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !inheritsFromType.IsInterface) throw new TypeArgumentMismatchException(inheritsFromType, typeof(IComponent), "Type must inherit from Component or be an interface.", "inheritsFromType");
            if (value != null && !inheritsFromType.IsAssignableFrom(value.GetType())) throw new TypeArgumentMismatchException(value.GetType(), inheritsFromType, "value must inherit from " + inheritsFromType.Name, "value");

            if (TypeUtil.IsType(inheritsFromType, typeof(Component)))
            {
                return EditorGUI.ObjectField(position, label, value, inheritsFromType, true) as Component;
            }
            else
            {
                value = EditorGUI.ObjectField(position, label, value, typeof(Component), true) as Component;
                var go = GameObjectUtil.GetGameObjectFromSource(value);
                if (go != null)
                {
                    return go.GetComponent(inheritsFromType);
                }
            }

            return null;
        }

        public static Component ComponentField(Rect position, GUIContent label, Component value, System.Type inheritsFromType, bool allowSceneObjects, System.Type targetComponentType)
        {
            //if (inheritsFromType == null) inheritsFromType = typeof(Component);
            //else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !typeof(IComponent).IsAssignableFrom(inheritsFromType)) throw new TypeArgumentMismatchException(inheritsFromType, typeof(IComponent), "Type must inherit from IComponent or Component.", "inheritsFromType");
            if (inheritsFromType == null) inheritsFromType = typeof(Component);
            else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !inheritsFromType.IsInterface) throw new TypeArgumentMismatchException(inheritsFromType, typeof(IComponent), "Type must inherit from Component or be an interface.", "inheritsFromType");
            if (targetComponentType == null) throw new System.ArgumentNullException("targetComponentType");
            if (!typeof(Component).IsAssignableFrom(targetComponentType)) throw new TypeArgumentMismatchException(targetComponentType, typeof(Component), "targetComponentType");
            if (value != null && !targetComponentType.IsAssignableFrom(value.GetType())) throw new TypeArgumentMismatchException(value.GetType(), inheritsFromType, "value must inherit from " + inheritsFromType.Name, "value");

            if (TypeUtil.IsType(inheritsFromType, typeof(Component)))
            {
                return EditorGUI.ObjectField(position, label, value, inheritsFromType, true) as Component;
            }
            else
            {
                value = EditorGUI.ObjectField(position, label, value, typeof(Component), true) as Component;
                var go = GameObjectUtil.GetGameObjectFromSource(value);
                if (go != null)
                {
                    foreach (var c in go.GetComponents(inheritsFromType))
                    {
                        if (TypeUtil.IsType(c.GetType(), targetComponentType))
                        {
                            return c as Component;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Path Textfields

        public const float ELLIPSIS_BTN_WIDTH = 22f;
        public static string FolderPathTextfield(Rect position, string label, string path, string popupTitle)
        {
            return FolderPathTextfield(position, EditorHelper.TempContent(label), path, popupTitle);
        }
        public static string FolderPathTextfield(Rect position, GUIContent label, string path, string popupTitle)
        {
            position = EditorGUI.PrefixLabel(position, label);

            float w1 = Mathf.Max(position.width - ELLIPSIS_BTN_WIDTH, 0f);
            float w2 = Mathf.Clamp(ELLIPSIS_BTN_WIDTH, 0f, position.width - w1);
            var r1 = new Rect(position.xMin, position.yMin, w1, EditorGUIUtility.singleLineHeight);
            var r2 = new Rect(r1.xMax, position.yMin, w2, EditorGUIUtility.singleLineHeight);
            path = EditorGUI.TextField(r1, path);
            if (GUI.Button(r2, "..."))
            {
                var result = EditorUtility.OpenFolderPanel(popupTitle, path, string.Empty);
                if (!string.IsNullOrEmpty(result))
                {
                    path = result;
                }
            }
            return path;
        }

        public static string SaveFilePathTextfield(Rect position, string label, string path, string popupTitle, string extension)
        {
            return SaveFilePathTextfield(position, EditorHelper.TempContent(label), path, popupTitle, extension);
        }
        public static string SaveFilePathTextfield(Rect position, GUIContent label, string path, string popupTitle, string extension)
        {
            position = EditorGUI.PrefixLabel(position, label);

            float w1 = Mathf.Max(position.width - ELLIPSIS_BTN_WIDTH, 0f);
            float w2 = Mathf.Clamp(ELLIPSIS_BTN_WIDTH, 0f, position.width - w1);
            var r1 = new Rect(position.xMin, position.yMin, w1, EditorGUIUtility.singleLineHeight);
            var r2 = new Rect(r1.xMax, position.yMin, w2, EditorGUIUtility.singleLineHeight);
            path = EditorGUI.TextField(r1, path);
            if (GUI.Button(r2, "..."))
            {
                var result = EditorUtility.SaveFilePanel(popupTitle, System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path), extension);
                if (!string.IsNullOrEmpty(result))
                {
                    path = result;
                }
            }
            return path;
        }

        #endregion





        #region Component Selection From Source

        public static Component SelectComponentFromSourceField(Rect position, string label, GameObject source, Component selectedComp, System.Predicate<Component> filter = null)
        {
            return SelectComponentFromSourceField(position, EditorHelper.TempContent(label), source, selectedComp, filter);
        }

        public static Component SelectComponentFromSourceField(Rect position, GUIContent label, GameObject source, Component selectedComp, System.Predicate<Component> filter = null)
        {
            if (source == null)
            {

            }

            //var selectedType = (selectedComp != null) ? selectedComp.GetType() : null;
            //System.Type[] availableMechanismTypes;
            //if (filter != null)
            //    availableMechanismTypes = (from c in source.GetComponents<Component>() where filter(c) select c.GetType()).ToArray();
            //else
            //    availableMechanismTypes = (from c in source.GetComponents<Component>() select c.GetType()).ToArray();
            //var availableMechanismTypeNames = availableMechanismTypes.Select((tp) => EditorHelper.TempContent(tp.Name)).ToArray();

            //var index = System.Array.IndexOf(availableMechanismTypes, selectedType);
            //index = EditorGUI.Popup(position, label, index, availableMechanismTypeNames);
            //return (index >= 0) ? source.GetComponent(availableMechanismTypes[index]) : null;

            Component[] components;
            if (filter != null)
                components = (from c in source.GetComponents<Component>() where filter(c) select c).ToArray();
            else
                components = source.GetComponents<Component>();

            return SelectComponentField(position, label, components, selectedComp);
        }

        public static Component SelectComponentField(Rect position, string label, Component[] components, Component selectedComp)
        {
            return SelectComponentField(position, EditorHelper.TempContent(label), components, selectedComp);
        }

        public static Component SelectComponentField(Rect position, GUIContent label, Component[] components, Component selectedComp)
        {
            if (components == null) throw new System.ArgumentNullException("components");

            System.Type[] availableMechanismTypes = (from c in components select c.GetType()).ToArray();
            var availableMechanismTypeNames = availableMechanismTypes.Select((tp) => EditorHelper.TempContent(tp.Name)).ToArray();

            var index = System.Array.IndexOf(components, selectedComp);
            index = EditorGUI.Popup(position, label, index, availableMechanismTypeNames);
            return (index >= 0) ? components[index] : null;
        }

        public static Component SelectComponentField(Rect position, string label, Component[] components, string[] componentLabels, Component selectedComp)
        {
            if (components == null) throw new System.ArgumentNullException("components");
            if (componentLabels == null || componentLabels.Length != components.Length) throw new System.ArgumentException("Component Labels collection must be the same size as the component collection.", "componentLabels");

            var index = System.Array.IndexOf(components, selectedComp);
            index = EditorGUI.Popup(position, label, index, componentLabels);
            return (index >= 0) ? components[index] : null;
        }

        public static Component SelectComponentField(Rect position, GUIContent label, Component[] components, GUIContent[] componentLabels, Component selectedComp)
        {
            if (components == null) throw new System.ArgumentNullException("components");
            if (componentLabels == null || componentLabels.Length != components.Length) throw new System.ArgumentException("Component Labels collection must be the same size as the component collection.", "componentLabels");

            var index = System.Array.IndexOf(components, selectedComp);
            index = EditorGUI.Popup(position, label, index, componentLabels);
            return (index >= 0) ? components[index] : null;
        }

        #endregion
        




        #region Curve Swatch

        public static void DrawCurveSwatch(Rect position, ICurve curve, Color color, Color bgColor)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            int previewWidth = (int)position.width;
            int previewHeight = (int)position.height;
            Color color1 = GUI.color;
            GUI.color = bgColor;
            EditorHelper.WhiteTextureStyle.Draw(position, false, false, false, false);
            GUI.color = color1;

            if (curve == null) return;

            Texture2D tex = GetCurveTexture(Mathf.RoundToInt(position.width), Mathf.RoundToInt(position.height), curve, color);
            GUIStyle basicTextureStyle = GetCurveTextureStyle(tex);
            position.width = (float)tex.width;
            position.height = (float)tex.height;
            basicTextureStyle.Draw(position, false, false, false, false);
        }


        private static Texture2D s_CurveTexture;
        private static GUIStyle s_CurveTextureStyle;
        private static Texture2D GetCurveTexture(int width, int height, ICurve curve, Color color)
        {
            if (s_CurveTexture == null)
                s_CurveTexture = new Texture2D(width, height);
            else
                s_CurveTexture.Resize(width, height);

            var c = new Color(1f, 1f, 1f, 0f);
            var pixels = s_CurveTexture.GetPixels();
            for (int i = 0; i < pixels.Length; i++) pixels[i] = c;
            s_CurveTexture.SetPixels(pixels);

            for (int i = 0; i < s_CurveTexture.width; i++)
            {
                var t = (float)i / (float)width;
                int j = (int)(curve.GetPosition(t) * height);
                s_CurveTexture.SetPixel(i, j, color);
            }

            s_CurveTexture.Apply();
            return s_CurveTexture;
        }
        private static GUIStyle GetCurveTextureStyle(Texture2D tex)
        {
            if (s_CurveTextureStyle == null)
                s_CurveTextureStyle = new GUIStyle();
            s_CurveTextureStyle.normal.background = tex;
            return s_CurveTextureStyle;
        }

        #endregion

        #region ReflectedPropertyField

        /// <summary>
        /// Reflects the available properties and shows them in a dropdown
        /// </summary>
        public static string ReflectedPropertyField(Rect position, GUIContent label, object targObj, string selectedMemberName, DynamicMemberAccess access, out System.Reflection.MemberInfo selectedMember, bool allowSetterMethods = false)
        {
            if(targObj is IDynamic)
            {
                var mask = allowSetterMethods ? System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Method : System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property;
                var members = DynamicUtil.GetEasilySerializedMembers(targObj, mask, access).ToArray();
                var entries = new GUIContent[members.Length + 1];

                int index = -1;
                for (int i = 0; i < members.Length; i++)
                {
                    var m = members[i];
                    if ((DynamicUtil.GetMemberAccessLevel(m) & DynamicMemberAccess.Write) != 0)
                        entries[i] = EditorHelper.TempContent(string.Format("{0} ({1}) -> {2}", m.Name, DynamicUtil.GetReturnType(m).Name, DynamicUtil.GetValueWithMember(m, targObj, true)));
                    else
                        entries[i] = EditorHelper.TempContent(string.Format("{0} (readonly - {1}) -> {2}", m.Name, DynamicUtil.GetReturnType(m).Name, DynamicUtil.GetValueWithMember(m, targObj, true)));

                    if (index < 0 && m.Name == selectedMemberName)
                    {
                        index = i;
                    }
                }

                entries[entries.Length - 1] = EditorHelper.TempContent("...Custom");
                if (index < 0)
                    index = entries.Length - 1;

                if(index < members.Length)
                {
                    index = EditorGUI.Popup(position, label, index, entries);
                    selectedMember = (index >= 0 && index < members.Length) ? members[index] : null;
                    return (selectedMember != null) ? selectedMember.Name : null;
                }
                else
                {
                    position = EditorGUI.PrefixLabel(position, label);
                    var r0 = new Rect(position.xMin, position.yMin, position.width / 2f, position.height);
                    var r1 = new Rect(r0.xMax, r0.yMin, position.width - r0.width, r0.height);
                    index = EditorGUI.Popup(r0, index, entries);
                    if (index < members.Length)
                    {
                        selectedMember = (index >= 0) ? members[index] : null;
                        return (selectedMember != null) ? selectedMember.Name : null;
                    }
                    else
                    {
                        selectedMemberName = EditorGUI.TextField(r1, selectedMemberName);
                        selectedMember = new DynamicPropertyInfo(selectedMemberName, targObj.GetType(), typeof(Variant));
                        return selectedMemberName;
                    }
                }
            }
            if (targObj != null)
            {
                var members = DynamicUtil.GetEasilySerializedMembers(targObj, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Method, access).ToArray();
                var entries = new GUIContent[members.Length];

                int index = -1;
                for (int i = 0; i < members.Length; i++)
                {
                    var m = members[i];
                    if((DynamicUtil.GetMemberAccessLevel(m) & DynamicMemberAccess.Write) != 0)
                        entries[i] = EditorHelper.TempContent(string.Format("{0} ({1}) -> {2}", m.Name, DynamicUtil.GetReturnType(m).Name, DynamicUtil.GetValueWithMember(m, targObj, true)));
                    else
                        entries[i] = EditorHelper.TempContent(string.Format("{0} (readonly - {1}) -> {2}", m.Name, DynamicUtil.GetReturnType(m).Name, DynamicUtil.GetValueWithMember(m, targObj, true)));

                    if (index < 0 && m.Name == selectedMemberName)
                    {
                        index = i;
                    }
                }
                
                index = EditorGUI.Popup(position, label, index, entries);
                selectedMember = (index >= 0) ? members[index] : null;
                return (selectedMember != null) ? selectedMember.Name : null;
            }
            else
            {
                selectedMember = null;
                EditorGUI.Popup(position, label, -1, new GUIContent[0]);
                return null;
            }
        }
        
        public static string ReflectedPropertyField(Rect position, GUIContent label, object targObj, string selectedMemberName, DynamicMemberAccess access, bool allowSetterMethods = false)
        {
            System.Reflection.MemberInfo selectedMember;
            return ReflectedPropertyField(position, label, targObj, selectedMemberName, access, out selectedMember, allowSetterMethods);
        }

        public static string ReflectedPropertyField(Rect position, object targObj, string selectedMemberName, DynamicMemberAccess access, out System.Reflection.MemberInfo selectedMember, bool allowSetterMethods = false)
        {
            return ReflectedPropertyField(position, GUIContent.none, targObj, selectedMemberName, access, out selectedMember, allowSetterMethods);
        }

        public static string ReflectedPropertyField(Rect position, object targObj, string selectedMemberName, DynamicMemberAccess access, bool allowSetterMethods = false)
        {
            System.Reflection.MemberInfo selectedMember;
            return ReflectedPropertyField(position, GUIContent.none, targObj, selectedMemberName, access, out selectedMember, allowSetterMethods);
        }



        /// <summary>
        /// Reflects the available properties and shows them in a dropdown
        /// </summary>
        public static string ReflectedPropertyField(Rect position, GUIContent label, System.Type targType, string selectedMemberName, out System.Reflection.MemberInfo selectedMember, bool allowSetterMethods = false)
        {
            if (targType != null)
            {
                var mask = allowSetterMethods ? System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Method : System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property;
                var members = com.spacepuppy.Dynamic.DynamicUtil.GetEasilySerializedMembersFromType(targType, mask).ToArray();

                int index = -1;
                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i].Name == selectedMemberName)
                    {
                        index = i;
                        break;
                    }
                }

                index = EditorGUI.Popup(position, label, index, (from m in members select new GUIContent(string.Format("{0} ({1})", m.Name, DynamicUtil.GetReturnType(m).Name))).ToArray());
                selectedMember = (index >= 0) ? members[index] : null;
                return (selectedMember != null) ? selectedMember.Name : null;
            }
            else
            {
                selectedMember = null;
                EditorGUI.Popup(position, label, -1, new GUIContent[0]);
                return null;
            }
        }

        public static string ReflectedPropertyField(Rect position, GUIContent label, System.Type targType, string selectedMemberName, bool allowSetterMethods = false)
        {
            System.Reflection.MemberInfo selectedMember;
            return ReflectedPropertyField(position, label, targType, selectedMemberName, out selectedMember, allowSetterMethods);
        }

        public static string ReflectedPropertyField(Rect position, System.Type targType, string selectedMemberName, out System.Reflection.MemberInfo selectedMember, bool allowSetterMethods = false)
        {
            return ReflectedPropertyField(position, GUIContent.none, targType, selectedMemberName, out selectedMember, allowSetterMethods);
        }

        public static string ReflectedPropertyField(Rect position, System.Type targType, string selectedMemberName, bool allowSetterMethods = false)
        {
            System.Reflection.MemberInfo selectedMember;
            return ReflectedPropertyField(position, GUIContent.none, targType, selectedMemberName, out selectedMember, allowSetterMethods);
        }

        #endregion
        
        #region X Button

        public const float X_BTN_WIDTH = 30f;
        public static bool XButton(Vector2 pos, string tooltip = null)
        {
            var r = new Rect(pos.x, pos.y, X_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            return GUI.Button(r, EditorHelper.TempContent("X", tooltip));
        }
        public static bool XButton(Rect position, string tooltip = null)
        {
            var r = new Rect(position.xMin, position.yMin, Mathf.Min(X_BTN_WIDTH, position.width), EditorGUIUtility.singleLineHeight);
            return GUI.Button(r, EditorHelper.TempContent("X", tooltip));
        }
        public static bool XButton(ref Rect position, string tooltip = null, bool rightSide = true)
        {
            var w = Mathf.Min(X_BTN_WIDTH, position.width);
            Rect r;
            if(rightSide)
            {
                r = new Rect(position.xMax - w, position.yMin, w, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.xMin, position.yMin, position.width - w, position.height);
            }
            else
            {
                r = new Rect(position.xMin, position.yMin, w, EditorGUIUtility.singleLineHeight);
                position = new Rect(r.xMax, position.yMin, position.width - w, position.height);
            }

            return GUI.Button(r, EditorHelper.TempContent("X", tooltip));
        }

        public static bool PlayButton(ref Rect position, string tooltip = null, bool rightSide = true)
        {
            var w = Mathf.Min(X_BTN_WIDTH, position.width);
            Rect r;
            if (rightSide)
            {
                r = new Rect(position.xMax - w, position.yMin, w, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.xMin, position.yMin, position.width - w, position.height);
            }
            else
            {
                r = new Rect(position.xMin, position.yMin, w, EditorGUIUtility.singleLineHeight);
                position = new Rect(r.xMax, position.yMin, position.width - w, position.height);
            }

            return GUI.Button(r, EditorHelper.TempContent(">", tooltip));
        }

        #endregion

    }
}
