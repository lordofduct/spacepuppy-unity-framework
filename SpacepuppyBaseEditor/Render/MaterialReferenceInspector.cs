using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Render;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Render
{
    [CustomPropertyDrawer(typeof(MaterialReference))]
    public class MaterialReferenceInspector : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var matProp = property.FindPropertyRelative("_material");
            var typeProp = property.FindPropertyRelative("_type");


            var r1 = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, 60f), position.height);
            var r2 = new Rect(r1.xMax, position.yMin, position.width - r1.width, position.height);

            var e = typeProp.GetEnumValue<MaterialReference.MaterialType>();
            e = (MaterialReference.MaterialType)EditorGUI.EnumPopup(r1, e);
            typeProp.SetEnumValue(e);
            switch(e)
            {
                case MaterialReference.MaterialType.Config:
                    {
                        matProp.objectReferenceValue = EditorGUI.ObjectField(r2, matProp.objectReferenceValue, typeof(Material), true);
                    }
                    break;
                case MaterialReference.MaterialType.Skybox:
                    {
                        matProp.objectReferenceValue = null;
                        GUI.enabled = false;
                        EditorGUI.ObjectField(r2, null, typeof(Material), false);
                        GUI.enabled = true;
                    }
                    break;
            }

        }

    }
}
