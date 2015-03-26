using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Modifiers;

namespace com.spacepuppyeditor.Internal
{
    internal class SPPropertyAttributePropertyHandler : IPropertyHandler
    {

        #region Static PollUsedHandlersForSerializedObject

        private struct HandlerInfo
        {
            public string propPath;
            public SPPropertyAttributePropertyHandler handler;

            public HandlerInfo(string path, SPPropertyAttributePropertyHandler handler)
            {
                this.propPath = path;
                this.handler = handler;
            }
        }

        private static ListDictionary<int, HandlerInfo> _usedHandlers = new ListDictionary<int, HandlerInfo>();

        internal static void OnInspectorGUIComplete(SerializedObject obj, bool validate)
        {
            if (obj == null || obj.targetObject == null) return;

            int id = obj.targetObject.GetInstanceID();
            if (_usedHandlers.ContainsKey(id))
            {
                var lst = _usedHandlers.Lists[id];
                if(validate)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var info = lst[i];
                        if (info.handler._visibleDrawer is PropertyModifier)
                        {
                            var prop = obj.FindProperty(info.propPath);
                            (info.handler._visibleDrawer as PropertyModifier).OnValidate(prop);
                        }
                    }
                }
                lst.Clear();
            }
        }

        private static void AddAsHandled(SerializedProperty property, SPPropertyAttributePropertyHandler handler)
        {
            if (property.serializedObject.targetObject != null)
            {
                _usedHandlers.Add(property.serializedObject.targetObject.GetInstanceID(), new HandlerInfo(property.propertyPath, handler));
            }
        }

        private static System.DateTime _lastPurge;
        private static System.TimeSpan OLD_AGE = System.TimeSpan.FromMinutes(1.0);
        private static void OnGUIHandler(SceneView view)
        {
            if (System.DateTime.Now - _lastPurge > OLD_AGE)
            {
                _usedHandlers.Purge();
            }

            foreach(var lst in _usedHandlers.Lists)
            {
                lst.Clear();
            }
        }

        static SPPropertyAttributePropertyHandler()
        {
            _lastPurge = System.DateTime.Now;
            SceneView.onSceneGUIDelegate -= OnGUIHandler;
            SceneView.onSceneGUIDelegate += OnGUIHandler;
        }

        #endregion



        #region Fields

        private System.Reflection.FieldInfo _fieldInfo;
        private SPPropertyAttribute[] _attribs;
        private PropertyDrawer _visibleDrawer;

        #endregion

        #region CONSTRUCTOR

        public SPPropertyAttributePropertyHandler(System.Reflection.FieldInfo fieldInfo, SPPropertyAttribute[] attribs)
        {
            _fieldInfo = fieldInfo;
            _attribs = attribs;
        }

        #endregion

        #region Methods

        private void Init()
        {
            var dtp = ScriptAttributeUtility.GetDrawerTypeForType(_attribs[0].GetType());
            var drawer = PropertyDrawerActivator.Create(dtp, _attribs[0], _fieldInfo);
            if (drawer is PropertyModifier) (drawer as PropertyModifier).Init(true);
            _visibleDrawer = drawer;
        }

        #endregion

        #region IPropertyHandler Interface

        public float GetHeight(SerializedProperty property, GUIContent label)
        {
            if (_visibleDrawer == null) this.Init();

            return _visibleDrawer.GetPropertyHeight(property, label);
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (_visibleDrawer == null) this.Init();

            _visibleDrawer.OnGUI(position, property, label);
            SPPropertyAttributePropertyHandler.AddAsHandled(property, this);
            return false;
        }

        public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            if (_visibleDrawer == null) this.Init();

            if (label == null) label = EditorHelper.TempContent(property.displayName);
            var rect = EditorGUILayout.GetControlRect(true, _visibleDrawer.GetPropertyHeight(property, label), options);
            _visibleDrawer.OnGUI(rect, property, label);
            SPPropertyAttributePropertyHandler.AddAsHandled(property, this);
            return false;
        }

        #endregion

    }
}
