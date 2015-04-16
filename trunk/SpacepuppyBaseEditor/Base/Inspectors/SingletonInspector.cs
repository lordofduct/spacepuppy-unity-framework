using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomEditor(typeof(SingletonManager))]
    public class SingletonInspector : SPEditor
    {

        #region Menu

        [MenuItem("Spacepuppy/Create SingletonSource", priority=1000000)]
        public static void CreateSingletonSource()
        {
            Singleton.CreateSpecialInstance<SingletonManager>(Singleton.GAMEOBJECT_NAME);
        }

        [MenuItem("Spacepuppy/Create SingletonSource", validate = true)]
        public static bool CreateSingletonSource_Validate()
        {
            if (Application.isPlaying) return false;
            return !Singleton.HasInstance<SingletonManager>();
        }

        [MenuItem("Spacepuppy/Create GameLoopEntry", priority = 1000001)]
        public static void CreateGameLoopEntry()
        {
            GameLoopEntry.Init();
        }

        [MenuItem("Spacepuppy/Create GameLoopEntry", validate = true)]
        public static bool CreateGameLoopEntry_Validate()
        {
            if (Application.isPlaying) return false;
            return !Singleton.HasInstance<GameLoopEntry>();
        }

        #endregion





        protected override void OnSPInspectorGUI()
        {
            this.DrawDefaultInspector();

            //var selectedTypes = (from c in (this.target as SingletonManager).GetComponents<Singleton>() select c.GetType()).ToArray();
            var selectedTypes = (from c in Singleton.AllSingletons select c.GetType()).ToArray();

            var singletonType = typeof(Singleton);
            //var types = (from a in System.AppDomain.CurrentDomain.GetAssemblies()
            //             from t in a.GetTypes()
            //             where singletonType.IsAssignableFrom(t) &&
            //             t != singletonType &&
            //             !selectedTypes.Contains(t)
            //             select t
            //             ).ToArray();
            var types = (from t in TypeUtil.GetTypesAssignableFrom(singletonType)
                         where t != singletonType &&
                         !selectedTypes.Contains(t)
                         select t
                        ).ToArray();
            var typeNames = (from t in types select t.Name).ToArray();

            int index = EditorGUILayout.Popup("Add Singleton", -1, typeNames);
            if (index >= 0)
            {
                var tp = types[index];
                (this.target as SingletonManager).gameObject.AddComponent(tp);
                EditorUtility.SetDirty(this.target);
            }

            if(this.target is SingletonManager)
            {
                var arr = (this.target as SingletonManager).GetComponents<Singleton>();
                for (int i = 0; i < arr.Length; i ++ )
                {
                    if(arr[i] != this.target && arr[i].MaintainOnLoad)
                    {
                        (this.target as SingletonManager).MaintainOnLoad = true;
                        EditorUtility.SetDirty(this.target);
                        break;
                    }
                }
            }
        }

    }

    [CustomPropertyDrawer(typeof(Singleton.Maintainer))]
    public class SingletonMaintainerPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative("_maintainOnLoad");
            EditorGUI.PropertyField(position, prop);
        }

    }
}
