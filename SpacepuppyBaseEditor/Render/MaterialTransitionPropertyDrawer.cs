using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Render;

using com.spacepuppyeditor.Base;
using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Render
{

    /// <summary>
    /// It is critical that MaterialTransition mirrors the same 4 properties found in MaterialPropertyReference. 
    /// </summary>
    [CustomPropertyDrawer(typeof(MaterialTransition))]
    public class MaterialTransitionPropertyDrawer : PropertyDrawer
    {
        
        public const string PROP_VALUES = "_values";
        public const string PROP_POSITION = "_position";


        private MaterialPropertyReferencePropertyDrawer _matPropRefDrawer = new MaterialPropertyReferencePropertyDrawer();
        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer() { RestrictVariantType = true };
        private SerializedProperty _property;
        private SerializedProperty _valuesProp;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            var valuesProp = property.FindPropertyRelative(PROP_VALUES);
            var lst = CachedReorderableList.GetListDrawer(valuesProp, _valuesList_DrawHeader, _valuesList_DrawElement);
            lst.headerHeight = 1f;
            lst.elementHeight = EditorGUIUtility.singleLineHeight;

            h = lst.GetHeight();
            h += EditorGUIUtility.singleLineHeight * 2.5f; //material line, position slider, and a small padding
            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;
            
            var cache = SPGUI.DisableIfPlaying();

            _property = property;

            GUI.Box(position, GUIContent.none);

            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            _matPropRefDrawer.OnGUI(r0, property, label); //because we mirror the properties of MaterialPropertyReference, this works
            position = new Rect(position.xMin, r0.yMax, position.width, position.height - r0.height);

            EditorGUI.indentLevel++;
            
            _valuesProp = property.FindPropertyRelative(PROP_VALUES);
            var lst = CachedReorderableList.GetListDrawer(_valuesProp, _valuesList_DrawHeader, _valuesList_DrawElement);
            lst.headerHeight = 0f;
            lst.elementHeight = EditorGUIUtility.singleLineHeight;

            var r2 = new Rect(position.xMin, position.yMin, position.width, lst.GetHeight());
            var r3 = new Rect(position.xMin, position.yMax - EditorGUIUtility.singleLineHeight * 1.25f, position.width, EditorGUIUtility.singleLineHeight);

            //lst.DoList(EditorGUI.IndentedRect(r2));
            lst.DoList(r2);
            EditorGUI.PropertyField(r3, property.FindPropertyRelative(PROP_POSITION));

            EditorGUI.indentLevel--;
            
            _property = null;
            _valuesProp = null;

            cache.Reset();
        }
        


        #region List Drawer Methods

        private void _valuesList_DrawHeader(Rect area)
        {
        }

        private void _valuesList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var valueProp = _valuesProp.GetArrayElementAtIndex(index);

            var evtp = _property.FindPropertyRelative(MaterialPropertyReferencePropertyDrawer.PROP_VALUETYPE).GetEnumValue<MaterialPropertyValueType>();
            var emtp = _property.FindPropertyRelative(MaterialPropertyReferencePropertyDrawer.PROP_VALUEMEMBER).GetEnumValue<MaterialPropertyValueTypeMember>();

            _variantDrawer.TypeRestrictedTo = GetType(evtp, emtp);
            _variantDrawer.OnGUI(area, valueProp, GUIContent.none);
        }

        #endregion

        private static VariantType GetVariantType(MaterialPropertyValueType evtp, MaterialPropertyValueTypeMember emtp)
        {
            switch (evtp)
            {
                case MaterialPropertyValueType.Float:
                    return VariantType.Float;
                case MaterialPropertyValueType.Color:
                    {
                        switch (emtp)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                return VariantType.Color;
                            default:
                                return VariantType.Float;
                        }
                    }
                case MaterialPropertyValueType.Vector:
                    {
                        switch (emtp)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                return VariantType.Vector4;
                            default:
                                return VariantType.Float;
                        }
                    }
                default:
                    return VariantType.Float;
            }
        }

        private static System.Type GetType(MaterialPropertyValueType evtp, MaterialPropertyValueTypeMember emtp)
        {
            switch (evtp)
            {
                case MaterialPropertyValueType.Float:
                    return typeof(float);
                case MaterialPropertyValueType.Color:
                    {
                        switch(emtp)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                return typeof(UnityEngine.Color);
                            default:
                                return typeof(float);
                        }
                    }
                case MaterialPropertyValueType.Vector:
                    {
                        switch (emtp)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                return typeof(UnityEngine.Vector4);
                            default:
                                return typeof(float);
                        }
                    }
                default:
                    return typeof(float);
            }
        }

    }
}
