using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils.Dynamic;

namespace com.spacepuppyeditor.Internal
{

    internal class StandardPropertyDrawer : IPropertyHandler
    {

        private static StandardPropertyDrawer _instance;

        public static StandardPropertyDrawer Instance
        {
            get
            {
                if (_instance == null) _instance = new StandardPropertyDrawer();
                return _instance;
            }
        }

        private StandardPropertyDrawer()
        {
            //block constructor
        }

        #region IPropertyHandler Interface

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUI.PropertyField(position, property, label, includeChildren);
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            return EditorGUILayout.PropertyField(property, label, includeChildren, options);
        }

        #endregion

    }


    /*

    /// <summary>
    /// Represents a PropertyHandler as it is defined in the standard UnityEditor.
    /// </summary>
    internal class StandardPropertyHandler : IPropertyHandler
    {

        #region Fields

        private TypeAccessWrapper _wrapper;

        private System.Func<Rect, SerializedProperty, GUIContent, bool, bool> _imp_OnGUI;
        private System.Func<SerializedProperty, GUIContent, bool, GUILayoutOption[], bool> _imp_OnGUILayout;


        private System.Func<SerializedProperty, GUIContent, bool, float> _imp_GetHeight;

        #endregion

        #region CONSTRUCTOR

        internal StandardPropertyHandler(object internalHandler)
        {
            _wrapper = new TypeAccessWrapper(InternalTypeUtil.UnityEditorAssembly.GetType("UnityEditor.PropertyHandler"), internalHandler);
        }

        #endregion

        #region Wrapped Members

        public PropertyDrawer propertyDrawer
        {
            get
            {
                return _wrapper.GetProperty("propertyDrawer") as PropertyDrawer;
            }
        }

        public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (_imp_GetHeight == null) _imp_GetHeight = _wrapper.GetMethod("GetHeight", typeof(System.Func<SerializedProperty, GUIContent, bool, float>)) as System.Func<SerializedProperty, GUIContent, bool, float>;
            return _imp_GetHeight(property, label, includeChildren);
        }

        #endregion

        #region IPropertyHandler Interface

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (_imp_OnGUI == null) _imp_OnGUI = _wrapper.GetMethod("OnGUI", typeof(System.Func<Rect, SerializedProperty, GUIContent, bool, bool>)) as System.Func<Rect, SerializedProperty, GUIContent, bool, bool>;
            return _imp_OnGUI(position, property, label, includeChildren);
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            if (_imp_OnGUILayout == null) _imp_OnGUILayout = _wrapper.GetMethod("OnGUILayout", typeof(System.Func<SerializedProperty, GUIContent, bool, GUILayoutOption[], bool>)) as System.Func<SerializedProperty, GUIContent, bool, GUILayoutOption[], bool>;
            return _imp_OnGUILayout(property, label, includeChildren, options);
        }

        #endregion

        #region Internal Wrapper Methods Interface

        bool IPropertyHandler.RequiresInternalUpdate { get { return true; } }

        void IPropertyHandler.UpdateInternal(object internalPropertyHandler)
        {
            if(_wrapper.WrappedObject != null)
            {
                _wrapper.WrappedObject = internalPropertyHandler;
                _imp_OnGUI = null;
                _imp_OnGUILayout = null;
            }
        }

        #endregion

    }

     */
}
