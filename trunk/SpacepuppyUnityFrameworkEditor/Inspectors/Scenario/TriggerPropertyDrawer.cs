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

    [CustomPropertyDrawer(typeof(Trigger))]
    public class TriggerPropertyDrawer : PropertyDrawer
    {

        private const float MARGIN = 2.0f;

        private const string PROP_TARGETS = "_targets";
        private const string PROP_TRIGGERABLETARG = "Triggerable";
        private const string PROP_TRIGGERABLEARGS = "TriggerableArgs";
        private const string PROP_ACTIVATIONTYPE = "ActivationType";
        private const string PROP_METHODNAME = "MethodName";

        #region Fields

        private bool _initialized;

        private GUIContent _propertyLabel;
        private ReorderableList _targetList;
        private bool _foldoutTargetExtra;
        private TriggerTargetPropertyDrawer _triggerTargetDrawer;

        #endregion

        #region CONSTRUCTOR

        private void Init(SerializedProperty prop)
        {
            _targetList = new ReorderableList(prop.serializedObject, prop.FindPropertyRelative("_targets"), true, true, true, true);
            _targetList.drawHeaderCallback = _targetList_DrawHeader;
            _targetList.drawElementCallback = _targetList_DrawElement;
            _targetList.onAddCallback = _targetList_OnAdd;

            _triggerTargetDrawer = new TriggerTargetPropertyDrawer();

            _initialized = true;
        }

        #endregion



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_initialized) this.Init(property);

            var h = MARGIN * 2f;
            h += _targetList.GetHeight();
            h += EditorGUIUtility.singleLineHeight;
            if (_foldoutTargetExtra)
            {
                if (_targetList.index >= 0)
                {
                    var element = _targetList.serializedProperty.GetArrayElementAtIndex(_targetList.index);
                    h += _triggerTargetDrawer.GetPropertyHeight(element, GUIContent.none);
                }
                else
                {
                    h += EditorGUIUtility.singleLineHeight * 3.0f;
                }
            }
            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_initialized) this.Init(property);

            _propertyLabel = label;
            GUI.Box(position, GUIContent.none);
            position = new Rect(position.xMin + MARGIN, position.yMin + MARGIN, position.width - MARGIN * 2f, position.height - MARGIN * 2f);
            EditorGUI.BeginProperty(position, label, property);

            var listRect = new Rect(position.xMin, position.yMin, position.width, _targetList.GetHeight());

            EditorGUI.BeginChangeCheck();
            _targetList.DoList(listRect);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            if (_targetList.index >= _targetList.count) _targetList.index = -1;

            const float FOLDOUT_MRG = 12f;
            var foldoutRect = new Rect(position.xMin + FOLDOUT_MRG, listRect.yMax, position.width - FOLDOUT_MRG, EditorGUIUtility.singleLineHeight); //for some reason the foldout needs to be pushed in an extra amount for the arrow...
            _foldoutTargetExtra = EditorGUI.Foldout(foldoutRect, _foldoutTargetExtra, "Advanced Target Settings");
            if (_foldoutTargetExtra)
            {
                //EditorGUI.indentLevel++;

                if (_targetList.index >= 0)
                {
                    var element = _targetList.serializedProperty.GetArrayElementAtIndex(_targetList.index);
                    const float INDENT_MRG = 14f;
                    var settingsRect = new Rect(position.xMin + INDENT_MRG, foldoutRect.yMax, position.width - INDENT_MRG, _triggerTargetDrawer.GetPropertyHeight(element, GUIContent.none));
                    _triggerTargetDrawer.OnGUI(settingsRect, element, GUIContent.none);
                }
                else
                {
                    var helpRect = new Rect(position.xMin, foldoutRect.yMax, position.width, EditorGUIUtility.singleLineHeight * 3.0f);
                    EditorGUI.HelpBox(helpRect, "Select a target to edit.", MessageType.Info);
                }

                //EditorGUI.indentLevel--;
            }


            EditorGUI.EndProperty();
        }




        #region ReorderableList Handlers

        private void _targetList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, _propertyLabel, new GUIContent("Trigger Targets"));
        }

        private void _targetList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _targetList.serializedProperty.GetArrayElementAtIndex(index);

            var trigProp = element.FindPropertyRelative(PROP_TRIGGERABLETARG);
            var actProp = element.FindPropertyRelative(PROP_ACTIVATIONTYPE);
            var act = (TriggerActivationType)actProp.enumValueIndex;

            const float MARGIN = 1.0f;
            if (act == TriggerActivationType.TriggerAllOnTarget)
            {
                //Draw Triggerable - this is the simple case to make a clean designer set up for newbs
                var trigRect = new Rect(area.xMin, area.yMin + MARGIN, area.width, EditorGUIUtility.singleLineHeight);
                var trigLabel = new GUIContent("Target");
                EditorGUI.BeginProperty(trigRect, trigLabel, trigProp);
                trigProp.objectReferenceValue = SPEditorGUI.ComponentField(trigRect,
                                                                           trigLabel,
                                                                           TriggerTargetPropertyDrawer.ValidateTriggerableTargAsMechanism(trigProp.objectReferenceValue) as Component,
                                                                           typeof(ITriggerableMechanism),
                                                                           true);
                EditorGUI.EndProperty();
            }
            else
            {
                //Draw Triggerable - this forces the user to use the advanced settings, not for newbs
                var trigRect = new Rect(area.xMin, area.yMin + MARGIN, area.width, EditorGUIUtility.singleLineHeight);
                var trigLabel = new GUIContent("Advanced Target", "A target is not set, see advanced settings section to set a target.");

                if (trigProp.objectReferenceValue != null)
                {
                    var go = GameObjectUtil.GetGameObjectFromSource(trigProp.objectReferenceValue);
                    var trigType = trigProp.objectReferenceValue.GetType();
                    GUIContent extraLabel;
                    switch (act)
                    {
                        case TriggerActivationType.SendMessage:
                            extraLabel = new GUIContent("(SendMessage) " + go.name);
                            break;
                        case TriggerActivationType.TriggerSelectedTarget:
                            extraLabel = new GUIContent("(TriggerSelectedTarget) " + go.name + " -> " + trigType.Name);
                            break;
                        case TriggerActivationType.CallMethodOnSelectedTarget:
                            extraLabel = new GUIContent("(CallMethodOnSelectedTarget) " + go.name + " -> " + trigType.Name + "." + element.FindPropertyRelative(PROP_METHODNAME).stringValue);
                            break;
                        default:
                            extraLabel = GUIContent.none;
                            break;
                    }
                    EditorGUI.LabelField(trigRect, trigLabel, extraLabel);
                }
                else
                {
                    EditorGUI.LabelField(trigRect, trigLabel, new GUIContent("No Target"), new GUIStyle("Label") { alignment = TextAnchor.MiddleCenter });
                }
            }

            ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_targetList, area, index, isActive, isFocused);
        }

        private void _targetList_OnAdd(ReorderableList lst)
        {
            lst.serializedProperty.arraySize++;
            lst.index = lst.serializedProperty.arraySize - 1;

            lst.serializedProperty.serializedObject.ApplyModifiedProperties();

            var obj = EditorHelper.GetTargetObjectOfProperty(lst.serializedProperty.GetArrayElementAtIndex(lst.index)) as TriggerTarget;
            if (obj != null)
            {
                obj.Triggerable = null;
                obj.TriggerableArgs = new VariantReference[0];
                obj.ActivationType = TriggerActivationType.TriggerAllOnTarget;
                obj.MethodName = null;
                lst.serializedProperty.serializedObject.Update();
            }
        }

        #endregion

    }
}
