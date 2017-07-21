using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.AI.BehaviourTree;
using com.spacepuppy.AI.BehaviourTree.Components;
using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.Diminish;

using com.spacepuppyeditor.Internal;
using com.spacepuppyeditor.Utils.Diminish;

namespace com.spacepuppyeditor.AI.BehaviourTree.Components
{

    [CustomEditor(typeof(AIActionWeightsComponent), true)]
    public class AIActionWeightsComponentInspector : SPEditor
    {

        #region Fields

        public const string PROP_DEFAULTWEIGHT = "_defaultWeight";
        public const string PROP_ACTIONS = "_actions";
        public const string PROP_WEIGHTS = "_weights";

        private SPReorderableList _lst;
        private DiminishingWeightOverDurationPropertyDrawer _weightDrawer;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            this.serializedObject.Update();
            this.NormalizeActionAndWeightArrays();

            _weightDrawer = new DiminishingWeightOverDurationPropertyDrawer();
            _weightDrawer.DrawFoldout = false;

            _lst = new SPReorderableList(this.serializedObject, this.serializedObject.FindProperty(PROP_ACTIONS));
            _lst.displayAdd = false;
            _lst.displayRemove = false;
            _lst.draggable = false;
            _lst.drawHeaderCallback += this.OnDrawHeader;
            _lst.drawElementCallback += this.OnDrawElement;
            _lst.elementHeight = EditorGUIUtility.singleLineHeight * 2f + 3f;
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();
            this.NormalizeActionAndWeightArrays();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_DEFAULTWEIGHT);

            _lst.DoLayoutList();

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_DEFAULTWEIGHT, PROP_ACTIONS, PROP_WEIGHTS);

            this.serializedObject.ApplyModifiedProperties();
        }



        private void NormalizeActionAndWeightArrays()
        {
            var go = GameObjectUtil.GetGameObjectFromSource(this.serializedObject.targetObject);
            if (go == null) return;

            var propActions = this.serializedObject.FindProperty(PROP_ACTIONS);
            var propWeights = this.serializedObject.FindProperty(PROP_WEIGHTS);
            var currentActions = go.GetComponents<IAIAction>();
            IAIAction[] serializedActions = new IAIAction[propActions.arraySize];
            for(int i = 0; i < serializedActions.Length; i++)
            {
                serializedActions[i] = propActions.GetArrayElementAtIndex(i).objectReferenceValue as IAIAction;
            }

            if (currentActions.Length == serializedActions.Length && currentActions.Compare(serializedActions))
            {
                if (propWeights.arraySize != serializedActions.Length) propWeights.arraySize = serializedActions.Length;
                return;
            }

            var serializedWeights = (EditorHelper.GetTargetObjectOfProperty(propWeights) as DiminishingWeightOverDuration[]);
            if (serializedWeights == null) serializedWeights = new DiminishingWeightOverDuration[] { };
            else serializedWeights = serializedWeights.ToArray();

            propActions.arraySize = currentActions.Length;
            var weights = new List<DiminishingWeightOverDuration>();
            float defaultWeight = this.serializedObject.FindProperty(PROP_DEFAULTWEIGHT).floatValue;
            for(int i = 0; i < currentActions.Length; i++)
            {
                propActions.GetArrayElementAtIndex(i).objectReferenceValue = currentActions[i] as Component;

                int j = serializedActions.IndexOf(currentActions[i]);
                if (j >= 0 && j < serializedWeights.Length && serializedWeights[j] != null)
                    weights.Add(serializedWeights[j]);
                else
                {
                    Debug.Log(currentActions[i].GetType().Name + " had no weight");
                    weights.Add(new DiminishingWeightOverDuration(defaultWeight));
                }
            }
            this.serializedObject.ApplyModifiedProperties();
            EditorHelper.SetTargetObjectOfProperty(propWeights, weights.ToArray());
            this.serializedObject.Update();
        }

        #endregion

        #region List Handlers

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Weights");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            //var wl = EditorGUIUtility.labelWidth;
            //var wr = Mathf.Max(rect.width - EditorGUIUtility.labelWidth, 0f);
            //var wr1 = Mathf.Min(50f, wr);
            //var wr2 = Mathf.Max(wr - wr1, 0f);
            //var r1 = new Rect(rect.xMin, rect.yMin, wl, EditorGUIUtility.singleLineHeight);
            //var r2 = new Rect(r1.xMax, rect.yMin, wr1, EditorGUIUtility.singleLineHeight);
            //var r3 = new Rect(r2.xMax, rect.yMin, wr2, EditorGUIUtility.singleLineHeight);

            //var propAction = this.serializedObject.FindProperty(PROP_ACTIONS).GetArrayElementAtIndex(index);
            //var propWeight = this.serializedObject.FindProperty(PROP_WEIGHTS).GetArrayElementAtIndex(index);

            //GUI.Label(r1, propAction.objectReferenceValue.GetType().Name);
            //GUI.Label(r2, "Weight:");
            //EditorGUI.PropertyField(r3, propWeight.FindPropertyRelative(DiminishingWeightOverDurationPropertyDrawer.PROP_WEIGHT), GUIContent.none);


            //float margin = Mathf.Min(50f, rect.width * 0.1f);
            //const float LBL_WIDTH = 100f;
            //wl = rect.width - margin - 2f;
            //wr = wl / 2f;
            //wr1 = Mathf.Min(LBL_WIDTH, wr);
            //wr2 = Mathf.Max(wr - wr1, 0f);

            //r1 = new Rect(rect.xMin + margin, rect.yMin + EditorGUIUtility.singleLineHeight + 1f, wr1, EditorGUIUtility.singleLineHeight);
            //r2 = new Rect(r1.xMax, r1.yMin, wr2, EditorGUIUtility.singleLineHeight);
            //r3 = new Rect(r2.xMax + 2f, r1.yMin, wr1, EditorGUIUtility.singleLineHeight);
            //var r4 = new Rect(r3.xMax, r1.yMin, wr2, EditorGUIUtility.singleLineHeight);

            //GUI.Label(r1, "Diminish Rate:");
            //EditorGUI.PropertyField(r2, propWeight.FindPropertyRelative(DiminishingWeightOverDurationPropertyDrawer.PROP_DIMINISHRATE), GUIContent.none);
            //GUI.Label(r3, "Diminish Period:");
            //EditorGUI.PropertyField(r4, propWeight.FindPropertyRelative(DiminishingWeightOverDurationPropertyDrawer.PROP_DIMINISHPERIOD), GUIContent.none);

            var propAction = this.serializedObject.FindProperty(PROP_ACTIONS).GetArrayElementAtIndex(index);
            var propWeight = this.serializedObject.FindProperty(PROP_WEIGHTS).GetArrayElementAtIndex(index);
            _weightDrawer.OnGUI(rect, propWeight, EditorHelper.TempContent(propAction.objectReferenceValue.GetType().Name));
        }

        #endregion

    }
}
