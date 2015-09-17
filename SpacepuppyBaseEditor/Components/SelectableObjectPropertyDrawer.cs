using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Components
{

    [CustomPropertyDrawer(typeof(SelectableObjectAttribute))]
    public class SelectableObjectPropertyDrawer : PropertyDrawer
    {

        private SelectableComponentPropertyDrawer _compPropDrawer;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue != null && property.objectReferenceValue is GameObject || property.objectReferenceValue is Component)
            {
                if (_compPropDrawer == null) _compPropDrawer = new SelectableComponentPropertyDrawer();
                return _compPropDrawer.GetPropertyHeight(property, label);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                this.DrawAsMismatchedAttribute(position, property);
                return;
            }

            bool allowSceneObject = (this.attribute is SelectableObjectAttribute) ? (this.attribute as SelectableObjectAttribute).AllowSceneObjects : true;

            if(property.objectReferenceValue != null && property.objectReferenceValue is Component)
            {
                if (_compPropDrawer == null) _compPropDrawer = new SelectableComponentPropertyDrawer();

                _compPropDrawer.OnGUI(position, property, label);
            }
            else
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(UnityEngine.Object), allowSceneObject);
                if (property.objectReferenceValue is GameObject) property.objectReferenceValue = (property.objectReferenceValue as GameObject).transform;
            }
        }






        private void DrawAsMismatchedAttribute(Rect position, SerializedProperty property)
        {
            EditorGUI.LabelField(position, EditorHelper.TempContent("Mismatched type of PropertyDrawer attribute with field."));
        }

    }
}