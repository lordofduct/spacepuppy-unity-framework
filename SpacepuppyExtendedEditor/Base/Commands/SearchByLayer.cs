using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{
    public class SearchByLayer : EditorWindow
    {

        public const string MENU_NAME = SPMenu.MENU_NAME_ROOT + "/Search By Layer";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_GROUP2;


        static LayerMask layerMask;
        static Vector2 scrollValue = Vector2.zero;
        static GameObject[] searchResult;

        [MenuItem(MENU_NAME, priority=MENU_PRIORITY)]
        static void OpenTagSearcher()
        {
            EditorWindow.GetWindow<SearchByLayer>();

            UpdateSearchResults();
        }

        void OnGUI()
        {
            var oldLayerMask = layerMask;
            layerMask = SPEditorGUILayout.LayerMaskField("Layers", layerMask);

            if (layerMask != oldLayerMask)
            {
                UpdateSearchResults();
            }

            scrollValue = EditorGUILayout.BeginScrollView(scrollValue);

            if (searchResult != null)
            {
                foreach (GameObject obj in searchResult)
                {
                    if (obj != null)
                    {
                        if (GUILayout.Button(obj.name,GUIStyle.none))
                        {
                            Selection.activeObject = obj;
                            EditorGUIUtility.PingObject(obj);
                        }
                    }
                    else
                    {
                        UpdateSearchResults();
                        break;
                    }
                }

            }

            EditorGUILayout.EndScrollView();
        }


        private static void UpdateSearchResults()
        {
            searchResult = (from g in Object.FindObjectsOfType<GameObject>() where g.IntersectsLayerMask(layerMask) select g).ToArray();
            Selection.objects = searchResult;
        }

    }
}
