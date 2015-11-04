using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppyeditor
{
    public static class SPEditorStyles
    {



        public static GUIStyle GetStyle(string styleName)
        {
            GUIStyle style = GUI.skin.FindStyle(styleName) ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if(style == null)
            {
                style = new GUIStyle();
            }
            return style;
        }

    }
}
