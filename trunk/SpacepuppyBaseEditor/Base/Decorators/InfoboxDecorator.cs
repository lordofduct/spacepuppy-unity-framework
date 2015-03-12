using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(InfoboxAttribute))]
    public class InfoboxDecorator : DecoratorDrawer
    {

        public override float GetHeight()
        {
            return InfoboxDecorator.GetHeight(this.attribute as InfoboxAttribute);
        }

        public override void OnGUI(Rect position)
        {
            InfoboxDecorator.OnGUI(position, this.attribute as InfoboxAttribute);
        }


        public static float GetHeight(InfoboxAttribute attrib)
        {
            GUIStyle style = GUI.skin.GetStyle("HelpBox");
            return Mathf.Max(40f, style.CalcHeight(new GUIContent(attrib.Message), EditorGUIUtility.currentViewWidth));
        }

        public static void OnGUI(Rect position, InfoboxAttribute attrib)
        {
            EditorGUI.HelpBox(position, attrib.Message, (MessageType)attrib.MessageType);
        }

    }
}
