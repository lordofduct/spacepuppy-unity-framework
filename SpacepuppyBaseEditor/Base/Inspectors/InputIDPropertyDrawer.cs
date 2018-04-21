using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(InputIDAttribute))]
    public class InputIDPropertyDrawer : PropertyDrawer
    {

        private string[] _inputIds;

        private void Init()
        {
            _inputIds = InputSettings.GetGlobalInputIds();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_inputIds == null) this.Init();

                var guiIds = (from s in _inputIds select EditorHelper.TempContent(s)).Append(EditorHelper.TempContent("Custom...")).ToArray();
                int index = System.Array.IndexOf(_inputIds, property.stringValue);
                if (index < 0) index = _inputIds.Length;
                if (index == _inputIds.Length)
                {
                    var fw = position.width - EditorGUIUtility.labelWidth;
                    var wl = EditorGUIUtility.labelWidth + (fw / 4f);
                    var wr = position.width - wl - 1f;

                    var rl = new Rect(position.xMin, position.yMin, wl, EditorGUIUtility.singleLineHeight);
                    var rr = new Rect(rl.xMax + 1f, rl.yMin, wr, EditorGUIUtility.singleLineHeight);

                    index = EditorGUI.Popup(rl, label, index, guiIds);
                    if (index >= 0 && index < _inputIds.Length)
                    {
                        property.stringValue = _inputIds[index];
                    }
                    else
                    {
                        property.stringValue = EditorGUI.TextField(rr, property.stringValue);
                    }
                }
                else
                {
                    index = EditorGUI.Popup(position, label, index, guiIds);
                    property.stringValue = (index >= 0 && index < _inputIds.Length) ? _inputIds[index] : null;
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
        
    }

}
