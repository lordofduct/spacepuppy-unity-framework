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
            if (ObjUtil.IsType(fieldInfo.FieldType, typeof(Component)) && property.objectReferenceValue == null)
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    var attrib = this.attribute as DefaultFromSelfAttribute;
                    var restrictAttrib = this.fieldInfo.GetCustomAttributes(typeof(ComponentTypeRestrictionAttribute), false).FirstOrDefault() as ComponentTypeRestrictionAttribute;
                    var componentType = (restrictAttrib != null && restrictAttrib.InheritsFromType != null) ? restrictAttrib.InheritsFromType : this.fieldInfo.FieldType;
                    if (attrib.FindInEntity)
                    {
                        property.objectReferenceValue = targ.FindLikeComponent(componentType);
                    }
                    else
                    {
                        property.objectReferenceValue = targ.GetFirstLikeComponent(componentType);
                    }
                }
            }
            else if (ObjUtil.IsType(fieldInfo.FieldType, typeof(GameObject)) && property.objectReferenceValue == null)
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    property.objectReferenceValue = targ;
                }
            }
            else if (ObjUtil.IsType(fieldInfo.FieldType, typeof(VariantReference)))
            {
                var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
                if (variant != null && variant.Value == null && variant.ValueType == VariantReference.VariantType.GameObject)
                {
                    variant.GameObjectValue = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                }
            }
        }

    }
}
