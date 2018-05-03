using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Scenario
{

    [CustomPropertyDrawer(typeof(ObservableTargetData))]
    public class ObservableTargetDataPropertyDrawer : PropertyDrawer
    {


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Box(new Rect(position.xMin, position.yMin + 1f, position.width, position.height - 2f), GUIContent.none);

            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(position.xMin, r0.yMax, position.width, EditorGUIUtility.singleLineHeight);

            var targProp = property.FindPropertyRelative("_target");
            var indexProp = property.FindPropertyRelative("_triggerIndex");

            EditorGUI.BeginChangeCheck();
            SPEditorGUI.PropertyField(r0, targProp);
            if (EditorGUI.EndChangeCheck())
                indexProp.intValue = 0;

            if (targProp.objectReferenceValue is IObservableTrigger)
            {
                var targ = targProp.objectReferenceValue as IObservableTrigger;
                var owner = new SerializedObject(targProp.objectReferenceValue);

                int i = 0;
                var events = (from e in targ.GetTriggers() select GetTriggerTargsId(owner, e, ++i)).ToArray();
                indexProp.intValue = EditorGUI.Popup(r1, "Trigger Event", indexProp.intValue, events);
            }
            else
            {
                EditorGUI.LabelField(r1, "Trigger Event", "Select Target First");
            }
        }


        private static string GetTriggerTargsId(SerializedObject owner, BaseSPEvent e, int index)
        {
            var child = owner.GetIterator();
            child.Next(true);
            do
            {
                var v = EditorHelper.GetTargetObjectOfProperty(child);
                if (v is BaseSPEvent && v == e) return child.displayName;
            }
            while (child.Next(false));

            if (string.IsNullOrEmpty(e.ObservableTriggerId))
                return "Trigger (" + index.ToString() + ")";
            else
                return e.ObservableTriggerId + "(" + index.ToString() + ")";
        }


    }

    /*
     * Old version
     * 
    [CustomPropertyDrawer(typeof(ObservableTargetData))]
    public class ObservableTargetDataPropertyDrawer : PropertyDrawer
    {

        #region Fields

        private SelectableComponentPropertyDrawer _componentDrawer = new SelectableComponentPropertyDrawer()
        {
            AllowSceneObjects = true,
            RestrictionType = typeof(IObservableTrigger),
            ChoiceSelector = new CustomComponentChoiceSelector()
        };

        #endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //if (this.GetPropertyIsComplex(property))
            //{
            //    var ra = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            //    var rb = new Rect(position.xMin, position.yMin + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            //    var triggerProp = property.FindPropertyRelative("_trigger");
            //    _componentDrawer.OnGUI(ra, triggerProp, label);

            //    var idProp = property.FindPropertyRelative("_triggerId");
            //    var ids = (triggerProp.objectReferenceValue as IObservableTrigger).GetComplexIds();
            //    var oi = ids.IndexOf(idProp.stringValue);
            //    var ni = EditorGUI.Popup(rb, oi, ids);
            //    if (oi != ni)
            //    {
            //        idProp.stringValue = (ni >= 0) ? ids[ni] : null;
            //    }
            //}
            //else
            //{
            //    _componentDrawer.OnGUI(position, property.FindPropertyRelative("_trigger"), label);
            //}

            (_componentDrawer.ChoiceSelector as CustomComponentChoiceSelector).TriggerIdProp = property.FindPropertyRelative("_triggerId");
            _componentDrawer.OnGUI(position, property.FindPropertyRelative("_trigger"), label);
            (_componentDrawer.ChoiceSelector as CustomComponentChoiceSelector).TriggerIdProp = null;
        }

        public bool GetPropertyIsComplex(SerializedProperty property)
        {
            var triggerProp = property.FindPropertyRelative("_trigger");
            if (triggerProp == null) return false;
            return triggerProp.objectReferenceValue != null && triggerProp.objectReferenceValue is IObservableTrigger && (triggerProp.objectReferenceValue as IObservableTrigger).GetTriggers().Length > 0;
        }




        private class CustomComponentChoiceSelector : DefaultComponentChoiceSelector, IComponentChoiceSelector
        {

            public SerializedProperty TriggerIdProp;
            private List<TriggerEntry> _entries = new List<TriggerEntry>();
            private static GUIContent _nothingContent = new GUIContent("Nothing...");

            protected override void OnBeforeGUI()
            {
                var componentNames = DefaultComponentChoiceSelector.GetUniqueComponentNames(this.Components).ToArray();
                _entries.Clear();
                Component c;
                string nm;
                for (int i = 0; i < this.Components.Length; i++)
                {
                    c = this.Components[i];
                    nm = componentNames[i];

                    if (c is IObservableTrigger)
                    {
                        var arr = (c as IObservableTrigger).GetTriggers();
                        if (arr.Length == 1)
                        {
                            var tid = arr[0].ObservableTriggerId;
                            var scont = string.IsNullOrEmpty(tid) ? nm : string.Format("{0} -> {1}", nm, tid);
                            _entries.Add(new TriggerEntry()
                            {
                                component = c,
                                triggerId = tid,
                                popupName = new GUIContent(scont)
                            });
                        }
                        else if(arr.Length > 1)
                        {
                            for (int j = 0; j < arr.Length; j++)
                            {
                                var tid = arr[j].ObservableTriggerId;
                                var scont = string.IsNullOrEmpty(tid) ? string.Format("{0}/Trigger_{1}", nm, j) : string.Format("{0}/{1}", nm, tid);
                                _entries.Add(new TriggerEntry()
                                {
                                    component = c,
                                    triggerId = tid,
                                    popupName = new GUIContent(scont)
                                });
                            }
                        }
                    }
                }
            }

            protected override void OnGUIComplete(int selectedIndex)
            {
                if(TriggerIdProp != null)
                {
                    if(selectedIndex <= 0 || selectedIndex > _entries.Count)
                    {
                        TriggerIdProp.stringValue = string.Empty;
                    }
                    else
                    {
                        TriggerIdProp.stringValue = _entries[selectedIndex - 1].triggerId;
                    }
                }
            }

            public override GUIContent[] GetPopupEntries()
            {
                using (var lst = TempCollection.GetList<GUIContent>())
                {
                    lst.Add(_nothingContent);
                    var e = _entries.GetEnumerator();
                    while(e.MoveNext())
                    {
                        lst.Add(e.Current.popupName);
                    }
                    return lst.ToArray();
                }
            }

            public override int GetPopupIndexOfComponent(Component comp)
            {
                int entryIndex = this.Components.IndexOf(comp);
                if (entryIndex < 0) return 0;

                if (this.TriggerIdProp != null && !string.IsNullOrEmpty(this.TriggerIdProp.stringValue))
                {
                    var tid = this.TriggerIdProp.stringValue;
                    for(int i = 0; i < _entries.Count; i++)
                    {
                        if (_entries[i].component == comp && _entries[i].triggerId == tid) return i + 1;
                    }
                }
                for (int i = 0; i < _entries.Count; i++)
                {
                    if (_entries[i].component == comp) return i + 1;
                }
                return 0;
            }


            public override Component GetComponentAtPopupIndex(int index)
            {
                if (index <= 0) return null;

                if (index < 1 || index > _entries.Count)
                {
                    if (this.TriggerIdProp != null) this.TriggerIdProp.stringValue = null;
                    return null;
                }

                return _entries[index - 1].component;
            }


            private struct TriggerEntry
            {
                public Component component;
                public string triggerId;
                public GUIContent popupName;
            }

        }

    }
    */
}
