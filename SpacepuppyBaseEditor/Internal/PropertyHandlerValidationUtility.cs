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
    internal class PropertyHandlerValidationUtility
    {

        private struct HandlerInfo
        {
            public string propPath;
            public IPropertyHandler handler;

            public HandlerInfo(string path, IPropertyHandler handler)
            {
                this.propPath = path;
                this.handler = handler;
            }
        }

        #region Fields

        private static ListDictionary<int, HandlerInfo> _usedHandlers = new ListDictionary<int, HandlerInfo>();

        #endregion

        #region CONSTRUCTOR

        static PropertyHandlerValidationUtility()
        {
            _lastPurge = System.DateTime.Now;
            SceneView.onSceneGUIDelegate -= OnGUIHandler;
            SceneView.onSceneGUIDelegate += OnGUIHandler;
        }

        #endregion

        #region Methods

        internal static void OnInspectorGUIComplete(SerializedObject obj, bool validate)
        {
            if (obj == null || obj.targetObject == null) return;

            int id = obj.targetObject.GetInstanceID();
            if (_usedHandlers.ContainsKey(id))
            {
                var lst = _usedHandlers.Lists[id];
                if(validate)
                {
                    HandlerInfo info;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        info = lst[i];
                        info.handler.OnValidate(obj.FindProperty(info.propPath));
                    }
                }
                lst.Clear();
            }
        }

        internal static void AddAsHandled(SerializedProperty property, IPropertyHandler handler)
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
                _lastPurge = System.DateTime.Now;
                _usedHandlers.Purge();
            }

            foreach(var lst in _usedHandlers.Lists)
            {
                lst.Clear();
            }
        }

        #endregion

    }
}
