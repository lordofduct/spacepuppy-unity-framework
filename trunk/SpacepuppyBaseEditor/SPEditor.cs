using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor
{

    [CustomEditor(typeof(SPComponent), true)]
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

        public new bool DrawDefaultInspector()
        {
            //draw header infobox if needed
            var attribs = this.serializedObject.targetObject.GetType().GetCustomAttributes(typeof(InfoboxAttribute), false);
            InfoboxAttribute infoboxAttrib = (attribs.Length > 0) ? attribs[0] as InfoboxAttribute : null;
            if (infoboxAttrib != null)
            {
                var position = EditorGUILayout.GetControlRect(false, com.spacepuppyeditor.Decorators.InfoboxDecorator.GetHeight(infoboxAttrib));
                com.spacepuppyeditor.Decorators.InfoboxDecorator.OnGUI(position, infoboxAttrib);
            }

            //draw properties
            this.serializedObject.Update();
            var result = SPEditor.DrawDefaultInspectorExcept(this.serializedObject);
            this.serializedObject.ApplyModifiedProperties();

            return result;
        }

        public void DrawDefaultInspectorExcept(params string[] propsNotToDraw)
        {
            DrawDefaultInspectorExcept(this.serializedObject, propsNotToDraw);
        }

        public bool DrawPropertyField(string prop)
        {
            return SPEditorGUILayout.PropertyField(this.serializedObject, prop);
        }

        public bool DrawPropertyField(string prop, bool includeChildren)
        {
            return SPEditorGUILayout.PropertyField(this.serializedObject, prop, includeChildren);
        }

        public bool DrawPropertyField(string prop, string label, bool includeChildren)
        {
            return SPEditorGUILayout.PropertyField(this.serializedObject, prop, label, includeChildren);
        }

        public bool DrawPropertyField(string prop, GUIContent content, bool includeChildren)
        {
            return SPEditorGUILayout.PropertyField(this.serializedObject, prop, content, includeChildren);
        }

        #endregion



        #region Static Interface

        public static bool DrawDefaultInspectorExcept(SerializedObject obj, params string[] propsNotToDraw)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            EditorGUI.BeginChangeCheck();
            var iterator = obj.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (propsNotToDraw == null || !propsNotToDraw.Contains(iterator.name))
                {
                    //EditorGUILayout.PropertyField(iterator, true);
                    SPEditorGUILayout.PropertyField(iterator, true);
                }
            }
            return EditorGUI.EndChangeCheck();
        }

        #endregion

    }

}
