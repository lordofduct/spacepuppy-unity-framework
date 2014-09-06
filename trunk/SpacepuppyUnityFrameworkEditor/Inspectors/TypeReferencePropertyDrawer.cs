using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{


    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {

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
            }

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(TypeReferenceRestrictionAttribute), true).FirstOrDefault() as TypeReferenceRestrictionAttribute;
            var baseType = typeof(Object);
            bool allowAbstractTypes = false;
            bool allowInterfaces = false;
            System.Type defaultType = null;
            if (attrib != null)
            {
                baseType = attrib.InheritsFromType;
                allowAbstractTypes = attrib.allowAbstractClasses;
                allowInterfaces = attrib.allowInterfaces;
                defaultType = attrib.defaultType;
            }

            tpref.Type = EditorHelper.TypeDropDown(position, label, baseType, tpref.Type, allowAbstractTypes, allowInterfaces, defaultType);

            EditorGUI.EndProperty();
        }

    }

}
