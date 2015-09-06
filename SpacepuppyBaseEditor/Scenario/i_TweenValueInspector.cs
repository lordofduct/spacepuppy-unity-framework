using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_TweenValue))]
    public class i_TweenValueInspector : SPEditor
    {
        public const string PROP_TIMESUPPLIER = "_timeSupplier";
        public const string PROP_TARGET = "_target";
        public const string PROP_TWEENDATA = "_data";
        public const string PROP_ONCOMPLETE = "_onComplete";

        private const string PROP_DATA_MODE = "Mode";
        private const string PROP_DATA_MEMBER = "MemberName";
        private const string PROP_DATA_EASE = "Ease";
        private const string PROP_DATA_VALUES = "ValueS";
        private const string PROP_DATA_VALUEE = "ValueE";
        private const string PROP_DATA_DUR = "Duration";


        private ReorderableList _dataList;
        private SerializedProperty _targetProp;
        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        protected override void OnEnable()
        {
            base.OnEnable();

            _dataList = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty(PROP_TWEENDATA));
            _dataList.drawHeaderCallback = _dataList_DrawHeader;
            _dataList.drawElementCallback = _dataList_DrawElement;
            _dataList.elementHeight = EditorGUIUtility.singleLineHeight * 6f + 7f;
            
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            _targetProp = this.serializedObject.FindProperty(PROP_TARGET);

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_TIMESUPPLIER);
            SPEditorGUILayout.PropertyField(_targetProp);
            _dataList.DoLayoutList();
            this.DrawPropertyField(PROP_ONCOMPLETE);

            this.serializedObject.ApplyModifiedProperties();
        }



        #region ReorderableList Handlers

        private void _dataList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Tween Data");
        }

        private void _dataList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            Rect position;
            var el = _dataList.serializedProperty.GetArrayElementAtIndex(index);

            position = CalcNextRect(ref area);
            SPEditorGUI.PropertyField(position, el.FindPropertyRelative(PROP_DATA_MODE));

            //TODO - member
            position = CalcNextRect(ref area);
            var memberProp = el.FindPropertyRelative(PROP_DATA_MEMBER);
            System.Reflection.MemberInfo selectedMember;
            memberProp.stringValue = SPEditorGUI.ReflectedPropertyField(position,
                                                                        EditorHelper.TempContent("Property", "The property on the target to set."),
                                                                        _targetProp.objectReferenceValue,
                                                                        memberProp.stringValue,
                                                                        out selectedMember);

            position = CalcNextRect(ref area);
            SPEditorGUI.PropertyField(position, el.FindPropertyRelative(PROP_DATA_EASE));

            position = CalcNextRect(ref area);
            SPEditorGUI.PropertyField(position, el.FindPropertyRelative(PROP_DATA_DUR));

            if(selectedMember != null)
            {
                var propType = com.spacepuppy.Dynamic.DynamicUtil.GetParameters(selectedMember).FirstOrDefault();

                switch (el.FindPropertyRelative(PROP_DATA_MODE).GetEnumValue<TweenHash.AnimMode>())
                {
                    case TweenHash.AnimMode.To:
                        {
                            position = CalcNextRect(ref area);
                            this.DrawVariant(position, EditorHelper.TempContent("To Value"), propType, el.FindPropertyRelative(PROP_DATA_VALUES));

                            position = CalcNextRect(ref area);
                        }
                        break;
                    case TweenHash.AnimMode.From:
                        {
                            position = CalcNextRect(ref area);
                            this.DrawVariant(position, EditorHelper.TempContent("From Value"), propType, el.FindPropertyRelative(PROP_DATA_VALUES));
                        }
                        break;
                    case TweenHash.AnimMode.By:
                        {
                            position = CalcNextRect(ref area);
                            this.DrawVariant(position, EditorHelper.TempContent("By Value"), propType, el.FindPropertyRelative(PROP_DATA_VALUES));
                        }
                        break;
                    case TweenHash.AnimMode.FromTo:
                        {
                            position = CalcNextRect(ref area);
                            this.DrawVariant(position, EditorHelper.TempContent("Start Value"), propType, el.FindPropertyRelative(PROP_DATA_VALUES));

                            position = CalcNextRect(ref area);
                            this.DrawVariant(position, EditorHelper.TempContent("End Value"), propType, el.FindPropertyRelative(PROP_DATA_VALUEE));
                        }
                        break;
                    case TweenHash.AnimMode.RedirectTo:
                        {
                            position = CalcNextRect(ref area);
                            this.DrawVariant(position, EditorHelper.TempContent("Start Value"), propType, el.FindPropertyRelative(PROP_DATA_VALUES));

                            position = CalcNextRect(ref area);
                            this.DrawVariant(position, EditorHelper.TempContent("End Value"), propType, el.FindPropertyRelative(PROP_DATA_VALUEE));
                        }
                        break;
                }
            }


        }

        private void DrawVariant(Rect position, GUIContent label, System.Type propType, SerializedProperty valueProp)
        {
            if (propType == typeof(object))
            {
                //draw the default variant as the method accepts anything
                _variantDrawer.RestrictVariantType = false;
                _variantDrawer.ForcedComponentType = null;
                _variantDrawer.OnGUI(position, valueProp, label);
            }
            else
            {
                var argType = VariantReference.GetVariantType(propType);
                _variantDrawer.RestrictVariantType = true;
                _variantDrawer.VariantTypeRestrictedTo = argType;
                _variantDrawer.ForcedComponentType = (TypeUtil.IsType(propType, typeof(Component))) ? propType : null;
                _variantDrawer.OnGUI(position, valueProp, label);
            }
        }


        private static Rect CalcNextRect(ref Rect area)
        {
            var pos = new Rect(area.xMin, area.yMin + 1f, area.width, EditorGUIUtility.singleLineHeight);
            area = new Rect(pos.xMin, pos.yMax, area.width, area.height - EditorGUIUtility.singleLineHeight + 1f);
            return pos;
        }

        #endregion

    }

}
