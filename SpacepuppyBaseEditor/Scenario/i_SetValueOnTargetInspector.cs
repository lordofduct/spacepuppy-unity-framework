using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_SetValueOnTarget))]
    public class i_SetValueOnTargetInspector : SPEditor
    {

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawDefaultInspectorExcept("_target", "_restrictedType", "_memberName", "_values", "_mode");
            this.DrawPropertyField("_target"); //uses the SelectableComponent PropertyDrawer
            this.DrawPropertyField("_restrictedType");
            this.serializedObject.ApplyModifiedProperties();

            var targProp = this.serializedObject.FindProperty("_target");
            var restrictTypeProp = this.serializedObject.FindProperty("_restrictedType");
            var memberProp = this.serializedObject.FindProperty("_memberName");
            var valuesArrProp = this.serializedObject.FindProperty("_values");
            var modeProp = this.serializedObject.FindProperty("_mode");

            var targetRef = EditorHelper.GetTargetObjectOfProperty(targProp) as TriggerableTargetObject;
            if (targetRef == null) return;

            //SELECT MEMBER
            System.Reflection.MemberInfo selectedMember = null;
            var restrictType = TypeReferencePropertyDrawer.GetTypeFromTypeReference(restrictTypeProp);
            
            if (restrictType == null && !targetRef.TargetsTriggerArg && targetRef.Target != null &&
                targetRef.Find == TriggerableTargetObject.FindCommand.Direct && targetRef.ResolveBy != TriggerableTargetObject.ResolveByCommand.WithType && 
                targetRef.Target.GetType() != restrictType)
            {
                memberProp.stringValue = SPEditorGUILayout.ReflectedPropertyField(EditorHelper.TempContent("Property", "The property on the target to set."),
                                                                                  targetRef.Target,
                                                                                  memberProp.stringValue,
                                                                                  com.spacepuppy.Dynamic.DynamicMemberAccess.ReadWrite,
                                                                                  out selectedMember,
                                                                                  true);
            }
            else
            {
                if(restrictType == null)
                {
                    if (targetRef.ResolveBy == TriggerableTargetObject.ResolveByCommand.WithType) restrictType = TypeUtil.FindType(targetRef.ResolveByQuery);
                    if (restrictType == null)
                        restrictType = typeof(object);
                    else
                        TypeReferencePropertyDrawer.SetTypeToTypeReference(restrictTypeProp, restrictType);
                }
                memberProp.stringValue = SPEditorGUILayout.ReflectedPropertyField(EditorHelper.TempContent("Property", "The property on the target to set."),
                                                                                  restrictType,
                                                                                  memberProp.stringValue,
                                                                                  out selectedMember,
                                                                                  true);
            }
            this.serializedObject.ApplyModifiedProperties();


            //MEMBER VALUE TO SET TO
            if (selectedMember != null && selectedMember.MemberType == System.Reflection.MemberTypes.Method)
            {
                var methodInfo = selectedMember as System.Reflection.MethodInfo;
                if (methodInfo == null) return;

                var parameters = methodInfo.GetParameters();
                valuesArrProp.arraySize = parameters.Length;

                for(int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    var valueProp = valuesArrProp.GetArrayElementAtIndex(i);
                    var propType = p.ParameterType;

                    if (DynamicUtil.TypeIsVariantSupported(propType))
                    {
                        //draw the default variant as the method accepts anything
                        _variantDrawer.RestrictVariantType = false;
                        _variantDrawer.ForcedObjectType = null;
                        var label = EditorHelper.TempContent("Parameter " + i.ToString(), "The value to set to.");
                        _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(true, _variantDrawer.GetPropertyHeight(valueProp, label)), valueProp, label);
                    }
                    else
                    {
                        _variantDrawer.RestrictVariantType = true;
                        _variantDrawer.TypeRestrictedTo = propType;
                        _variantDrawer.ForcedObjectType = (TypeUtil.IsType(propType, typeof(UnityEngine.Object))) ? propType : null;
                        var label = EditorHelper.TempContent("Parameter " + i.ToString(), "The value to set to.");
                        _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(true, _variantDrawer.GetPropertyHeight(valueProp, label)), valueProp, label);
                    }
                }

                modeProp.SetEnumValue(i_SetValueOnTarget.SetMode.Set);
            }
            else if (selectedMember != null)
            {
                var propType = com.spacepuppy.Dynamic.DynamicUtil.GetInputType(selectedMember);
                var emode = modeProp.GetEnumValue<i_SetValueOnTarget.SetMode>();
                if (emode == i_SetValueOnTarget.SetMode.Toggle)
                {
                    //EditorGUILayout.LabelField(EditorHelper.TempContent(valueProp.displayName), EditorHelper.TempContent(propType.Name));
                    valuesArrProp.arraySize = 0;
                    var evtp = VariantReference.GetVariantType(propType);
                    var cache = SPGUI.Disable();
                    EditorGUILayout.EnumPopup(EditorHelper.TempContent("Value"), evtp);
                    cache.Reset();
                }
                else
                {
                    valuesArrProp.arraySize = 1;
                    var valueProp = valuesArrProp.GetArrayElementAtIndex(0);
                    if (DynamicUtil.TypeIsVariantSupported(propType))
                    {
                        //draw the default variant as the method accepts anything
                        _variantDrawer.RestrictVariantType = false;
                        _variantDrawer.ForcedObjectType = null;
                        var label = EditorHelper.TempContent("Value", "The value to set to.");
                        _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(true, _variantDrawer.GetPropertyHeight(valueProp, label)), valueProp, label);
                    }
                    else
                    {
                        _variantDrawer.RestrictVariantType = true;
                        _variantDrawer.TypeRestrictedTo = propType;
                        _variantDrawer.ForcedObjectType = (TypeUtil.IsType(propType, typeof(UnityEngine.Object))) ? propType : null;
                        var label = EditorHelper.TempContent("Value", "The value to set to.");
                        _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(true, _variantDrawer.GetPropertyHeight(valueProp, label)), valueProp, label);
                    }
                }

                if (com.spacepuppy.Dynamic.Evaluator.WillArithmeticallyCompute(propType))
                {
                    EditorGUILayout.PropertyField(modeProp);
                }
                else
                {
                    //modeProp.SetEnumValue(i_SetValueOnTarget.SetMode.Set);
                    EditorGUI.BeginChangeCheck();
                    emode = (i_SetValueOnTarget.SetMode)SPEditorGUILayout.EnumPopupExcluding(EditorHelper.TempContent(modeProp.displayName), emode, i_SetValueOnTarget.SetMode.Decrement, i_SetValueOnTarget.SetMode.Increment);
                    if (EditorGUI.EndChangeCheck())
                        modeProp.SetEnumValue(emode);
                }
            }
            else
            {
                modeProp.SetEnumValue(i_SetValueOnTarget.SetMode.Set);
            }

            this.serializedObject.ApplyModifiedProperties();
        }


    }

}
