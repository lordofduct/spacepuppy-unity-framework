using UnityEngine;
using UnityEditor;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(ShortGuid))]
    public class ShortGuidInspector : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative("_value");
            var str = prop.stringValue;
            if(string.IsNullOrEmpty(str))
            {
                str = ShortGuid.Encode(System.Guid.NewGuid());
                prop.stringValue = str;
            }
            EditorGUI.TextField(position, label, str);
        }

    }
}
