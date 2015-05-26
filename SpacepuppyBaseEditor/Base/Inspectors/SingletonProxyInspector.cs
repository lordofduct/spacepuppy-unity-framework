using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomEditor(typeof(SingletonProxy))]
    public class SingletonProxyInspector : SPEditor
    {

        private bool _propFoldout;
        private bool _methFoldout;

        protected override void OnSPInspectorGUI()
        {
            this.DrawDefaultInspector();

            var comp = this.serializedObject.targetObject as SingletonProxy;
            if (comp == null) return;

            var members = DynamicUtil.GetEasilySerializedMembers(comp).ToArray();

            _propFoldout = EditorGUILayout.Foldout(_propFoldout, "Properties:");
            if(_propFoldout)
            {
                EditorGUI.indentLevel++;
                var mask = System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Field;
                foreach(var m in members)
                {
                    if((m.MemberType & mask) != 0)
                    {
                        EditorGUILayout.LabelField(m.Name);
                    }
                }
                EditorGUI.indentLevel--;
            }

            _methFoldout = EditorGUILayout.Foldout(_methFoldout, "Methods:");
            if(_methFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var m in members)
                {
                    if (m.MemberType == System.Reflection.MemberTypes.Method)
                    {
                        EditorGUILayout.LabelField(m.Name);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

    }
}
