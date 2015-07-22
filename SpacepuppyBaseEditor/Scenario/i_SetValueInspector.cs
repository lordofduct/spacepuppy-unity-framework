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

    [CustomEditor(typeof(i_SetValue))]
    public class i_SetValueInspector : SPEditor
    {

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawDefaultInspectorExcept("_target", "_memberName", "_value", "_mode");
            this.DrawPropertyField("_target");

            var targProp = this.serializedObject.FindProperty("_target");
            var memberProp = this.serializedObject.FindProperty("_memberName");
            var valueProp = this.serializedObject.FindProperty("_value");
            var modeProp = this.serializedObject.FindProperty("_mode");

            ////TARGET
            //var targLabel = EditorHelper.TempContent("Target Object");
            //var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            //var newTargGo = EditorGUILayout.ObjectField(targLabel, targGo, typeof(GameObject), true) as GameObject;
            //if (newTargGo != targGo)
            //{
            //    targGo = newTargGo;
            //    targProp.objectReferenceValue = (targGo != null) ? targGo.transform : null;
            //}

            //if (targGo != null)
            //{
            //    EditorGUI.BeginChangeCheck();
            //    var selectedComp = SPEditorGUILayout.SelectComponentFromSourceField("Target Component", targGo, targProp.objectReferenceValue as Component);
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        targProp.objectReferenceValue = selectedComp;
            //    }
            //}
            //else
            //{
            //    EditorGUILayout.LabelField("Target Component", "(First Select a Target)");
            //}

            //SELECT MEMBER
            System.Reflection.MemberInfo selectedMember = null;
            if(targProp.objectReferenceValue != null)
            {
                var members = com.spacepuppy.Dynamic.DynamicUtil.GetEasilySerializedMembers(targProp.objectReferenceValue, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property).ToArray();
                var memberNames = members.Select((m) => m.Name).ToArray();

                int index = System.Array.IndexOf(memberNames, memberProp.stringValue);
                index = EditorGUILayout.Popup(new GUIContent("Property", "The method on the target to set."), index, (from n in memberNames select new GUIContent(n)).ToArray());
                memberProp.stringValue = (index >= 0) ? memberNames[index] : null;
                selectedMember = (index >= 0) ? members[index] : null;
            }
            else
            {
                EditorGUILayout.Popup(new GUIContent("Property", "The property on the target to set."), -1, new GUIContent[0]);
            }
            this.serializedObject.ApplyModifiedProperties();


            //MEMBER VALUE TO SET TO
            if(selectedMember != null)
            {
                var propType = com.spacepuppy.Dynamic.DynamicUtil.GetParameters(selectedMember).FirstOrDefault();

                if(propType == typeof(object))
                {
                    //draw the default variant as the method accepts anything
                    _variantDrawer.RestrictVariantType = false;
                    _variantDrawer.ForcedComponentType = null;
                    _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(), valueProp, EditorHelper.TempContent("Value", "The value to set to."));
                }
                else
                {
                    var variantRef = EditorHelper.GetTargetObjectOfProperty(valueProp) as VariantReference;
                    var argType = VariantReference.GetVariantType(propType);
                    if (variantRef.ValueType != argType)
                    {
                        variantRef.ValueType = argType;
                        this.serializedObject.Update();
                    }

                    _variantDrawer.RestrictVariantType = true;
                    _variantDrawer.ForcedComponentType = (TypeUtil.IsType(propType, typeof(Component))) ? propType : null;
                    _variantDrawer.OnGUI(EditorGUILayout.GetControlRect(), valueProp, EditorHelper.TempContent("Value", "The value to set to."));
                }


                if(com.spacepuppy.Dynamic.DynamicUtil.WillArithmeticallyCompute(propType))
                {
                    EditorGUILayout.PropertyField(modeProp);
                }
                else
                {
                    modeProp.SetEnumValue(i_SetValue.SetMode.Set);
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
