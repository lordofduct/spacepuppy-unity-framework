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

        private bool? _cached = null;

        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            if(Application.isPlaying)
            {
                _cached = GUI.enabled;
                GUI.enabled = false;
            }
            else
            {
                _cached = null;
            }
        }

        protected internal override void OnPostGUI(SerializedProperty property)
        {
            if(_cached != null)
            {
                GUI.enabled = _cached.Value;
            }
        }

    }

}
