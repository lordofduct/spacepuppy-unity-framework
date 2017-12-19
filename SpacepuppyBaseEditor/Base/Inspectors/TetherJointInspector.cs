using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomEditor(typeof(TetherJoint))]
    public class TetherJointInspector : SPEditor
    {
        
        public const string PROP_LINKTARGET = "_linkTarget";
        public const string PROP_UPDATEMODE = "_updateMode";
        public const string PROP_CONSTRAINT = "_constraint";
        public const string PROP_DOPOSDAMPING = "_doPositionDamping";
        public const string PROP_POSDAMPING = "_positionDampingStyle";
        public const string PROP_POSDAMPINGSTR = "_positionDampingStrength";
        public const string PROP_DOROTDAMPING = "_doRotationDamping";
        public const string PROP_ROTDAMPING = "_rotationDampingStyle";
        public const string PROP_ROTDAMPINGSTR = "_rotationDampingStrength";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_LINKTARGET);
            this.DrawPropertyField(PROP_UPDATEMODE);
            this.DrawPropertyField(PROP_CONSTRAINT);

            SerializedProperty prop;
            Rect r;

            prop = this.serializedObject.FindProperty(PROP_DOPOSDAMPING);
            r = EditorGUILayout.GetControlRect();
            prop.isExpanded = EditorGUI.Foldout(new Rect(r.xMin, r.yMin, EditorGUIUtility.labelWidth, r.height), prop.isExpanded, "Ease Position");
            r = new Rect(r.xMin + EditorGUIUtility.labelWidth, r.yMin, r.width - EditorGUIUtility.labelWidth, r.height);
            prop.boolValue = GUI.Toggle(r, prop.boolValue, GUIContent.none);
            if(prop.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.DrawPropertyField(PROP_POSDAMPING);
                this.DrawPropertyField(PROP_POSDAMPINGSTR);
                EditorGUI.indentLevel--;
            }

            prop = this.serializedObject.FindProperty(PROP_DOROTDAMPING);
            r = EditorGUILayout.GetControlRect();
            prop.isExpanded = EditorGUI.Foldout(new Rect(r.xMin, r.yMin, EditorGUIUtility.labelWidth, r.height), prop.isExpanded, "Ease Rotation");
            r = new Rect(r.xMin + EditorGUIUtility.labelWidth, r.yMin, r.width - EditorGUIUtility.labelWidth, r.height);
            prop.boolValue = GUI.Toggle(r, prop.boolValue, GUIContent.none);
            if (prop.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.DrawPropertyField(PROP_ROTDAMPING);
                this.DrawPropertyField(PROP_ROTDAMPINGSTR);
                EditorGUI.indentLevel--;
            }


            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_LINKTARGET, PROP_UPDATEMODE, PROP_CONSTRAINT, PROP_DOPOSDAMPING, PROP_POSDAMPING, PROP_POSDAMPINGSTR, PROP_DOROTDAMPING, PROP_ROTDAMPING, PROP_ROTDAMPINGSTR);

            this.serializedObject.ApplyModifiedProperties();
        }

    }

}
