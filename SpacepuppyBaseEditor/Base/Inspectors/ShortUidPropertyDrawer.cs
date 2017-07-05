using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{


    [CustomPropertyDrawer(typeof(ShortUid))]
    public class ShortUidPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            var r2 = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, 75f), position.height);
            var r1 = new Rect(position.xMin, position.yMin, Mathf.Max(position.width - r2.width, 0f), position.height);

            var idProp = property.FindPropertyRelative("_id");
            EditorGUI.LabelField(r1, idProp.longValue.ToString("X16"));
            if (GUI.Button(r2, "New Id"))
            {
                idProp.longValue = ShortUid.NewId().Value;
            }

            EditorGUI.EndProperty();
        }

    }
}
