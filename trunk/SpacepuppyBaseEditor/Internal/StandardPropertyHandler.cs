using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils.Dynamic;

namespace com.spacepuppyeditor.Internal
{

    /// <summary>
    /// Represents a PropertyHandler as it is defined in the standard UnityEditor.
    /// </summary>
    internal class StandardPropertyHandler : IPropertyHandler
    {

        #region Fields

        private TypeAccessWrapper _wrapper;

        private System.Func<Rect, SerializedProperty, GUIContent, bool, bool> _imp_OnGUI;
        private System.Func<SerializedProperty, GUIContent, bool, GUILayoutOption[], bool> _imp_OnGUILayout;

        #endregion

        #region CONSTRUCTOR

        internal StandardPropertyHandler(object internalHandler)
        {
            _wrapper = new TypeAccessWrapper(InternalTypeUtil.UnityEditorAssembly.GetType("UnityEditor.PropertyHandler"), internalHandler);
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

}
