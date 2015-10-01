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
            


            //var tpref = EditorHelper.GetTargetObjectOfProperty(property) as TypeReference;
            //if(tpref == null)
            //{
            //    tpref = new TypeReference();
            //    EditorHelper.SetTargetObjectOfProperty(property, tpref);
            //    property.serializedObject.ApplyModifiedProperties();
            //}

            //EditorGUI.BeginChangeCheck();
            //tpref.Type = SPEditorGUI.TypeDropDown(position, label, baseType, tpref.Type, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, style);
            //if (EditorGUI.EndChangeCheck())
            //    property.serializedObject.Update();



            var tp = GetTypeFromTypeReference(property);
            EditorGUI.BeginChangeCheck();
            tp = SPEditorGUI.TypeDropDown(position, label, baseType, tp, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, style);
            if (EditorGUI.EndChangeCheck())
                SetTypeToTypeReference(property, tp);

            EditorGUI.EndProperty();
        }


        public static System.Type GetTypeFromTypeReference(SerializedProperty property)
        {
            if (property == null) return null;
            var hashProp = property.FindPropertyRelative(PROP_TYPEHASH);
            if (hashProp == null) return null;
            return TypeReference.UnHashType(hashProp.stringValue);
        }

        public static void SetTypeToTypeReference(SerializedProperty property, System.Type tp)
        {
            if (property == null) return;

            var hashProp = property.FindPropertyRelative(PROP_TYPEHASH);
            if (hashProp == null) return;

            hashProp.stringValue = TypeReference.HashType(tp);

            if(Application.isPlaying)
            {
                var tpref = EditorHelper.GetTargetObjectOfProperty(property) as TypeReference;
                if(tpref != null)
                {
                    tpref.Type = tp;
                }
            }
        }

    }

}
