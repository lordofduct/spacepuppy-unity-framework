using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomEditor(typeof(SingletonManager))]
    public class SingletonManagerInspector : SPEditor
    {

        private static System.Type[] IgnoredSpecialSingletonTypes;
        static SingletonManagerInspector()
        {
            var lst = new List<System.Type>();
            foreach(var tp in TypeUtil.GetTypesAssignableFrom(typeof(ISingleton)))
            {
                var attribs = tp.GetCustomAttributes(typeof(Singleton.ConfigAttribute), false) as Singleton.ConfigAttribute[];
                if(attribs != null && attribs.Length > 0)
                {
                    if (attribs[0].ExcludeFromSingletonManager) lst.Add(tp);
                }
            }
            IgnoredSpecialSingletonTypes = lst.ToArray();
        }

        protected override void OnSPInspectorGUI()
        {
            this.DrawDefaultInspector();

            //var selectedTypes = (from c in (this.target as SingletonManager).GetComponents<Singleton>() select c.GetType()).ToArray();
            //var selectedTypes = (from c in Singleton.AllSingletons select c.GetType()).ToArray();

            var singletonType = typeof(Singleton);
            var types = (from t in TypeUtil.GetTypesAssignableFrom(singletonType)
                         where t != singletonType &&
                         !IgnoredSpecialSingletonTypes.Contains(t) && 
                         SingletonCount(t) == 0 //!selectedTypes.Contains(t)
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
                    if(arr[i] != this.target && arr[i].LifeCycle.HasFlag(SingletonLifeCycleRule.LivesForever))
                    {
                        (this.target as SingletonManager).MaintainOnLoad = true;
                        EditorUtility.SetDirty(this.target);
                        break;
                    }
                }
            }
        }

        internal static int SingletonCount(System.Type tp)
        {
            return GameObject.FindObjectsOfType(tp).Length;
        }

    }

    [CustomPropertyDrawer(typeof(Singleton.Maintainer))]
    public class SingletonMaintainerPropertyDrawer : PropertyDrawer
    {

        private string _message;
        private MessageType _messageType;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return 0f;

            if (property.serializedObject.targetObject is ISingleton && SingletonManagerInspector.SingletonCount(property.serializedObject.targetObject.GetType()) > 1)
            {
                _message = "Multiple Singletons of this type exist, you should purge the scene of duplicates!";
                _messageType = MessageType.Error;
                return EditorGUIUtility.singleLineHeight * 2f;
            }

            var go = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (object.ReferenceEquals(go, null))
            {
                _message = "This Singleton appears to not be attached to a GameObject.";
                _messageType = MessageType.Error;
                return EditorGUIUtility.singleLineHeight * 2f;
            }

            if (go.HasComponent<SingletonManager>())
            {
                _message = "This Singleton is managed by a SingletonManager.";
                _messageType = MessageType.Info;
                return EditorGUIUtility.singleLineHeight * 3f;
            }
            else if (go.GetComponentsAlt<ISingleton>().Count() > 1)
            {
                _message = "A GameObject with multiple Singletons on it should have a SingletonManager attached!";
                _messageType = MessageType.Warning;
                return EditorGUIUtility.singleLineHeight * 2f;
            }
            else
            {
                _message = null;
                _messageType = MessageType.None;
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return;

            var lifeCycleProp = property.FindPropertyRelative("_lifeCycle");
            if (_message != null)
            {
                var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight * 2f);
                EditorGUI.HelpBox(r, _message, _messageType);

                r = new Rect(r.xMin, r.yMax, r.width, EditorGUIUtility.singleLineHeight);
                var b = lifeCycleProp.GetEnumValue<SingletonLifeCycleRule>().HasFlag(SingletonLifeCycleRule.AlwaysReplace);
                b = EditorGUI.Toggle(r, "Always Replace", b);
                lifeCycleProp.SetEnumValue((b) ? SingletonLifeCycleRule.AlwaysReplace : SingletonLifeCycleRule.LivesForDurationOfScene);
            }
            else
            {
                SPEditorGUI.PropertyField(position, lifeCycleProp);
            }
        }

    }

    [CustomEditor(typeof(Singleton), true)]
    public class GameLoopEntryInspector : SPEditor
    {


        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            if(this.target is Singleton)
            {
                var attrib = this.target.GetType().GetCustomAttributes(typeof(Singleton.ConfigAttribute), false).FirstOrDefault() as Singleton.ConfigAttribute;
                if(attrib != null && attrib.LifeCycleReadOnly)
                {
                    this.DrawReadOnlyInspector(attrib);
                }
                else
                {
                    this.DrawDefaultInspector();
                }
            }
            else
            {
                this.DrawDefaultInspector();
            }

            this.serializedObject.ApplyModifiedProperties();
        }


        private void DrawReadOnlyInspector(Singleton.ConfigAttribute attrib)
        {
            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            var prop = this.serializedObject.FindProperty("_maintainer._lifeCycle");
            if (prop.GetEnumValue<SingletonLifeCycleRule>() != attrib.DefaultLifeCycle) prop.SetEnumValue(attrib.DefaultLifeCycle);

            GUI.enabled = false;
            EditorGUILayout.EnumPopup("Life Cycle", prop.GetEnumValue<SingletonLifeCycleRule>());
            GUI.enabled = true;

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, "_maintainer");
        }

    }

}
