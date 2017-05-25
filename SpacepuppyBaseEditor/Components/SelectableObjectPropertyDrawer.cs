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

        #region Fields

        private SelectableComponentPropertyDrawer _compPropDrawer = new SelectableComponentPropertyDrawer()
        {
            AllowNonComponents = true
        };

        private bool _allowSceneObjects = true;
        private System.Type _inheritsFromType;
        private bool _allowProxy;
        private bool _manuallyConfigured;

        #endregion

        #region CONSTRUCTOR

        public SelectableObjectPropertyDrawer()
        {

        }

        public SelectableObjectPropertyDrawer(bool allowSceneObjects, System.Type inheritsFromType, bool allowProxy)
        {
            this.AllowSceneObjects = allowSceneObjects;
            this.InheritsFromType = inheritsFromType;
            this.AllowProxy = allowProxy;
        }

        #endregion

        #region Properties

        public bool AllowSceneObjects
        {
            get { return _allowSceneObjects; }
            set
            {
                _allowSceneObjects = value;
                _manuallyConfigured = true;
            }
        }

        public System.Type InheritsFromType
        {
            get { return _inheritsFromType; }
            set
            {
                _inheritsFromType = value;
                _manuallyConfigured = true;
            }
        }

        public bool AllowProxy
        {
            get { return _allowProxy; }
            set
            {
                _allowProxy = value;
                _manuallyConfigured = true;
            }
        }

        #endregion

        #region Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return EditorGUIUtility.singleLineHeight;

            return _compPropDrawer.GetPropertyHeight(property, label);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                this.DrawAsMismatchedAttribute(position, property);
                return;
            }

            this.InitOnGUI();
            _compPropDrawer.AllowSceneObjects = _allowSceneObjects;
            _compPropDrawer.RestrictionType = _inheritsFromType;
            _compPropDrawer.AllowProxy = _allowProxy;
            _compPropDrawer.OnGUI(position, property, label);
        }




        private void InitOnGUI()
        {
            if (_manuallyConfigured) return;

            var attrib = this.attribute as SelectableObjectAttribute;
            if(attrib != null)
            {
                _allowSceneObjects = attrib.AllowSceneObjects;
                _inheritsFromType = attrib.InheritsFromType;
                _allowProxy = attrib.AllowProxy;
            }
            else
            {
                _allowSceneObjects = true;
                _inheritsFromType = null;
                _allowProxy = false;
            }
        }

        private void DrawAsMismatchedAttribute(Rect position, SerializedProperty property)
        {
            EditorGUI.LabelField(position, EditorHelper.TempContent("Mismatched type of PropertyDrawer attribute with field."));
        }

        #endregion

    }
}