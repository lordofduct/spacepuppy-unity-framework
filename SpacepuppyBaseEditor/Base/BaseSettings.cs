using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{
    public class BaseSettings : EditorWindow
    {

        #region Consts

        public const string MENU_NAME = SPMenu.MENU_NAME_SETTINGS + "/Base Settings";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_SETTINGS;
        public const string SETTING_SPEDITOR_ISDEFAULT_ACTIVE = "UseSPEditor.IsDefault.Active";
        public const string SETTING_ADVANCEDANIMINSPECTOR_ACTIVE = "AdvancedAnimationInspector.Active";
        public const string SETTING_HIERARCHYDRAWER_ACTIVE = "EditorHierarchyEvents.Active";
        public const string SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE = "EditorHierarchyAlternateContextMenu.Active";

        #endregion

        #region Menu Entries

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        public static void OpenBaseSettings()
        {
            if (_openWindow == null)
            {
                EditorWindow.GetWindow<BaseSettings>();
            }
            else
            {
                GUI.BringWindowToFront(_openWindow.GetInstanceID());
            }
        }

        #endregion

        #region Window

        private static BaseSettings _openWindow;


        private CustomTimeLayersData.EditorHelper _timeLayersHelper;
        private ReorderableList _timeLayersListDrawer;

        private void OnEnable()
        {
            if (_openWindow == null)
                _openWindow = this;
            else
                Object.DestroyImmediate(this);

            this.title = "Base Settings";

            var timeLayersData = (CustomTimeLayersData)AssetDatabase.LoadAssetAtPath(@"Assets/Resources/CustomTimeLayersData.asset", typeof(CustomTimeLayersData));
            if (timeLayersData != null) _timeLayersHelper = new CustomTimeLayersData.EditorHelper(timeLayersData);
        }

        private void OnDisable()
        {
            if(_openWindow == this) _openWindow = null;
        }

        private void OnGUI()
        {
            //var labelWidthCache = EditorGUIUtility.labelWidth;
            //EditorGUIUtility.labelWidth = Mathf.Min(this.position.width - 20f, 300f);

            EditorGUI.BeginChangeCheck();
            bool useSPEditor = EditorGUILayout.ToggleLeft("Use SPEditor as default editor for MonoBehaviour", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_SPEDITOR_ISDEFAULT_ACTIVE, true));
            if (EditorGUI.EndChangeCheck()) EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_SPEDITOR_ISDEFAULT_ACTIVE, useSPEditor);

            EditorGUI.BeginChangeCheck();
            bool useAdvancedAnimInspector = EditorGUILayout.ToggleLeft("Use Advanced Animation Inspector", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, true));
            if (EditorGUI.EndChangeCheck()) EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, useAdvancedAnimInspector);

            EditorGUI.BeginChangeCheck();
            bool hierarchyDrawerActive = EditorGUILayout.ToggleLeft("Use Hierarchy Drawers", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_HIERARCHYDRAWER_ACTIVE, true));
            if (EditorGUI.EndChangeCheck())
            {
                EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_HIERARCHYDRAWER_ACTIVE, hierarchyDrawerActive);
                EditorHierarchyDrawerEvents.SetActive(hierarchyDrawerActive);
            }

            EditorGUI.BeginChangeCheck();
            bool hierarchCustomContextMenu = EditorGUILayout.ToggleLeft("Use Alternate Hierarchy Context Menu", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, true));
            if(EditorGUI.EndChangeCheck())
            {
                EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, hierarchCustomContextMenu);
                EditorHierarchyAlternateContextMenuEvents.SetActive(hierarchCustomContextMenu);
            }

            if (_timeLayersHelper == null)
            {
                var rect = EditorGUILayout.GetControlRect();
                rect.width = Mathf.Min(rect.width, 275f);
                if(GUI.Button(rect, "Create Custom Time Layers Data Resource"))
                {
                    if (!System.IO.Directory.Exists(Application.dataPath + "/Resources"))
                    {
                        System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
                    }
                    var timeLayersData = ScriptableObjectHelper.CreateAsset<CustomTimeLayersData>(@"Assets/Resources/CustomTimeLayersData.asset");
                    if (timeLayersData != null) _timeLayersHelper = new CustomTimeLayersData.EditorHelper(timeLayersData);
                }
            }
            else
            {
                if(_timeLayersListDrawer == null)
                {
                    _timeLayersListDrawer = new ReorderableList(_timeLayersHelper.Layers, typeof(string));
                    _timeLayersListDrawer.drawHeaderCallback = _timeLayersList_DrawHeader;
                    _timeLayersListDrawer.drawElementCallback = _timeLayers_DrawElement;
                    _timeLayersListDrawer.onAddCallback = _timeLayers_AddElement;
                }

                EditorGUI.BeginChangeCheck();
                _timeLayersListDrawer.DoLayoutList();
                if(EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_timeLayersHelper.Data);
                    AssetDatabase.SaveAssets();
                }
            }

            //EditorGUIUtility.labelWidth = labelWidthCache;
        }


        #endregion

        #region Time Layers Draw Callback

        private void _timeLayersList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Custom Time Layers");
        }

        private void _timeLayers_AddElement(ReorderableList lst)
        {
            _timeLayersHelper.Layers.Add(null);
        }

        private void _timeLayers_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            string layerName = _timeLayersHelper.Layers[index] as string;

            EditorGUI.BeginChangeCheck();
            layerName = EditorGUI.TextField(area, layerName);
            if (EditorGUI.EndChangeCheck())
                _timeLayersHelper.Layers[index] = layerName;

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_timeLayersListDrawer, area, index, isActive, isFocused);
        }

        #endregion

    }
}
