using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(VariantReference))]
    public class VariantReferencePropertyDrawer : PropertyDrawer
    {

        public bool RestrictVariantType = false;
        private System.Type _forcedComponentType;
        public System.Type ForcedComponentType
        {
            get { return _forcedComponentType; }
            set
            {
                if (value == null)
                    _forcedComponentType = null;
                else if (typeof(Component).IsAssignableFrom(value))
                    _forcedComponentType = value;
                else
                    throw new System.ArgumentException("Type must inherit from Component", "value");
            }
        }

        private System.Type _selectedComponentType;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);

            var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
            if (variant == null)
            {
                variant = new VariantReference();
                EditorHelper.SetTargetObjectOfProperty(property, variant);
                property.serializedObject.ApplyModifiedProperties();
            }

            var totalRect = EditorGUI.PrefixLabel(position, label);

            var r0 = new Rect(totalRect.xMin, totalRect.yMin, 90.0f, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(r0.xMax, totalRect.yMin, totalRect.xMax - r0.xMax, EditorGUIUtility.singleLineHeight);

            var bCache = GUI.enabled;
            if (this.RestrictVariantType) GUI.enabled = false;
            variant.ValueType = (VariantReference.VariantType)EditorGUI.EnumPopup(r0, GUIContent.none, variant.ValueType);
            GUI.enabled = bCache;

            switch (variant.ValueType)
            {
                case VariantReference.VariantType.Null:
                    GUI.enabled = false;
                    EditorGUI.TextField(r1, "Null");
                    GUI.enabled = true;
                    break;
                case VariantReference.VariantType.String:
                    variant.StringValue = EditorGUI.TextField(r1, variant.StringValue);
                    break;
                case VariantReference.VariantType.Boolean:
                    variant.BoolValue = EditorGUI.Toggle(r1, variant.BoolValue);
                    break;
                case VariantReference.VariantType.Integer:
                    variant.IntValue = EditorGUI.IntField(r1, variant.IntValue);
                    break;
                case VariantReference.VariantType.Float:
                    variant.FloatValue = EditorGUI.FloatField(r1, variant.FloatValue);
                    break;
                case VariantReference.VariantType.Double:
                    variant.DoubleValue = ConvertUtil.ToDouble(EditorGUI.TextField(r1, variant.DoubleValue.ToString()));
                    break;
                case VariantReference.VariantType.Vector2:
                    variant.Vector2Value = EditorGUI.Vector2Field(r1, GUIContent.none, variant.Vector2Value);
                    break;
                case VariantReference.VariantType.Vector3:
                    variant.Vector3Value = EditorGUI.Vector3Field(r1, GUIContent.none, variant.Vector3Value);
                    break;
                case VariantReference.VariantType.Quaternion:
                    variant.QuaternionValue = SPEditorGUI.QuaternionField(r1, GUIContent.none, variant.QuaternionValue);
                    break;
                case VariantReference.VariantType.Color:
                    variant.ColorValue = EditorGUI.ColorField(r1, variant.ColorValue);
                    break;
                case VariantReference.VariantType.DateTime:
                    variant.DateValue = ConvertUtil.ToDate(EditorGUI.TextField(r1, variant.DateValue.ToString()));
                    break;
                case VariantReference.VariantType.GameObject:
                    variant.GameObjectValue = EditorGUI.ObjectField(r1, variant.GameObjectValue, typeof(GameObject), true) as GameObject;
                    break;
                case VariantReference.VariantType.Component:
                    if (_forcedComponentType == null)
                    {
                        var totalMax = r1.xMax;
                        r1 = new Rect(r1.xMin, r1.yMin, Mathf.Max(100f, r1.width - 100f), r1.height);
                        var r2 = new Rect(r1.xMax, r1.yMin, totalMax - r1.xMax, r1.height);
                        if (_selectedComponentType == null && variant.ComponentValue != null)
                        {
                            _selectedComponentType = variant.ComponentValue.GetType();
                        }
                        _selectedComponentType = SPEditorGUI.TypeDropDown(r2, GUIContent.none, typeof(Component), _selectedComponentType, false, false, null, TypeDropDownListingStyle.ComponentMenu);
                        if (_selectedComponentType != null)
                        {
                            variant.ComponentValue = EditorGUI.ObjectField(r1, variant.ComponentValue, _selectedComponentType, true) as Component;

                            //DefaultFromSelfAttribute done here because DefaultFromSelfAttribute class can't do it itself
                            if (variant.ComponentValue == null && GameObjectUtil.IsGameObjectSource(property.serializedObject) && this.fieldInfo.GetCustomAttributes(typeof(com.spacepuppy.DefaultFromSelfAttribute), false).Count() > 0)
                            {
                                var go = GameObjectUtil.GetGameObjectFromSource(property.serializedObject);
                                variant.ComponentValue = go.GetComponent(_selectedComponentType);
                            }
                        }
                        else
                        {
                            //EditorGUI.LabelField(r1, "Select Component Type");
                            r1.xMin += 10.0f;
                            EditorGUI.HelpBox(r1, "Select Component Type", MessageType.Info);
                        }
                    }
                    else
                    {
                        variant.ComponentValue = EditorGUI.ObjectField(r1, variant.ComponentValue, _forcedComponentType, true) as Component;

                        //DefaultFromSelfAttribute done here because DefaultFromSelfAttribute class can't do it itself
                        if (variant.ComponentValue == null && GameObjectUtil.IsGameObjectSource(property.serializedObject) && this.fieldInfo.GetCustomAttributes(typeof(com.spacepuppy.DefaultFromSelfAttribute), false).Count() > 0)
                        {
                            var go = GameObjectUtil.GetGameObjectFromSource(property.serializedObject);
                            variant.ComponentValue = go.GetComponent(_forcedComponentType);
                        }
                    }

                    break;
                case VariantReference.VariantType.Object:
                    variant.ObjectValue = EditorGUI.ObjectField(r1, variant.ObjectValue, typeof(UnityEngine.Object), true);
                    break;
            }

            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck()) property.serializedObject.Update();
        }

    }
}
