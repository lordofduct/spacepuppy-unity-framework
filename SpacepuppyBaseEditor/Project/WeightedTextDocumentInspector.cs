using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Project
{

    [CustomEditor(typeof(WeightedTextDocument))]
    public class WeightedTextDocumentInspector : SPEditor
    {

        private ReorderableList _lst;
        private SerializedProperty _weightsProp;
        private float _totalOdds;

        protected override void OnEnable()
        {
            base.OnEnable();

            _lst = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty("_text"));
            _lst.drawHeaderCallback += _maskList_DrawHeader;
            _lst.drawElementCallback += _maskList_DrawElement;
            _lst.elementHeight = EditorGUIUtility.singleLineHeight * 4f;
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            _weightsProp = this.serializedObject.FindProperty("_weights");
            _lst.serializedProperty = this.serializedObject.FindProperty("_text");
            _weightsProp.arraySize = _lst.serializedProperty.arraySize;

            _totalOdds = 0f;
            for (int i = 0; i < _weightsProp.arraySize; i++)
            {
                _totalOdds += _weightsProp.GetArrayElementAtIndex(i).floatValue;
            }

            _lst.DoLayoutList();

            this.serializedObject.ApplyModifiedProperties();
        }


        private void _maskList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Text");
        }

        private void _maskList_DrawElement(Rect position, int index, bool isActive, bool isFocused)
        {
            var element = _lst.serializedProperty.GetArrayElementAtIndex(index);

            var labelRect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var textRect = new Rect(position.xMin, position.yMin + labelRect.height + 2f, position.width, position.height - labelRect.height - 3f);

            //draw label with odds
            var label = EditorHelper.TempContent("Element " + index.ToString("00"));
            var area = EditorGUI.PrefixLabel(labelRect, label);

            const float WEIGHT_FIELD_WIDTH = 60f;
            const float PERC_FIELD_WIDTH = 45f;
            const float FULLWEIGHT_WIDTH = WEIGHT_FIELD_WIDTH + PERC_FIELD_WIDTH;
            var weightRect = new Rect(area.xMax - FULLWEIGHT_WIDTH, area.yMin, WEIGHT_FIELD_WIDTH, EditorGUIUtility.singleLineHeight);
            var percRect = new Rect(area.xMax - PERC_FIELD_WIDTH, area.yMin, PERC_FIELD_WIDTH, EditorGUIUtility.singleLineHeight);
            if (index >= _weightsProp.arraySize) _weightsProp.arraySize = index + 1;

            var weightsProp = _weightsProp.GetArrayElementAtIndex(index);
            weightsProp.floatValue = EditorGUI.FloatField(weightRect, weightsProp.floatValue);
            float p = (_totalOdds > 0f) ? (100f * weightsProp.floatValue / _totalOdds) : ((index == 0) ? 100f : 0f);
            EditorGUI.LabelField(percRect, string.Format("{0:0.#}%", p));

            //draw text
            element.stringValue = EditorGUI.TextArea(textRect, element.stringValue);
        }

    }
}
