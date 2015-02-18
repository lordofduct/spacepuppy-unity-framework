using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(ModifierChainAttribute))]
    public class ModifierChainPropertyDrawer : PropertyDrawer
    {

        private PropertyAttribute[] _attributes;
        private PropertyModifier[] _modifiers;
        private PropertyDrawer _visibleDrawer;
        private bool _initialized;

        private void Init(SerializedProperty property)
        {
            _attributes = null;
            _modifiers = null;
            _visibleDrawer = null;

            _attributes = (from a in this.fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), true) 
                           where !(a is ModifierChainAttribute) 
                           orderby (a as PropertyAttribute).order ascending 
                           select a as PropertyAttribute).ToArray();

            var propDrawerTp = typeof(PropertyDrawer);
            var drawerTypes = ScriptAttributeUtility.GetDrawerTypesForType((from a in _attributes select a.GetType()).ToArray());

            var lst = new List<PropertyModifier>();
            if (_attributes.Length > 0)
            {
                for (int i = 0; i < _attributes.Length - 1; i++)
                {
                    var attrib = _attributes[i];
                    if (attrib is PropertyModifierAttribute)
                    {
                        foreach (var dtp in drawerTypes)
                        {
                            var a = dtp.GetCustomAttributes(typeof(CustomPropertyDrawer), false).FirstOrDefault() as CustomPropertyDrawer;
                            if (a != null && attrib.GetType() == ObjUtil.GetValue(a, "m_Type"))
                            {
                                var drawer = System.Activator.CreateInstance(dtp) as PropertyModifier;
                                ObjUtil.SetValue(drawer, "m_Attribute", attrib);
                                ObjUtil.SetValue(drawer, "m_FieldInfo", this.fieldInfo);
                                drawer.Init(false);
                                lst.Add(drawer);
                                break;
                            }
                        }
                    }
                }
                _modifiers = lst.ToArray();

                var lastAttrib = _attributes.Last();
                foreach (var dtp in drawerTypes)
                {
                    var a = dtp.GetCustomAttributes(typeof(CustomPropertyDrawer), false).FirstOrDefault() as CustomPropertyDrawer;
                    if (a != null && lastAttrib.GetType() == ObjUtil.GetValue(a, "m_Type"))
                    {
                        var drawer = System.Activator.CreateInstance(dtp) as PropertyDrawer;
                        ObjUtil.SetValue(drawer, "m_Attribute", lastAttrib);
                        ObjUtil.SetValue(drawer, "m_FieldInfo", this.fieldInfo);
                        if (drawer is PropertyModifier) (drawer as PropertyModifier).Init(true);
                        _visibleDrawer = drawer;
                        break;
                    }
                }
            }

            _initialized = true;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_initialized) this.Init(property);

            if (_modifiers != null)
            {
                foreach (var d in _modifiers)
                {
                    d.OnBeforeGUI(property);
                }
            }

            if (_visibleDrawer != null)
            {
                _visibleDrawer.OnGUI(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

            if (_modifiers != null)
            {
                foreach (var d in _modifiers)
                {
                    d.OnPostGUI(property);
                }
            }
        }


    }
}