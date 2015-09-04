using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(SPTime))]
    public class SPTimePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            TimePeriodPropertyDrawer.DrawTimeSupplier(position, property, position.width, null); //we mirror the TimePeriod prop drawer, we can do this because the property names are identical
        }

    }

}
