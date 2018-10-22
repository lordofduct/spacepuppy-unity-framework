using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    public class SearchByTag : EditorWindow
    {

        public const string MENU_NAME = SPMenu.MENU_NAME_ROOT + "/Search By Tag";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_GROUP2;


        static string tagValue = "";
        static Vector2 scrollValue = Vector2.zero;
        static GameObject[] searchResult;

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        static void OpenTagSearcher()
        {
            EditorWindow.GetWindow<SearchByTag>();

            searchResult = FindGameObjectsWithMultiTagSlow(tagValue).ToArray(); //GameObject.FindGameObjectsWithTag(tagValue);
            Selection.objects = searchResult;
        }

        void OnGUI()
        {
            var oldTagValue = tagValue;
            tagValue = EditorGUILayout.TagField(tagValue);

            if (tagValue != oldTagValue)
            {
                searchResult = FindGameObjectsWithMultiTagSlow(tagValue).ToArray(); //GameObject.FindGameObjectsWithTag(tagValue);
                Selection.objects = searchResult;
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
                        searchResult = FindGameObjectsWithMultiTagSlow(tagValue).ToArray(); //GameObject.FindGameObjectsWithTag(tagValue);
                        Selection.objects = searchResult;
                        break;
                    }
                }

            }

            EditorGUILayout.EndScrollView();
        }



        public static GameObject[] FindGameObjectsWithMultiTagSlow(string tag)
        {
            if (tag == SPConstants.TAG_MULTITAG)
            {
                return GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG);
            }
            else
            {
                using (var tmp = TempList<GameObject>.GetList())
                {
                    foreach (var go in GameObject.FindGameObjectsWithTag(tag)) tmp.Add(go);

                    foreach (var m in GameObject.FindObjectsOfType<MultiTag>())
                    {
                        if (m.HasTag(tag)) tmp.Add(m.gameObject);
                    }

                    return tmp.ToArray();
                }
            }
        }

    }

}