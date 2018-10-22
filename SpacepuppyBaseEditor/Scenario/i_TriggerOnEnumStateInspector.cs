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

    [CustomEditor(typeof(i_TriggerOnEnumState), true)]
    public class i_TriggerOnEnumStateInspector : SPEditor
    {
        
        public const string PROP_TARGET = "_target";
        public const string PROP_CONDITIONS = "_conditions";
        public const string PROP_DEFAULTCONDITION = "_defaultCondition";
        public const string PROP_PASSALONG = "_passAlongTriggerArg";

        private const string PROP_CONDITIONBLOCK_VALUE = "_value";
        private const string PROP_CONDITIONBLOCK_FLAGS = "_enumFlags";
        private const string PROP_CONDITIONBLOCK_TRIGGER = "_trigger";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(EditorHelper.PROP_ORDER);
            this.DrawPropertyField(EditorHelper.PROP_ACTIVATEON);
            this.DrawPropertyField(PROP_TARGET);

            this.DrawConditionsBlock();

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, EditorHelper.PROP_ORDER, EditorHelper.PROP_ACTIVATEON, PROP_TARGET, PROP_CONDITIONS, PROP_DEFAULTCONDITION);

            this.serializedObject.ApplyModifiedProperties();
        }

        protected void DrawConditionsBlock()
        {
            EditorGUILayout.BeginVertical("Box");
            
            VariantReference vref = EditorHelper.GetTargetObjectOfProperty(this.serializedObject.FindProperty(PROP_TARGET)) as VariantReference;
            System.Type enumType = vref != null ? vref.GetPropertyReturnType() : null;
            if (enumType != null && !enumType.IsEnum) enumType = null;

            //draw conditions blocks
            var conditionsArrayProp = this.serializedObject.FindProperty(PROP_CONDITIONS);

            for (int i = 0; i < conditionsArrayProp.arraySize; i++)
            {
                var conditionBlockProp = conditionsArrayProp.GetArrayElementAtIndex(i);
                var valueProp = conditionBlockProp.FindPropertyRelative(PROP_CONDITIONBLOCK_VALUE);
                var flagsProp = conditionBlockProp.FindPropertyRelative(PROP_CONDITIONBLOCK_FLAGS);
                
                EditorGUILayout.LabelField((enumType != null) ? "Case " + ConvertUtil.ToEnumOfType(enumType, valueProp.intValue).ToString() + ":": "Case " + valueProp.intValue.ToString() +":", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Value");

                if(enumType == null)
                {
                    valueProp.intValue = EditorGUILayout.IntField(valueProp.intValue);
                }
                else
                {
                    switch ((i_TriggerOnEnumState.EnumFlagTestModes)flagsProp.intValue)
                    {
                        case i_TriggerOnEnumState.EnumFlagTestModes.NoFlag:
                            {
                                valueProp.intValue = ConvertUtil.ToInt(EditorGUILayout.EnumPopup(ConvertUtil.ToEnumOfType(enumType, valueProp.intValue)));
                            }
                            break;
                        default:
                            {
                                //valueProp.intValue = ConvertUtil.ToInt(EditorGUILayout.EnumFlagsField(ConvertUtil.ToEnumOfType(enumType, valueProp.intValue)));
                                valueProp.intValue = SPEditorGUILayout.EnumFlagField(enumType, valueProp.intValue);
                            }
                            break;
                    }
                }
                
                flagsProp.intValue = ConvertUtil.ToInt(EditorGUILayout.EnumPopup((i_TriggerOnEnumState.EnumFlagTestModes)flagsProp.intValue));
                EditorGUILayout.EndHorizontal();

                var triggerProp = conditionBlockProp.FindPropertyRelative(PROP_CONDITIONBLOCK_TRIGGER);
                SPEditorGUILayout.PropertyField(triggerProp);

                EditorGUILayout.Space();
            }

            //draw else
            EditorGUILayout.LabelField("Default Case:", EditorStyles.boldLabel);
            SPEditorGUILayout.PropertyField(this.serializedObject.FindProperty(PROP_DEFAULTCONDITION));

            EditorGUILayout.Space();

            //draw add buttons
            var fullRect = EditorGUILayout.GetControlRect();
            var leftRect = new Rect(fullRect.xMin, fullRect.yMin, fullRect.width / 2f, fullRect.height);
            var rightRect = new Rect(fullRect.xMin + leftRect.width, fullRect.yMin, fullRect.width / 2f, fullRect.height);
            if (GUI.Button(leftRect, "Add Condition"))
            {
                conditionsArrayProp.arraySize++;
            }
            if (GUI.Button(rightRect, "Remove Condition"))
            {
                conditionsArrayProp.arraySize--;
            }

            EditorGUILayout.EndVertical();
        }

    }

}
