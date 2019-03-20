using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfModifier : PropertyModifier
    {
        
        protected internal override void OnBeforeGUI(SerializedProperty property, ref bool cancelDraw)
        {
            var attrib = this.attribute as DisableIfAttribute;
            var targ = EditorHelper.GetTargetObjectWithProperty(property);

            bool disable = false;
            if (attrib != null && targ != null)
            {
                disable = ConvertUtil.ToBool(DynamicUtil.GetValue(targ, attrib.MemberName));
                if (attrib.DisableIfNot) disable = !disable;
            }

            EditorGUI.BeginDisabledGroup(disable);
        }

        protected internal override void OnPostGUI(SerializedProperty property)
        {
            EditorGUI.EndDisabledGroup();
        }

    }

}
