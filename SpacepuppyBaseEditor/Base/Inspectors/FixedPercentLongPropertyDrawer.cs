using UnityEngine;
using UnityEditor;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(FixedPercentLong))]
    public class FixedPercentLongPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = EditorHelper.GetTargetObjectWithProperty(property);
            FixedPercentLong value = (FixedPercentLong)this.fieldInfo.GetValue(obj);

            EditorGUI.BeginChangeCheck();
            value = (FixedPercentLong)EditorGUI.DoubleField(position, label, (double)value);
            if(EditorGUI.EndChangeCheck())
                this.fieldInfo.SetValue(obj, value);
        }

    }
}
