using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Render;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;
using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Render
{

    [CustomPropertyDrawer(typeof(MaterialPropertyReference))]
    public class MaterialPropertyReferencePropertyDrawer : PropertyDrawer
    {

        public const string PROP_MATERIAL = "_material";
        public const string PROP_USESHARED = "_useSharedMaterial";
        public const string PROP_VALUETYPE = "_valueType";
        public const string PROP_PROPERTYNAME = "_propertyName";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, 15f, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);
            position = EditorGUI.PrefixLabel(position, label);


            var r0 = new Rect(position.xMin, position.yMin, position.width / 2f, position.height);
            var r1 = new Rect(r0.xMax, r0.yMin, position.width - r0.width, r0.height);

            var matProp = property.FindPropertyRelative(PROP_MATERIAL);
            var useSharedProp = property.FindPropertyRelative(PROP_USESHARED);
            var valTypeProp = property.FindPropertyRelative(PROP_VALUETYPE);
            var propNameProp = property.FindPropertyRelative(PROP_PROPERTYNAME);


            //matProp.objectReferenceValue = EditorGUI.ObjectField(r1, matProp.objectReferenceValue, typeof(Material), true);
            EditorGUI.BeginChangeCheck();
            matProp.objectReferenceValue = EditorGUI.ObjectField(r1, matProp.objectReferenceValue, typeof(UnityEngine.Object), true);
            if (EditorGUI.EndChangeCheck())
            {
                if (!MaterialUtil.IsMaterialSource(matProp.objectReferenceValue))
                {
                    var go = GameObjectUtil.GetGameObjectFromSource(matProp.objectReferenceValue);
                    matProp.objectReferenceValue = (go != null) ? go.GetComponent<Renderer>() : null;
                }
            }

            var mat = MaterialUtil.GetMaterialFromSource(matProp.objectReferenceValue, useSharedProp.boolValue);
            if (mat != null && mat.shader != null)
            {
                int cnt = ShaderUtil.GetPropertyCount(mat.shader);
                using (var infoLst = TempCollection.GetList<PropInfo>(cnt))
                using (var contentLst = TempCollection.GetList<GUIContent>(cnt))
                {
                    int index = -1;

                    for (int i = 0; i < cnt; i++)
                    {
                        var nm = ShaderUtil.GetPropertyName(mat.shader, i);
                        var tp = ShaderUtil.GetPropertyType(mat.shader, i);

                        switch (tp)
                        {
                            case ShaderUtil.ShaderPropertyType.Float:
                                if (propNameProp.stringValue == nm) index = infoLst.Count;
                                infoLst.Add(new PropInfo(nm, MaterialPropertyValueType.Float));
                                contentLst.Add(EditorHelper.TempContent(nm + " (float)"));
                                break;
                            case ShaderUtil.ShaderPropertyType.Range:
                                if (propNameProp.stringValue == nm) index = infoLst.Count;
                                infoLst.Add(new PropInfo(nm, MaterialPropertyValueType.Float));
                                var min = ShaderUtil.GetRangeLimits(mat.shader, i, 1);
                                var max = ShaderUtil.GetRangeLimits(mat.shader, i, 2);
                                contentLst.Add(EditorHelper.TempContent(string.Format("{0} (Range [{1}, {2}]])", nm, min, max)));
                                break;
                            case ShaderUtil.ShaderPropertyType.Color:
                                if (propNameProp.stringValue == nm) index = infoLst.Count;
                                infoLst.Add(new PropInfo(nm, MaterialPropertyValueType.Color));
                                contentLst.Add(EditorHelper.TempContent(nm + " (color)"));
                                break;
                            case ShaderUtil.ShaderPropertyType.Vector:
                                if (propNameProp.stringValue == nm) index = infoLst.Count;
                                infoLst.Add(new PropInfo(nm, MaterialPropertyValueType.Vector));
                                contentLst.Add(EditorHelper.TempContent(nm + " (vector)"));
                                break;
                        }
                    }

                    index = EditorGUI.Popup(r0, index, contentLst.ToArray());

                    if (index < 0)
                    {
                        valTypeProp.SetEnumValue(MaterialPropertyValueType.Float);
                        propNameProp.stringValue = string.Empty;
                    }
                    else
                    {
                        var info = infoLst[index];
                        valTypeProp.SetEnumValue(info.MatType);
                        propNameProp.stringValue = info.Name;
                    }
                }
            }
        }







        private struct PropInfo
        {
            public string Name;
            public MaterialPropertyValueType MatType;

            public PropInfo(string nm, MaterialPropertyValueType tp)
            {
                Name = nm;
                MatType = tp;
            }
        }

    }

}
