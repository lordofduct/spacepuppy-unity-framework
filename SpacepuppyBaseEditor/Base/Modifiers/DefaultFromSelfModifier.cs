using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(DefaultFromSelfAttribute))]
    public class DefaultFromSelfModifier : PropertyModifier
    {
        
        private HashSet<int> _handled = new HashSet<int>();

        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetIndexRespectingPropertyHash(property);
            if (_handled.Contains(hash)) return;
            if ((this.attribute as DefaultFromSelfAttribute).HandleOnce) _handled.Add(hash);

            var relativity = (this.attribute as DefaultFromSelfAttribute).Relativity;

            if (property.isArray && TypeUtil.IsListType(fieldInfo.FieldType, true))
            {
                var elementType = TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType);
                var restrictionType = EditorHelper.GetRestrictedFieldType(this.fieldInfo, true) ?? elementType;
                ApplyDefaultAsList(property, elementType, restrictionType, relativity);
            }
            else
            {
                ApplyDefaultAsSingle(property, this.fieldInfo.FieldType, EditorHelper.GetRestrictedFieldType(this.fieldInfo), relativity);
            }
        }



        private static void ApplyDefaultAsSingle(SerializedProperty property, System.Type fieldType, System.Type restrictionType, EntityRelativity relativity)
        {
            if (fieldType == null) return;
            
            if (TypeUtil.IsType(fieldType, typeof(VariantReference)))
            {
                var variant = EditorHelper.GetTargetObjectOfProperty(property) as VariantReference;
                if (variant == null) return;
                if (variant.Value != null) return;

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
                    return;
                }

                switch (relativity)
                {
                    case EntityRelativity.Entity:
                        {
                            targ = targ.FindRoot();

                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ);
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if (obj != null)
                            {
                                variant.Value = obj;
                                property.serializedObject.Update();
                            }
                        }
                        break;
                    case EntityRelativity.Self:
                        {
                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ);
                            if (obj != null)
                            {
                                variant.Value = obj;
                                property.serializedObject.Update();
                            }
                        }
                        break;
                    case EntityRelativity.SelfAndChildren:
                        {
                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ);
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if (obj != null)
                            {
                                variant.Value = obj;
                                property.serializedObject.Update();
                            }
                        }
                        break;
                }
            }
            else if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null) return;

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

                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ) as UnityEngine.Object;
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if (obj != null)
                            {
                                property.objectReferenceValue = obj;
                                GUI.changed = true;
                            }
                        }
                        break;
                    case EntityRelativity.Self:
                        {
                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ) as UnityEngine.Object;
                            if (obj != null)
                            {
                                property.objectReferenceValue = obj;
                                GUI.changed = true;
                            }
                        }
                        break;
                    case EntityRelativity.SelfAndChildren:
                        {
                            var obj = ObjUtil.GetAsFromSource(restrictionType, targ) as UnityEngine.Object;
                            if (obj == null && ComponentUtil.IsAcceptableComponentType(restrictionType)) obj = targ.GetComponentInChildren(restrictionType);
                            if (obj != null)
                            {
                                property.objectReferenceValue = obj;
                                GUI.changed = true;
                            }
                        }
                        break;
                }
            }
        }
        
        private static void ApplyDefaultAsList(SerializedProperty property, System.Type elementType, System.Type restrictionType, EntityRelativity relativity)
        {
            if (property.arraySize != 0) return;
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

                            property.arraySize = arr.Length;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                property.GetArrayElementAtIndex(i).objectReferenceValue = arr[i] as UnityEngine.Object;
                            }
                        }
                        break;
                    case EntityRelativity.Self:
                        {
                            var arr = ObjUtil.GetAllFromSource(restrictionType, targ, false);

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

    }
}
