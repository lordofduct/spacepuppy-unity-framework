using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Scenario
{

    [CustomPropertyDrawer(typeof(ObservableTargetData))]
    public class ObservableTargetDataPropertyDrawer : PropertyDrawer
    {

        #region Fields

        private SelectableComponentPropertyDrawer _componentDrawer = new SelectableComponentPropertyDrawer()
        {
            AllowSceneObject = true,
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
            return triggerProp.objectReferenceValue != null && triggerProp.objectReferenceValue is IObservableTrigger && (triggerProp.objectReferenceValue as IObservableTrigger).IsComplex;
        }




        private class CustomComponentChoiceSelector : DefaultComponentChoiceSelector, IComponentChoiceSelector
        {

            public SerializedProperty TriggerIdProp;
            private List<GUIContent> _names = new List<GUIContent>();
            private string[] _componentNames;

            protected override void OnBeforeGUI()
            {
                _componentNames = DefaultComponentChoiceSelector.GetUniqueComponentNames(this.Components).ToArray();
                _names.Clear();
                _names.Add(new GUIContent("Nothing..."));
                Component c;
                string nm;
                for (int i = 0; i < this.Components.Length; i++)
                {
                    c = this.Components[i];
                    nm = _componentNames[i];
                    if (c is IObservableTrigger && (c as IObservableTrigger).IsComplex)
                    {
                        var arr = (c as IObservableTrigger).GetComplexIds();
                        for (int j = 0; j < arr.Length; j++)
                        {
                            _names.Add(new GUIContent(nm + "/" + arr[j]));
                        }
                    }
                    else
                    {
                        _names.Add(new GUIContent(nm));
                    }
                }
            }

            public override GUIContent[] GetPopupEntries()
            {
                if (_componentNames == null) return new GUIContent[] { };

                return _names.ToArray();
            }

            public override int GetPopupIndexOfComponent(Component comp)
            {
                if (_componentNames == null) return -1;

                int entryIndex = this.Components.IndexOf(comp);
                if (this.TriggerIdProp != null && !string.IsNullOrEmpty(this.TriggerIdProp.stringValue))
                {
                    var nm = _componentNames[entryIndex] + "/" + this.TriggerIdProp.stringValue;
                    for (int i = 0; i < _names.Count; i++)
                    {
                        if (_names[i].text == nm) return i;
                    }
                }
                for (int i = 0; i < _names.Count; i++)
                {
                    if (_names[i].text.StartsWith(_componentNames[entryIndex])) return i;
                }
                return -1;
            }


            public override Component GetComponentAtPopupIndex(int index)
            {
                if (_componentNames == null) return null;
                if (index == 0) return null;

                if (index < 0 || index >= _names.Count)
                {
                    if (this.TriggerIdProp != null) this.TriggerIdProp.stringValue = null;
                    return null;
                }

                var componentNames = DefaultComponentChoiceSelector.GetUniqueComponentNames(this.Components).ToArray();
                var arr = StringUtil.SplitFixedLength(_names[index].text, "/", 2);

                for (int i = 0; i < _names.Count; i++)
                {
                    if(componentNames[i] == arr[0])
                    {
                        if (this.TriggerIdProp != null) this.TriggerIdProp.stringValue = arr[1];
                        return this.Components[i];
                    }
                }

                if (this.TriggerIdProp != null) this.TriggerIdProp.stringValue = null;
                return null;
            }

        }

    }

}
