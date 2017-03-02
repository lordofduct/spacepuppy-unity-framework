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

}
