using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppyeditor.Internal
{
    internal interface IPropertyHandler
    {

        #region Properties

        #endregion

        #region Methods

        bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren);

        bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options);

        #endregion

    }

}
