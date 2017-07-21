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
            float w = Mathf.Min(position.width, 60f);
            var r2 = new Rect(position.xMax - w, position.yMin, w, position.height);
            var r1 = new Rect(position.xMin, position.yMin, Mathf.Max(position.width - w, 0f), position.height);

            var lowProp = property.FindPropertyRelative("_low");
            var highProp = property.FindPropertyRelative("_high");
            long value = (lowProp.longValue & uint.MaxValue) | (highProp.longValue << 32);

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(ShortUid.ConfigAttribute), false).FirstOrDefault() as ShortUid.ConfigAttribute;
            bool resetOnZero = attrib == null || !attrib.AllowZero;
            bool readWrite = attrib == null || !attrib.ReadOnly;

            if(readWrite)
            {
                //read-write
                EditorGUI.BeginChangeCheck();
                var sval = EditorGUI.TextField(r1, value.ToString("X16"));
                if (EditorGUI.EndChangeCheck())
                {
                    if (long.TryParse(sval, System.Globalization.NumberStyles.HexNumber, null, out value))
                    {
                        lowProp.longValue = (value & uint.MaxValue);
                        highProp.longValue = (value >> 32);
                    }
                }
            }
            else
            {
                //read-only
                EditorGUI.SelectableLabel(r1, value.ToString("X16"), EditorStyles.textField);
            }

            if (GUI.Button(r2, "New Id") || (resetOnZero && value == 0))
            {
                value = ShortUid.NewId().Value;
                lowProp.longValue = (value & uint.MaxValue);
                highProp.longValue = (value >> 32);
            }

            EditorGUI.EndProperty();
        }

    }

    /*
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
            float w = Mathf.Min(position.width, 60f);
            var r2 = new Rect(position.xMax - w, position.yMin, w, position.height);
            var r1 = new Rect(position.xMin, position.yMin, Mathf.Max(position.width - w, 0f), position.height);

            var idProp = property.FindPropertyRelative("_id");

            //read-write
            EditorGUI.BeginChangeCheck();
            var sval = EditorGUI.TextField(r1, idProp.longValue.ToString("X16"));
            if(EditorGUI.EndChangeCheck())
            {
                long val;
                if(long.TryParse(sval, System.Globalization.NumberStyles.HexNumber, null, out val))
                {
                    idProp.longValue = val;
                }
            }
            //read-only
            //EditorGUI.SelectableLabel(r1, idProp.longValue.ToString("X16"), EditorStyles.textField);

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(ShortUid.ConfigAttribute), false).FirstOrDefault() as ShortUid.ConfigAttribute;
            bool resetOnZero = attrib == null || !attrib.AllowZero;
            if (GUI.Button(r2, "New Id") || (resetOnZero && idProp.longValue == 0))
            {
                idProp.longValue = ShortUid.NewId().Value;
            }

            EditorGUI.EndProperty();
        }

    }
    */
}
