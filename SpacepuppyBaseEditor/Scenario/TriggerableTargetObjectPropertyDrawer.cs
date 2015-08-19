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
    public class TriggerableTargetObjectPropertyDrawer : PropertyDrawer
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
                    else if(TypeUtil.IsType(_configAttrib.TargetType, typeof(IComponent)))
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
                            return GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject).FindRoot().GetComponent(_configAttrib.TargetType);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if(TypeUtil.IsType(_configAttrib.TargetType, typeof(IComponent)))
                    {
                        if (GameObjectUtil.IsGameObjectSource(property.serializedObject.targetObject))
                        {
                            return GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject).FindRoot().GetComponent(_configAttrib.TargetType);
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

        private bool PropertyIsComplex(SerializedProperty property)
        {
            if (_configAttrib == null) this.Init(property);

            var sourceProp = property.FindPropertyRelative("_source");
            var targetProp = property.FindPropertyRelative("_target");

            var esrc = sourceProp.GetEnumValue<TriggerableTargetObject.TargetSource>();
            var go = GameObjectUtil.GetGameObjectFromSource(targetProp.objectReferenceValue);
            if (go == null) return false;

            switch(esrc)
            {
                case TriggerableTargetObject.TargetSource.Self:
                case TriggerableTargetObject.TargetSource.Root:
                case TriggerableTargetObject.TargetSource.Configurable:
                    {
                        if (TypeUtil.IsType(_configAttrib.TargetType, typeof(Component)))
                        {
                            return go.GetComponents(_configAttrib.TargetType).Length > 1;
                        }
                        else if (TypeUtil.IsType(_configAttrib.TargetType, typeof(IComponent)))
                        {
                            return go.GetComponents(_configAttrib.TargetType).Length > 1;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case TriggerableTargetObject.TargetSource.TriggerArg:
                    return false;
            }

            return false;
        }





        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_configAttrib == null) this.Init(property);

            if (this.PropertyIsComplex(property))
            {
                return EditorGUIUtility.singleLineHeight * 2f;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
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

                if(this.PropertyIsComplex(property))
                {
                    var r2 = new Rect(r1.xMin, r1.yMax, r1.width, r1.height);
                    var go = GameObjectUtil.GetGameObjectFromSource(targetProp.objectReferenceValue);

                    var selectedType = targetProp.objectReferenceValue.GetType();
                    var availableTypes = (from c in go.GetComponents(_configAttrib.TargetType) select c.GetType()).ToArray();
                    var availableTypeNames = availableTypes.Select((tp) => EditorHelper.TempContent(tp.Name)).ToArray();

                    var index = System.Array.IndexOf(availableTypes, selectedType);
                    EditorGUI.BeginChangeCheck();
                    index = EditorGUI.Popup(r2, GUIContent.none, index, availableTypeNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        targetProp.objectReferenceValue = (index >= 0) ? go.GetComponent(availableTypes[index]) : null;
                    }
                }
            }

            EditorGUI.EndProperty();
        }


    }

}
