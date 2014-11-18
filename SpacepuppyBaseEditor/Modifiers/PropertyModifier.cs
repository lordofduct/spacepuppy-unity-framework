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

        private bool _initialized = false;
        private PropertyDrawer _subDrawer;

        internal void Init(bool bIsVisibleDrawer)
        {
            if(_initialized) return;

            if(bIsVisibleDrawer)
            {
                var drawerTp = ScriptAttributeUtility.GetDrawerTypeForType(this.fieldInfo.FieldType);
                if(drawerTp != null)
                {
                    _subDrawer = PropertyDrawerActivator.Create(drawerTp, null, this.fieldInfo);
                }
            }

            _initialized = true;
        }

        #endregion

        #region Draw

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_initialized) this.Init(true);

            if (_subDrawer != null)
                return _subDrawer.GetPropertyHeight(property, label);
            else
                return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_initialized) this.Init(true);

            this.OnBeforeGUI(property);

            if (_subDrawer != null)
            {
                _subDrawer.OnGUI(position, property, label);
            }
            else
            {
                bool includeChildren = false;
                if (this.attribute is PropertyModifierAttribute)
                {
                    includeChildren = (this.attribute as PropertyModifierAttribute).IncludeChidrenOnDraw;
                }

                EditorGUI.PropertyField(position, property, label, includeChildren);
            }

            this.OnPostGUI(property);
        }

        #endregion

        #region Overridables

        protected internal virtual void OnBeforeGUI(SerializedProperty property)
        {

        }

        protected internal virtual void OnPostGUI(SerializedProperty property)
        {

        }

        #endregion

    }

}