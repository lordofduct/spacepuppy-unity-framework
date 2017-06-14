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

        #region Manually Configured Properties

        private System.Type _inheritsFromType;
        private bool _allowAbstractTypes;
        private bool _allowInterfaces;
        private System.Type _defaultType;
        private System.Type[] _excludedTypes;
        private TypeDropDownListingStyle _dropDownStyle = TypeDropDownListingStyle.Namespace;
        private System.Predicate<System.Type> _searchPredicate;
        private bool _isManuallyConfigured;

        public System.Type InheritsFromType
        {
            get { return _inheritsFromType; }
            set
            {
                _inheritsFromType = value;
                _isManuallyConfigured = true;
            }
        }

        public bool AllowAbstractTypes
        {
            get { return _allowAbstractTypes; }
            set
            {
                _allowAbstractTypes = value;
                _isManuallyConfigured = true;
            }
        }

        public bool AllowInterfaces
        {
            get { return _allowInterfaces; }
            set
            {
                _allowInterfaces = value;
                _isManuallyConfigured = true;
            }
        }

        public System.Type DefaultType
        {
            get { return _defaultType; }
            set
            {
                _defaultType = value;
                _isManuallyConfigured = true;
            }
        }

        public System.Type[] ExcludedTypes
        {
            get { return _excludedTypes; }
            set
            {
                _excludedTypes = value;
                _isManuallyConfigured = true;
            }
        }

        public TypeDropDownListingStyle DropDownStyle
        {
            get { return _dropDownStyle; }
            set
            {
                _dropDownStyle = value;
                _isManuallyConfigured = true;
            }
        }

        public System.Predicate<System.Type> SearchPredicate
        {
            get { return _searchPredicate; }
            set
            {
                _searchPredicate = value;
                _isManuallyConfigured = true;
            }
        }

        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            
            var baseType = typeof(object);
            bool allowAbstractTypes = false;
            bool allowInterfaces = false;
            System.Type defaultType = null;
            System.Type[] excludedTypes = null;
            TypeDropDownListingStyle style = TypeDropDownListingStyle.Namespace;
            System.Predicate<System.Type> searchPredicate = null;

            if(_isManuallyConfigured)
            {
                baseType = _inheritsFromType ?? typeof(object);
                allowAbstractTypes = _allowAbstractTypes;
                allowInterfaces = _allowInterfaces;
                defaultType = _defaultType;
                excludedTypes = _excludedTypes;
                style = _dropDownStyle;
                searchPredicate = _searchPredicate;
            }
            else if(this.fieldInfo != null)
            {
                var attrib = this.fieldInfo.GetCustomAttributes(typeof(TypeReference.ConfigAttribute), true).FirstOrDefault() as TypeReference.ConfigAttribute;
                if (attrib != null)
                {
                    baseType = attrib.inheritsFromType;
                    allowAbstractTypes = attrib.allowAbstractClasses;
                    allowInterfaces = attrib.allowInterfaces;
                    defaultType = attrib.defaultType;
                    excludedTypes = attrib.excludedTypes;
                    style = attrib.dropDownStyle;
                    searchPredicate = null;
                }
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
            tp = SPEditorGUI.TypeDropDown(position, label, baseType, tp, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, style, searchPredicate);
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
