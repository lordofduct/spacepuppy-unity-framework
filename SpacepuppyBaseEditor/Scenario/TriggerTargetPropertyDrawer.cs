using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Scenario
{

    [CustomPropertyDrawer(typeof(TriggerTarget))]
    public class TriggerTargetPropertyDrawer : PropertyDrawer
    {

        public const string PROP_TRIGGERABLE = "_triggerable";
        public const string PROP_TRIGGERABLEARGS = "_triggerableArgs";
        public const string PROP_ACTIVATIONTYPE = "_activationType";
        public const string PROP_METHODNAME = "_methodName";


        private const float ARG_BTN_WIDTH = 18f;

        public bool DrawWeight;

        private GUIContent _defaultArgLabel = new GUIContent("Triggerable Arg");
        private GUIContent _undefinedArgLabel = new GUIContent("Undefined Arg", "The argument is not explicitly defined unless the trigger's event defines it.");
        private GUIContent _messageArgLabel = new GUIContent("Message Arg", "A parameter to be passed to the message if one is desired.");
        private GUIContent _argBtnLabel = new GUIContent("||", "Change between accepting a configured argument or not.");
        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();
        private int _callMethodModeExtraLines = 0;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var actProp = property.FindPropertyRelative(TriggerTargetProps.PROP_ACTIVATIONTYPE);
            //var act = (TriggerActivationType)actProp.enumValueIndex;
            var act = actProp.GetEnumValue<TriggerActivationType>();

            float h = EditorGUIUtility.singleLineHeight;

            switch (act)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    h += EditorGUIUtility.singleLineHeight * 2.0f;
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    h += EditorGUIUtility.singleLineHeight * 3.0f;
                    break;
                case TriggerActivationType.SendMessage:
                    h += EditorGUIUtility.singleLineHeight * 3.0f;
                    break;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    h += EditorGUIUtility.singleLineHeight * (3.0f + _callMethodModeExtraLines);
                    break;
            }

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //Draw ActivationType Popup
            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var actProp = property.FindPropertyRelative(TriggerTargetProps.PROP_ACTIVATIONTYPE);
            EditorGUI.PropertyField(r0, actProp);
            //var act = (TriggerActivationType)actProp.enumValueIndex;
            var act = actProp.GetEnumValue<TriggerActivationType>();

            //Draw Advanced
            var area = new Rect(position.xMin, r0.yMax, position.width, position.height - r0.height);
            switch (act)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    this.DrawAdvanced_TriggerAll(area, property);
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    this.DrawAdvanced_TriggerSelected(area, property);
                    break;
                case TriggerActivationType.SendMessage:
                    this.DrawAdvanced_SendMessage(area, property);
                    break;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    this.DrawAdvanced_CallMethodOnSelected(area, property);
                    break;
            }

            EditorGUI.EndProperty();
        }


        private void DrawAdvanced_TriggerAll(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            //targProp.objectReferenceValue = SPEditorGUI.ComponentField(targRect,
            //                                                           targLabel,
            //                                                            ValidateTriggerableTargAsMechanism(targProp.objectReferenceValue),
            //                                                            typeof(Transform),
            //                                                            true);
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                targProp.objectReferenceValue = (targGo != null) ? targGo.transform : null;
            }


            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, targRect.yMax, area.width - ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(argRect.xMax, argRect.yMin, ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var argArrayProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLEARGS);
            if(argArrayProp.arraySize == 0)
            {
                EditorGUI.LabelField(argRect, _defaultArgLabel, _undefinedArgLabel);
                if (GUI.Button(btnRect, _argBtnLabel))
                {
                    argArrayProp.arraySize = 1;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (argArrayProp.arraySize > 1) argArrayProp.arraySize = 1;
                var argProp = argArrayProp.GetArrayElementAtIndex(0);
                //EditorGUI.PropertyField(argRect, argProp, _defaultArgLabel);
                _variantDrawer.RestrictVariantType = false;
                _variantDrawer.ForcedObjectType = null;
                _variantDrawer.OnGUI(argRect, argProp, _defaultArgLabel);

                if (GUI.Button(btnRect, _argBtnLabel))
                {
                    argArrayProp.arraySize = 0;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawAdvanced_TriggerSelected(Rect area, SerializedProperty property)
        {

            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                targProp.objectReferenceValue = (targGo != null) ? targGo.GetComponentAlt<ITriggerableMechanism>() as Component : null;
            }

            var targCompPopupRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            if (targProp.objectReferenceValue != null)
            {
                var selectedType = targProp.objectReferenceValue.GetType();
                var availableMechanismTypes = (from c in targGo.GetComponentsAlt<ITriggerableMechanism>() select c.GetType()).ToArray();
                var availableMechanismTypeNames = availableMechanismTypes.Select((tp) => tp.Name).ToArray();

                var index = System.Array.IndexOf(availableMechanismTypes, selectedType);
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(targCompPopupRect, "Target Component", index, availableMechanismTypeNames);
                if (EditorGUI.EndChangeCheck())
                {
                    targProp.objectReferenceValue = (index >= 0) ? targGo.GetComponent(availableMechanismTypes[index]) : null;
                }
            }
            else
            {
                EditorGUI.LabelField(targCompPopupRect, "Target Component", "(First Select a Target)");
            }


            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, targCompPopupRect.yMax, area.width - ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(argRect.xMax, argRect.yMin, ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var argArrayProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLEARGS);
            if (argArrayProp.arraySize == 0)
            {
                EditorGUI.LabelField(argRect, _defaultArgLabel, _undefinedArgLabel);
                if (GUI.Button(btnRect, _argBtnLabel))
                {
                    argArrayProp.arraySize = 1;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (argArrayProp.arraySize > 1) argArrayProp.arraySize = 1;
                var argProp = argArrayProp.GetArrayElementAtIndex(0);
                //EditorGUI.PropertyField(argRect, argProp, _defaultArgLabel);
                _variantDrawer.RestrictVariantType = false;
                _variantDrawer.ForcedObjectType = null;
                _variantDrawer.OnGUI(argRect, argProp, _defaultArgLabel);

                if (GUI.Button(btnRect, _argBtnLabel))
                {
                    argArrayProp.arraySize = 0;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawAdvanced_SendMessage(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                if (targGo != null)
                {
                    //targProp.objectReferenceValue = (targGo.HasLikeComponent<ITriggerableMechanism>()) ? targGo.GetFirstLikeComponent<ITriggerableMechanism>() as Component : targGo.transform;
                    targProp.objectReferenceValue = targGo.transform;
                }
                else
                {
                    targProp.objectReferenceValue = null;
                }
            }

            //Draw MessageName
            var msgRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(msgRect, property.FindPropertyRelative(TriggerTargetProps.PROP_METHODNAME), new GUIContent("Message", "Name of the message that should be sent."), false);

            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, msgRect.yMax, area.width - ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(argRect.xMax, argRect.yMin, ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var argArrayProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLEARGS);
            if (argArrayProp.arraySize == 0)
            {
                EditorGUI.LabelField(argRect, _messageArgLabel, _undefinedArgLabel);
                if (GUI.Button(btnRect, _argBtnLabel))
                {
                    argArrayProp.arraySize = 1;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (argArrayProp.arraySize > 1) argArrayProp.arraySize = 1;
                var argProp = argArrayProp.GetArrayElementAtIndex(0);
                //EditorGUI.PropertyField(argRect, argProp, _messageArgLabel);
                _variantDrawer.RestrictVariantType = false;
                _variantDrawer.ForcedObjectType = null;
                _variantDrawer.OnGUI(argRect, argProp, _messageArgLabel);

                if (GUI.Button(btnRect, _argBtnLabel))
                {
                    argArrayProp.arraySize = 0;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawAdvanced_CallMethodOnSelected(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                targProp.objectReferenceValue = (targGo != null) ? targGo.transform : null;
            }

            var targCompPopupRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            if (targGo != null)
            {
                EditorGUI.BeginChangeCheck();
                var selectedComp = SPEditorGUI.SelectComponentFromSourceField(targCompPopupRect, "Target Component", targGo, targProp.objectReferenceValue as Component);
                if (EditorGUI.EndChangeCheck())
                {
                    targProp.objectReferenceValue = selectedComp;
                }
            }
            else
            {
                EditorGUI.LabelField(targCompPopupRect, "Target Component", "(First Select a Target)");
            }

            //Draw Method Name
            var methNameRect = new Rect(area.xMin, targCompPopupRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            System.Reflection.MemberInfo selectedMember = null;
            if (targProp.objectReferenceValue != null)
            {
                var methProp = property.FindPropertyRelative(TriggerTargetProps.PROP_METHODNAME);

                //var tp = targProp.objectReferenceValue.GetType();
                //var members = GetAvailableMethods(tp).ToArray();

                //var members = com.spacepuppy.Dynamic.DynamicUtil.GetEasilySerializedMembers(targProp.objectReferenceValue, System.Reflection.MemberTypes.Method).ToArray();
                var members = com.spacepuppy.Dynamic.DynamicUtil.GetEasilySerializedMembers(targProp.objectReferenceValue, System.Reflection.MemberTypes.All, spacepuppy.Dynamic.DynamicMemberAccess.Write).ToArray();
                System.Array.Sort(members, (a,b) => string.Compare(a.Name,b.Name, true));
                var memberNames = members.Select((m) => m.Name).ToArray();

                int index = System.Array.IndexOf(memberNames, methProp.stringValue);
                index = EditorGUI.Popup(methNameRect, new GUIContent("Method", "The method/prop on the target to call."), index, (from n in memberNames select new GUIContent(n)).ToArray());
                methProp.stringValue = (index >= 0) ? memberNames[index] : null;
                selectedMember = (index >= 0) ? members[index] : null;
            }
            else
            {
                EditorGUI.Popup(methNameRect, new GUIContent("Method", "The method/prop on the target to call."), -1, new GUIContent[0]);
            }

            property.serializedObject.ApplyModifiedProperties();

            //Draw Triggerable Arg
            var parr = (selectedMember != null) ? com.spacepuppy.Dynamic.DynamicUtil.GetDynamicParameterInfo(selectedMember) : null;
            if (parr == null || parr.Length == 0)
            {
                //NO PARAMETERS
                _callMethodModeExtraLines = 1;

                var argRect = new Rect(area.xMin, methNameRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
                var argArrayProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLEARGS);
                if(argArrayProp.arraySize > 0)
                {
                    argArrayProp.arraySize = 0;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }

                var cache = SPGUI.Disable();
                EditorGUI.LabelField(argRect, GUIContent.none, new GUIContent("*Zero Parameter Count*"));
                cache.Reset();
            }
            else
            {
                //MULTIPLE PARAMETERS - special case, does not support trigger event arg
                _callMethodModeExtraLines = parr.Length;

                var argArrayProp = property.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLEARGS);

                if (argArrayProp.arraySize != parr.Length)
                {
                    argArrayProp.arraySize = parr.Length;
                    argArrayProp.serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.indentLevel++;
                for (int i = 0; i < parr.Length; i++)
                {
                    var paramType = parr[i].ParameterType;
                    var argRect = new Rect(area.xMin, methNameRect.yMax + i * EditorGUIUtility.singleLineHeight, area.width, EditorGUIUtility.singleLineHeight);
                    var argProp = argArrayProp.GetArrayElementAtIndex(i);

                    if (paramType == typeof(object))
                    {
                        //draw the default variant as the method accepts anything
                        _variantDrawer.RestrictVariantType = false;
                        _variantDrawer.ForcedObjectType = null;
                        _variantDrawer.OnGUI(argRect, argProp, EditorHelper.TempContent("Arg " + i.ToString() + ": " + parr[i].ParameterName, "A parameter to be passed to the method if needed."));
                    }
                    else
                    {
                        _variantDrawer.RestrictVariantType = true;
                        _variantDrawer.TypeRestrictedTo = paramType;
                        _variantDrawer.ForcedObjectType = (paramType.IsInterface || TypeUtil.IsType(paramType, typeof(Component))) ? paramType : null;
                        _variantDrawer.OnGUI(argRect, argProp, EditorHelper.TempContent("Arg " + i.ToString() + ": " + parr[i].ParameterName, "A parameter to be passed to the method if needed."));
                    }
                }
                EditorGUI.indentLevel--;
            }
            
        }





        #region Utils

        internal static Component ValidateTriggerableTargAsMechanism(object value)
        {
            if (value == null) return null;
            if (!(value is Component)) return null;

            if (value is ITriggerableMechanism) return value as Component;
            else return (value as Component).GetComponentAlt<ITriggerableMechanism>() as Component;
        }

        #endregion

    }

}
