using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Render;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Render
{

    [CustomPropertyDrawer(typeof(MaterialTransition))]
    public class MaterialTransitionPropertyDrawer : PropertyDrawer
    {

        public const string PROP_MATERIAL = "_material";
        public const string PROP_VALUETYPE = "_valueType";
        public const string PROP_PROPERTYNAME = "_propertyName";
        public const string PROP_VALUESTART = "_valueStart";
        public const string PROP_VALUEEND = "_valueEnd";
        public const string PROP_POSITION = "_position";


        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer() { RestrictVariantType = true };


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight * 4.5f;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            this.DrawMaterialLine(r0, property, label);

            if(property.isExpanded)
            {
                EditorGUI.indentLevel++;
                var r1 = new Rect(position.xMin, position.yMin + EditorGUIUtility.singleLineHeight * 1f, position.width, EditorGUIUtility.singleLineHeight);
                var r2 = new Rect(position.xMin, position.yMin + EditorGUIUtility.singleLineHeight * 2f, position.width, EditorGUIUtility.singleLineHeight);
                var r3 = new Rect(position.xMin, position.yMin + EditorGUIUtility.singleLineHeight * 3f, position.width, EditorGUIUtility.singleLineHeight);

                var startProp = property.FindPropertyRelative(PROP_VALUESTART);
                var endProp = property.FindPropertyRelative(PROP_VALUEEND);
                var posProp = property.FindPropertyRelative(PROP_POSITION);

                _variantDrawer.VariantTypeRestrictedTo = GetVariantType(property.FindPropertyRelative(PROP_VALUETYPE).GetEnumValue<MaterialTransition.MatValueType>());

                _variantDrawer.OnGUI(r1, startProp, EditorHelper.TempContent("Start Value"));
                _variantDrawer.OnGUI(r2, endProp, EditorHelper.TempContent("End Value"));
                EditorGUI.PropertyField(r3, posProp);

                EditorGUI.indentLevel--;
            }
        }

        private void DrawMaterialLine(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, 15f, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);
            position = EditorGUI.PrefixLabel(position, label);


            var r0 = new Rect(position.xMin, position.yMin, position.width / 2f, position.height);
            var r1 = new Rect(r0.xMax, r0.yMin, position.width - r0.width, r0.height);

            var matProp = property.FindPropertyRelative(PROP_MATERIAL);
            var valTypeProp = property.FindPropertyRelative(PROP_VALUETYPE);
            var propNameProp = property.FindPropertyRelative(PROP_PROPERTYNAME);

            matProp.objectReferenceValue = EditorGUI.ObjectField(r1, matProp.objectReferenceValue, typeof(Material), true);

            var mat = matProp.objectReferenceValue as Material;
            if(mat != null && mat.shader != null)
            {
                int cnt = ShaderUtil.GetPropertyCount(mat.shader);
                var infoLst = TempCollection<PropInfo>.GetCollection(cnt);
                var contentLst = TempCollection<GUIContent>.GetCollection(cnt);
                int index = -1;

                for(int i = 0; i < cnt; i++)
                {
                    var nm = ShaderUtil.GetPropertyName(mat.shader, i);
                    var tp = ShaderUtil.GetPropertyType(mat.shader, i);
                    
                    switch(tp)
                    {
                        case ShaderUtil.ShaderPropertyType.Float:
                            if (propNameProp.stringValue == nm) index = infoLst.Count;
                            infoLst.Add(new PropInfo(nm, MaterialTransition.MatValueType.Float));
                            contentLst.Add(EditorHelper.TempContent(nm + " (float)"));
                            break;
                        case ShaderUtil.ShaderPropertyType.Range:
                            if (propNameProp.stringValue == nm) index = infoLst.Count;
                            infoLst.Add(new PropInfo(nm, MaterialTransition.MatValueType.Float));
                            var min = ShaderUtil.GetRangeLimits(mat.shader, i, 1);
                            var max = ShaderUtil.GetRangeLimits(mat.shader, i, 2);
                            contentLst.Add(EditorHelper.TempContent(string.Format("{0} (Range [{1}, {2}]])", nm, min, max)));
                            break;
                        case ShaderUtil.ShaderPropertyType.Color:
                            if (propNameProp.stringValue == nm) index = infoLst.Count;
                            infoLst.Add(new PropInfo(nm, MaterialTransition.MatValueType.Color));
                            contentLst.Add(EditorHelper.TempContent(nm + " (color)"));
                            break;
                        case ShaderUtil.ShaderPropertyType.Vector:
                            if (propNameProp.stringValue == nm) index = infoLst.Count;
                            infoLst.Add(new PropInfo(nm, MaterialTransition.MatValueType.Vector));
                            contentLst.Add(EditorHelper.TempContent(nm + " (vector)"));
                            break;
                    }
                }

                index = EditorGUI.Popup(r0, index, contentLst.ToArray());

                if(index < 0)
                {
                    valTypeProp.SetEnumValue(MaterialTransition.MatValueType.Float);
                    propNameProp.stringValue = string.Empty;
                }
                else
                {
                    var info = infoLst[index];
                    valTypeProp.SetEnumValue(info.MatType);
                    propNameProp.stringValue = info.Name;
                }

                infoLst.Release();
                contentLst.Release();
            }

        }





        private struct PropInfo
        {
            public string Name;
            public MaterialTransition.MatValueType MatType;

            public PropInfo(string nm, MaterialTransition.MatValueType tp)
            {
                Name = nm;
                MatType = tp;
            }
        }

        private static VariantType GetVariantType(MaterialTransition.MatValueType etp)
        {
            switch(etp)
            {
                case MaterialTransition.MatValueType.Float:
                    return VariantType.Float;
                case MaterialTransition.MatValueType.Color:
                    return VariantType.Color;
                case MaterialTransition.MatValueType.Vector:
                    return VariantType.Vector4;
                default:
                    return VariantType.Float;
            }
        }

    }
}
