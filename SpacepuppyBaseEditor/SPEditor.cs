using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;
using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor
{

    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects()]
    public class SPEditor : Editor
    {

        #region Fields

        private List<GUIDrawer> _headerDrawers;

        private List<ShownPropertyInfo> _shownFields;

        #endregion

        #region CONSTRUCTOR

        protected virtual void OnEnable()
        {
            var tp = this.target.GetType();
            var fields = tp.GetFields();
            foreach(var f in fields)
            {
                var attribs = f.GetCustomAttributes(typeof(ShowNonSerializedPropertyAttribute), false) as ShowNonSerializedPropertyAttribute[];
                if(attribs != null && attribs.Length > 0)
                {
                    if (_shownFields == null) _shownFields = new List<ShownPropertyInfo>();
                    var attrib = attribs[0];
                    _shownFields.Add(new ShownPropertyInfo()
                    {
                        Attrib = attrib,
                        FieldInfo = f,
                        Label = new GUIContent(attrib.Label ?? f.Name, attrib.Tooltip)
                    });
                }
            }
        }

        protected virtual void OnDisable()
        {

        }

        #endregion

        #region GUI Methods

        public sealed override void OnInspectorGUI()
        {
            if (!(this.target is SPComponent) && !EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_SPEDITOR_ISDEFAULT_ACTIVE, true))
            {
                base.OnInspectorGUI();
                return;
            }

            //draw header infobox if needed
            this.DrawDefaultInspectorHeader();

            EditorGUI.BeginChangeCheck();
            this.OnSPInspectorGUI();
            if(EditorGUI.EndChangeCheck())
            {
                //do call onValidate
                PropertyHandlerValidationUtility.OnInspectorGUIComplete(this.serializedObject, true);
                this.OnValidate();
            }
            else
            {
                PropertyHandlerValidationUtility.OnInspectorGUIComplete(this.serializedObject, false);
            }

            if(_shownFields != null && UnityEngine.Application.isPlaying)
            {
                GUILayout.Label("Runtime Values", EditorStyles.boldLabel);

                foreach(var info in _shownFields)
                {
                    GUI.enabled = !info.Attrib.Readonly;

                    var value = info.FieldInfo.GetValue(this.target);
                    EditorGUI.BeginChangeCheck();
                    value = SPEditorGUILayout.DefaultPropertyField(info.Label, value, info.FieldInfo.FieldType);
                    if(EditorGUI.EndChangeCheck())
                    {
                        info.FieldInfo.SetValue(this.target, value);
                    }

                    GUI.enabled = true;
                }
            }
        }

        protected virtual void OnSPInspectorGUI()
        {
            this.DrawDefaultInspector();
        }

        protected virtual void OnValidate()
        {

        }

        private void DrawDefaultInspectorHeader()
        {
            //var attribs = this.serializedObject.targetObject.GetType().GetCustomAttributes(typeof(InfoboxAttribute), false);
            //InfoboxAttribute infoboxAttrib = (attribs.Length > 0) ? attribs[0] as InfoboxAttribute : null;
            //if (infoboxAttrib != null)
            //{
            //    var position = EditorGUILayout.GetControlRect(false, com.spacepuppyeditor.Decorators.InfoboxDecorator.GetHeight(infoboxAttrib));
            //    com.spacepuppyeditor.Decorators.InfoboxDecorator.OnGUI(position, infoboxAttrib);
            //}

            if (_headerDrawers == null)
            {
                _headerDrawers = new List<GUIDrawer>();
                var componentType = serializedObject.targetObject.GetType();
                if(TypeUtil.IsType(componentType, typeof(Component)))
                {
                    var attribs = (from o in componentType.GetCustomAttributes(typeof(ComponentHeaderAttribute), true) 
                                   let a = o as ComponentHeaderAttribute 
                                   where a != null 
                                   orderby a.order 
                                   select a).ToArray();
                    foreach (var attrib in attribs)
                    {
                        var dtp = ScriptAttributeUtility.GetDrawerTypeForType(attrib.GetType());
                        if (dtp != null)
                        {
                            if(TypeUtil.IsType(dtp, typeof(DecoratorDrawer)))
                            {
                                var decorator = System.Activator.CreateInstance(dtp) as DecoratorDrawer;
                                DynamicUtil.SetValue(decorator, "m_Attribute", attrib);
                                _headerDrawers.Add(decorator);
                            }
                            else if (TypeUtil.IsType(dtp, typeof(ComponentHeaderDrawer)))
                            {
                                var drawer = System.Activator.CreateInstance(dtp) as ComponentHeaderDrawer;
                                drawer.Init(attrib, componentType);
                                _headerDrawers.Add(drawer);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < _headerDrawers.Count; i++)
            {
                var drawer = _headerDrawers[i];
                if(drawer is DecoratorDrawer)
                {
                    var decorator = drawer as DecoratorDrawer;
                    var h = decorator.GetHeight();
                    Rect position = EditorGUILayout.GetControlRect(false, h);
                    decorator.OnGUI(position);
                }
                else if (drawer is ComponentHeaderDrawer)
                {
                    var compDrawer = drawer as ComponentHeaderDrawer;
                    var h = compDrawer.GetHeight(this.serializedObject);
                    Rect position = EditorGUILayout.GetControlRect(false, h);
                    compDrawer.OnGUI(position, this.serializedObject);
                }
            }
        }


        public override bool RequiresConstantRepaint()
        {
            return base.RequiresConstantRepaint() || (Application.isPlaying && _shownFields != null);
        }

        #endregion

        #region Draw Methods

        /// <summary>
        /// Draw the inspector as it would have been if not an SPEditor.
        /// </summary>
        public void DrawDefaultStandardInspector()
        {
            base.DrawDefaultInspector();
        }

        public new bool DrawDefaultInspector()
        {
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

        public bool DrawPropertyField(string prop, GUIContent label, bool includeChildren)
        {
            return SPEditorGUILayout.PropertyField(this.serializedObject, prop, label, includeChildren);
        }

        #endregion

        #region Static Interface

        public static bool DrawDefaultInspectorExcept(SerializedObject serializedObject, params string[] propsNotToDraw)
        {
            if (serializedObject == null) throw new System.ArgumentNullException("serializedObject");

            EditorGUI.BeginChangeCheck();
            var iterator = serializedObject.GetIterator();
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



        #region Special Types

        private class ShownPropertyInfo
        {

            public ShowNonSerializedPropertyAttribute Attrib;
            public System.Reflection.FieldInfo FieldInfo;
            public GUIContent Label;

        }

        #endregion

    }

}
