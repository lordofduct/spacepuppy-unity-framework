using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

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
                try
                {
                    //this is hack as shit, but it reflects out the method used internally by Unity to get the default property drawer for a type if any.
                    var ass = System.Reflection.Assembly.GetAssembly(typeof(PropertyDrawer));
                    var ScriptAttributeUtilityType = ass.GetType("UnityEditor.ScriptAttributeUtility");
                    var meth = ScriptAttributeUtilityType.GetMethod("GetDrawerTypeForType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    var drawerTp = meth.Invoke(null, new object[] { this.fieldInfo.FieldType }) as System.Type;
                    if(drawerTp != null)
                    {
                        var drawer = System.Activator.CreateInstance(drawerTp) as PropertyDrawer;
                        ObjUtil.SetValue(drawer, "m_Attribute", null);
                        ObjUtil.SetValue(drawer, "m_FieldInfo", this.fieldInfo);
                        _subDrawer = drawer;
                    }
                }
                catch
                {
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