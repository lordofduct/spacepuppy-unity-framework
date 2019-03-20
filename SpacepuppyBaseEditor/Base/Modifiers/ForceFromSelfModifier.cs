using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(ForceFromSelfAttribute))]
    public class ForceFromSelfModifier : PropertyModifier
    {
        
        protected internal override void OnBeforeGUI(SerializedProperty property, ref bool cancelDraw)
        {
            var relativity = (this.attribute as ForceFromSelfAttribute).Relativity;

            if (property.isArray && TypeUtil.IsListType(fieldInfo.FieldType, true))
            {
                var elementType = TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType);
                var restrictedType = EditorHelper.GetRestrictedFieldType(this.fieldInfo, true) ?? elementType;
                ApplyDefaultAsList(property, elementType, restrictedType, relativity);
            }
            else
            {
                ApplyDefaultAsSingle(property, this.fieldInfo.FieldType, EditorHelper.GetRestrictedFieldType(this.fieldInfo), relativity);
            }
        }
        
        private static void ApplyDefaultAsSingle(SerializedProperty property, System.Type fieldType, System.Type restrictionType, EntityRelativity relativity)
        {
            if (fieldType == null || restrictionType == null) return;
            
            if (TypeUtil.IsType(fieldType, typeof(VariantReference)))
            {
                var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
                if (variant == null) return;

                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ == null)
                {
                    var obj = ObjUtil.GetAsFromSource(restrictionType, property.serializedObject.targetObject);
                    if (obj != null)
                    {
                        variant.Value = obj;
                        property.serializedObject.Update();
                        GUI.changed = true;
                    }
                    else if (variant.Value != null)
                    {
                        variant.Value = null;
                        property.serializedObject.Update();
                        GUI.changed = true;
                    }
                    return;
                }

                switch (relativity)
                {
                    case EntityRelativity.Entity:
                        {
                            targ = targ.FindRoot();
                            if (ObjUtil.IsRelatedTo(targ, variant.ObjectValue)) return;
                            
                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ);
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if (obj != null)
                            {
                                variant.Value = obj;
                                property.serializedObject.Update();
                                GUI.changed = true;
                            }
                            else if(variant.Value != null)
                            {
                                variant.Value = null;
                                property.serializedObject.Update();
                                GUI.changed = true;
                            }
                        }
                        break;
                    case EntityRelativity.Self:
                        {
                            if (ObjUtil.IsRelatedTo(targ, variant.ObjectValue)) return;

                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ);
                            if (obj != null)
                            {
                                variant.Value = obj;
                                property.serializedObject.Update();
                            }
                            else if (variant.Value != null)
                            {
                                variant.Value = null;
                                property.serializedObject.Update();
                                GUI.changed = true;
                            }
                        }
                        break;
                    case EntityRelativity.SelfAndChildren:
                        {
                            if (ObjUtil.IsRelatedTo(targ, variant.ObjectValue)) return;

                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ);
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if(obj != null)
                            {
                                variant.Value = obj;
                                property.serializedObject.Update();
                            }
                            else if (variant.Value != null)
                            {
                                variant.Value = null;
                                property.serializedObject.Update();
                                GUI.changed = true;
                            }
                        }
                        break;
                }
            }
            else if(property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ == null)
                {
                    property.objectReferenceValue = ObjUtil.GetAsFromSource(restrictionType, property.serializedObject.targetObject) as UnityEngine.Object;
                    return;
                }

                switch (relativity)
                {
                    case EntityRelativity.Entity:
                        {
                            targ = targ.FindRoot();
                            if (ObjUtil.IsRelatedTo(targ, property.objectReferenceValue)) return;

                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ) as UnityEngine.Object;
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if (obj != null)
                            {
                                property.objectReferenceValue = obj;
                                GUI.changed = true;
                            }
                            else if(property.objectReferenceValue != null)
                            {
                                property.objectReferenceValue = null;
                                GUI.changed = true;
                            }
                        }
                        break;
                    case EntityRelativity.Self:
                        {
                            if (ObjUtil.IsRelatedTo(targ, property.objectReferenceValue)) return;

                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ) as UnityEngine.Object;
                            if (obj != null)
                            {
                                property.objectReferenceValue = obj;
                                GUI.changed = true;
                            }
                            else if (property.objectReferenceValue != null)
                            {
                                property.objectReferenceValue = null;
                                GUI.changed = true;
                            }
                        }
                        break;
                    case EntityRelativity.SelfAndChildren:
                        {
                            if (ObjUtil.IsRelatedTo(targ, property.objectReferenceValue)) return;

                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ) as UnityEngine.Object;
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if (obj != null)
                            {
                                property.objectReferenceValue = obj;
                                GUI.changed = true;
                            }
                            else if (property.objectReferenceValue != null)
                            {
                                property.objectReferenceValue = null;
                                GUI.changed = true;
                            }
                        }
                        break;
                }
            }
        }


        private static void ApplyDefaultAsList(SerializedProperty property, System.Type elementType, System.Type restrictionType, EntityRelativity relativity)
        {
            if (elementType == null || restrictionType == null) return;

            if (TypeUtil.IsType(elementType, typeof(VariantReference)))
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ == null)
                {
                    var obj = ObjUtil.GetAsFromSource(restrictionType, property.serializedObject.targetObject);
                    if (obj != null)
                    {
                        property.arraySize = 1;
                        var variant = EditorHelper.GetTargetObjectOfProperty(property.GetArrayElementAtIndex(0)) as VariantReference;
                        if (variant == null) return;
                        variant.Value = obj;
                        property.serializedObject.Update();
                        GUI.changed = true;
                    }
                    else if (property.arraySize > 0)
                    {
                        property.arraySize = 0;
                        GUI.changed = true;
                    }
                    return;
                }

                switch (relativity)
                {
                    case EntityRelativity.Entity:
                        {
                            targ = targ.FindRoot();
                            var arr = ObjUtil.GetAllFromSource(restrictionType, targ, true);
                            if (ValidateSerializedPropertyArray(property, arr, true)) return;

                            property.arraySize = arr.Length;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                var variant = EditorHelper.GetTargetObjectOfProperty(property.GetArrayElementAtIndex(i)) as VariantReference;
                                if (variant != null) variant.Value = arr[i];
                            }
                            property.serializedObject.Update();
                            GUI.changed = true;
                        }
                        break;
                    case EntityRelativity.Self:
                        {
                            var arr = ObjUtil.GetAllFromSource(restrictionType, targ, false);
                            if (ValidateSerializedPropertyArray(property, arr, true)) return;

                            property.arraySize = arr.Length;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                var variant = EditorHelper.GetTargetObjectOfProperty(property.GetArrayElementAtIndex(i)) as VariantReference;
                                if (variant != null) variant.Value = arr[i];
                            }
                            property.serializedObject.Update();
                            GUI.changed = true;
                        }
                        break;
                    case EntityRelativity.SelfAndChildren:
                        {
                            var arr = ObjUtil.GetAllFromSource(restrictionType, targ, true);
                            if (ValidateSerializedPropertyArray(property, arr, true)) return;

                            property.arraySize = arr.Length;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                var variant = EditorHelper.GetTargetObjectOfProperty(property.GetArrayElementAtIndex(i)) as VariantReference;
                                if (variant != null) variant.Value = arr[i];
                            }
                            property.serializedObject.Update();
                            GUI.changed = true;
                        }
                        break;
                }
            }
            else if (TypeUtil.IsType(elementType, typeof(UnityEngine.Object)))
            {
                var targ = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targ == null)
                {
                    var obj = ObjUtil.GetAsFromSource(restrictionType, property.serializedObject.targetObject) as UnityEngine.Object;
                    if (obj != null)
                    {
                        property.arraySize = 1;
                        property.GetArrayElementAtIndex(0).objectReferenceValue = obj;
                        GUI.changed = true;
                    }
                    else if (property.arraySize > 0)
                    {
                        property.arraySize = 0;
                        GUI.changed = true;
                    }
                    return;
                }

                switch (relativity)
                {
                    case EntityRelativity.Entity:
                        {
                            targ = targ.FindRoot();
                            var arr = ObjUtil.GetAllFromSource(restrictionType, targ, true);
                            if (ValidateSerializedPropertyArray(property, arr, false)) return;

                            property.arraySize = arr.Length;
                            for(int i = 0; i < arr.Length; i++)
                            {
                                property.GetArrayElementAtIndex(i).objectReferenceValue = arr[i] as UnityEngine.Object;
                            }
                        }
                        break;
                    case EntityRelativity.Self:
                        {
                            var arr = ObjUtil.GetAllFromSource(restrictionType, targ, false);
                            if (ValidateSerializedPropertyArray(property, arr, false)) return;

                            property.arraySize = arr.Length;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                property.GetArrayElementAtIndex(i).objectReferenceValue = arr[i] as UnityEngine.Object;
                            }
                        }
                        break;
                    case EntityRelativity.SelfAndChildren:
                        {
                            var arr = ObjUtil.GetAllFromSource(restrictionType, targ, true);
                            if (ValidateSerializedPropertyArray(property, arr, false)) return;

                            property.arraySize = arr.Length;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                property.GetArrayElementAtIndex(i).objectReferenceValue = arr[i] as UnityEngine.Object;
                            }
                        }
                        break;
                }
            }
        }

        private static bool ValidateSerializedPropertyArray(SerializedProperty property, object[] arr, bool isVariant)
        {
            if (arr == null && property.arraySize > 0) return false;
            if (property.arraySize != arr.Length) return false;
            if (property.arraySize == 0) return true;

            using (var lst = TempCollection.GetList<object>())
            {
                for(int i = 0; i < property.arraySize; i++)
                {
                    var el = property.GetArrayElementAtIndex(i);
                    if (isVariant)
                    {
                        var variant = EditorHelper.GetTargetObjectOfProperty(el) as VariantReference;
                        if (variant == null)
                            lst.Add(null);
                        else
                            lst.Add(variant.Value);
                    }
                    else if(el.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        lst.Add(el.objectReferenceValue);
                    }
                    else
                    {
                        lst.Add(null);
                    }
                }

                return lst.SimilarTo(arr);
            }
        }

    }

}
