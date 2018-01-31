using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Scenario
{

    [CustomPropertyDrawer(typeof(BaseSPEvent), true)]
    public class TriggerPropertyDrawer : PropertyDrawer
    {

        private const float MARGIN = 2.0f;
        private const float BTN_ACTIVATE_HEIGHT = 24f;

        public const string PROP_YIELDING = "_yield";
        public const string PROP_TARGETS = "_targets";
        private const string PROP_WEIGHT = "_weight";

        #region Fields

        private GUIContent _currentLabel;
        private CachedReorderableList _targetList;
        private bool _foldoutTargetExtra;
        private TriggerTargetPropertyDrawer _triggerTargetDrawer = new TriggerTargetPropertyDrawer();

        private bool _drawWeight;
        private float _totalWeight = 0f;

        private bool _alwaysExpanded;

        private bool _customInspector;

        #endregion

        #region CONSTRUCTOR

        private void Init(SerializedProperty prop, GUIContent label)
        {
            _currentLabel = label;

            _targetList = CachedReorderableList.GetListDrawer(prop.FindPropertyRelative(PROP_TARGETS), _targetList_DrawHeader, _targetList_DrawElement, _targetList_OnAdd);

            if(!_customInspector)
            {
                if (this.fieldInfo != null)
                {
                    var attribs = this.fieldInfo.GetCustomAttributes(typeof(Trigger.ConfigAttribute), false) as Trigger.ConfigAttribute[];
                    if (attribs != null && attribs.Length > 0)
                    {
                        _drawWeight = attribs[0].Weighted;
                        _alwaysExpanded = attribs[0].AlwaysExpanded;
                    }
                }
                else
                {
                    _drawWeight = false;
                    _alwaysExpanded = false;
                }
            }
            _triggerTargetDrawer.DrawWeight = _drawWeight;
        }

        #endregion

        #region Properties

        public bool DrawWeight
        {
            get { return _drawWeight; }
            set
            {
                _drawWeight = value;
                _customInspector = true;
            }
        }

        public bool AlwaysExpanded
        {
            get { return _alwaysExpanded; }
            set
            {
                _alwaysExpanded = value;
                _customInspector = true;
            }
        }

        public System.Action<GUIContent, int> CustomizeEntryLabel
        {
            get;
            set;
        }

        #endregion

        #region Draw Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            this.Init(property, label);
            
            if (_alwaysExpanded || property.isExpanded)
            {
                h = MARGIN * 2f;
                h += _targetList.GetHeight();
                h += EditorGUIUtility.singleLineHeight * 2f;
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

                if (Application.isPlaying)
                {
                    h += BTN_ACTIVATE_HEIGHT;
                }
            }
            else
            {
                h = EditorGUIUtility.singleLineHeight;
            }

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;

            this.Init(property, label);

            //const float WIDTH_FOLDOUT = 5f;
            //if(!_alwaysExpanded) property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, WIDTH_FOLDOUT, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);
            if (!_alwaysExpanded) property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none, true);

            if (_alwaysExpanded || property.isExpanded)
            {
                if (_drawWeight) this.CalculateTotalWeight();

                if(!_alwaysExpanded) GUI.Box(position, GUIContent.none);

                position = new Rect(position.xMin + MARGIN, position.yMin + MARGIN, position.width - MARGIN * 2f, position.height - MARGIN * 2f);
                EditorGUI.BeginProperty(position, label, property);
                
                position = this.DrawList(position, property);
                position = this.DrawYieldToggle(position, property);
                position = this.DrawAdvancedTargetSettings(position, property);

                EditorGUI.EndProperty();

                if (Application.isPlaying && !property.serializedObject.isEditingMultipleObjects)
                {
                    var w = position.width * 0.6f;
                    var pad = (position.width - w) / 2f;
                    var rect = new Rect(position.xMin + pad, position.yMax + -BTN_ACTIVATE_HEIGHT + 2f, w, 20f);
                    if (GUI.Button(rect, "Activate Trigger"))
                    {
                        var targ = EditorHelper.GetTargetObjectOfProperty(property) as Trigger;
                        if (targ != null) targ.ActivateTrigger(property.serializedObject.targetObject, null);
                    }
                }
            }
            else
            {
                EditorGUI.BeginProperty(position, label, property);

                ReorderableListHelper.DrawRetractedHeader(position, label, EditorHelper.TempContent("Trigger Targets"));

                EditorGUI.EndProperty();
            }

        }


        private void CalculateTotalWeight()
        {
            _totalWeight = 0f;
            for(int i = 0; i < _targetList.serializedProperty.arraySize; i++)
            {
                _totalWeight += _targetList.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(PROP_WEIGHT).floatValue;
            }
        }


        private Rect DrawList(Rect position, SerializedProperty property)
        {
            var listRect = new Rect(position.xMin, position.yMin, position.width, _targetList.GetHeight());

            EditorGUI.BeginChangeCheck();
            _targetList.DoList(listRect);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            if (_targetList.index >= _targetList.count) _targetList.index = -1;

            return new Rect(position.xMin, listRect.yMax, position.width, position.height - listRect.height);
        }

        private Rect DrawAdvancedTargetSettings(Rect position, SerializedProperty property)
        {
            const float FOLDOUT_MRG = 12f;
            var foldoutRect = new Rect(position.xMin + FOLDOUT_MRG, position.yMin, position.width - FOLDOUT_MRG, EditorGUIUtility.singleLineHeight); //for some reason the foldout needs to be pushed in an extra amount for the arrow...
            position = new Rect(position.xMin, foldoutRect.yMax, position.width, position.yMax - foldoutRect.yMax);
            _foldoutTargetExtra = EditorGUI.Foldout(foldoutRect, _foldoutTargetExtra, "Advanced Target Settings");

            if (_foldoutTargetExtra)
            {
                if (_targetList.index >= 0)
                {
                    var element = _targetList.serializedProperty.GetArrayElementAtIndex(_targetList.index);
                    const float INDENT_MRG = 14f;
                    var settingsRect = new Rect(position.xMin + INDENT_MRG, position.yMin, position.width - INDENT_MRG, _triggerTargetDrawer.GetPropertyHeight(element, GUIContent.none));
                    _triggerTargetDrawer.OnGUI(settingsRect, element, GUIContent.none);

                    position = new Rect(position.xMin, settingsRect.yMax, position.width, position.yMax - settingsRect.yMax);
                }
                else
                {
                    var helpRect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight * 3.0f);
                    EditorGUI.HelpBox(helpRect, "Select a target to edit.", MessageType.Info);

                    position = new Rect(position.xMin, helpRect.yMax, position.width, position.yMax - helpRect.yMax);
                }
            }

            return position;
        }

        private Rect DrawYieldToggle(Rect position, SerializedProperty property)
        {
            var yieldProp = property.FindPropertyRelative(PROP_YIELDING);
            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);

            yieldProp.boolValue = EditorGUI.ToggleLeft(r, EditorHelper.TempContent("Yield", "Should we yield if called from a coroutine."), yieldProp.boolValue);

            return new Rect(position.xMin, r.yMax, position.width, position.yMax - r.yMax);
        }

        #endregion

        #region ReorderableList Handlers

        private void _targetList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, _currentLabel, EditorHelper.TempContent("Trigger Targets"));
        }

        private void _targetList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _targetList.serializedProperty.GetArrayElementAtIndex(index);

            var targProp = element.FindPropertyRelative(TriggerTargetPropertyDrawer.PROP_TRIGGERABLETARG);

            const float MARGIN = 1.0f;
            const float WEIGHT_FIELD_WIDTH = 60f;
            const float PERC_FIELD_WIDTH = 45f;
            const float FULLWEIGHT_WIDTH = WEIGHT_FIELD_WIDTH + PERC_FIELD_WIDTH;

            EditorGUI.BeginProperty(area, GUIContent.none, targProp);

            Rect trigRect;
            var actInfo = TriggerTargetPropertyDrawer.GetTriggerActivationInfo(element);
            GUIContent labelContent = EditorHelper.TempContent(index.ToString("00: ") + actInfo.ActivationTypeDisplayName);
            if (this.CustomizeEntryLabel != null) this.CustomizeEntryLabel(labelContent, index);
            if (_drawWeight && area.width > FULLWEIGHT_WIDTH)
            {
                var top = area.yMin + MARGIN;
                var labelRect = new Rect(area.xMin, top, EditorGUIUtility.labelWidth - FULLWEIGHT_WIDTH, EditorGUIUtility.singleLineHeight);
                var weightRect = new Rect(area.xMin + EditorGUIUtility.labelWidth - FULLWEIGHT_WIDTH, top, WEIGHT_FIELD_WIDTH, EditorGUIUtility.singleLineHeight);
                var percRect = new Rect(area.xMin + EditorGUIUtility.labelWidth - PERC_FIELD_WIDTH, top, PERC_FIELD_WIDTH, EditorGUIUtility.singleLineHeight);
                trigRect = new Rect(area.xMin + EditorGUIUtility.labelWidth, top, area.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

                var weightProp = element.FindPropertyRelative(PROP_WEIGHT);
                float weight = weightProp.floatValue;

                EditorGUI.LabelField(labelRect, labelContent);
                weightProp.floatValue = EditorGUI.FloatField(weightRect, weight);
                float p = (_totalWeight > 0f) ? (100f * weight / _totalWeight) : ((index == 0) ? 100f : 0f);
                EditorGUI.LabelField(percRect, string.Format("{0:0.#}%", p));
            }
            else
            {
                //Draw Triggerable - this is the simple case to make a clean designer set up for newbs
                var top = area.yMin + MARGIN;
                var labelRect = new Rect(area.xMin, top, area.width, EditorGUIUtility.singleLineHeight);

                trigRect = EditorGUI.PrefixLabel(labelRect, labelContent);
            }

            //Draw Triggerable - this is the simple case to make a clean designer set up for newbs
            EditorGUI.BeginChangeCheck();
            var targObj = TriggerTargetPropertyDrawer.TargetObjectField(trigRect, GUIContent.none, targProp.objectReferenceValue);
            if(EditorGUI.EndChangeCheck())
            {
                targProp.objectReferenceValue = TriggerTargetPropertyDrawer.IsValidTriggerTarget(targObj, actInfo.ActivationType) ? targObj : null;
            }
            EditorGUI.EndProperty();
            
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
                obj.Weight = 1f;
                lst.serializedProperty.serializedObject.Update();
            }
        }

        #endregion

    }
}
