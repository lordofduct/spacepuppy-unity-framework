using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Internal
{
    internal class UnityInternalPropertyHandler : IPropertyHandler
    {

        #region Fields

        private TypeAccessWrapper _internalPropertyHandler;

        private System.Func<SerializedProperty, GUIContent, bool, float> _imp_GetHeight;
        private System.Func<Rect, SerializedProperty, GUIContent, bool, bool> _imp_OnGUI;
        private System.Func<SerializedProperty, GUIContent, bool, GUILayoutOption[], bool> _imp_OnGUILayout;

        private System.Action<PropertyAttribute, System.Reflection.FieldInfo, System.Type> _imp_HandleAttribute;

        #endregion

        #region CONSTRUCTOR

        public UnityInternalPropertyHandler()
        {
            var klass = InternalTypeUtil.UnityEditorAssembly.GetType("UnityEditor.PropertyHandler");
            var obj = System.Activator.CreateInstance(klass);
            _internalPropertyHandler = new TypeAccessWrapper(klass, obj, true);
        }

        #endregion

        #region Properties

        protected PropertyDrawer InternalDrawer
        {
            get
            {
                return _internalPropertyHandler.GetProperty("m_PropertyDrawer") as PropertyDrawer;
            }
            set
            {
                _internalPropertyHandler.SetProperty("m_PropertyDrawer", value);
            }
        }

        #endregion

        #region Methods

        protected virtual void HandleAttribute(PropertyAttribute attribute, System.Reflection.FieldInfo field, System.Type propertyType)
        {
            if (_imp_HandleAttribute == null) _imp_HandleAttribute = _internalPropertyHandler.GetMethod("HandleAttribute", typeof(System.Action<PropertyAttribute, System.Reflection.FieldInfo, System.Type>)) as System.Action<PropertyAttribute, System.Reflection.FieldInfo, System.Type>;
            _imp_HandleAttribute(attribute, field, propertyType);
        }

        #endregion

        #region IPropertyHandler Interface

        public virtual float GetHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label, bool includeChildren)
        {
            if (_imp_GetHeight == null) _imp_GetHeight = _internalPropertyHandler.GetMethod("GetHeight", typeof(System.Func<SerializedProperty, GUIContent, bool, float>)) as System.Func<SerializedProperty, GUIContent, bool, float>;
            return _imp_GetHeight(property, label, includeChildren);
        }

        public virtual bool OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label, bool includeChildren)
        {
            if (_imp_OnGUI == null) _imp_OnGUI = _internalPropertyHandler.GetMethod("OnGUI", typeof(System.Func<Rect, SerializedProperty, GUIContent, bool, bool>)) as System.Func<Rect, SerializedProperty, GUIContent, bool, bool>;
            return _imp_OnGUI(position, property, label, includeChildren);
        }

        public virtual bool OnGUILayout(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label, bool includeChildren, UnityEngine.GUILayoutOption[] options)
        {
            if (_imp_OnGUILayout == null) _imp_OnGUILayout = _internalPropertyHandler.GetMethod("OnGUILayout", typeof(System.Func<SerializedProperty, GUIContent, bool, GUILayoutOption[], bool>)) as System.Func<SerializedProperty, GUIContent, bool, GUILayoutOption[], bool>;
            return _imp_OnGUILayout(property, label, includeChildren, options);
        }

        public virtual void OnValidate(SerializedProperty property)
        {
            //TODO
        }

        #endregion

    }
}
