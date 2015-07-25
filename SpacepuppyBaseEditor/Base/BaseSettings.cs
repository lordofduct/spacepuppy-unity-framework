using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

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

        private void OnEnable()
        {
            if (_openWindow == null)
                _openWindow = this;
            else
                Object.DestroyImmediate(this);

            this.title = "Base Settings";
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

            //EditorGUIUtility.labelWidth = labelWidthCache;
        }

        #endregion

    }
}
