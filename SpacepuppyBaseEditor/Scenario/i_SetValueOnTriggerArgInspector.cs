using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_SetValueOnTriggerArg))]
    public class i_SetValueOnTriggerArgInspector : SPEditor
    {

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawDefaultInspectorExcept("_searchEntity", "_componentType", "_memberName", "_value", "_mode");
            this.DrawPropertyField("_searchEntity");
            this.DrawPropertyField("_componentType"); //uses the TypeReference PropertyDrawer

            var compTypeProp = this.serializedObject.FindProperty("_componentType");
            var memberProp = this.serializedObject.FindProperty("_memberName");
            var valueProp = this.serializedObject.FindProperty("_value");
            var modeProp = this.serializedObject.FindProperty("_mode");


            //SELECT MEMBER
            System.Reflection.MemberInfo selectedMember;
            memberProp.stringValue = SPEditorGUILayout.ReflectedPropertyField(EditorHelper.TempContent("Property", "The property on the target to set."),
                                                                              TypeReferencePropertyDrawer.GetTypeFromTypeReference(compTypeProp),
                                                                              memberProp.stringValue,
                                                                              out selectedMember);
            this.serializedObject.ApplyModifiedProperties();


            //MEMBER VALUE TO SET TO
            if (selectedMember != null)
            {
                var propType = com.spacepuppy.Dynamic.DynamicUtil.GetParameters(selectedMember).FirstOrDefault();
                var emode = modeProp.GetEnumValue<i_SetValue.SetMode>();
                if (emode == i_SetValue.SetMode.Toggle)
                {
                    //EditorGUILayout.LabelField(EditorHelper.TempContent(valueProp.displayName), EditorHelper.TempContent(propType.Name));
                    var evtp = VariantReference.GetVariantType(propType);
                    GUI.enabled = false;
                    EditorGUILayout.EnumPopup(EditorHelper.TempContent(valueProp.displayName), evtp);
                    GUI.enabled = true;
                }
                else
                {
                    if (propType == typeof(object))
                    {
                        //draw the default variant as the method accepts anything
                        _variantDrawer.RestrictVariantType = false;
                        _variantDrawer.ForcedObjectType = null;
                        _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(), valueProp, EditorHelper.TempContent("Value", "The value to set to."));
                    }
                    else
                    {
                        var argType = VariantReference.GetVariantType(propType);
                        _variantDrawer.RestrictVariantType = true;
                        _variantDrawer.VariantTypeRestrictedTo = argType;
                        _variantDrawer.ForcedObjectType = (TypeUtil.IsType(propType, typeof(Component))) ? propType : null;
                        _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(), valueProp, EditorHelper.TempContent("Value", "The value to set to."));
                    }
                }

                if (com.spacepuppy.Dynamic.Evaluator.WillArithmeticallyCompute(propType))
                {
                    EditorGUILayout.PropertyField(modeProp);
                }
                else
                {
                    //modeProp.SetEnumValue(i_SetValue.SetMode.Set);
                    EditorGUI.BeginChangeCheck();
                    emode = (i_SetValue.SetMode)SPEditorGUILayout.EnumPopupExcluding(EditorHelper.TempContent(modeProp.displayName), emode, i_SetValue.SetMode.Decrement, i_SetValue.SetMode.Increment);
                    if (EditorGUI.EndChangeCheck())
                        modeProp.SetEnumValue(emode);
                }
            }
            else
            {
                modeProp.SetEnumValue(i_SetValue.SetMode.Set);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
