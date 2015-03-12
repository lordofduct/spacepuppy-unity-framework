using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppyeditor
{

    public class CustomSceneGizmoAttribute : System.Attribute
    {

        #region Fields

        private System.Type _componentType;

        /// <summary>
        /// Renders gizmo if the active object is a child of this component.
        /// </summary>
        public bool RenderIfParent;

        #endregion

        #region CONSTRUCTOR

        public CustomSceneGizmoAttribute(System.Type componentType)
        {
            _componentType = componentType;
        }

        #endregion

        #region Properties

        public System.Type ComponentType
        {
            get { return _componentType; }
        }

        #endregion

    }

}
