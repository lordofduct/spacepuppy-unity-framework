using UnityEngine;
using UnityEditor;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(FixedPercentDecimal))]
    public class FixedPercentDecimalPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = EditorHelper.GetTargetObjectWithProperty(property);
            FixedPercentDecimal value = (FixedPercentDecimal)this.fieldInfo.GetValue(obj);

            EditorGUI.BeginChangeCheck();
            value = (FixedPercentDecimal)EditorGUI.DoubleField(position, label, (double)value);
            if (EditorGUI.EndChangeCheck())
                this.fieldInfo.SetValue(obj, value);
        }

    }
}
