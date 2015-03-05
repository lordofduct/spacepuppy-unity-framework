using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor
{
    public class ComponentHeaderDrawer : GUIDrawer
    {

        #region Fields

        private ComponentHeaderAttribute _attribute;
        private System.Type _componentType;

        #endregion

        #region CONSTRUCTOR

        internal void Init(ComponentHeaderAttribute attrib, System.Type compType)
        {
            _attribute = attrib;
            _componentType = compType;
        }

        #endregion

        #region Properties

        public ComponentHeaderAttribute Attribute { get { return _attribute; } }

        public System.Type ComponentType { get { return _componentType; } }

        #endregion

        #region Methods

        public virtual float GetHeight(SerializedObject serializedObject)
        {
            return 0f;
        }

        public virtual void OnGUI(Rect position, SerializedObject serializedObject)
        {

        }

        #endregion

    }
}
