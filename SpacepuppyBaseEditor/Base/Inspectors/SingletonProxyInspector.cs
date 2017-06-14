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

        public const string PROP_SINGLETONTYPE = "_singletonType";
        public const string PROP_CREATEIFNONE = "_createIfNone";

        private TypeReferencePropertyDrawer _typeRefDrawer;

        protected override void OnEnable()
        {
            base.OnEnable();

            _typeRefDrawer = new TypeReferencePropertyDrawer();
            _typeRefDrawer.AllowAbstractTypes = true;
            _typeRefDrawer.AllowInterfaces = true;
            _typeRefDrawer.DropDownStyle = TypeDropDownListingStyle.Flat;
            _typeRefDrawer.SearchPredicate = (tp) =>
            {
                if (typeof(IManagedSingleton).IsAssignableFrom(tp))
                {
                    //managed singletons can't be interfaces/abstract
                    return !tp.IsInterface && !tp.IsAbstract;
                }
                else if (typeof(IService).IsAssignableFrom(tp))
                {
                    if (tp == typeof(IService)) return false;

                    //currently only allow interface services be listed
                    return tp.IsInterface;
                }
                else if (typeof(ISingleton).IsAssignableFrom(tp))
                {
                    //standard singletons can't be interfaces/abstract
                    return !tp.IsInterface && !tp.IsAbstract;
                }
                else
                    return false;
            };
        }

        protected override void OnSPInspectorGUI()
        {
            //this.DrawDefaultInspector();

            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            var typeProp = this.serializedObject.FindProperty(PROP_SINGLETONTYPE);
            var label = EditorHelper.TempContent("Singleton Type");
            var area = EditorGUILayout.GetControlRect(true, _typeRefDrawer.GetPropertyHeight(typeProp, label));
            _typeRefDrawer.OnGUI(area, typeProp, label);

            var tp = TypeReferencePropertyDrawer.GetTypeFromTypeReference(typeProp);
            if(tp != null && typeof(IManagedSingleton).IsAssignableFrom(tp))
            {
                this.DrawPropertyField(PROP_CREATEIFNONE);
            }
            
            this.serializedObject.ApplyModifiedProperties();





            //DRAW MEMBERS FOR PREVIEW
            this.DrawTargetMembersPreview();
        }


        private bool _propFoldout;
        private bool _methFoldout;

        private void DrawTargetMembersPreview()
        {
            var comp = this.serializedObject.targetObject as SingletonProxy;
            if (comp == null) return;

            const MemberTypes MASK_PROP = MemberTypes.Property | MemberTypes.Field;
            const MemberTypes MASK_METH = MemberTypes.Method;
            var members = DynamicUtil.GetEasilySerializedMembers(comp, MASK_PROP | MASK_METH, DynamicMemberAccess.Read).ToArray();

            _propFoldout = EditorGUILayout.Foldout(_propFoldout, "Properties:");
            if (_propFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var m in members)
                {
                    if ((m.MemberType & MASK_PROP) != 0)
                    {
                        if ((DynamicUtil.GetMemberAccessLevel(m) & DynamicMemberAccess.Write) != 0)
                            EditorGUILayout.LabelField(m.Name);
                        else
                            EditorGUILayout.LabelField(m.Name + " (readonly)");
                    }
                }
                EditorGUI.indentLevel--;
            }

            _methFoldout = EditorGUILayout.Foldout(_methFoldout, "Methods:");
            if (_methFoldout)
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
