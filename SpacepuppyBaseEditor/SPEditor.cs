using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    //[CustomEditor(typeof(SPComponent), true)]
    public class SPEditor : Editor
    {

        #region Fields

        #endregion

        #region CONSTRUCTOR

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        #endregion

        #region Methods

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();
        }




        /// <summary>
        /// Draw the inspector as it would have been if not an SPEditor.
        /// </summary>
        public void DrawDefaultStandardInspector()
        {
            base.DrawDefaultInspector();
        }

        public new void DrawDefaultInspector()
        {
            this.serializedObject.Update();
            SerializedProperty iterator = this.serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                //TODO - switch over to SPEditorGUILayout to implement our own custom 'IPropertyHandler'
                //SPEditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }
            this.serializedObject.ApplyModifiedProperties();
        }

        #endregion

    }

}
