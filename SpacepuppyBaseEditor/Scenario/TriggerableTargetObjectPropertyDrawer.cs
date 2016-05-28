using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;
using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Scenario
{

    [CustomPropertyDrawer(typeof(TriggerableTargetObject))]
    public class TriggerableTargetObjectPropertyDrawer : PropertyDrawer
    {

        private const string PROP_CONFIGURED = "_configured";
        private const string PROP_TARGET = "_target";
        private const string PROP_FIND = "_find";
        private const string PROP_RESOLVEBY = "_resolveBy";
        private const string PROP_QUERY = "_queryString";

        public enum TargetSource
        {
            Arg = 0,
            Self = 1,
            Root = 2,
            Config = 3
        }

        #region Fields

        private TriggerableTargetObject.ConfigAttribute _configAttrib;
        private SelectableComponentPropertyDrawer _objectDrawer = new SelectableComponentPropertyDrawer()
        {
            AllowNonComponents = true
        };
        private bool _defaultSet;

        #endregion

        #region Methods

        private void Init(SerializedProperty property)
        {
            _configAttrib = this.fieldInfo.GetCustomAttributes(typeof(TriggerableTargetObject.ConfigAttribute), false).FirstOrDefault() as TriggerableTargetObject.ConfigAttribute;
        }





        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.Init(property);

            if(property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight * 2f;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.Init(property);

            EditorGUI.BeginProperty(position, label, property);

            //################################
            //FIRST LINE
            var rect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);

            var configProp = property.FindPropertyRelative(PROP_CONFIGURED);
            var targetProp = property.FindPropertyRelative(PROP_TARGET);

            //var r0 = new Rect(rect.xMin, rect.yMin, EditorGUIUtility.labelWidth, rect.height);
            //rect = new Rect(r0.xMax, rect.yMin, rect.width - r0.width, rect.height);
            //property.isExpanded = EditorGUI.Foldout(r0, property.isExpanded, label);
            property.isExpanded = SPEditorGUI.PrefixFoldoutLabel(ref rect, property.isExpanded, label);

            var r0 = new Rect(rect.xMin, rect.yMin, Mathf.Min(rect.width * 0.25f, 50f), rect.height);
            var e = (configProp.boolValue) ? TargetSource.Config : TargetSource.Arg;
            EditorGUI.BeginChangeCheck();
            e = (TargetSource)EditorGUI.EnumPopup(r0, e);
            if(EditorGUI.EndChangeCheck())
            {
                UpdateTargetFromSource(targetProp, e, (_configAttrib != null) ? _configAttrib.TargetType : null);
                configProp.boolValue = (e != TargetSource.Arg);
            }
            else if(e == TargetSource.Config && !_defaultSet && targetProp.objectReferenceValue == null)
            {
                UpdateTargetFromSource(targetProp, e, (_configAttrib != null) ? _configAttrib.TargetType : null);
                _defaultSet = true;
            }
            else
            {
                _defaultSet = true;
            }

            var r1 = new Rect(rect.xMin + r0.width, rect.yMin, rect.width - r0.width, rect.height);
            if(!configProp.boolValue)
            {
                EditorGUI.LabelField(r1, "Target determined by activating trigger.");
                targetProp.objectReferenceValue = null;
            }
            else
            {
                _objectDrawer.RestrictionType = (_configAttrib != null) ? _configAttrib.TargetType : null;
                _objectDrawer.OnGUI(r1, targetProp, GUIContent.none);
            }


            //################################
            //SECOND LINE
            if (property.isExpanded)
            {
                var indent = EditorGUIUtility.labelWidth * 0.5f;
                rect = new Rect(position.xMin + indent, position.yMin + EditorGUIUtility.singleLineHeight, Mathf.Max(0f, position.width - indent), EditorGUIUtility.singleLineHeight);

                var w0 = Mathf.Min(rect.width * 0.3f, 120f);
                var w1 = Mathf.Min(rect.width * 0.3f, 80f);
                var w2 = rect.width - w0 - w1;
                r0 = new Rect(rect.xMin, rect.yMin, w0, rect.height);
                r1 = new Rect(r0.xMax, rect.yMin, w1, rect.height);
                var r2 = new Rect(r1.xMax, rect.yMin, w2, rect.height);

                var findProp = property.FindPropertyRelative(PROP_FIND);
                var resolveProp = property.FindPropertyRelative(PROP_RESOLVEBY);
                var queryProp = property.FindPropertyRelative(PROP_QUERY);

                var e0 = findProp.GetEnumValue<TriggerableTargetObject.FindCommand>();
                EditorGUI.BeginChangeCheck();
                e0 = (TriggerableTargetObject.FindCommand)EditorGUI.EnumPopup(r0, e0);
                if (EditorGUI.EndChangeCheck())
                    findProp.SetEnumValue(e0);

                var e1 = resolveProp.GetEnumValue<TriggerableTargetObject.ResolveByCommand>();
                EditorGUI.BeginChangeCheck();
                e1 = (TriggerableTargetObject.ResolveByCommand)EditorGUI.EnumPopup(r1, e1);
                if (EditorGUI.EndChangeCheck())
                    resolveProp.SetEnumValue(e1);

                switch(e1)
                {
                    case TriggerableTargetObject.ResolveByCommand.Nothing:
                        {
                            var cache = SPGUI.Disable();
                            EditorGUI.TextField(r2, string.Empty);
                            queryProp.stringValue = string.Empty;
                            cache.Reset();
                        }
                        break;
                    case TriggerableTargetObject.ResolveByCommand.WithTag:
                        {
                            queryProp.stringValue = EditorGUI.TagField(r2, queryProp.stringValue);
                        }
                        break;
                    case TriggerableTargetObject.ResolveByCommand.WithName:
                        {
                            queryProp.stringValue = EditorGUI.TextField(r2, queryProp.stringValue);
                        }
                        break;
                }

            }

            EditorGUI.EndProperty();
        }

        #endregion


        #region Utils

        private static void UpdateTargetFromSource(SerializedProperty property, TargetSource esrc, System.Type targetType)
        {
            switch(esrc)
            {
                case TargetSource.Arg:
                    {
                        property.objectReferenceValue = null;
                    }
                    break;
                case TargetSource.Self:
                    {
                        UnityEngine.Object obj = property.serializedObject.targetObject;
                        if (targetType != null)
                            obj = ObjUtil.GetAsFromSource(targetType, obj) as UnityEngine.Object;
                        property.objectReferenceValue = obj;
                    }
                    break;
                case TargetSource.Root:
                    {
                        UnityEngine.Object obj = property.serializedObject.targetObject;
                        var go = GameObjectUtil.GetGameObjectFromSource(obj);
                        if (go != null)
                            obj = go.FindRoot();

                        if (targetType != null)
                            obj = ObjUtil.GetAsFromSource(targetType, obj) as UnityEngine.Object;
                        property.objectReferenceValue = obj;
                    }
                    break;
                case TargetSource.Config:
                    {
                        if(property.objectReferenceValue == null)
                        {
                            UnityEngine.Object obj = property.serializedObject.targetObject;
                            if (targetType != null)
                                obj = ObjUtil.GetAsFromSource(targetType, obj) as UnityEngine.Object;
                            property.objectReferenceValue = obj;
                        }
                    }
                    break;
            }
        }

        #endregion

    }

}
