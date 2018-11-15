using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;
using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor
{

    [CustomPropertyDrawer(typeof(QueryProxy))]
    public class QueryProxyPropertyDrawer : PropertyDrawer
    {
        
        private const string PROP_TARGET = "_target";
        private const string PROP_SEARCHBY = "_searchBy";
        private const string PROP_QUERY = "_queryString";

        private enum SearchByAlt
        {
            Direct = SearchBy.Nothing,
            Tag = SearchBy.Tag,
            Name = SearchBy.Name,
            Type = SearchBy.Type
        }


        #region Fields
        
        private SelectableComponentPropertyDrawer _selectableDrawer;
        
        private System.Type _inheritsFromType;
        private bool _allowProxy;
        private bool _manuallyConfigured;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public System.Type InheritsFromType
        {
            get { return _inheritsFromType; }
            set
            {
                _inheritsFromType = value;
                _manuallyConfigured = true;
            }
        }

        #endregion

        #region Methods

        private void Init(SerializedProperty property)
        {
            if (_manuallyConfigured) return;

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(QueryProxy.ConfigAttribute), false).FirstOrDefault() as QueryProxy.ConfigAttribute;
            if(attrib != null)
            {
                _inheritsFromType = attrib.TargetType;
                _allowProxy = attrib.AllowProxy;
            }
            else
            {
                _inheritsFromType = null;
                _allowProxy = true;
            }
        }

        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.Init(property);

            EditorGUI.BeginProperty(position, label, property);

            //################################
            //FIRST LINE
            var rect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);

            rect = EditorGUI.PrefixLabel(rect, label);

            var targetProp = property.FindPropertyRelative(PROP_TARGET);

            var w0 = Mathf.Min(rect.width * 0.3f, 80f);
            var w1 = rect.width - w0;
            var r0 = new Rect(rect.xMin, rect.yMin, w0, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(r0.xMax, rect.yMin, w1, EditorGUIUtility.singleLineHeight);
            
            var searchProp = property.FindPropertyRelative(PROP_SEARCHBY);
            var queryProp = property.FindPropertyRelative(PROP_QUERY);
            
            var eSearchBy = (SearchByAlt)searchProp.GetEnumValue<SearchBy>();
            EditorGUI.BeginChangeCheck();
            eSearchBy = (SearchByAlt)EditorGUI.EnumPopup(r0, eSearchBy);
            if (EditorGUI.EndChangeCheck())
                searchProp.SetEnumValue((SearchBy)eSearchBy);

            switch (eSearchBy)
            {
                case SearchByAlt.Direct:
                    {
                        //SPEditorGUI.PropertyField(r1, targetProp, GUIContent.none);
                        if(_selectableDrawer == null)
                        {
                            _selectableDrawer = new SelectableComponentPropertyDrawer();
                        }
                        _selectableDrawer.AllowSceneObjects = true;
                        _selectableDrawer.RestrictionType = _inheritsFromType;
                        _selectableDrawer.AllowProxy = _allowProxy;
                        _selectableDrawer.OnGUI(r1, targetProp, GUIContent.none);
                    }
                    break;
                case SearchByAlt.Tag:
                    {
                        queryProp.stringValue = EditorGUI.TagField(r1, queryProp.stringValue);
                        targetProp.objectReferenceValue = null;
                    }
                    break;
                case SearchByAlt.Name:
                    {
                        queryProp.stringValue = EditorGUI.TextField(r1, queryProp.stringValue);
                        targetProp.objectReferenceValue = null;
                    }
                    break;
                case SearchByAlt.Type:
                    {
                        var tp = TypeUtil.FindType(queryProp.stringValue);
                        if (!TypeUtil.IsType(tp, typeof(UnityEngine.Object))) tp = null;
                        tp = SPEditorGUI.TypeDropDown(r1, GUIContent.none, typeof(UnityEngine.Object),tp);
                        queryProp.stringValue = (tp != null) ? tp.FullName : null;
                        targetProp.objectReferenceValue = null;
                    }
                    break;
            }
            
            EditorGUI.EndProperty();
        }

        #endregion


    }

    
    [CustomPropertyDrawer(typeof(MemberProxy))]
    public class MemberProxyPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DynamicMemberAccess access = DynamicMemberAccess.Read;
            if(this.fieldInfo != null)
            {
                var attrib = this.fieldInfo.GetCustomAttributes(typeof(MemberProxy.ConfigAttribute), true).FirstOrDefault() as MemberProxy.ConfigAttribute;
                if (attrib != null) access = attrib.MemberAccessLevel;
            }

            DrawMemberProxy(position, property, label);
        }
        
        public static System.Reflection.MemberInfo DrawMemberProxy(Rect position, SerializedProperty property, GUIContent label, DynamicMemberAccess memberAccessLevel = DynamicMemberAccess.Read)
        {
            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(position.xMin, r0.yMax, position.width, EditorGUIUtility.singleLineHeight);

            var targProp = property.FindPropertyRelative("_target");
            var memberProp = property.FindPropertyRelative("_memberName");

            SPEditorGUI.PropertyField(r0, targProp, label);
            System.Reflection.MemberInfo selectedMember;
            memberProp.stringValue = SPEditorGUI.ReflectedPropertyField(r1,
                                                                        EditorHelper.TempContent(" - Property", "The property on the target to set."),
                                                                        targProp.objectReferenceValue,
                                                                        memberProp.stringValue,
                                                                        memberAccessLevel,
                                                                        out selectedMember);
            return selectedMember;
        }

    }
    
}
