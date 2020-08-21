using UnityEngine;
using UnityEditor;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(System.Enum), true)]
    public class EnumPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tp = this.fieldInfo.FieldType;
            if (TypeUtil.IsListType(tp)) tp = TypeUtil.GetElementTypeOfListType(tp);
            if (!tp.IsEnum)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            System.Enum e = property.GetEnumValue(tp);
            e = SPEditorGUI.EnumPopup(position, label, e);
            property.SetEnumValue(e);
        }

    }

}
