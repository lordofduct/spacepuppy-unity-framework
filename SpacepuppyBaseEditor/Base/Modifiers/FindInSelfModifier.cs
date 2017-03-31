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

        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            string name = (this.attribute as FindInSelfAttribute).Name;
            bool bUseEntity = (this.attribute as FindInSelfAttribute).UseEntity;

            if (property.isArray && TypeUtil.IsListType(fieldInfo.FieldType, true))
            {
                this.ApplyDefaultAsList(property, TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType), name, bUseEntity);
            }
            else
            {
                this.ApplyDefaultAsSingle(property, name, bUseEntity);
            }
        }




        private void ApplyDefaultAsSingle(SerializedProperty property, string name, bool bUseEntity)
        {
            if (TypeUtil.IsType(fieldInfo.FieldType, typeof(Component)) && property.objectReferenceValue == null)
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    var restrictAttrib = this.fieldInfo.GetCustomAttributes(typeof(TypeRestrictionAttribute), false).FirstOrDefault() as TypeRestrictionAttribute;
                    var componentType = (restrictAttrib != null && restrictAttrib.InheritsFromType != null) ? restrictAttrib.InheritsFromType : this.fieldInfo.FieldType;
                    var root = (bUseEntity) ? targ.FindRoot() : targ;
                    
                    foreach(var obj in root.transform.FindAllByName(name))
                    {
                        var c = obj.GetComponent(componentType);
                        if(c != null)
                        {
                            property.objectReferenceValue = c;
                            property.serializedObject.ApplyModifiedProperties();
                            return;
                        }
                    }

                }
            }
            else if (TypeUtil.IsType(fieldInfo.FieldType, typeof(GameObject)) && property.objectReferenceValue == null)
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    var root = (bUseEntity) ? targ.FindRoot() : targ;
                    var go = root.FindByName(name);
                    if(go != null)
                    {
                        property.objectReferenceValue = go;
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
            }
            else if (TypeUtil.IsType(fieldInfo.FieldType, typeof(VariantReference)))
            {
                var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
                if (variant != null && variant.Value == null && variant.ValueType == VariantType.GameObject)
                {
                    var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                    var root = (bUseEntity) ? targ.FindRoot() : targ;
                    var go = root.FindByName(name);
                    if(go != null)
                    {
                        variant.GameObjectValue = go;
                        property.serializedObject.Update();
                        return;
                    }
                }
            }
        }


        private void ApplyDefaultAsList(SerializedProperty property, System.Type elementType, string name, bool bUseEntity)
        {
            if (property.arraySize != 0) return;

            if (TypeUtil.IsType(elementType, typeof(Component)))
            {
                using (var lst = TempCollection.GetList<Component>())
                {
                    var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                    if (targ != null)
                    {
                        var root = (bUseEntity) ? targ.FindRoot() : targ;
                        foreach(var child in root.transform.FindAllByName(name))
                        {
                            child.GetComponents(elementType, lst);
                            if(lst.Count > 0)
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
            }
            else if (TypeUtil.IsType(elementType, typeof(GameObject)))
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ != null)
                {
                    var root = (bUseEntity) ? targ.FindRoot() : targ;
                    var arr = root.transform.FindAllByName(name);
                    if(arr.Length > 0)
                    {
                        property.arraySize = arr.Length;
                        for(int i = 0; i < arr.Length; i++)
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
