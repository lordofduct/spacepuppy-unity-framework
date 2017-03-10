using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Internal
{

    /// <summary>
    /// Draws a SerializedProperty as if itself doesn't have any custom drawers, while respecting the custom drawers of its children. 
    /// </summary>
    internal class DefaultPropertyHandler : IPropertyHandler
    {

        public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (label == null) label = EditorHelper.TempContent(property.displayName);

            if (!includeChildren || !property.hasVisibleChildren)
            {
                return ScriptAttributeUtility.SharedNullInternalPropertyHandler.GetHeight(property, label, includeChildren);
                //return EditorGUI.GetPropertyHeight(property, label, includeChildren);
            }
            
            float h = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                var iterator = property.Copy();
                var end = property.GetEndProperty();
                for (bool enterChildren = true; iterator.Next(enterChildren); enterChildren = false)
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        break;

                    h += SPEditorGUI.GetPropertyHeight(iterator, EditorHelper.TempContent(iterator.displayName), iterator.hasVisibleChildren);
                }
            }

            return h;
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (label == null) label = EditorHelper.TempContent(property.displayName);

            if (!includeChildren || !property.hasVisibleChildren)
            {
                return ScriptAttributeUtility.SharedNullInternalPropertyHandler.OnGUI(position, property, label, includeChildren);
                //return EditorGUI.PropertyField(position, property, label, includeChildren);
            }

            float h;
            Rect r;
            EditorGUI.BeginChangeCheck();

            //draw header
            h = EditorGUIUtility.singleLineHeight;
            r = new Rect(position.xMin, position.yMin, position.width, h);
            position = new Rect(r.xMin, r.yMax, r.width, position.height - h);
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label);

            //draw children
            if(property.isExpanded)
            {
                var iterator = property.Copy();
                var end = property.GetEndProperty();
                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        break;

                    h = SPEditorGUI.GetPropertyHeight(iterator);
                    r = new Rect(position.xMin, position.yMin, position.width, h);
                    position = new Rect(r.xMin, r.yMax, r.width, position.height - h);

                    SPEditorGUI.PropertyField(r, iterator, EditorHelper.TempContent(iterator.displayName), iterator.hasVisibleChildren);
                }
            }

            return EditorGUI.EndChangeCheck();
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            if (label == null) label = EditorHelper.TempContent(property.displayName);

            if (!includeChildren || !property.hasVisibleChildren)
            {
                //return ScriptAttributeUtility.SharedNullInternalPropertyHandler.OnGUILayout(property, label, includeChildren, options);
                return EditorGUILayout.PropertyField(property, label, includeChildren, options);
            }

            EditorGUI.BeginChangeCheck();

            //draw header
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label);

            //draw children
            if (property.isExpanded)
            {
                var iterator = property.Copy();
                var end = property.GetEndProperty();
                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        break;

                    SPEditorGUILayout.PropertyField(iterator, EditorHelper.TempContent(iterator.displayName), iterator.hasVisibleChildren, options);
                }
            }

            return EditorGUI.EndChangeCheck();
        }
        
        public void OnValidate(SerializedProperty property)
        {
            //TODO
        }

    }





    /// <summary>
    /// Handler that simulates the default drawing for a SerializedProperty with no special behaviour. Does NOT support arrays.
    /// 
    /// Potential candiate for obsolescence, was designed for the 'DefaultPropertyField' methods in SPEditorGUI and SPEditorGUILayout. But because it 
    /// still pulled the PropertyDrawer, it doesn't perform what I suspect it was originally intended for (drawing properties without any custom editor. 
    /// 
    /// In the same regard, I can't remember if that IS what DefaultPropertyDrawer was intended for either... so holding onto just in case 
    /// SPEditor.DefaultPropertyField isn't working as expected... since it has several dependencies around.
    /// </summary>
    internal class DefaultPropertyHandler_Old : IPropertyHandler
    {

        #region Singleton Interface

        private static DefaultPropertyHandler_Old _instance;

        public static DefaultPropertyHandler_Old Instance
        {
            get
            {
                if (_instance == null) _instance = new DefaultPropertyHandler_Old();
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

        private DefaultPropertyHandler_Old()
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

                PropertyDrawer drawer = null;
                if (_fieldTypeToDrawer.TryGetValue(tp, out drawer))
                {
                    if (drawer != null) PropertyDrawerActivator.InitializePropertyDrawer(drawer, null, fieldInfo);
                    return drawer;
                }

                var drawerType = ScriptAttributeUtility.GetDrawerTypeForType(tp);
                if (drawerType == null)
                {
                    _fieldTypeToDrawer[tp] = null;
                    return null;
                }

                drawer = PropertyDrawerActivator.Create(drawerType, null, fieldInfo);
                _fieldTypeToDrawer[tp] = drawer;
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
            if (drawer != null)
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
            if (drawer != null)
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
