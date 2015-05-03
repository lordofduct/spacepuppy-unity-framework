using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    [InitializeOnLoad()]
    public static class SPMenu
    {

        public const string MENU_NAME_ROOT = "Spacepuppy";
        public const string MENU_NAME_SETTINGS = MENU_NAME_ROOT + "/Settings";
        public const string MENU_NAME_MODELS = MENU_NAME_ROOT + "/Models";

        public const int MENU_GAP = 1000;

        public const int MENU_PRIORITY_GROUP1 = 0;
        public const int MENU_PRIORITY_GROUP2 = MENU_PRIORITY_GROUP1 + MENU_GAP;
        public const int MENU_PRIORITY_GROUP3 = MENU_PRIORITY_GROUP2 + MENU_GAP;
        public const int MENU_PRIORITY_SETTINGS = MENU_PRIORITY_GROUP1 - 1;
        public const int MENU_PRIORITY_MODELS = MENU_PRIORITY_GROUP3 - 1;

        public const int MENU_PRIORITY_SINGLETON = 1000000;

        #region Special Menu Entries

        [MenuItem(SPMenu.MENU_NAME_SETTINGS + "/Sync TagData", priority = MENU_PRIORITY_SETTINGS)]
        public static void SyncTagData()
        {
            var tagData = (TagData)AssetDatabase.LoadAssetAtPath(@"Assets/Resources/TagData.asset", typeof(TagData));
            if (tagData == null)
            {
                if(!System.IO.Directory.Exists(Application.dataPath + "/Resources"))
                {
                    System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
                }
                tagData = ScriptableObjectHelper.CreateAsset<TagData>(@"Assets/Resources/TagData.asset");
            }

            if (!tagData.SimilarTo(UnityEditorInternal.InternalEditorUtility.tags))
            {
                var helper = new TagData.EditorHelper(tagData);
                helper.UpdateTags(UnityEditorInternal.InternalEditorUtility.tags);
                EditorUtility.SetDirty(tagData);
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem(SPMenu.MENU_NAME_ROOT + "/Create SingletonSource", priority = MENU_PRIORITY_SINGLETON)]
        public static void CreateSingletonSource()
        {
            Singleton.CreateSpecialInstance<SingletonManager>(Singleton.GAMEOBJECT_NAME);
        }

        [MenuItem(SPMenu.MENU_NAME_ROOT + "/Create SingletonSource", validate = true)]
        public static bool CreateSingletonSource_Validate()
        {
            if (Application.isPlaying) return false;
            return !Singleton.HasInstance<SingletonManager>();
        }

        [MenuItem(SPMenu.MENU_NAME_ROOT + "/Create GameLoopEntry", priority = MENU_PRIORITY_SINGLETON)]
        public static void CreateGameLoopEntry()
        {
            GameLoopEntry.Init();
        }

        [MenuItem(SPMenu.MENU_NAME_ROOT + "/Create GameLoopEntry", validate = true)]
        public static bool CreateGameLoopEntry_Validate()
        {
            if (Application.isPlaying) return false;
            return !Singleton.HasInstance<GameLoopEntry>();
        }

        

        #endregion

    }
}
