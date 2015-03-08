using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Modifiers;

namespace com.spacepuppyeditor.Internal
{
    internal class SPPropertyAttributePropertyHandler : IPropertyHandler
    {

        #region Fields

        private System.Reflection.FieldInfo _fieldInfo;
        private SPPropertyAttribute[] _attribs;
        private PropertyDrawer _visibleDrawer;

        #endregion

        #region CONSTRUCTOR

        public SPPropertyAttributePropertyHandler(System.Reflection.FieldInfo fieldInfo, SPPropertyAttribute[] attribs)
        {
            _fieldInfo = fieldInfo;
            _attribs = attribs;
        }

        #endregion

        #region Methods

        private void Init()
        {
            var dtp = ScriptAttributeUtility.GetDrawerTypeForType(_attribs[0].GetType());
            var drawer = PropertyDrawerActivator.Create(dtp, _attribs[0], _fieldInfo);
            if (drawer is PropertyModifier) (drawer as PropertyModifier).Init(true);
            _visibleDrawer = drawer;
        }

        #endregion

        #region IPropertyHandler Interface

        public float GetHeight(SerializedProperty property, GUIContent label)
        {
            if (_visibleDrawer == null) this.Init();

            return _visibleDrawer.GetPropertyHeight(property, label);
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (_visibleDrawer == null) this.Init();

            _visibleDrawer.OnGUI(position, property, label);
            return false;
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            if (_visibleDrawer == null) this.Init();

            if (label == null) label = EditorHelper.TempContent(property.displayName);
            var rect = EditorGUILayout.GetControlRect(true, _visibleDrawer.GetPropertyHeight(property, label), options);
            _visibleDrawer.OnGUI(rect, property, label);
            return false;
        }

        #endregion

    }
}
