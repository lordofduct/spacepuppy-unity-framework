using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{


    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {

        public const string PROP_TYPEHASH = "_typeHash";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //var targOwner = EditorHelper.GetTargetObjectWithProperty(property);
            //var tpref = this.fieldInfo.GetValue(targOwner) as TypeReference;
            //if (tpref == null)
            //{
            //    tpref = new TypeReference();
            //    this.fieldInfo.SetValue(targOwner, tpref);
            //}
            var tpref = EditorHelper.GetTargetObjectOfProperty(property) as TypeReference;
            if(tpref == null)
            {
                tpref = new TypeReference();
                EditorHelper.SetTargetObjectOfProperty(property, tpref);
                property.serializedObject.ApplyModifiedProperties();
            }

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(TypeReference.ConfigAttribute), true).FirstOrDefault() as TypeReference.ConfigAttribute;
            var baseType = typeof(Object);
            bool allowAbstractTypes = false;
            bool allowInterfaces = false;
            System.Type defaultType = null;
            System.Type[] excludedTypes = null;
            TypeDropDownListingStyle style = TypeDropDownListingStyle.Namespace;
            if (attrib != null)
            {
                baseType = attrib.inheritsFromType;
                allowAbstractTypes = attrib.allowAbstractClasses;
                allowInterfaces = attrib.allowInterfaces;
                defaultType = attrib.defaultType;
                excludedTypes = attrib.excludedTypes;
                style = attrib.dropDownStyle;
            }

            EditorGUI.BeginChangeCheck();
            tpref.Type = SPEditorGUI.TypeDropDown(position, label, baseType, tpref.Type, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, style);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.Update();

            EditorGUI.EndProperty();
        }


        public static System.Type GetTypeFromTypeReference(SerializedProperty property)
        {
            if (property == null) return null;
            var hashProp = property.FindPropertyRelative(PROP_TYPEHASH);
            if (hashProp == null) return null;
            return TypeReference.UnHashType(hashProp.stringValue);
        }

    }

}
