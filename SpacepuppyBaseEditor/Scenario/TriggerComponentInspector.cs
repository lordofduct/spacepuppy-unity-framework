using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Scenario
{
    
    /// <summary>
    /// TODO - remove this class, deprecated.
    /// </summary>
    //[CustomEditor(typeof(TriggerComponent), true)]
    [System.Obsolete("TriggerComponent now lets its trigger properties drawn by the TriggerPropertyDrawer.")]
    public class TriggerComponentInspector : SPEditor
    {

        private const string PROP_TARGETS = "_targets";

        private ReorderableList _targetList;
        private bool _foldoutTargetExtra;

        void OnEnable()
        {
            _targetList = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty(PROP_TARGETS), true, true, true, true);
            _targetList.drawHeaderCallback = _targetList_DrawHeader;
            _targetList.drawElementCallback = _targetList_DrawElement;
            _targetList.onAddCallback = _targetList_OnAdd;
        }

        protected override void OnSPInspectorGUI()
        {
            if (_targetList.index >= _targetList.count) _targetList.index = -1;

            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            this.DrawTargets();

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_TARGETS);

            this.serializedObject.ApplyModifiedProperties();
        }


        #region Draw Targets

        private void DrawTargets()
        {
            EditorGUI.BeginChangeCheck();
            _targetList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
                this.serializedObject.ApplyModifiedProperties();
            if (_targetList.index >= _targetList.count) _targetList.index = -1;

            _foldoutTargetExtra = EditorGUILayout.Foldout(_foldoutTargetExtra, "Advanced Target Settings");
            if (_foldoutTargetExtra)
            {
                EditorGUILayout.BeginVertical("Box");

                if (_targetList.index >= 0)
                {
                    var element = _targetList.serializedProperty.GetArrayElementAtIndex(_targetList.index);

                    EditorGUILayout.PropertyField(element);
                }
                else
                {
                    EditorGUILayout.HelpBox("Select a target to edit.", MessageType.Info);
                }

                EditorGUILayout.EndVertical();
            }

        }

        #endregion


        #region ReorderableList Handlers

        private void _targetList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Trigger Targets");
        }

        private void _targetList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _targetList.serializedProperty.GetArrayElementAtIndex(index);

            var trigProp = element.FindPropertyRelative(TriggerTargetProps.PROP_TRIGGERABLETARG);
            var actProp = element.FindPropertyRelative(TriggerTargetProps.PROP_ACTIVATIONTYPE);
            //var act = (TriggerActivationType)actProp.enumValueIndex;
            var act = actProp.GetEnumValue<TriggerActivationType>();

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
                            extraLabel = new GUIContent("(CallMethodOnSelectedTarget) " + go.name + " -> " + trigType.Name + "." + element.FindPropertyRelative(TriggerTargetProps.PROP_METHODNAME).stringValue);
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
                obj.Clear();
                lst.serializedProperty.serializedObject.Update();
            }
        }

        #endregion

    }
}
