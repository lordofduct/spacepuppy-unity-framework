using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors.Scenario
{

    [CustomPropertyDrawer(typeof(Trigger.TriggerTarget))]
    public class TriggerTargetPropertyDrawer : PropertyDrawer
    {
        private const string PROP_TRIGGERABLETARG = "Triggerable";
        private const string PROP_TRIGGERABLEARG = "TriggerableArg";
        private const string PROP_ACTIVATIONTYPE = "ActivationType";
        private const string PROP_METHODNAME = "MethodName";

        private VariantReferencePropertyDrawer _variantDrawer;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var actProp = property.FindPropertyRelative(PROP_ACTIVATIONTYPE);
            var act = (Trigger.TriggerActivationType)actProp.enumValueIndex;

            float h = EditorGUIUtility.singleLineHeight;

            switch (act)
            {
                case Trigger.TriggerActivationType.TriggerAllOnTarget:
                    h += EditorGUIUtility.singleLineHeight * 2.0f;
                    break;
                case Trigger.TriggerActivationType.TriggerSelectedTarget:
                    h += EditorGUIUtility.singleLineHeight * 3.0f;
                    break;
                case Trigger.TriggerActivationType.SendMessage:
                    h += EditorGUIUtility.singleLineHeight * 3.0f;
                    break;
                case Trigger.TriggerActivationType.CallMethodOnSelectedTarget:
                    h += EditorGUIUtility.singleLineHeight * 4.0f;
                    break;
            }

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Draw ActivationType Popup
            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var actProp = property.FindPropertyRelative(PROP_ACTIVATIONTYPE);
            EditorGUI.PropertyField(r0, actProp);
            var act = (Trigger.TriggerActivationType)actProp.enumValueIndex;

            //Draw Advanced
            var area = new Rect(position.xMin, r0.yMax, position.width, position.height - r0.height);
            switch (act)
            {
                case Trigger.TriggerActivationType.TriggerAllOnTarget:
                    this.DrawAdvanced_TriggerAll(area, property);
                    break;
                case Trigger.TriggerActivationType.TriggerSelectedTarget:
                    this.DrawAdvanced_TriggerSelected(area, property);
                    break;
                case Trigger.TriggerActivationType.SendMessage:
                    this.DrawAdvanced_SendMessage(area, property);
                    break;
                case Trigger.TriggerActivationType.CallMethodOnSelectedTarget:
                    this.DrawAdvanced_CallMethodOnSelected(area, property);
                    break;
            }
        }



        private void DrawAdvanced_TriggerAll(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            targProp.objectReferenceValue = SPEditorGUI.ComponentField(targRect,
                                                                       targLabel,
                                                                        ValidateTriggerableTargAsMechanism(targProp.objectReferenceValue),
                                                                        typeof(ITriggerableMechanism),
                                                                        true);


            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            var argProp = property.FindPropertyRelative(PROP_TRIGGERABLEARG);
            EditorGUI.PropertyField(argRect, argProp);
        }

        private void DrawAdvanced_TriggerSelected(Rect area, SerializedProperty property)
        {

            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                targProp.objectReferenceValue = (targGo != null) ? targGo.GetFirstLikeComponent<ITriggerableMechanism>() as Component : null;
            }

            var targCompPopupRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            if (targProp.objectReferenceValue != null)
            {
                var selectedType = targProp.objectReferenceValue.GetType();
                var availableMechanismTypes = (from c in targGo.GetLikeComponents<ITriggerableMechanism>() select c.GetType()).ToArray();
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
            var argRect = new Rect(area.xMin, targCompPopupRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            var argProp = property.FindPropertyRelative(PROP_TRIGGERABLEARG);
            EditorGUI.PropertyField(argRect, argProp);
        }

        private void DrawAdvanced_SendMessage(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                if (targGo != null)
                {
                    targProp.objectReferenceValue = (targGo.HasLikeComponent<ITriggerableMechanism>()) ? targGo.GetFirstLikeComponent<ITriggerableMechanism>() as Component : targGo.transform;
                }
                else
                {
                    targProp.objectReferenceValue = null;
                }
            }

            //Draw MessageName
            var msgRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(msgRect, property.FindPropertyRelative(PROP_METHODNAME), new GUIContent("Message", "Name of the message that should be sent."), false);

            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, msgRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            var argProp = property.FindPropertyRelative(PROP_TRIGGERABLEARG);
            EditorGUI.PropertyField(argRect, argProp, new GUIContent("Message Arg", "A parameter to be passed to the message if one is desired."));
        }

        private void DrawAdvanced_CallMethodOnSelected(Rect area, SerializedProperty property)
        {
            //Draw Target
            var targRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
            var targProp = property.FindPropertyRelative(PROP_TRIGGERABLETARG);
            var targLabel = new GUIContent("Triggerable Target");
            var targGo = GameObjectUtil.GetGameObjectFromSource(targProp.objectReferenceValue);
            var newTargGo = EditorGUI.ObjectField(targRect, targLabel, targGo, typeof(GameObject), true) as GameObject;
            if (newTargGo != targGo)
            {
                targGo = newTargGo;
                targProp.objectReferenceValue = (targGo != null) ? targGo.GetFirstComponent<MonoBehaviour>() : null;
            }

            var targCompPopupRect = new Rect(area.xMin, targRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            if (targProp.objectReferenceValue != null)
            {
                var selectedType = targProp.objectReferenceValue.GetType();
                var availableMechanismTypes = (from c in targGo.GetComponents<MonoBehaviour>() select c.GetType()).ToArray();
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

            //Draw Method Name
            var methNameRect = new Rect(area.xMin, targCompPopupRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            System.Reflection.MethodInfo selectedMethod = null;
            if (targProp.objectReferenceValue != null)
            {
                var methProp = property.FindPropertyRelative(PROP_METHODNAME);

                var tp = targProp.objectReferenceValue.GetType();
                var methods = GetAvailableMethods(tp).ToArray();
                var methodNames = methods.Select((m) => m.Name).ToArray();

                int index = System.Array.IndexOf(methodNames, methProp.stringValue);
                index = EditorGUI.Popup(methNameRect, new GUIContent("Method", "The method on the target to call."), index, (from n in methodNames select new GUIContent(n)).ToArray());
                methProp.stringValue = (index >= 0) ? methodNames[index] : null;
                selectedMethod = (index >= 0) ? methods[index] : null;
            }
            else
            {
                EditorGUI.Popup(methNameRect, new GUIContent("Method", "The method on the target to call."), -1, new GUIContent[0]);
            }

            property.serializedObject.ApplyModifiedProperties();

            //Draw Triggerable Arg
            var argRect = new Rect(area.xMin, methNameRect.yMax, area.width, EditorGUIUtility.singleLineHeight);
            var argProp = property.FindPropertyRelative(PROP_TRIGGERABLEARG);
            var variantRef = EditorHelper.GetTargetObjectOfProperty(argProp) as VariantReference;
            var argLabel = new GUIContent("Method Arg", "A parameter to be passed to the method if needed.");

            if (selectedMethod != null)
            {
                var parr = selectedMethod.GetParameters();
                if (parr.Length == 0)
                {
                    if (variantRef.ValueType != VariantReference.VariantType.Null)
                    {
                        variantRef.ValueType = VariantReference.VariantType.Null;
                        property.serializedObject.Update();
                    }

                    GUI.enabled = false;
                    EditorGUI.PropertyField(argRect, argProp, argLabel);
                    GUI.enabled = true;
                }
                else
                {
                    if (parr[0].ParameterType == typeof(object))
                    {
                        //draw the default variant as the method accepts anything
                        EditorGUI.PropertyField(argRect, argProp, argLabel);
                    }
                    else
                    {
                        var argType = VariantReference.GetVariantType(parr[0].ParameterType);
                        if (variantRef.ValueType != argType)
                        {
                            variantRef.ValueType = argType;
                            property.serializedObject.Update();
                        }

                        if (_variantDrawer == null) _variantDrawer = new VariantReferencePropertyDrawer();
                        _variantDrawer.RestrictVariantType = true;
                        _variantDrawer.ForcedComponentType = (ObjUtil.IsType(parr[0].ParameterType, typeof(Component))) ? parr[0].ParameterType : null;
                        _variantDrawer.OnGUI(argRect, argProp, argLabel);
                    }
                }
            }
            else
            {
                if (variantRef.ValueType != VariantReference.VariantType.Null)
                {
                    variantRef.ValueType = VariantReference.VariantType.Null;
                    property.serializedObject.Update();
                }

                GUI.enabled = false;
                EditorGUI.PropertyField(argRect, argProp, argLabel);
                GUI.enabled = true;
            }

        }





        #region Utils

        internal static Component ValidateTriggerableTargAsMechanism(object value)
        {
            if (value == null) return null;
            if (!(value is Component)) return null;

            if (value is ITriggerableMechanism) return value as Component;
            else return (value as Component).GetFirstLikeComponent<ITriggerableMechanism>() as Component;
        }

        private static IEnumerable<System.Reflection.MethodInfo> GetAvailableMethods(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");

            var methods = tp.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var m in methods)
            {
                if (m.IsSpecialName) continue;
                if (m.IsGenericMethod) continue;
                if (m.DeclaringType.IsAssignableFrom(typeof(MonoBehaviour))) continue;

                var parr = m.GetParameters();
                if (parr.Length == 0)
                {
                    yield return m;
                }
                else if (parr.Length == 1)
                {
                    if (VariantReference.AcceptableType(parr[0].ParameterType))
                        yield return m;
                    else if (parr[0].ParameterType == typeof(object))
                        yield return m;
                }
            }
        }

        #endregion

    }

}
