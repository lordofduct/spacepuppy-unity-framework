using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Internal
{

    internal class StandardPropertyHandler : IPropertyHandler
    {

        private static StandardPropertyHandler _instance;

        public static StandardPropertyHandler Instance
        {
            get
            {
                if (_instance == null) _instance = new StandardPropertyHandler();
                return _instance;
            }
        }

        private StandardPropertyHandler()
        {
            //block constructor
        }

        #region IPropertyHandler Interface

        public float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUI.PropertyField(position, property, label, includeChildren);
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            return EditorGUILayout.PropertyField(property, label, includeChildren, options);
        }

        public void OnValidate(SerializedProperty property)
        {
            //do nothing
        }

        #endregion




    }

    internal class DefaultPropertyHandler : IPropertyHandler
    {

        #region Singleton Interface

        private static DefaultPropertyHandler _instance;

        public static DefaultPropertyHandler Instance
        {
            get
            {
                if (_instance == null) _instance = new DefaultPropertyHandler();
                return _instance;
            }
        }

        #endregion

        #region Fields

        private Dictionary<System.Type, PropertyDrawer> _fieldTypeToDrawer = new Dictionary<System.Type, PropertyDrawer>();

        private static TypeAccessWrapper _editorGuiAccessWrapper;
        private static System.Func<Rect, SerializedProperty, GUIContent, bool> _imp_DefaultPropertyField;

        #endregion

        #region CONSTRUCTOR

        private DefaultPropertyHandler()
        {
            //block constructor
            _editorGuiAccessWrapper = new TypeAccessWrapper(typeof(EditorGUI));
            _editorGuiAccessWrapper.IncludeNonPublic = true;
        }

        #endregion

        #region Methods

        private PropertyDrawer GetDrawer(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Generic || property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var fieldInfo = ScriptAttributeUtility.GetFieldInfoFromProperty(property);
                if (fieldInfo == null) return null;

                var tp = fieldInfo.FieldType;
                if (tp.IsListType()) tp = tp.GetElementTypeOfListType();
                if (_fieldTypeToDrawer.ContainsKey(tp))
                {
                    var result = _fieldTypeToDrawer[tp];
                    if(result != null) PropertyDrawerActivator.InitializePropertyDrawer(result, null, fieldInfo);
                    return result;
                }

                var drawerType = ScriptAttributeUtility.GetDrawerTypeForType(tp);
                if (drawerType == null)
                {
                    _fieldTypeToDrawer.Add(tp, null);
                    return null;
                }

                var drawer = PropertyDrawerActivator.Create(drawerType, null, fieldInfo);
                _fieldTypeToDrawer.Add(tp, drawer);
                return drawer;
            }

            return null;
        }

        #endregion

        #region IPropertyHandler Interface

        public float GetHeight(SerializedProperty property, GUIContent label)
        {
            var drawer = this.GetDrawer(property);
            if (drawer != null)
            {
                return drawer.GetPropertyHeight(property, label);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            var drawer = this.GetDrawer(property);
            if(drawer != null)
            {
                EditorGUI.BeginChangeCheck();
                drawer.OnGUI(position, property, label);
                return EditorGUI.EndChangeCheck();
            }
            else
            {
                if (_imp_DefaultPropertyField == null) _imp_DefaultPropertyField = _editorGuiAccessWrapper.GetStaticMethod("DefaultPropertyField", typeof(System.Func<Rect, SerializedProperty, GUIContent, bool>)) as System.Func<Rect, SerializedProperty, GUIContent, bool>;
                return _imp_DefaultPropertyField(position, property, label);
            }
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            var drawer = this.GetDrawer(property);
            if(drawer != null)
            {
                var h = drawer.GetPropertyHeight(property, label);
                var position = EditorGUILayout.GetControlRect(true, h);
                EditorGUI.BeginChangeCheck();
                drawer.OnGUI(position, property, label);
                return EditorGUI.EndChangeCheck();
            }
            else
            {
                if (_imp_DefaultPropertyField == null) _imp_DefaultPropertyField = _editorGuiAccessWrapper.GetStaticMethod("DefaultPropertyField", typeof(System.Func<Rect, SerializedProperty, GUIContent, bool>)) as System.Func<Rect, SerializedProperty, GUIContent, bool>;
                var position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                return _imp_DefaultPropertyField(position, property, label);
            }
        }

        public void OnValidate(SerializedProperty property)
        {
            //do nothing
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
