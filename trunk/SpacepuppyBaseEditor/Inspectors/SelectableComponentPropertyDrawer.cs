using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(SelectableComponentAttribute))]
    public class SelectableComponentPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tp = this.fieldInfo.FieldType;
            if (tp.IsListType()) tp = tp.GetElementTypeOfListType();
            if (property.propertyType != SerializedPropertyType.ObjectReference || !ObjUtil.IsType(tp, typeof(Component)))
            {
                this.DrawAsMismatchedAttribute(position, property, label);
                return;
            }

            if(property.objectReferenceValue == null)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
            }
            else
            {
                const float POPUP_WIDTH = 100f;
                if (position.width > POPUP_WIDTH + EditorGUIUtility.labelWidth)
                {
                    var ra = new Rect(position.xMin, position.yMin, POPUP_WIDTH + EditorGUIUtility.labelWidth, position.height);
                    var rb = new Rect(ra.xMax, position.yMin, position.width - ra.width, position.height);

                    var go = (property.objectReferenceValue as Component).gameObject;
                    var components = go.GetComponents(tp);
                    var names = (from c in components select new GUIContent(c.GetType().Name)).ToArray();
                    int oi = components.IndexOf(property.objectReferenceValue);
                    int ni = EditorGUI.Popup(ra, label, oi, names);
                    if(oi != ni)
                    {
                        property.objectReferenceValue = components[ni];
                    }

                    property.objectReferenceValue = EditorGUI.ObjectField(rb, property.objectReferenceValue, tp, (this.attribute as SelectableComponentAttribute).AllowSceneObjects);
                }
                else
                {
                    property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, tp, (this.attribute as SelectableComponentAttribute).AllowSceneObjects);
                }
            }

        }


        private void DrawAsMismatchedAttribute(Rect position, SerializedProperty property, GUIContent label)
        {

        }

    }

}
