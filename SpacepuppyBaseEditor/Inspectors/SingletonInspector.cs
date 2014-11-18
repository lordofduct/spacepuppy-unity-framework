using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomEditor(typeof(SingletonManager))]
    public class SingletonInspector : Editor
    {
        [MenuItem("Spacepuppy/Create SingletonSource", priority=1000000)]
        public static void CreateSingletonSource()
        {
            var go = new GameObject(Singleton.GAMEOBJECT_NAME);
            go.AddComponent<SingletonManager>();
        }

        [MenuItem("Spacepuppy/Create SingletonSource", validate = true)]
        public static bool CreateSingletonSource_Validate()
        {
            if (Application.isPlaying) return false;
            var go = GameObject.Find(Singleton.GAMEOBJECT_NAME);
            return (go == null);
        }





        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();

            var selectedTypes = (from c in (this.target as SingletonManager).GetComponents<Singleton>() select c.GetType()).ToArray();

            var singletonType = typeof(Singleton);
            var types = (from a in System.AppDomain.CurrentDomain.GetAssemblies()
                         from t in a.GetTypes()
                         where singletonType.IsAssignableFrom(t) &&
                         t != singletonType &&
                         !selectedTypes.Contains(t)
                         select t
                         ).ToArray();
            var typeNames = (from t in types select t.Name).ToArray();

            int index = EditorGUILayout.Popup("Add Singleton", -1, typeNames);
            if (index >= 0)
            {
                var tp = types[index];
                (this.target as SingletonManager).gameObject.AddComponent(tp);
            }
        }
    }
}
