using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(t_OnEventTrigger), true)]
    public class t_OnEventTriggerInspector : SPEditor
    {

        public const string PROP_TRIGGERS = "_triggers";
        private const string PROP_EVENTID = "EventID";

        private TriggerPropertyDrawer _drawer;

        private SerializedProperty _triggerProp;
        private List<EventTriggerType> _toAdd = new List<EventTriggerType>();

        protected override void OnEnable()
        {
            base.OnEnable();

            _drawer = new TriggerPropertyDrawer();
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            _toAdd.Clear();
            _toAdd.AddRange(System.Enum.GetValues(typeof(EventTriggerType)).Cast<EventTriggerType>());

            var triggersProp = this.serializedObject.FindProperty(PROP_TRIGGERS);
            for (int i = 0; i < triggersProp.arraySize; i++)
            {
                var prop = triggersProp.GetArrayElementAtIndex(i);
                var eventId = prop.FindPropertyRelative(PROP_EVENTID).GetEnumValue<EventTriggerType>();
                var label = EditorHelper.TempContent(eventId.ToString());
                var h = _drawer.GetPropertyHeight(prop, label);
                var rect = EditorGUILayout.GetControlRect(true, h);
                _drawer.OnGUI(rect, prop, label);
                _toAdd.Remove(eventId);
            }

            var btnHeight = EditorGUIUtility.singleLineHeight * 1.5f;
            var btnVerPadding = EditorGUIUtility.singleLineHeight / 2f;
            var btnRect = EditorGUILayout.GetControlRect(false, btnVerPadding + btnVerPadding + btnHeight);
            var btnWidth = Mathf.Min(btnRect.width, 200f);
            var btnHorPadding = Mathf.Max(0f, (btnRect.width - btnWidth) / 2f);
            btnRect = new Rect(btnRect.xMin + btnHorPadding, btnRect.yMin + btnVerPadding, btnWidth, btnHeight);
            if(GUI.Button(btnRect, "Add New Event Type"))
            {
                var labels = (from e in _toAdd select e.ToString()).ToArray();

                var menu = new GenericMenu();
                foreach(var e in _toAdd)
                {
                    menu.AddItem(EditorHelper.TempContent(e.ToString()), false, () =>
                    {
                        triggersProp.arraySize++;
                        var el = triggersProp.GetArrayElementAtIndex(triggersProp.arraySize - 1);
                        el.FindPropertyRelative(PROP_EVENTID).SetEnumValue(e);
                        el.FindPropertyRelative(TriggerPropertyDrawer.PROP_TARGETS).arraySize = 0;
                        triggersProp.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_TRIGGERS);

            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
