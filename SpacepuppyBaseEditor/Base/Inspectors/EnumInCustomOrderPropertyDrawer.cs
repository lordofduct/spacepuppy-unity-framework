using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(EnumInCustomOrderAttribute))]
    public class EnumInCustomOrderPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumType = this.fieldInfo.FieldType;
            if (!enumType.IsEnum)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            System.Enum evalue = property.GetEnumValue(enumType);
            var attrib = this.attribute as EnumInCustomOrderAttribute;
            if(attrib != null && attrib.customOrder != null)
            {
                var values = attrib.customOrder.Select(i => EditorHelper.TempContent(EnumUtil.GetFriendlyName(System.Enum.ToObject(enumType, i) as System.Enum))).ToArray();
                int index = System.Array.IndexOf(attrib.customOrder, property.intValue);
                index = EditorGUI.Popup(position, label, index, values);
                property.intValue = index >= 0 && index < attrib.customOrder.Length ? attrib.customOrder[index] : -1;
            }
            else
            {
                property.SetEnumValue(EditorGUI.EnumPopup(position, label, evalue));
            }
        }

    }

}
