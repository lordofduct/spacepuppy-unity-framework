using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Modifiers
{

    public abstract class PropertyModifier : PropertyDrawer
    {

        #region Fields
        
        public bool IsDrawer
        {
            get;
            internal set;
        }

        #endregion
        
        #region Draw

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ScriptAttributeUtility.SharedNullPropertyHandler.GetHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ScriptAttributeUtility.SharedNullPropertyHandler.OnGUI(position, property, label, true);
        }

        #endregion

        #region Overridables

        protected internal virtual void OnBeforeGUI(SerializedProperty property, ref bool cancelDraw)
        {

        }

        protected internal virtual void OnPostGUI(SerializedProperty property)
        {

        }

        protected internal virtual void OnValidate(SerializedProperty property)
        {

        }

        #endregion

    }

}