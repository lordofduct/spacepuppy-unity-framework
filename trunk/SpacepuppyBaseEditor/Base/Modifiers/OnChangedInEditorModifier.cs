﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(OnChangedInEditorAttribute))]
    public class OnChangedInEditorModifier : PropertyModifier
    {
        private object _valueBeforeChange;

        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            _valueBeforeChange = EditorHelper.GetTargetObjectOfProperty(property);
        }

        protected internal override void OnPostGUI(SerializedProperty property)
        {
            if(GUI.changed)
            {
                var attrib = this.attribute as OnChangedInEditorAttribute;
                if (attrib.OnlyAtRuntime && !Application.isPlaying) return;
                var targ = EditorHelper.GetTargetObjectWithProperty(property);
                if (targ == null) return;

                var tp = targ.GetType();
                try
                {
                    var methInfo = tp.GetMethod(attrib.MethodName,
                                                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic,
                                                null,
                                                new System.Type[] { this.fieldInfo.FieldType },
                                                null);
                    if (methInfo != null)
                    {
                        methInfo.Invoke(targ, new object[] { _valueBeforeChange });
                        return;
                    }
                    methInfo = tp.GetMethod(attrib.MethodName,
                                                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic,
                                                null,
                                                new System.Type[] { typeof(object) },
                                                null);
                    if (methInfo != null)
                    {
                        methInfo.Invoke(targ, new object[] { _valueBeforeChange });
                        return;
                    }
                    methInfo = tp.GetMethod(attrib.MethodName,
                                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic,
                                            null,
                                            System.Type.EmptyTypes,
                                            null);
                    if(methInfo != null)
                    {
                        methInfo.Invoke(targ, null);
                        return;
                    }
                }
                catch
                {

                }
            }
        }

    }
}
