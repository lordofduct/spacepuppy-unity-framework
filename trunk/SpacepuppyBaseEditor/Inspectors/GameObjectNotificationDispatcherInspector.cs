using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;


namespace com.spacepuppyeditor.Tools
{

    [CustomEditor(typeof(GameObjectNotificationDispatcher))]
    class GameObjectNotificationDispatcherInspector : SPEditor
    {

        private bool _expanded;


        public override void OnInspectorGUI()
        {
            var targ = this.target as GameObjectNotificationDispatcher;
            if(targ == null) return;

            _expanded = EditorGUILayout.Foldout(_expanded, "Observed Notifications");

            if(_expanded)
            {
                var observeredNotifications = targ.ListObserveredNotifications();
                foreach (var tp in observeredNotifications)
                {
                    EditorGUILayout.LabelField(tp.Name);
                }
            }
        }

    }
}