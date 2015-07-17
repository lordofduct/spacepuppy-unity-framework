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

        public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren);
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


    /// <summary>
    /// Handler that simulates the default drawing for a SerializedProperty with no special behaviour. Does NOT support arrays.
    /// 
    /// TODO - add array/list support.
    /// </summary>
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

        public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
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

}
