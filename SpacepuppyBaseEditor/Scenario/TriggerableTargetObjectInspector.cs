using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Scenario
{

    [CustomPropertyDrawer(typeof(TriggerableTargetObject))]
    public class TriggerableTargetObjectInspector : PropertyDrawer
    {

        private TriggerableTargetObject.ConfigAttribute _configAttrib;

        private void Init(SerializedProperty property)
        {
            _configAttrib = this.fieldInfo.GetCustomAttributes(typeof(TriggerableTargetObject.ConfigAttribute), false).FirstOrDefault() as TriggerableTargetObject.ConfigAttribute;

            if(_configAttrib == null)
            {
                _configAttrib = new TriggerableTargetObject.ConfigAttribute();
            }
        }
        
        private UnityEngine.Object GetTargetFromSource(SerializedProperty property, TriggerableTargetObject.TargetSource esrc)
        {
            switch (esrc)
            {
                case TriggerableTargetObject.TargetSource.Self:
                    if(_configAttrib.TargetType == typeof(GameObject))
                    {
                        if (GameObjectUtil.IsGameObjectSource(property.serializedObject.targetObject))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if(TypeUtil.IsType(_configAttrib.TargetType, typeof(Component)))
                    {
                        if (GameObjectUtil.IsGameObjectSource(property.serializedObject.targetObject))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject).GetComponent(_configAttrib.TargetType);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if(TypeUtil.IsType(property.serializedObject.targetObject.GetType(), _configAttrib.TargetType))
                    {
                        return property.serializedObject.targetObject as UnityEngine.Object;
                    }
                    else
                    {
                        return null;
                    }

                case TriggerableTargetObject.TargetSource.Root:
                    if (_configAttrib.TargetType == typeof(GameObject))
                    {
                        if (GameObjectUtil.IsGameObjectSource(property.serializedObject.targetObject))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject).FindRoot();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (TypeUtil.IsType(_configAttrib.TargetType, typeof(Component)))
                    {
                        if (GameObjectUtil.IsGameObjectSource(property.serializedObject.targetObject))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject).FindComponent(_configAttrib.TargetType);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }

                default:
                    return null;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_configAttrib == null) this.Init(property);

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            var sourceProp = property.FindPropertyRelative("_source");
            var targetProp = property.FindPropertyRelative("_target");

            var r0 = new Rect(position.xMin, position.yMin, Mathf.Min(position.width * 0.3f, 80f), EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(r0, sourceProp, GUIContent.none, false);
            //TriggerableTargetObject.TargetSource esrc = (TriggerableTargetObject.TargetSource)sourceProp.enumValueIndex;
            var esrc = sourceProp.GetEnumValue<TriggerableTargetObject.TargetSource>();

            if(esrc == TriggerableTargetObject.TargetSource.TriggerArg)
            {
                var r1 = new Rect(r0.xMax, position.yMin, position.width - r0.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(r1, "Target determined by activating trigger.");
                targetProp.objectReferenceValue = null;
            }
            else
            {
                var r1 = new Rect(r0.xMax, position.yMin, position.width - r0.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r1, targetProp, GUIContent.none, false);

                if (targetProp.objectReferenceValue == null)
                {
                    targetProp.objectReferenceValue = GetTargetFromSource(property, esrc);
                }
                else if(esrc != TriggerableTargetObject.TargetSource.Configurable)
                {
                    var expectedTarg = GetTargetFromSource(property, esrc);
                    if(targetProp.objectReferenceValue != expectedTarg)
                    {
                        //sourceProp.enumValueIndex = (int)TriggerableTargetObject.TargetSource.Configurable;
                        sourceProp.SetEnumValue(TriggerableTargetObject.TargetSource.Configurable);
                    }
                }
            }

            EditorGUI.EndProperty();
        }


    }

}
