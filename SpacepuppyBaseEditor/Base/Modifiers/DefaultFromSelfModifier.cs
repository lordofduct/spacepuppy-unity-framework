using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(DefaultFromSelfAttribute))]
    public class DefaultFromSelfModifier : PropertyModifier
    {
        
        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            bool bUseEntity = (this.attribute as DefaultFromSelfAttribute).UseEntity;

            if(property.isArray && TypeUtil.IsListType(fieldInfo.FieldType, true))
            {
                this.ApplyDefaultAsList(property, TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType), bUseEntity);
            }
            else
            {
                this.ApplyDefaultAsSingle(property, bUseEntity);
            }
            
        }




        private void ApplyDefaultAsSingle(SerializedProperty property, bool bUseEntity)
        {
            if (TypeUtil.IsType(fieldInfo.FieldType, typeof(Component)) && property.objectReferenceValue == null)
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    var restrictAttrib = this.fieldInfo.GetCustomAttributes(typeof(ComponentTypeRestrictionAttribute), false).FirstOrDefault() as ComponentTypeRestrictionAttribute;
                    var componentType = (restrictAttrib != null && restrictAttrib.InheritsFromType != null) ? restrictAttrib.InheritsFromType : this.fieldInfo.FieldType;
                    if (bUseEntity)
                    {
                        property.objectReferenceValue = targ.FindComponent(componentType);
                    }
                    else
                    {
                        property.objectReferenceValue = targ.GetComponent(componentType);
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else if (TypeUtil.IsType(fieldInfo.FieldType, typeof(GameObject)) && property.objectReferenceValue == null)
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    if (bUseEntity)
                    {
                        property.objectReferenceValue = targ.FindRoot();
                    }
                    else
                    {
                        property.objectReferenceValue = targ;
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else if (TypeUtil.IsType(fieldInfo.FieldType, typeof(VariantReference)))
            {
                var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
                if (variant != null && variant.Value == null && variant.ValueType == VariantType.GameObject)
                {
                    var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                    if (bUseEntity)
                    {
                        variant.GameObjectValue = targ.FindRoot();
                    }
                    else
                    {
                        variant.GameObjectValue = targ;
                    }

                    property.serializedObject.Update();
                }
            }
        }


        private void ApplyDefaultAsList(SerializedProperty property, System.Type elementType, bool bUseEntity)
        {
            if (property.arraySize != 0) return;

            if(TypeUtil.IsType(elementType, typeof(Component)))
            {
                using (var lst = TempCollection.GetList<Component>())
                {
                    var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                    if(targ != null)
                    {
                        if (bUseEntity)
                            targ.FindComponents(elementType, lst, true);
                        else
                            targ.GetComponents(elementType, lst);
                    }
                    
                    property.arraySize = lst.Count;
                    for(int i = 0; i < lst.Count; i++)
                    {
                        property.GetArrayElementAtIndex(i).objectReferenceValue = lst[i];
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else if(TypeUtil.IsType(elementType, typeof(GameObject)))
            {

                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    if (bUseEntity)
                    {
                        property.arraySize = 1;
                        property.GetArrayElementAtIndex(0).objectReferenceValue = targ.FindRoot();
                    }
                    else
                    {
                        property.arraySize = 1;
                        property.GetArrayElementAtIndex(0).objectReferenceValue = targ;
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

    }
}
