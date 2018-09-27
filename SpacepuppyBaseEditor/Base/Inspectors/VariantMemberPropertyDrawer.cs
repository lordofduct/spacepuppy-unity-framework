using UnityEngine;
using UnityEditor;

using com.spacepuppy;
using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(VariantMember))]
    public class VariantMemberPropertyDrawer : PropertyDrawer
    {

        public const string PROP_TARGET = "_target";
        public const string PROP_MEMBER = "_memberName";

        private SelectableObjectPropertyDrawer _objectDrawer = new SelectableObjectPropertyDrawer();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propTarget = property.FindPropertyRelative(PROP_TARGET);
            var propMember = property.FindPropertyRelative(PROP_MEMBER);

            position = EditorGUI.PrefixLabel(position, label);
            
            if(propTarget.objectReferenceValue == null)
            {
                _objectDrawer.OnGUI(position, propTarget, GUIContent.none);
            }
            else
            {
                var r0 = new Rect(position.xMin, position.yMin, position.width * 0.5f, position.height);
                var r1 = new Rect(r0.xMax, position.yMin, position.width - r0.width, position.height);

                EditorGUI.BeginChangeCheck();
                _objectDrawer.OnGUI(r0, propTarget, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    this.PurgeIfPlaying(property);
                    if (propTarget.objectReferenceValue == null) return;
                }

                EditorGUI.BeginChangeCheck();
                System.Reflection.MemberInfo info;
                propMember.stringValue = SPEditorGUI.ReflectedPropertyField(r1, GUIContent.none,
                                                                                propTarget.objectReferenceValue,
                                                                                propMember.stringValue,
                                                                                com.spacepuppy.Dynamic.DynamicMemberAccess.ReadWrite,
                                                                                out info);
                if(EditorGUI.EndChangeCheck())
                {
                    this.PurgeIfPlaying(property);
                }
            }
        }

        private void PurgeIfPlaying(SerializedProperty property)
        {
            if(Application.isPlaying)
            {
                var obj = EditorHelper.GetTargetObjectOfProperty(property);
                if (obj is VariantMember) (obj as VariantMember).SetDirty();
            }
        }

    }

}
