using UnityEngine;
using UnityEditor;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(VersionInfo))]
    public class VersionInfoPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var p1 = property.FindPropertyRelative("Major");
            var p2 = property.FindPropertyRelative("Minor");
            var p3 = property.FindPropertyRelative("Patch");
            var p4 = property.FindPropertyRelative("Build");

            position = EditorGUI.PrefixLabel(position, label);

            const float MARGIN = 9f;
            const float TOTAL_MARGIN = MARGIN * 3f;
            float w = (position.width - TOTAL_MARGIN) / 4f;

            if (w <= 0f) return;

            Rect r;

            r = new Rect(position.xMin, position.yMin, w, position.height);
            p1.intValue = EditorGUI.IntField(r, p1.intValue);
            r = new Rect(r.xMax, r.yMin, MARGIN, r.height);
            EditorGUI.LabelField(r, ".");

            r = new Rect(r.xMax, r.yMin, w, r.height);
            p2.intValue = EditorGUI.IntField(r, p2.intValue);
            r = new Rect(r.xMax, r.yMin, MARGIN, r.height);
            EditorGUI.LabelField(r, ".");

            r = new Rect(r.xMax, r.yMin, w, r.height);
            p3.intValue = EditorGUI.IntField(r, p3.intValue);
            r = new Rect(r.xMax, r.yMin, MARGIN, r.height);
            EditorGUI.LabelField(r, ".");

            r = new Rect(r.xMax, r.yMin, w, r.height);
            p4.intValue = EditorGUI.IntField(r, p4.intValue);
        }

    }

}
