using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(FindInSelfAttribute))]
    public class FindInSelfModifier : PropertyModifier
    {

        protected internal override void OnBeforeGUI(SerializedProperty property, ref bool cancelDraw)
        {
            string name = (this.attribute as FindInSelfAttribute).Name;
            bool bUseEntity = (this.attribute as FindInSelfAttribute).UseEntity;

            if (property.isArray && TypeUtil.IsListType(fieldInfo.FieldType, true))
            {
                var elementType = EditorHelper.GetRestrictedFieldType(this.fieldInfo, true) ?? TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType);
                ApplyDefaultAsList(property, elementType, name, bUseEntity);
            }
            else
            {
                var elementType = EditorHelper.GetRestrictedFieldType(this.fieldInfo);
                ApplyDefaultAsSingle(property, elementType, name, bUseEntity);
            }
        }




        private static void ApplyDefaultAsSingle(SerializedProperty property, System.Type fieldType, string name, bool bUseEntity)
        {
            if (fieldType == null) return;

            var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (targ == null) return;
            if (bUseEntity) targ = targ.FindRoot();

            if (TypeUtil.IsType(fieldType, typeof(VariantReference)))
            {
                var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
                if (variant != null && variant.Value == null && variant.ValueType == VariantType.GameObject)
                {
                    var go = targ.FindByName(name);
                    if (go != null)
                    {
                        variant.GameObjectValue = go;
                        property.serializedObject.Update();
                        return;
                    }
                }
            }
            else if (property.objectReferenceValue == null)
            {
                foreach (var obj in targ.transform.FindAllByName(name))
                {
                    var o = ObjUtil.GetAsFromSource(fieldType, obj) as UnityEngine.Object;
                    if(o != null)
                    {
                        property.objectReferenceValue = o;
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
            }
        }


        private static void ApplyDefaultAsList(SerializedProperty property, System.Type elementType, string name, bool bUseEntity)
        {
            if (elementType == null) return;
            if (property.arraySize != 0) return;
            
            var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (targ == null) return;
            if (bUseEntity) targ = targ.FindRoot();

            if (TypeUtil.IsType(elementType, typeof(VariantReference)))
            {
                var arr = targ.transform.FindAllByName(name);
                if (arr.Length > 0)
                {
                    property.arraySize = arr.Length;
                    property.serializedObject.ApplyModifiedProperties();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var variant = EditorHelper.GetTargetObjectOfProperty(property.GetArrayElementAtIndex(i)) as VariantReference;
                        if (variant != null && variant.Value == null && variant.ValueType == VariantType.GameObject)
                        {
                            variant.GameObjectValue = arr[i].gameObject;
                        }
                    }
                    property.serializedObject.Update();
                }
            }
            else
            {
                if (ComponentUtil.IsAcceptableComponentType(elementType))
                {
                    using (var lst = TempCollection.GetList<Component>())
                    {
                        foreach (var child in targ.transform.FindAllByName(name))
                        {
                            child.GetComponents(elementType, lst);
                            if (lst.Count > 0)
                            {
                                int low = property.arraySize;
                                property.arraySize += lst.Count;
                                for (int i = 0; i < lst.Count; i++)
                                {
                                    property.GetArrayElementAtIndex(low + i).objectReferenceValue = lst[i];
                                }
                            }
                            lst.Clear();
                        }
                    }
                }
                else if (TypeUtil.IsType(elementType, typeof(GameObject)))
                {
                    var arr = targ.transform.FindAllByName(name);
                    if (arr.Length > 0)
                    {
                        property.arraySize = arr.Length;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            property.GetArrayElementAtIndex(i).objectReferenceValue = arr[i].gameObject;
                        }
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

    }

}
