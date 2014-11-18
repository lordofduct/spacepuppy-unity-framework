using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppyeditor.Internal
{
    internal class SPPropertyHandler : IPropertyHandler
    {

        #region Fields

        IPropertyHandler _wrappedHandler;

        #endregion

        #region CONSTRUCTOR

        internal SPPropertyHandler(IPropertyHandler propertyHandler)
        {
            if (propertyHandler == null) throw new System.ArgumentNullException("propertyHandler");
            _wrappedHandler = propertyHandler;
        }

        #endregion


        #region IPropertyHandler Interface

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return _wrappedHandler.OnGUI(position, property, label, includeChildren);
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            return _wrappedHandler.OnGUILayout(property, label, includeChildren, options);
        }

        #endregion

        #region Internal Wrapper Methods Interface

        bool IPropertyHandler.RequiresInternalUpdate { get { return _wrappedHandler.RequiresInternalUpdate; } }

        void IPropertyHandler.UpdateInternal(object internalPropertyHandler)
        {
            _wrappedHandler.UpdateInternal(internalPropertyHandler);
        }

        #endregion

    }
}
