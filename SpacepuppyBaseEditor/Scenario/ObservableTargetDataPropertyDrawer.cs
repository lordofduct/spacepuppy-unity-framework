using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Scenario
{

    [CustomPropertyDrawer(typeof(ObservableTargetData), true)]
    public class ObservableTargetDataPropertyDrawer : PropertyDrawer
    {

        public const string PROP_TARGET = "_target";
        public const string PROP_TRIGGERINDEX = "_triggerIndex";

        private const float BOX_MARGIN_V = 1f;
        private const float PROP_MARGIN_V = 1f;
        private const float MARGIN_V = BOX_MARGIN_V + PROP_MARGIN_V;

        private const float BOX_MARGIN_H = 0f;
        private const float PROP_MARGIN_H = 1f;
        private const float MARGIN_H = BOX_MARGIN_H + PROP_MARGIN_H;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + MARGIN_V * 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Box(new Rect(position.xMin + BOX_MARGIN_H, position.yMin + BOX_MARGIN_V, position.width - BOX_MARGIN_H * 2f, position.height - BOX_MARGIN_V * 2f), GUIContent.none);

            var r0 = new Rect(position.xMin + MARGIN_H, position.yMin + MARGIN_V, position.width - MARGIN_H * 2f, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(position.xMin + MARGIN_H, r0.yMax, position.width - MARGIN_H * 2f, EditorGUIUtility.singleLineHeight);

            var targProp = property.FindPropertyRelative(PROP_TARGET);
            var indexProp = property.FindPropertyRelative(PROP_TRIGGERINDEX);

            EditorGUI.BeginChangeCheck();
            SPEditorGUI.PropertyField(r0, targProp);
            if (EditorGUI.EndChangeCheck())
                indexProp.intValue = 0;

            if (targProp.objectReferenceValue is IObservableTrigger)
            {
                var targ = targProp.objectReferenceValue as IObservableTrigger;
                var owner = new SerializedObject(targProp.objectReferenceValue);

                int i = 0;
                var events = (from e in targ.GetTriggers() select GetTriggerTargsId(owner, e, ++i)).ToArray();
                indexProp.intValue = EditorGUI.Popup(r1, "Trigger Event", indexProp.intValue, events);
            }
            else
            {
                EditorGUI.LabelField(r1, "Trigger Event", "Select Target First");
            }
        }


        private static string GetTriggerTargsId(SerializedObject owner, BaseSPEvent e, int index)
        {
            var child = owner.GetIterator();
            child.Next(true);
            do
            {
                var v = EditorHelper.GetTargetObjectOfProperty(child);
                if (v is BaseSPEvent && v == e) return child.displayName;
            }
            while (child.Next(false));

            if (string.IsNullOrEmpty(e.ObservableTriggerId))
                return "Trigger (" + index.ToString() + ")";
            else
                return e.ObservableTriggerId + "(" + index.ToString() + ")";
        }


    }
    
}
