using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            const MemberTypes MASK_PROP = MemberTypes.Property | MemberTypes.Field;
            const MemberTypes MASK_METH = MemberTypes.Method;
            var members = DynamicUtil.GetEasilySerializedMembers(comp, MASK_PROP | MASK_METH, DynamicMemberAccess.Read).ToArray();

            _propFoldout = EditorGUILayout.Foldout(_propFoldout, "Properties:");
            if(_propFoldout)
            {
                EditorGUI.indentLevel++;
                foreach(var m in members)
                {
                    if((m.MemberType & MASK_PROP) != 0)
                    {
                        if((DynamicUtil.GetMemberAccessLevel(m) & DynamicMemberAccess.Write) != 0)
                            EditorGUILayout.LabelField(m.Name);
                        else
                            EditorGUILayout.LabelField(m.Name + " (readonly)");
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
                    if (m.MemberType == MASK_METH)
                    {
                        EditorGUILayout.LabelField(m.Name);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

    }
}
