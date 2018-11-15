using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(WeightedValueCollectionAttribute))]
    public class WeightedValueCollectionPropertyDrawer : ReorderableArrayPropertyDrawer
    {

        #region Fields

        public bool ManuallyConfigured;
        public string WeightPropertyName = "Weight";

        private float _totalWeight;

        public string ValuePropertyName
        {
            get { return this.ChildPropertyAsEntry; }
            set { this.ChildPropertyAsEntry = value; }
        }

        #endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isArray)
            {
                return SPEditorGUI.GetDefaultPropertyHeight(property, label);
            }

            if (!this.ManuallyConfigured && !(this.attribute is WeightedValueCollectionAttribute))
            {
                return SPEditorGUI.GetDefaultPropertyHeight(property, label);
            }

            return base.GetPropertyHeight(property, label);
        }

        protected override float GetElementHeight(SerializedProperty element, GUIContent label, bool elementIsAtBottom)
        {
            if (elementIsAtBottom)
            {
                return base.GetElementHeight(element, label, elementIsAtBottom);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _totalWeight = 0f;
            if (!property.isArray)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            if (!this.ManuallyConfigured)
            {
                var attrib = this.attribute as WeightedValueCollectionAttribute;
                if (attrib == null)
                {
                    SPEditorGUI.DefaultPropertyField(position, property, label);
                    return;
                }
                else
                {
                    this.WeightPropertyName = attrib.WeightPropertyName;
                }
            }

            for (int i = 0; i < property.arraySize; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                var weightProp = element.FindPropertyRelative(this.WeightPropertyName);
                if (weightProp != null && weightProp.propertyType == SerializedPropertyType.Float)
                {
                    _totalWeight += weightProp.floatValue;
                }
            }

            base.OnGUI(position, property, label);
        }

        #region Reorderable List

        protected override CachedReorderableList GetList(SerializedProperty property, GUIContent label)
        {
            var result = base.GetList(property, label);
            result.onAddCallback = (lst) =>
            {
                ReorderableList.defaultBehaviours.DoAddButton(lst);
                if (lst.serializedProperty != null && lst.serializedProperty.arraySize > 0)
                {
                    var element = lst.serializedProperty.GetArrayElementAtIndex(lst.serializedProperty.arraySize - 1);
                    if (element != null) this.SetWeight(element, 1f);
                }
            };
            return result;
        }

        protected override void DrawElement(Rect area, SerializedProperty element, GUIContent label, int elementIndex)
        {
            var weightProp = element.FindPropertyRelative(this.WeightPropertyName);
            if (weightProp == null || weightProp.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(area, EditorHelper.TempContent("Malformed DataType for WeightValueCollection"));
                return;
            }

            //DO DRAW
            const float MARGIN = 1.0f;
            const float WEIGHT_FIELD_WIDTH = 60f;
            const float PERC_FIELD_WIDTH = 45f;
            const float FULLWEIGHT_WIDTH = WEIGHT_FIELD_WIDTH + PERC_FIELD_WIDTH;

            Rect valueRect;
            if (area.width > FULLWEIGHT_WIDTH)
            {
                var top = area.yMin + MARGIN;
                var labelRect = new Rect(area.xMin, top, EditorGUIUtility.labelWidth - FULLWEIGHT_WIDTH, EditorGUIUtility.singleLineHeight);
                var weightRect = new Rect(area.xMin + EditorGUIUtility.labelWidth - FULLWEIGHT_WIDTH, top, WEIGHT_FIELD_WIDTH, EditorGUIUtility.singleLineHeight);
                var percRect = new Rect(area.xMin + EditorGUIUtility.labelWidth - PERC_FIELD_WIDTH, top, PERC_FIELD_WIDTH, EditorGUIUtility.singleLineHeight);
                valueRect = new Rect(area.xMin + EditorGUIUtility.labelWidth, top, area.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

                float weight = weightProp.floatValue;

                EditorGUI.LabelField(labelRect, label);
                weightProp.floatValue = EditorGUI.FloatField(weightRect, weight);
                float p = (_totalWeight > 0f) ? (100f * weight / _totalWeight) : ((elementIndex == 0) ? 100f : 0f);
                EditorGUI.LabelField(percRect, string.Format("{0:0.#}%", p));
            }
            else
            {
                //Draw Triggerable - this is the simple case to make a clean designer set up for newbs
                var top = area.yMin + MARGIN;
                var labelRect = new Rect(area.xMin, top, area.width, EditorGUIUtility.singleLineHeight);

                valueRect = EditorGUI.PrefixLabel(labelRect, label);
            }

            this.DrawElementValue(valueRect, element, label, elementIndex);
        }

        protected virtual void DrawElementValue(Rect area, SerializedProperty element, GUIContent label, int elementIndex)
        {
            var valueProp = element.FindPropertyRelative(this.ValuePropertyName);
            if (valueProp != null)
            {
                SPEditorGUI.PropertyField(area, valueProp, GUIContent.none);
            }
        }

        #endregion


        #region Drag & Drop

        protected override void DoDragAndDrop(SerializedProperty property, Rect listArea)
        {
            if (this.AllowDragAndDrop && this.fieldInfo != null && Event.current != null)
            {
                var ev = Event.current;
                switch (ev.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        {
                            if (listArea.Contains(ev.mousePosition))
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected; //default

                                var valueMember = com.spacepuppy.Dynamic.DynamicUtil.GetMemberFromType(TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType), this.ValuePropertyName, true);
                                if (valueMember == null) return;
                                var tp = com.spacepuppy.Dynamic.DynamicUtil.GetReturnType(valueMember);
                                if (tp == null) return;

                                var refs = (from o in DragAndDrop.objectReferences let obj = ObjUtil.GetAsFromSource(tp, o, false) where obj != null select obj);
                                DragAndDrop.visualMode = refs.Any() ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;

                                if (ev.type == EventType.DragPerform && refs.Any())
                                {
                                    DragAndDrop.AcceptDrag();
                                    this.AddObjectsToArray(property, refs.ToArray());
                                    GUI.changed = true;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void AddObjectsToArray(SerializedProperty listProp, object[] objs)
        {
            if (listProp == null) throw new System.ArgumentNullException("listProp");
            if (!listProp.isArray) throw new System.ArgumentException("Must be a SerializedProperty for an array/list.", "listProp");
            if (objs == null || objs.Length == 0) return;

            try
            {
                int start = listProp.arraySize;
                listProp.arraySize += objs.Length;
                for (int i = 0; i < objs.Length; i++)
                {
                    var element = listProp.GetArrayElementAtIndex(start + i);
                    if(element != null)
                    {
                        this.SetWeight(element, 1f);
                        var objProp = element.FindPropertyRelative(this.ValuePropertyName);
                        if (objProp != null && objProp.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            objProp.objectReferenceValue = objs[i] as UnityEngine.Object;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Static Utils

        private void SetWeight(SerializedProperty element, float value)
        {
            var weight = element.FindPropertyRelative(this.WeightPropertyName);
            if (weight == null) return;

            switch (weight.propertyType)
            {
                case SerializedPropertyType.Float:
                    weight.floatValue = value;
                    break;
                case SerializedPropertyType.Integer:
                    weight.intValue = (int)value;
                    break;
            }
        }

        #endregion

    }

}
