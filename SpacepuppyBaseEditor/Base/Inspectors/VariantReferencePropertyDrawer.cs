using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(VariantReference))]
    public class VariantReferencePropertyDrawer : PropertyDrawer
    {

        #region Fields

        const float REF_SELECT_WIDTH = 70f;

        public bool RestrictVariantType = false;
        public VariantType VariantTypeRestrictedTo;
        private System.Type _forcedComponentType;
        private System.Type _selectedComponentType;

        private VariantReference.EditorHelper _helper = new VariantReference.EditorHelper(new VariantReference());
        private SelectableComponentPropertyDrawer _selectComponentDrawer = new SelectableComponentPropertyDrawer();

        #endregion

        #region Properties

        public System.Type ForcedComponentType
        {
            get { return _forcedComponentType; }
            set
            {
                //if (value == null)
                //    _forcedComponentType = null;
                //else if (value.IsInterface || typeof(Component).IsAssignableFrom(value))
                //    _forcedComponentType = value;
                //else
                //    throw new TypeArgumentMismatchException(value, typeof(Component), "value");

                _forcedComponentType = value; //lets just allow anything... if they set it to something not working, it jsut won't list anything
            }
        }

        #endregion

        #region Methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidthCache = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Max(0f, labelWidthCache - REF_SELECT_WIDTH);
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            this.DrawValueField(position, property);

            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = labelWidthCache;
        }

        public void DrawValueField(Rect position, SerializedProperty property)
        {
            CopyValuesToHelper(property, _helper);

            //draw ref selection
            position = this.DrawRefModeSelectionDropDown(position, property, _helper);

            //draw value
            switch(_helper._mode)
            {
                case VariantReference.RefMode.Value:
                    {
                        EditorGUI.BeginChangeCheck();
                        this.DrawValueFieldInValueMode(position, property, _helper);
                        if (EditorGUI.EndChangeCheck())
                        {
                            CopyValuesFromHelper(property, _helper);
                        }
                    }
                    break;
                case VariantReference.RefMode.Property:
                    this.DrawValueFieldInPropertyMode(position, property, _helper);
                    break;
                case VariantReference.RefMode.Eval:
                    this.DrawValueFieldInEvalMode(position, property, _helper);
                    break;
            }
        }

        private Rect DrawRefModeSelectionDropDown(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            var r0 = new Rect(position.xMin, position.yMin, Mathf.Min(REF_SELECT_WIDTH, position.width), position.height);

            EditorGUI.BeginChangeCheck();
            var mode = (VariantReference.RefMode)EditorGUI.EnumPopup(r0, GUIContent.none, _helper._mode);
            if (EditorGUI.EndChangeCheck())
            {
                _helper.PrepareForRefModeChange(mode);
                CopyValuesFromHelper(property, helper);
            }

            return new Rect(r0.xMax, r0.yMin, position.width - r0.width, r0.height);
        }

        private void DrawValueFieldInValueMode(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            if (helper.Target == null) return;
            var variant = helper.Target;

            if (this.RestrictVariantType && helper._type != this.VariantTypeRestrictedTo)
            {
                helper.PrepareForValueTypeChange(this.VariantTypeRestrictedTo);
                GUI.changed = true; //force change
            }

            var r0 = new Rect(position.xMin, position.yMin, 90.0f, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(r0.xMax, position.yMin, position.xMax - r0.xMax, EditorGUIUtility.singleLineHeight);

            if (this.RestrictVariantType) GUI.enabled = false;

            EditorGUI.BeginChangeCheck();
            var valueType = (VariantType)EditorGUI.EnumPopup(r0, GUIContent.none, variant.ValueType);
            if (EditorGUI.EndChangeCheck())
            {
                helper.PrepareForValueTypeChange(valueType);
            }

            if (this.RestrictVariantType) GUI.enabled = true;

            switch (valueType)
            {
                case VariantType.Null:
                    GUI.enabled = false;
                    EditorGUI.TextField(r1, "Null");
                    GUI.enabled = true;
                    break;
                case VariantType.String:
                    variant.StringValue = EditorGUI.TextField(r1, variant.StringValue);
                    break;
                case VariantType.Boolean:
                    variant.BoolValue = EditorGUI.Toggle(r1, variant.BoolValue);
                    break;
                case VariantType.Integer:
                    variant.IntValue = EditorGUI.IntField(r1, variant.IntValue);
                    break;
                case VariantType.Float:
                    variant.FloatValue = EditorGUI.FloatField(r1, variant.FloatValue);
                    break;
                case VariantType.Double:
                    variant.DoubleValue = ConvertUtil.ToDouble(EditorGUI.TextField(r1, variant.DoubleValue.ToString()));
                    break;
                case VariantType.Vector2:
                    variant.Vector2Value = EditorGUI.Vector2Field(r1, GUIContent.none, variant.Vector2Value);
                    break;
                case VariantType.Vector3:
                    variant.Vector3Value = EditorGUI.Vector3Field(r1, GUIContent.none, variant.Vector3Value);
                    break;
                case VariantType.Vector4:
                    variant.Vector4Value = EditorGUI.Vector4Field(r1, null, variant.Vector4Value);
                    break;
                case VariantType.Quaternion:
                    variant.QuaternionValue = SPEditorGUI.QuaternionField(r1, GUIContent.none, variant.QuaternionValue);
                    break;
                case VariantType.Color:
                    variant.ColorValue = EditorGUI.ColorField(r1, variant.ColorValue);
                    break;
                case VariantType.DateTime:
                    variant.DateValue = ConvertUtil.ToDate(EditorGUI.TextField(r1, variant.DateValue.ToString()));
                    break;
                case VariantType.GameObject:
                    variant.GameObjectValue = EditorGUI.ObjectField(r1, variant.GameObjectValue, typeof(GameObject), true) as GameObject;
                    break;
                case VariantType.Component:
                    {
                        _selectComponentDrawer.AllowNonComponents = false;
                        _selectComponentDrawer.RestrictionType = _forcedComponentType;
                        _selectComponentDrawer.ShowXButton = true;
                        var targProp = property.FindPropertyRelative("_unityObjectReference");
                        EditorGUI.BeginChangeCheck();
                        _selectComponentDrawer.OnGUI(r1, targProp);
                        if (EditorGUI.EndChangeCheck())
                        {
                            variant.ComponentValue = targProp.objectReferenceValue as Component;
                        }
                    }
                    break;
                case VariantType.Object:
                    {
                        var obj = variant.ObjectValue;
                        if(_forcedComponentType != null && _forcedComponentType.IsInterface)
                        {
                            if (obj is GameObject || obj is Component)
                            {
                                _selectComponentDrawer.AllowNonComponents = false;
                                _selectComponentDrawer.RestrictionType = _forcedComponentType;
                                _selectComponentDrawer.ShowXButton = true;
                                var targProp = property.FindPropertyRelative("_unityObjectReference");
                                EditorGUI.BeginChangeCheck();
                                _selectComponentDrawer.OnGUI(r1, targProp);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    variant.ObjectValue = targProp.objectReferenceValue as Component;
                                }
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                obj = EditorGUI.ObjectField(r1, obj, typeof(UnityEngine.Object), true);
                                if(EditorGUI.EndChangeCheck())
                                {
                                    if(obj == null)
                                    {
                                        variant.ObjectValue = null;
                                    }
                                    else if(TypeUtil.IsType(obj.GetType(), _forcedComponentType))
                                    {
                                        variant.ObjectValue = obj;
                                    }
                                    else
                                    {
                                        var go = GameObjectUtil.GetGameObjectFromSource(obj);
                                        if (go != null)
                                            variant.ObjectValue = go.GetComponent(_forcedComponentType);
                                        else
                                            variant.ObjectValue = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            variant.ObjectValue = EditorGUI.ObjectField(r1, obj, _forcedComponentType, true);
                        }
                    }
                    break;
                case VariantType.LayerMask:
                    {
                        variant.LayerMaskValue = SPEditorGUI.LayerMaskField(r1, GUIContent.none, (int)variant.LayerMaskValue);
                    }
                    break;
                case VariantType.Rect:
                    {
                        variant.RectValue = EditorGUI.RectField(r1, variant.RectValue);
                    }
                    break;
            }
        }

        private void DrawValueFieldInPropertyMode(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            _selectComponentDrawer.AllowNonComponents = true;
            _selectComponentDrawer.RestrictionType = null;
            _selectComponentDrawer.ShowXButton = false;
            var targProp = property.FindPropertyRelative("_unityObjectReference");
            var memberProp = property.FindPropertyRelative("_string");

            if (targProp.objectReferenceValue == null)
            {
                _selectComponentDrawer.OnGUI(position, targProp);
            }
            else
            {
                var r1 = new Rect(position.xMin, position.yMin, position.width * 0.4f, position.height);
                var r2 = new Rect(r1.xMax, position.yMin, position.width - r1.width, position.height);
                _selectComponentDrawer.OnGUI(r1, targProp);
                memberProp.stringValue = SPEditorGUI.ReflectedPropertyField(r2, targProp.objectReferenceValue, memberProp.stringValue);
            }
        }

        private void DrawValueFieldInEvalMode(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            _selectComponentDrawer.AllowNonComponents = true;
            _selectComponentDrawer.RestrictionType = null;
            _selectComponentDrawer.ShowXButton = false;
            var targProp = property.FindPropertyRelative("_unityObjectReference");
            var evalProp = property.FindPropertyRelative("_string");

            var r1 = new Rect(position.xMin, position.yMin, position.width * 0.4f, position.height);
            var r2 = new Rect(r1.xMax, position.yMin, position.width - r1.width, position.height);
            _selectComponentDrawer.OnGUI(r1, targProp);
            evalProp.stringValue = EditorGUI.TextField(r2, evalProp.stringValue);
        }

        #endregion

        #region Static Utils

        public static void CopyValuesToHelper(SerializedProperty property, VariantReference.EditorHelper helper)
        {
            helper._mode = property.FindPropertyRelative("_mode").GetEnumValue<VariantReference.RefMode>();
            helper._type = property.FindPropertyRelative("_type").GetEnumValue<VariantType>();
            helper._x = property.FindPropertyRelative("_x").floatValue;
            helper._y = property.FindPropertyRelative("_y").floatValue;
            helper._z = property.FindPropertyRelative("_z").floatValue;
            helper._w = property.FindPropertyRelative("_w").doubleValue;
            helper._string = property.FindPropertyRelative("_string").stringValue;
            helper._unityObjectReference = property.FindPropertyRelative("_unityObjectReference").objectReferenceValue;
        }

        public static void CopyValuesFromHelper(SerializedProperty property, VariantReference.EditorHelper helper)
        {
            property.FindPropertyRelative("_mode").SetEnumValue(helper._mode);
            property.FindPropertyRelative("_type").SetEnumValue(helper._type);
            property.FindPropertyRelative("_x").floatValue = helper._x;
            property.FindPropertyRelative("_y").floatValue = helper._y;
            property.FindPropertyRelative("_z").floatValue = helper._z;
            property.FindPropertyRelative("_w").doubleValue = helper._w;
            property.FindPropertyRelative("_string").stringValue = helper._string;
            property.FindPropertyRelative("_unityObjectReference").objectReferenceValue = helper._unityObjectReference;
        }

        #endregion

    }
}
