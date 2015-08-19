using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(DefaultFromSelfAttribute))]
    public class DefaultFromSelfModifier : PropertyModifier
    {

        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            bool bUseEntity = (this.attribute as DefaultFromSelfAttribute).UseEntity;

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
                if (variant != null && variant.Value == null && variant.ValueType == VariantReference.VariantType.GameObject)
                {
                    var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                    if(bUseEntity)
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

    }
}
