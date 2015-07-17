using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(ComparisonOperator))]
    public class ComparisonOperatorPropertyDrawer : PropertyDrawer
    {

        private static GUIContent[] _names = new GUIContent[] {
            new GUIContent(">"),
            new GUIContent(">="),
            new GUIContent("<"),
            new GUIContent("<="),
            new GUIContent("=="),
            new GUIContent("!="),
            new GUIContent("Always")
        };
        private static ComparisonOperator[] _ops = new ComparisonOperator[] {
            ComparisonOperator.GreaterThan,
            ComparisonOperator.GreaterThan | ComparisonOperator.Equal,
            ComparisonOperator.LessThan,
            ComparisonOperator.LessThan | ComparisonOperator.Equal,
            ComparisonOperator.Equal,
            ComparisonOperator.NotEqual,
            ComparisonOperator.Always
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var e = ConvertUtil.ToEnum<ComparisonOperator>(property.intValue);
            if (e == ComparisonOperator.NotEqualAlt) e = ComparisonOperator.NotEqual;
            int i = _ops.IndexOf(e);

            i = EditorGUI.Popup(position, label, i, _names);
            property.intValue = (i >= 0) ? (int)(_ops[i]) : (int)ComparisonOperator.GreaterThan;
        }

    }

}
