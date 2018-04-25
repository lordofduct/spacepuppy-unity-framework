using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Tween;
using com.spacepuppy.Tween.Accessors;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;
using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_TweenValue))]
    public class i_TweenValueInspector : SPEditor
    {
        public const string PROP_ORDER = "_order";
        public const string PROP_ACTIVATEON = "_activateOn";

        public const string PROP_TIMESUPPLIER = "_timeSupplier";
        public const string PROP_TARGET = "_target";
        public const string PROP_TWEENDATA = "_data";
        public const string PROP_ONCOMPLETE = "_onComplete";
        public const string PROP_TWEENTOKEN = "_tweenToken";

        private const string PROP_DATA_MODE = "Mode";
        private const string PROP_DATA_MEMBER = "MemberName";
        private const string PROP_DATA_EASE = "Ease";
        private const string PROP_DATA_VALUES = "ValueS";
        private const string PROP_DATA_VALUEE = "ValueE";
        private const string PROP_DATA_DUR = "Duration";
        private const string PROP_DATA_OPTION = "Option";


        private SPReorderableList _dataList;
        private SerializedProperty _targetProp;
        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        protected override void OnEnable()
        {
            base.OnEnable();

            _dataList = new SPReorderableList(this.serializedObject, this.serializedObject.FindProperty(PROP_TWEENDATA));
            _dataList.drawHeaderCallback = _dataList_DrawHeader;
            _dataList.drawElementCallback = _dataList_DrawElement;
            _dataList.elementHeight = EditorGUIUtility.singleLineHeight * 7f + 7f;
            
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            _targetProp = this.serializedObject.FindProperty(PROP_TARGET);

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_ORDER);
            this.DrawPropertyField(PROP_ACTIVATEON);
            this.DrawPropertyField(PROP_TIMESUPPLIER);
            SPEditorGUILayout.PropertyField(_targetProp);
            this.DrawPropertyField(PROP_TWEENTOKEN);
            _dataList.DoLayoutList();
            this.DrawPropertyField(PROP_ONCOMPLETE);


            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_ORDER, PROP_ACTIVATEON, PROP_TARGET, PROP_TIMESUPPLIER, PROP_TWEENDATA, PROP_ONCOMPLETE, PROP_TWEENTOKEN);

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
            System.Type propType;
            memberProp.stringValue = i_TweenValueInspector.ReflectedPropertyAndCustomTweenAccessorField(position,
                                                                                                        EditorHelper.TempContent("Property", "The property on the target to set."),
                                                                                                        _targetProp.objectReferenceValue,
                                                                                                        memberProp.stringValue,
                                                                                                        com.spacepuppy.Dynamic.DynamicMemberAccess.ReadWrite,
                                                                                                        out propType);

            position = CalcNextRect(ref area);
            SPEditorGUI.PropertyField(position, el.FindPropertyRelative(PROP_DATA_EASE));
            
            position = CalcNextRect(ref area);
            this.DrawOption(position, ref propType, el.FindPropertyRelative(PROP_DATA_OPTION));
            
            position = CalcNextRect(ref area);
            SPEditorGUI.PropertyField(position, el.FindPropertyRelative(PROP_DATA_DUR));

            if(propType != null)
            {
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
            if (com.spacepuppy.Dynamic.DynamicUtil.TypeIsVariantSupported(propType))
            {
                //draw the default variant as the method accepts anything
                _variantDrawer.RestrictVariantType = false;
                _variantDrawer.ForcedObjectType = null;
                _variantDrawer.OnGUI(position, valueProp, label);
            }
            else
            {
                _variantDrawer.RestrictVariantType = true;
                _variantDrawer.TypeRestrictedTo = propType;
                _variantDrawer.ForcedObjectType = (TypeUtil.IsType(propType, typeof(Component))) ? propType : null;
                _variantDrawer.OnGUI(position, valueProp, label);
            }
        }


        private static Rect CalcNextRect(ref Rect area)
        {
            var pos = new Rect(area.xMin, area.yMin + 1f, area.width, EditorGUIUtility.singleLineHeight);
            area = new Rect(pos.xMin, pos.yMax, area.width, area.height - EditorGUIUtility.singleLineHeight + 1f);
            return pos;
        }

        private void DrawOption(Rect position, ref System.Type propType, SerializedProperty optionProp)
        {
            if(propType == typeof(Vector2) || propType == typeof(Vector3) || propType == typeof(Vector4) || propType == typeof(Color))
            {
                bool value = ConvertUtil.ToBool(optionProp.intValue);
                value = EditorGUI.Toggle(position, "Option (Use Slerp)", value);
                optionProp.intValue = value ? 1 : 0;
            }
            else if(propType == typeof(Quaternion))
            {
                QuaternionTweenOption value = QuaternionTweenOption.Spherical;
                if (System.Enum.IsDefined(typeof(QuaternionTweenOption), optionProp.intValue))
                    value = (QuaternionTweenOption)optionProp.intValue;
                
                value = (QuaternionTweenOption)EditorGUI.EnumPopup(position, "Option", value);
                optionProp.intValue = (int)value;
                if (value == QuaternionTweenOption.Long)
                    propType = typeof(Vector3);
            }
            else
            {
                EditorGUI.LabelField(position, "Option", "(no option available)");
            }
        }

        #endregion


        #region Custom Reflected PropertyField

        public static string ReflectedPropertyAndCustomTweenAccessorField(Rect position, GUIContent label, object targObj, string selectedMemberName, DynamicMemberAccess access, out System.Type propType)
        {
            if (targObj != null)
            {
                var members = DynamicUtil.GetEasilySerializedMembers(targObj, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, access).ToArray();
                var accessors = CustomTweenMemberAccessorFactory.GetCustomAccessorIds(targObj.GetType(), (d) => VariantReference.AcceptableType(d.MemberType));
                System.Array.Sort(accessors);

                using (var entries = TempCollection.GetList<GUIContent>(members.Length))
                {
                    int index = -1;
                    for (int i = 0; i < members.Length; i++)
                    {
                        var m = members[i];
                        if ((DynamicUtil.GetMemberAccessLevel(m) & DynamicMemberAccess.Write) != 0)
                            entries.Add(EditorHelper.TempContent(string.Format("{0} ({1}) -> {2}", m.Name, DynamicUtil.GetReturnType(m).Name, DynamicUtil.GetValueWithMember(m, targObj))));
                        else
                            entries.Add(EditorHelper.TempContent(string.Format("{0} (readonly - {1}) -> {2}", m.Name, DynamicUtil.GetReturnType(m).Name, DynamicUtil.GetValueWithMember(m, targObj))));

                        if (index < 0 && m.Name == selectedMemberName)
                        {
                            //index = i;
                            index = entries.Count - 1;
                        }
                    }

                    for(int i = 0; i < accessors.Length; i++)
                    {
                        entries.Add(EditorHelper.TempContent(accessors[i]));
                        if(index < 0 && accessors[i] == selectedMemberName)
                        {
                            index = entries.Count - 1;
                        }
                    }

                    
                    index = EditorGUI.Popup(position, label, index, entries.ToArray());
                    //selectedMember = (index >= 0) ? members[index] : null;
                    //return (selectedMember != null) ? selectedMember.Name : null;

                    if(index < 0)
                    {
                        propType = null;
                        return null;
                    }
                    else if (index < members.Length)
                    {
                        propType = DynamicUtil.GetReturnType(members[index]);
                        return members[index].Name;
                    }
                    else
                    {
                        var nm = accessors[index - members.Length];
                        /*
                        ITweenMemberAccessor acc;
                        if (CustomTweenMemberAccessorFactory.TryGetMemberAccessor(targObj, nm, out acc))
                        {
                            propType = acc.GetMemberType();
                            if (VariantReference.AcceptableType(propType))
                            {
                                return nm;
                            }
                        }
                        */
                        CustomTweenMemberAccessorFactory.CustomAccessorData info;
                        if(CustomTweenMemberAccessorFactory.TryGetMemberAccessorInfo(targObj, nm, out info))
                        {
                            propType = info.MemberType;
                            if(VariantReference.AcceptableType(propType))
                            {
                                return nm;
                            }
                        }
                    }

                    propType = null;
                    return null;
                }
            }
            else
            {
                propType = null;
                EditorGUI.Popup(position, label, -1, new GUIContent[0]);
                return null;
            }
        }

        #endregion


    }

}
