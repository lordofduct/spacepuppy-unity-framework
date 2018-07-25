using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.spacepuppyeditor
{

    public static class SpacepuppySettings
    {

        private const string SETTING_STORESETTINGSLOCAL = "Spacepuppy.StoreSettingsLocal";

        private const string SETTING_SPEDITOR_ISDEFAULT_ACTIVE = "UseSPEditor.IsDefault.Active";
        private const string SETTING_ADVANCEDANIMINSPECTOR_ACTIVE = "AdvancedAnimationInspector.Active";
        private const string SETTING_HIERARCHYDRAWER_ACTIVE = "EditorHierarchyEvents.Active";
        private const string SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE = "EditorHierarchyAlternateContextMenu.Active";

        private const string SETTING_MODELIMPORT_USE = "ModelImportManager.UseSpacepuppyModelImportSettings";
        private const string SETTING_MODELIMPORT_SETMATERIALSEARCH = "ModelImportManager.SetMaterialSearch";
        private const string SETTING_MODELIMPORT_MATERIALSEARCH = "ModelImportManager.MaterialSearch";
        private const string SETTING_MODELIMPORT_SETANIMSETTINGS = "ModelImportManager.SetAnimationSettings";
        private const string SETTING_MODELIMPORT_ANIMRIGTYPE = "ModelImportManager.AnimRigType";

        
        public static bool StoreSettingsLocal
        {
            get
            {
                return EditorProjectPrefs.Local.GetBool(SETTING_STORESETTINGSLOCAL, false);
            }
            set
            {
                EditorProjectPrefs.Local.SetBool(SETTING_STORESETTINGSLOCAL, value);
            }
        }

        /*
         * EDITOR SETTINGS
         */

        public static bool UseSPEditorAsDefaultEditor
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetBool(SETTING_SPEDITOR_ISDEFAULT_ACTIVE, true);
                else
                    return EditorProjectPrefs.Group.GetBool(SETTING_SPEDITOR_ISDEFAULT_ACTIVE, true);
            }
            set
            {
                if(StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetBool(SETTING_SPEDITOR_ISDEFAULT_ACTIVE, value);
                else
                    EditorProjectPrefs.Group.SetBool(SETTING_SPEDITOR_ISDEFAULT_ACTIVE, value);
            }
        }

        public static bool UseAdvancedAnimationInspector
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetBool(SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, true);
                else
                    return EditorProjectPrefs.Group.GetBool(SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, true);
            }
            set
            {
                if (StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetBool(SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, value);
                else
                    EditorProjectPrefs.Group.SetBool(SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, value);
            }
        }

        public static bool UseHierarchDrawer
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetBool(SETTING_HIERARCHYDRAWER_ACTIVE, true);
                else
                    return EditorProjectPrefs.Group.GetBool(SETTING_HIERARCHYDRAWER_ACTIVE, true);
            }
            set
            {
                if (StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetBool(SETTING_HIERARCHYDRAWER_ACTIVE, value);
                else
                    EditorProjectPrefs.Group.SetBool(SETTING_HIERARCHYDRAWER_ACTIVE, value);
            }
        }

        public static bool UseHierarchyAlternateContextMenu
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetBool(SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, true);
                else
                    return EditorProjectPrefs.Group.GetBool(SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, true);
            }
            set
            {
                if (StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetBool(SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, value);
                else
                    EditorProjectPrefs.Group.SetBool(SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, value);
            }
        }

        /*
         * MODELIMPORT SETTINGS
         */
         
        //Material Import Settings
        
        public static bool SetMaterialSearchOnImport
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetBool(SETTING_MODELIMPORT_SETMATERIALSEARCH, true);
                else
                    return EditorProjectPrefs.Group.GetBool(SETTING_MODELIMPORT_SETMATERIALSEARCH, true);
            }
            set
            {
                if (StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetBool(SETTING_MODELIMPORT_SETMATERIALSEARCH, value);
                else
                    EditorProjectPrefs.Group.SetBool(SETTING_MODELIMPORT_SETMATERIALSEARCH, value);
            }
        }

        public static ModelImporterMaterialSearch MaterialSearch
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetEnum(SETTING_MODELIMPORT_MATERIALSEARCH, ModelImporterMaterialSearch.Everywhere);
                else
                    return (ModelImporterMaterialSearch)EditorProjectPrefs.Group.GetInt(SETTING_MODELIMPORT_MATERIALSEARCH, (int)ModelImporterMaterialSearch.Everywhere);
            }
            set
            {
                if (StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetEnum(SETTING_MODELIMPORT_MATERIALSEARCH, value);
                else
                    EditorProjectPrefs.Group.SetInt(SETTING_MODELIMPORT_MATERIALSEARCH, (int)value);
            }
        }

        //Animation Import Settings

        public static bool SetAnimationSettingsOnImport
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetBool(SETTING_MODELIMPORT_SETANIMSETTINGS, true);
                else
                    return EditorProjectPrefs.Group.GetBool(SETTING_MODELIMPORT_SETANIMSETTINGS, true);
            }
            set
            {
                if (StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetBool(SETTING_MODELIMPORT_SETANIMSETTINGS, value);
                else
                    EditorProjectPrefs.Group.SetBool(SETTING_MODELIMPORT_SETANIMSETTINGS, value);
            }
        }

        public static ModelImporterAnimationType ImportAnimRigType
        {
            get
            {
                if (StoreSettingsLocal)
                    return EditorProjectPrefs.Local.GetEnum(SETTING_MODELIMPORT_ANIMRIGTYPE, ModelImporterAnimationType.Legacy);
                else
                    return (ModelImporterAnimationType)EditorProjectPrefs.Group.GetInt(SETTING_MODELIMPORT_ANIMRIGTYPE, (int)ModelImporterAnimationType.Legacy);
            }
            set
            {
                if (StoreSettingsLocal)
                    EditorProjectPrefs.Local.SetEnum(SETTING_MODELIMPORT_ANIMRIGTYPE, value);
                else
                    EditorProjectPrefs.Group.SetInt(SETTING_MODELIMPORT_ANIMRIGTYPE, (int)value);
            }
        }
        
    }

}
