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
        
        public const string PROP_TRIGGERABLETARG = "_triggerable";
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
            var actProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_ACTIVATIONTYPE);
            //var act = (TriggerActivationType)actProp.enumValueIndex;
            var act = actProp.GetEnumValue<TriggerActivationType>();

            float h = EditorGUIUtility.singleLineHeight;

            switch (act)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    h += EditorGUIUtility.singleLineHeight * 2.0f;
                    break;
                case TriggerActivationType.TriggerSelectedTarget:
                    //h += EditorGUIUtility.singleLineHeight * 3.0f;
                    h += EditorGUIUtility.singleLineHeight * 2.0f;
                    break;
                case TriggerActivationType.SendMessage:
                    h += EditorGUIUtility.singleLineHeight * 3.0f;
                    break;
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    //h += EditorGUIUtility.singleLineHeight * (3.0f + _callMethodModeExtraLines);
                    h += EditorGUIUtility.singleLineHeight * (2.0f + _callMethodModeExtraLines);
                    break;
                case TriggerActivationType.EnableTarget:
                    h += EditorGUIUtility.singleLineHeight;
                    break;
                case TriggerActivationType.DestroyTarget:
                    h += EditorGUIUtility.singleLineHeight;
                    break;
            }

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //Draw ActivationType Popup
            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var act = TriggerTargetPropertyDrawer.DrawTriggerActivationTypeDropdown(r0, property, true);

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
                case TriggerActivationType.EnableTarget:
                    this.DrawAdvanced_EnableTarget(area, property);
                    break;
                case TriggerActivationType.DestroyTarget:
                    this.DrawAdvanced_DestroyTarget(area, property);
                    break;
            }

            EditorGUI.EndProperty();
        }


        public static TriggerActivationType DrawTriggerActivationTypeDropdown(Rect area, SerializedProperty property, bool drawLabel)
        {
            var actProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_ACTIVATIONTYPE);
            //EditorGUI.PropertyField(area, actProp);
            
            var actInfo = GetTriggerActivationInfo(property);
            int index = System.Array.IndexOf(_triggerActivationTypeDisplayNames, actInfo.ActivationTypeDisplayName);
            EditorGUI.BeginChangeCheck();

            if (drawLabel)
                index = EditorGUI.Popup(area, actInfo.ActivationTypeProperty.displayName, index, _triggerActivationTypeDisplayNames);
            else
                index = EditorGUI.Popup(area, index, _triggerActivationTypeDisplayNames);

            if(EditorGUI.EndChangeCheck())
            {
                if (index <= 3)
                {
                    //the main ones
                    actInfo.ActivationTypeProperty.SetEnumValue<TriggerActivationType>((TriggerActivationType)index);
                }
                else if(index == 4)
                {
                    //enable
                    actInfo.ActivationTypeProperty.SetEnumValue<TriggerActivationType>(TriggerActivationType.EnableTarget);
                    var argProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_METHODNAME);
                    argProp.stringValue = EnableMode.Enable.ToString();
                }
                else if(index == 5)
                {
                    //disable
                    actInfo.ActivationTypeProperty.SetEnumValue<TriggerActivationType>(TriggerActivationType.EnableTarget);
                    var argProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_METHODNAME);
                    argProp.stringValue = EnableMode.Disable.ToString();
                }
                else if(index == 6)
                {
                    //toggle
                    actInfo.ActivationTypeProperty.SetEnumValue<TriggerActivationType>(TriggerActivationType.EnableTarget);
                    var argProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_METHODNAME);
                    argProp.stringValue = EnableMode.Toggle.ToString();
                }
                else if(index == 7)
                {
                    //destroy
                    actInfo.ActivationTypeProperty.SetEnumValue<TriggerActivationType>(TriggerActivationType.DestroyTarget);
                }
                else
                {
                    //unknown
                    actInfo.ActivationTypeProperty.SetEnumValue<TriggerActivationType>(TriggerActivationType.TriggerAllOnTarget);
                }
            }

            return actInfo.ActivationTypeProperty.GetEnumValue<TriggerActivationType>();
        }


        private void DrawAdvanced_TriggerAll(Rect area, SerializedProperty property)
        {
            //Draw Target
            /*
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            var targLabel = EditorHelper.TempContent("Triggerable Target");
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                targProp.objectReferenceValue = (targGo != null) ? targGo.transform : null;
            }
            */
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            var targLabel = EditorHelper.TempContent("Triggerable Target");

            EditorGUI.BeginChangeCheck();
            var targObj = TargetObjectField(targRect, targLabel, targProp.objectReferenceValue);
            if(EditorGUI.EndChangeCheck())
            {
                targProp.objectReferenceValue = TriggerTarget.IsValidTriggerTarget(targObj, TriggerActivationType.TriggerAllOnTarget) ? targObj : null;
            }

            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, targRect.yMax, area.width - ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(argRect.xMax, argRect.yMin, ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var argArrayProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLEARGS);
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

        private void DrawAdvanced_TriggerSelected(Rect area, SerializedProperty property)
        {
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            targRect = EditorGUI.PrefixLabel(targRect, EditorHelper.TempContent("Triggerable Target"));
            
            //validate
            if (!GameObjectUtil.IsGameObjectSource(targProp.objectReferenceValue) && !(targProp.objectReferenceValue is ITriggerableMechanism))
                targProp.objectReferenceValue = null;

            //draw obj field
            if (targProp.objectReferenceValue != null)
            {
                if (SPEditorGUI.XButton(ref targRect, "Clear Selected Object", true))
                {
                    targProp.objectReferenceValue = null;
                    goto DrawTriggerableArg;
                }

                var targObj = targProp.objectReferenceValue;

                var availableMechanisms = ObjUtil.GetAllFromSource<ITriggerableMechanism>(targObj);
                var availableMechanismTypeNames = availableMechanisms.Select((o) => EditorHelper.TempContent(o.GetType().Name)).ToArray();

                var index = System.Array.IndexOf(availableMechanisms, targObj);
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(targRect, GUIContent.none, index, availableMechanismTypeNames);
                if (EditorGUI.EndChangeCheck())
                {
                    targObj = (index >= 0) ? availableMechanisms[index] as UnityEngine.Object : null;
                    targProp.objectReferenceValue = targObj;
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var targObj = TargetObjectField(targRect, GUIContent.none, targProp.objectReferenceValue);
                if (EditorGUI.EndChangeCheck())
                {
                    targObj = ObjUtil.GetAsFromSource<ITriggerableMechanism>(targObj) as UnityEngine.Object;
                    targProp.objectReferenceValue = targObj;
                }
            }

            
            //Draw Triggerable Arg
DrawTriggerableArg:
            var argRect = new Rect(area.xMin, targRect.yMax, area.width - ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(argRect.xMax, argRect.yMin, ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var argArrayProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLEARGS);
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
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            var targLabel = EditorHelper.TempContent("Triggerable Target");
            //targProp.objectReferenceValue = TransformField(targRect, targLabel, targProp.objectReferenceValue);
            targProp.objectReferenceValue = TransformOrProxyField(targRect, targLabel, targProp.objectReferenceValue);
            

            //Draw MessageName
            var msgRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(msgRect, property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_METHODNAME), new GUIContent("Message", "Name of the message that should be sent."), false);

            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, msgRect.yMax, area.width - ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(argRect.xMax, argRect.yMin, ARG_BTN_WIDTH, EditorGUIUtility.singleLineHeight);
            var argArrayProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLEARGS);
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
            /*
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            var targLabel = EditorHelper.TempContent("Triggerable Target");
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
            */


            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            targRect = EditorGUI.PrefixLabel(targRect, EditorHelper.TempContent("Triggerable Target"));
            
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            if(targGo != null)
            {
                if (SPEditorGUI.XButton(ref targRect, "Clear Selected Object", true))
                {
                    targProp.objectReferenceValue = null;
                    goto DrawMethodName;
                }

                EditorGUI.BeginChangeCheck();
                var selectedComp = SPEditorGUI.SelectComponentFromSourceField(targRect, GUIContent.none, targGo, targProp.objectReferenceValue as Component);
                if (EditorGUI.EndChangeCheck())
                {
                    targProp.objectReferenceValue = selectedComp;
                }
            }
            else
            {
                targProp.objectReferenceValue = TargetObjectField(targRect, GUIContent.none, targProp.objectReferenceValue);
            }


            //Draw Method Name
DrawMethodName:
            //var methNameRect = new Rect(area.xMin, targCompPopupRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            var methNameRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            System.Reflection.MemberInfo selectedMember = null;
            if (targProp.objectReferenceValue != null)
            {
                var methProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_METHODNAME);

                //var tp = targProp.objectReferenceValue.GetType();
                //var members = GetAvailableMethods(tp).ToArray();

                //var members = com.spacepuppy.Dynamic.DynamicUtil.GetEasilySerializedMembers(targProp.objectReferenceValue, System.Reflection.MemberTypes.Method).ToArray();
                var members = com.spacepuppy.Dynamic.DynamicUtil.GetEasilySerializedMembers(targProp.objectReferenceValue, System.Reflection.MemberTypes.All, spacepuppy.Dynamic.DynamicMemberAccess.Write).ToArray();
                System.Array.Sort(members, (a, b) => string.Compare(a.Name, b.Name, true));
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
                var argArrayProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLEARGS);
                if (argArrayProp.arraySize > 0)
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

                var argArrayProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLEARGS);

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

        private void DrawAdvanced_EnableTarget(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            var targLabel = EditorHelper.TempContent("Triggerable Target");
            //targProp.objectReferenceValue = TransformField(targRect, targLabel, targProp.objectReferenceValue);
            targProp.objectReferenceValue = TransformOrProxyField(targRect, targLabel, targProp.objectReferenceValue);

            /*
            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            var argProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_METHODNAME);

            var e = ConvertUtil.ToEnum<EnableMode>(argProp.stringValue, EnableMode.Enable);
            e = (EnableMode)EditorGUI.EnumPopup(argRect, "Mode", e);
            argProp.stringValue = e.ToString();
            */
        }

        private void DrawAdvanced_DestroyTarget(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            var targLabel = EditorHelper.TempContent("Triggerable Target");
            //targProp.objectReferenceValue = TransformField(targRect, targLabel, targProp.objectReferenceValue);
            targProp.objectReferenceValue = TransformOrProxyField(targRect, targLabel, targProp.objectReferenceValue);
        }



        #region Utils

        private static string[] _triggerActivationTypeDisplayNames = new string[]
        {
            "Trigger All On Target",
            "Trigger Selected Target",
            "Send Message",
            "Call Method On Selected Target",
            "Enable Target",
            "Disable Target",
            "Toggle Target",
            "Destroy Target"
        };
        public struct TriggerActivationInfo
        {
            public TriggerActivationType ActivationType;
            public string ActivationTypeDisplayName;
            public SerializedProperty ActivationTypeProperty;
        }
        public static TriggerActivationInfo GetTriggerActivationInfo(SerializedProperty triggerTargetProperty)
        {
            var result = new TriggerActivationInfo();
            result.ActivationTypeProperty = triggerTargetProperty.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_ACTIVATIONTYPE);
            result.ActivationType = result.ActivationTypeProperty.GetEnumValue<TriggerActivationType>();
            switch(result.ActivationType)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                case TriggerActivationType.TriggerSelectedTarget:
                case TriggerActivationType.SendMessage:
                case TriggerActivationType.CallMethodOnSelectedTarget:
                    result.ActivationTypeDisplayName = _triggerActivationTypeDisplayNames[(int)result.ActivationType];
                    break;
                case TriggerActivationType.EnableTarget:
                    {
                        var argProp = triggerTargetProperty.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_METHODNAME);
                        switch (ConvertUtil.ToEnum<EnableMode>(argProp.stringValue))
                        {
                            case EnableMode.Enable:
                                result.ActivationTypeDisplayName = _triggerActivationTypeDisplayNames[4];
                                break;
                            case EnableMode.Disable:
                                result.ActivationTypeDisplayName = _triggerActivationTypeDisplayNames[5];
                                break;
                            case EnableMode.Toggle:
                                result.ActivationTypeDisplayName = _triggerActivationTypeDisplayNames[6];
                                break;
                        }
                    }
                    break;
                case TriggerActivationType.DestroyTarget:
                    result.ActivationTypeDisplayName = _triggerActivationTypeDisplayNames[7];
                    break;
            }
            return result;
        }

        /// <summary>
        /// Validates target object is appropriate for the activation type. Null is considered valid.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>Returns false if invalid</returns>
        public static bool ValidateTriggerTargetProperty(SerializedProperty property)
        {
            if (property == null) return false;

            var targProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);
            if (targProp.objectReferenceValue == null) return true;

            var actProp = property.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_ACTIVATIONTYPE);
            var act = actProp.GetEnumValue<TriggerActivationType>();
            
            if(!TriggerTarget.IsValidTriggerTarget(targProp.objectReferenceValue, act))
            {
                targProp.objectReferenceValue = null;
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public static UnityEngine.Object TargetObjectField(Rect position, GUIContent label, UnityEngine.Object target)
        {
            EditorGUI.BeginChangeCheck();

            UnityEngine.Object result = target;
            if (GameObjectUtil.IsGameObjectSource(result))
                result = GameObjectUtil.GetGameObjectFromSource(result);
            result = EditorGUI.ObjectField(position, label, result, typeof(UnityEngine.Object), true);
            if (EditorGUI.EndChangeCheck())
            {
                if (GameObjectUtil.IsGameObjectSource(result))
                {
                    return GameObjectUtil.GetGameObjectFromSource(result).transform;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return target;
            }
        }

        private static Transform TransformField(Rect position, GUIContent label, UnityEngine.Object target)
        {
            if (!GameObjectUtil.IsGameObjectSource(target))
            {
                GUI.changed = true;
                target = null;
            }

            var go = GameObjectUtil.GetGameObjectFromSource(target);
            go = EditorGUI.ObjectField(position, label, go, typeof(GameObject), true) as GameObject;
            return go != null ? go.transform : null;
        }

        private static UnityEngine.Object TransformOrProxyField(Rect position, GUIContent label, UnityEngine.Object target)
        {
            if(!GameObjectUtil.IsGameObjectSource(target))
            {
                GUI.changed = true;
                target = null;
            }

            if(target == null)
            {
                var go = EditorGUI.ObjectField(position, label, target, typeof(GameObject), true) as GameObject;
                return (go != null) ? go.transform : null;
            }
            else
            {
                var targGo = GameObjectUtil.GetGameObjectFromSource(target);
                if(target is IProxy || targGo.HasComponent<IProxy>())
                {
                    using (var lst = com.spacepuppy.Collections.TempCollection.GetList<IProxy>())
                    {
                        targGo.GetComponents<IProxy>(lst);
                        GUIContent[] entries = new GUIContent[lst.Count + 1];
                        int index = -1;
                        entries[0] = EditorHelper.TempContent("GameObject");
                        for(int i = 0; i < lst.Count; i++)
                        {
                            entries[i + 1] = EditorHelper.TempContent(string.Format("Proxy -> ({0})", lst[i].GetType().Name));
                            if (index < 0 && object.ReferenceEquals(target, lst[i]))
                                index = i + 1;
                        }
                        if (index < 0)
                            index = 0;

                        index = EditorGUI.Popup(position, label, index, entries);
                        if (index < 0 || index >= entries.Length)
                            return null;

                        return (index == 0) ? targGo.transform : lst[index - 1] as UnityEngine.Object;
                    }
                }
                else
                {
                    var go = EditorGUI.ObjectField(position, label, targGo, typeof(GameObject), true) as GameObject;
                    return (go != null) ? go.transform : null;
                }
            }
        }
        
        #endregion

    }

}
