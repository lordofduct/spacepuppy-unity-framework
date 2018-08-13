using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor
{

    [InitializeOnLoad()]
    public static class EditorHierarchyDrawerEvents
    {


        #region Static Fields

        private static Dictionary<System.Type, System.Type> _componentTypeToDrawerType = new Dictionary<System.Type, System.Type>();
        private static List<System.Type> _supportsChildTypes = new List<System.Type>();
        private static List<System.Type> _supportedInterfaces = new List<System.Type>();

        private static Dictionary<int, HierarchyDrawer> _activeObjects = new Dictionary<int, HierarchyDrawer>();
        private static List<int> _activeGameObjects = new List<int>();
        private static List<int> _ignoredIds = new List<int>();

        private static bool _isActive;

        #endregion

        #region STATIC CONSTRUCTOR

        static EditorHierarchyDrawerEvents()
        {
            SetActive(SpacepuppySettings.UseHierarchDrawer);
        }

        #endregion

        #region Properties

        public static bool IsActive
        {
            get { return _isActive; }
        }

        #endregion

        #region Methods

        public static void SetActive(bool active)
        {
            if (_isActive == active) return;

            _isActive = active;
            if(_isActive)
            {
                SyncAvailableDrawerTypes();
                SyncActiveDrawers();

                EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
                EditorApplication.playModeStateChanged -= OnPlaymodeChanged;

                EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
                EditorApplication.playModeStateChanged += OnPlaymodeChanged;
            }
            else
            {
                _componentTypeToDrawerType.Clear();
                _supportsChildTypes.Clear();
                _supportedInterfaces.Clear();
                foreach (var drawer in _activeObjects.Values)
                {
                    drawer.OnDisable();
                }
                _activeObjects.Clear();
                _activeGameObjects.Clear();
                _ignoredIds.Clear();

                EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
                EditorApplication.playModeStateChanged -= OnPlaymodeChanged;
            }
        }

        public static bool HasDrawer(System.Type tp)
        {
            System.Type drawerType;
            return HasDrawer(tp, out drawerType);
        }

        public static bool HasDrawer(System.Type tp, out System.Type drawerType)
        {
            if (_componentTypeToDrawerType.ContainsKey(tp))
            {
                drawerType = _componentTypeToDrawerType[tp];
                return true;
            }
            else
            {
                var compTp = typeof(Component);
                bool found = false;
                if (_supportsChildTypes.Count > 0)
                {
                    var btp = tp.BaseType;
                    while (TypeUtil.IsType(btp, compTp))
                    {
                        if (_supportsChildTypes.Contains(btp))
                        {
                            drawerType = _componentTypeToDrawerType[btp];
                            return true;
                        }
                        btp = btp.BaseType;
                    }
                }

                if (!found && _supportedInterfaces.Count > 0)
                {
                    foreach (var itp in _supportedInterfaces)
                    {
                        if (TypeUtil.IsType(tp, itp))
                        {
                            drawerType = _componentTypeToDrawerType[itp];
                            return true;
                        }
                    }
                }
            }

            drawerType = null;
            return false;
        }





        private static void SyncAvailableDrawerTypes()
        {
            var attribTp = typeof(CustomHierarchyDrawerAttribute);
            var compTp = typeof(Component);
            _componentTypeToDrawerType.Clear();
            _supportsChildTypes.Clear();
            _supportedInterfaces.Clear();
            foreach(var tp in TypeUtil.GetTypesAssignableFrom(typeof(HierarchyDrawer)))
            {
                var attribs = tp.GetCustomAttributes(attribTp, false) as CustomHierarchyDrawerAttribute[];
                foreach(var attrib in attribs)
                {
                    if(TypeUtil.IsType(attrib.TargetType, compTp))
                    {
                        _componentTypeToDrawerType[attrib.TargetType] = tp;
                        if(attrib.UseForChildren)
                        {
                            _supportsChildTypes.Add(tp);
                        }
                    }
                    else if(attrib.TargetType.IsInterface)
                    {
                        _componentTypeToDrawerType[attrib.TargetType] = tp;
                        _supportedInterfaces.Add(attrib.TargetType);
                    }
                }
            }
        }

        private static void SyncActiveDrawers()
        {
            var gos = Object.FindObjectsOfType<GameObject>();
            var comps = Object.FindObjectsOfType<Component>();
            var ids = (from c in comps select c.GetInstanceID()).ToArray();

            //remove dead entries
            foreach(var id in _activeObjects.Keys.Except(ids).ToArray())
            {
                _activeObjects[id].OnDisable();
                _activeObjects.Remove(id);
            }
            foreach(var id in _activeGameObjects.Except(from g in gos select g.GetInstanceID()).ToArray())
            {
                _activeGameObjects.Remove(id);
            }
            foreach (var id in _ignoredIds.Except(ids).ToArray())
            {
                _ignoredIds.Remove(id);
            }

            //add new entries
            for(int i = 0; i < ids.Length; i++)
            {
                if (_activeObjects.ContainsKey(ids[i])) continue;
                if (_ignoredIds.Contains(ids[i])) continue;

                var c = comps[i];
                System.Type tp;
                if(HasDrawer(c.GetType(), out tp))
                {
                    try
                    {
                        var drawer = System.Activator.CreateInstance(tp) as HierarchyDrawer;
                        drawer.Init(c);
                        drawer.OnEnable();
                        _activeObjects[c.GetInstanceID()] = drawer;
                        var gid = c.gameObject.GetInstanceID();
                        if (!_activeGameObjects.Contains(gid)) _activeGameObjects.Add(gid);
                    }
                    catch { }
                }
            }
        }

        #endregion

        #region HierarchWindowItemGUI

        private static void OnHierarchyChanged()
        {
            SyncActiveDrawers();
        }

        private static void OnPlaymodeChanged(PlayModeStateChange mode)
        {
            foreach(var pair in _activeObjects)
            {
                var c = EditorUtility.InstanceIDToObject(pair.Key) as Component;
                pair.Value.Init(c);
            }
        }

        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            if (!_activeGameObjects.Contains(instanceID)) return;

            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;
            HierarchyDrawer drawer;
            foreach(var c in go.GetComponents<Component>())
            {
                if (c == null) continue;

                var id = c.GetInstanceID();
                if (_activeObjects.TryGetValue(id, out drawer))
                {
                    //drawer.Init(c);
                    drawer.OnHierarchyGUI(selectionRect);
                }
            }
        }

        #endregion



    }
}
