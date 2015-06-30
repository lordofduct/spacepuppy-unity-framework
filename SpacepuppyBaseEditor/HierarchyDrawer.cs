using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class CustomHierarchyDrawerAttribute : System.Attribute
    {

        public readonly System.Type TargetType;
        public readonly bool UseForChildren;

        public CustomHierarchyDrawerAttribute(System.Type type)
        {
            this.TargetType = type;
            this.UseForChildren = false;
        }

        public CustomHierarchyDrawerAttribute(System.Type type, bool useForChildren)
        {
            this.TargetType = type;
            this.UseForChildren = useForChildren;
        }

    }

    public class HierarchyDrawer
    {

        #region Fields

        private Component _target;

        #endregion

        #region CONSTRUCTOR

        internal void Init(Component targ)
        {
            _target = targ;
        }

        #endregion

        #region Properties

        public Component Target { get { return _target; } }

        #endregion

        #region Methods

        /// <summary>
        /// Get any components also on the GameObject that also have hierarchy drawers.
        /// </summary>
        /// <returns></returns>
        public Component[] GetSiblings()
        {
            if (_target == null) return new Component[] { };
            return (from c in _target.GetComponents<Component>() where c != _target && EditorHierarchyDrawerEvents.HasDrawer(c.GetType()) select c).ToArray();
        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnHierarchyGUI(Rect selectionRect)
        {

        }

        #endregion

    }
}
