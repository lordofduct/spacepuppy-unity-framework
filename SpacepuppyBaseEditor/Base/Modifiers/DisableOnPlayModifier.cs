using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(DisableOnPlayAttribute))]
    public class DisableOnPlayModifier : PropertyModifier
    {
        
        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
            }
        }

        protected internal override void OnPostGUI(SerializedProperty property)
        {
            if (Application.isPlaying)
            {
                EditorGUI.EndDisabledGroup();
            }
        }

    }

}
