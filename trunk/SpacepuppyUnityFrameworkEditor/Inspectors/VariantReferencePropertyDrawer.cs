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

        private System.Type _componentType;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var h = base.GetPropertyHeight(property, label);

            if(property.isExpanded)
            {
                var targ = EditorHelper.GetTargetObjectWithProperty(property);
                var variant = this.fieldInfo.GetValue(targ) as VariantReference;
                if (variant != null && variant.ValueType == VariantReference.VariantType.Component)
                {
                    h = h * 4f + 4f;
                }
                else
                {
                    h = h * 3f + 2f;
                }
            }
            
            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.isExpanded)
            {
                //var targ = EditorHelper.GetTargetObjectWithProperty(property);
                //var variant = this.fieldInfo.GetValue(targ) as VariantReference;
                //if (variant == null)
                //{
                //    variant = new VariantReference();
                //    this.fieldInfo.SetValue(targ, variant);
                //}
                var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
                if(variant == null)
                {
                    variant = new VariantReference();
                    EditorHelper.SetTargetObjectOfProperty(property, variant);
                }

                var h = base.GetPropertyHeight(property, label);
                var r0 = new Rect(position.xMin, position.yMin, position.width, h);
                var r1 = new Rect(position.xMin + 10f, position.yMin + h, position.width - 10f, h);
                var r2 = new Rect(position.xMin + 10f, position.yMin + h * 2f + 2f, position.width - 10f, h);


                property.isExpanded = EditorGUI.Foldout(r0, property.isExpanded, label);
                variant.ValueType = (VariantReference.VariantType)EditorGUI.EnumPopup(r1, "Value Type", variant.ValueType);

                switch(variant.ValueType)
                {
                    case VariantReference.VariantType.Null:
                        GUI.enabled = false;
                        EditorGUI.TextField(r2, "Value", "Null");
                        GUI.enabled = true;
                        break;
                    case VariantReference.VariantType.String:
                        variant.StringValue = EditorGUI.TextField(r2, "Value", variant.StringValue);
                        break;
                    case VariantReference.VariantType.Boolean:
                        variant.BoolValue = EditorGUI.Toggle(r2, "Value", variant.BoolValue);
                        break;
                    case VariantReference.VariantType.Integer:
                        variant.IntValue = EditorGUI.IntField(r2, "Value", variant.IntValue);
                        break;
                    case VariantReference.VariantType.Float:
                        variant.FloatValue = EditorGUI.FloatField(r2, "Value", variant.FloatValue);
                        break;
                    case VariantReference.VariantType.Double:
                        variant.DoubleValue = ConvertUtil.ToDouble(EditorGUI.TextField(r2, "Value", variant.DoubleValue.ToString()));
                        break;
                    case VariantReference.VariantType.Vector2:
                        variant.Vector2Value = EditorGUI.Vector2Field(r2, "Value", variant.Vector2Value);
                        break;
                    case VariantReference.VariantType.Vector3:
                        variant.Vector3Value = EditorGUI.Vector3Field(r2, "Value", variant.Vector3Value);
                        break;
                    case VariantReference.VariantType.Quaternion:
                        variant.QuaternionValue = EditorHelper.QuaternionField(r2, new GUIContent("Value"), variant.QuaternionValue);
                        break;
                    case VariantReference.VariantType.Color:
                        variant.ColorValue = EditorGUI.ColorField(r2, "Value", variant.ColorValue);
                        break;
                    case VariantReference.VariantType.DateTime:
                        variant.DateValue = ConvertUtil.ToDate(EditorGUI.TextField(r2, "Value", variant.DateValue.ToString()));
                        break;
                    case VariantReference.VariantType.GameObject:
                        variant.GameObjectValue = EditorGUI.ObjectField(r2, "Value", variant.GameObjectValue, typeof(GameObject), true) as GameObject;
                        break;
                    case VariantReference.VariantType.Component:
                        var r3 = new Rect(position.xMin + 10f, position.yMin + h * 3f + 4f, position.width - 10f, h);
                        if(_componentType == null && variant.ComponentValue != null)
                        {
                            _componentType = variant.ComponentValue.GetType();
                        }
                        _componentType = EditorHelper.TypeDropDown(r2, new GUIContent("Component Type"), typeof(Component), _componentType, false, false, null, TypeDropDownListingStyle.ComponentMenu);
                        if (_componentType != null)
                        {
                            variant.ComponentValue = EditorGUI.ObjectField(r3, "Value", variant.ComponentValue, _componentType, true) as Component;

                            //DefaultFromSelfAttribute done here because DefaultFromSelfAttribute class can't do it itself
                            if(variant.ComponentValue == null && GameObjectUtil.IsGameObjectSource(property.serializedObject) && this.fieldInfo.GetCustomAttributes(typeof(com.spacepuppy.DefaultFromSelfAttribute), false).Count() > 0)
                            {
                                var go = GameObjectUtil.GetGameObjectFromSource(property.serializedObject);
                                variant.ComponentValue = go.GetComponent(_componentType);
                            }
                        }
                        else
                        {
                            EditorGUI.LabelField(r3, "Select Component Type");
                        }

                        break;
                }
            }
            else
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            }

            EditorGUI.EndProperty();
        }

    }
}
