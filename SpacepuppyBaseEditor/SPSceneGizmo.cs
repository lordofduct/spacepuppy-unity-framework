using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    [InitializeOnLoad()]
    public abstract class SPSceneGizmo
    {

        #region Fields

        private Component _target;

        #endregion

        #region CONSTRUCTOR

        public SPSceneGizmo()
        {

        }

        #endregion

        #region Properties

        public Component Target { get { return _target; } }

        #endregion

        #region Methods

        protected abstract void OnSceneGUI(SceneView scene);

        #endregion



        #region Static Interface

        #region Static Fields

        private static Dictionary<System.Type, System.Type> _gizmoTypes;
        private static Dictionary<System.Type, System.Type> _gizmoTypesThatRenderAsParent;

        private static Transform _activeTransform;
        private static List<SPSceneGizmo> _activeGizmos;

        #endregion

        #region STATIC CONSTRUCTOR

        static SPSceneGizmo()
        {
            _gizmoTypes = new Dictionary<System.Type, System.Type>();
            _gizmoTypesThatRenderAsParent = new Dictionary<System.Type, System.Type>();
            var knownComponentTypes = TypeUtil.GetTypesAssignableFrom(typeof(Component)).ToArray();
            foreach(var tp in TypeUtil.GetTypesAssignableFrom(typeof(SPSceneGizmo)))
            {
                var attrib = tp.GetCustomAttributes(typeof(CustomSceneGizmoAttribute), false).Cast<CustomSceneGizmoAttribute>().FirstOrDefault();
                if (attrib == null) continue;

                if (attrib.ComponentType == null) continue;
                if (!knownComponentTypes.Contains(attrib.ComponentType)) continue;
                if (_gizmoTypes.ContainsKey(attrib.ComponentType)) return;

                _gizmoTypes[attrib.ComponentType] = tp;
                if (attrib.RenderIfParent) _gizmoTypesThatRenderAsParent[tp] = attrib.ComponentType;
            }

            //if (_gizmoTypes.Count != 0)
            //{
                _activeGizmos = new List<SPSceneGizmo>();
                SceneView.onSceneGUIDelegate -= OnSceneGUIHandler;
                SceneView.onSceneGUIDelegate += OnSceneGUIHandler;
            //}
        }

        #endregion

        #region OnSceneGUI Message

        private static void OnSceneGUIHandler(SceneView scene)
        {
            if (!UpdateActiveTransform()) return;

            for(int i = 0; i < _activeGizmos.Count; i++)
            {
                _activeGizmos[i].OnSceneGUI(scene);
            }
        }

        private static bool UpdateActiveTransform()
        {
            if (Selection.activeTransform == _activeTransform) return Selection.activeTransform != null;

            _activeGizmos.Clear();
            if (Selection.activeTransform == null)
            {
                _activeTransform = null;
                return false;
            }

            _activeTransform = Selection.activeTransform;

            var baseType = typeof(Component);
            foreach(var c in _activeTransform.GetChildComponents<Component>(true))
            {
                //find gizmoType
                System.Type gizmoTp = null;
                var typeToFind = c.GetType();
                while(baseType.IsAssignableFrom(typeToFind))
                {
                    if(_gizmoTypes.ContainsKey(typeToFind))
                    {
                        gizmoTp = _gizmoTypes[typeToFind];
                        break;
                    }
                    typeToFind = typeToFind.BaseType;
                }
                //if found, create gizmo
                if (gizmoTp != null)
                {
                    try
                    {
                        var gizmo = System.Activator.CreateInstance(gizmoTp) as SPSceneGizmo;
                        gizmo._target = c;
                        _activeGizmos.Add(gizmo);
                    }
                    catch
                    {

                    }
                }
            }
            if(_gizmoTypesThatRenderAsParent.Count > 0)
            {
                foreach (var gizmoTp in _gizmoTypesThatRenderAsParent.Keys)
                {
                    var c = _activeTransform.GetComponentInParent(_gizmoTypesThatRenderAsParent[gizmoTp]);
                    if (c == null) continue;
                    try
                    {
                        var gizmo = System.Activator.CreateInstance(gizmoTp) as SPSceneGizmo;
                        gizmo._target = c;
                        _activeGizmos.Add(gizmo);
                    }
                    catch
                    {

                    }
                }
            }


            return true;
        }

        #endregion

        #endregion

    }

}
