using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(ReorderableArrayAttribute))]
    public class ReorderableArrayPropertyDrawer : PropertyDrawer, IArrayHandlingPropertyDrawer
    {

        public delegate string FormatElementLabelCallback(SerializedProperty property, int index, bool isActive, bool isFocused);

        private static readonly float TOP_PAD = 2f + EditorGUIUtility.singleLineHeight;
        private const float BOTTOM_PAD = 2f;
        private const float MARGIN = 2f;

        #region Fields

        public string Label;
        private CachedReorderableList _lst;
        private GUIContent _label;
        private bool _disallowFoldout;
        private bool _removeBackgroundWhenCollapsed;
        private bool _draggable = true;
        private bool _drawElementAtBottom;
        private bool _hideElementLabel = false;
        private string _childPropertyAsLabel;
        private string _childPropertyAsEntry;
        private ReorderableList.AddCallbackDelegate _addCallback;
        private bool _allowDragAndDrop = true;

        private PropertyDrawer _internalDrawer;
        
        #endregion

        #region CONSTRUCTOR

        public ReorderableArrayPropertyDrawer()
        {

        }

        /// <summary>
        /// Use this to set the element type of the list for drag & drop, if you're manually calling the drawer.
        /// </summary>
        /// <param name="elementType"></param>
        public ReorderableArrayPropertyDrawer(System.Type dragDropElementType)
        {
            this.DragDropElementType = dragDropElementType;
        }


        protected virtual CachedReorderableList GetList(SerializedProperty property, GUIContent label)
        {
            var lst = CachedReorderableList.GetListDrawer(property, _maskList_DrawHeader, _maskList_DrawElement, _addCallback);
            lst.draggable = _draggable;

            if(property.arraySize > 0)
            {
                if(_drawElementAtBottom)
                {
                    lst.elementHeight = EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    var pchild = property.GetArrayElementAtIndex(0);
                    /*
                    if (_internalDrawer != null)
                    {
                        lst.elementHeight = _internalDrawer.GetPropertyHeight(pchild, label);
                    }
                    else if(ElementIsFlatChildField(pchild))
                    {
                        //we don't draw this way if it's a built-in type from Unity
                        pchild.isExpanded = true;
                        if(_hideElementLabel)
                        {
                            lst.elementHeight = SPEditorGUI.GetDefaultPropertyHeight(pchild, label, true) + 2f - EditorGUIUtility.singleLineHeight;
                        }
                        else
                        {
                            lst.elementHeight = SPEditorGUI.GetDefaultPropertyHeight(pchild, label, true) + 2f; //height when showing label
                        }
                    }
                    else
                    {
                        lst.elementHeight = SPEditorGUI.GetDefaultPropertyHeight(pchild, label) + 1f;
                    }
                    */
                    lst.elementHeight = this.GetElementHeight(pchild, label, false) + 2f;
                }
            }
            else
            {
                lst.elementHeight = EditorGUIUtility.singleLineHeight;
            }

            return lst;
        }

        private void StartOnGUI(SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as ReorderableArrayAttribute;
            if (attrib != null)
            {
                _disallowFoldout = attrib.DisallowFoldout;
                _removeBackgroundWhenCollapsed = attrib.RemoveBackgroundWhenCollapsed;
                _draggable = attrib.Draggable;
                _drawElementAtBottom = attrib.DrawElementAtBottom;
                _hideElementLabel = attrib.HideElementLabel;
                _childPropertyAsLabel = attrib.ChildPropertyToDrawAsElementLabel;
                _childPropertyAsEntry = attrib.ChildPropertyToDrawAsElementEntry;
                _allowDragAndDrop = attrib.AllowDragAndDrop;
                if (!string.IsNullOrEmpty(attrib.OnAddCallback))
                {
                    _addCallback = (lst) =>
                    {
                        lst.serializedProperty.arraySize++;
                        lst.index = lst.serializedProperty.arraySize - 1;
                        lst.serializedProperty.serializedObject.ApplyModifiedProperties();

                        var prop = lst.serializedProperty.GetArrayElementAtIndex(lst.index);
                        var obj = EditorHelper.GetTargetObjectOfProperty(prop);
                        obj = com.spacepuppy.Dynamic.DynamicUtil.InvokeMethod(lst.serializedProperty.serializedObject.targetObject, attrib.OnAddCallback, obj);
                        EditorHelper.SetTargetObjectOfProperty(prop, obj);
                        lst.serializedProperty.serializedObject.Update();
                    };
                }
                else
                {
                    _addCallback = null;
                }
            }

            _label = label;

            _lst = this.GetList(property, label);
            if (_lst.index >= _lst.count) _lst.index = -1;

            if (this.fieldInfo != null)
            {
                this.DragDropElementType = TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType);

                if (!string.IsNullOrEmpty(_childPropertyAsEntry) && this.DragDropElementType != null)
                {
                    var field = this.DragDropElementType.GetMember(_childPropertyAsEntry,
                                                                   System.Reflection.MemberTypes.Field,
                                                                   System.Reflection.BindingFlags.Public |
                                                                   System.Reflection.BindingFlags.NonPublic |
                                                                   System.Reflection.BindingFlags.Instance).FirstOrDefault() as System.Reflection.FieldInfo;
                    if (field != null) this.DragDropElementType = field.FieldType;
                }
            }
        }

        private void EndOnGUI(SerializedProperty property, GUIContent label)
        {
            _lst.serializedProperty = null;
            _label = null;
        }

        #endregion

        #region Properties

        public bool DisallowFoldout
        {
            get { return _disallowFoldout; }
            set { _disallowFoldout = value; }
        }

        public bool RemoveBackgroundWhenCollapsed
        {
            get { return _removeBackgroundWhenCollapsed; }
            set { _removeBackgroundWhenCollapsed = value; }
        }

        public bool Draggable
        {
            get { return _draggable; }
            set { _draggable = value; }
        }

        public bool DrawElementAtBottom
        {
            get { return _drawElementAtBottom; }
            set { _drawElementAtBottom = value; }
        }

        public string ChildPropertyAsLabel
        {
            get { return _childPropertyAsLabel; }
            set { _childPropertyAsLabel = value; }
        }

        public string ChildPropertyAsEntry
        {
            get { return _childPropertyAsEntry; }
            set { _childPropertyAsEntry = value; }
        }

        public ReorderableList.AddCallbackDelegate OnAddCallback
        {
            get { return _addCallback; }
            set { _addCallback = value; }
        }
        
        public FormatElementLabelCallback FormatElementLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Can drag entries onto the inspector without needing to click + button. Only works for array/list of UnityEngine.Object sub/types.
        /// </summary>
        public bool AllowDragAndDrop
        {
            get { return _allowDragAndDrop; }
            set { _allowDragAndDrop = false; }
        }

        /// <summary>
        /// The type of the element in the array/list, will effect drag & drop filtering (unless overriden).
        /// </summary>
        public System.Type DragDropElementType
        {
            get;
            set;
        }

        #endregion

        #region OnGUI

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            if (property.isArray)
            {
                this.StartOnGUI(property, label);
                if (_disallowFoldout || property.isExpanded)
                {
                    h = _lst.GetHeight();
                    if(_drawElementAtBottom && _lst.index >= 0 && _lst.index < property.arraySize)
                    {
                        var pchild = property.GetArrayElementAtIndex(_lst.index);
                        /*
                        if (_internalDrawer != null)
                        {
                            h += _internalDrawer.GetPropertyHeight(pchild, label) + BOTTOM_PAD + TOP_PAD;
                        }
                        else if (ElementIsFlatChildField(pchild))
                        {
                            //we don't draw this way if it's a built-in type from Unity
                            pchild.isExpanded = true;
                            h += SPEditorGUI.GetDefaultPropertyHeight(pchild, label, true) + BOTTOM_PAD + TOP_PAD - EditorGUIUtility.singleLineHeight;
                        }
                        else
                        {
                            h += SPEditorGUI.GetDefaultPropertyHeight(pchild, label, false) + BOTTOM_PAD + TOP_PAD;
                        }
                        */
                        h += this.GetElementHeight(pchild, label, true) + BOTTOM_PAD + TOP_PAD;
                    }
                }
                else
                {
                    h = EditorGUIUtility.singleLineHeight;
                }
            }
            else
            {
                h = EditorGUIUtility.singleLineHeight;
            }
            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;

            if (property.isArray)
            {
                //const float WIDTH_FOLDOUT = 5f;
                var foldoutRect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                position = EditorGUI.IndentedRect(position);
                Rect listArea = position;

                if(_disallowFoldout)
                {
                    listArea = new Rect(position.xMin, position.yMin, position.width, _lst.GetHeight());
                    this.StartOnGUI(property, label);
                    //_lst.DoList(EditorGUI.IndentedRect(position));
                    _lst.DoList(listArea);
                    this.EndOnGUI(property, label);
                }
                else
                {
                    if (property.isExpanded)
                    {
                        listArea = new Rect(position.xMin, position.yMin, position.width, _lst.GetHeight());
                        this.StartOnGUI(property, label);
                        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
                        //_lst.DoList(EditorGUI.IndentedRect(position));
                        _lst.DoList(listArea);
                        this.EndOnGUI(property, label);
                    }
                    else
                    {
                        if(_removeBackgroundWhenCollapsed)
                        {
                            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
                        }
                        else
                        {
                            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
                            //ReorderableListHelper.DrawRetractedHeader(EditorGUI.IndentedRect(position), label);
                            ReorderableListHelper.DrawRetractedHeader(position, label);
                        }
                    }
                }

                this.DoDragAndDrop(property, listArea);

                if(property.isExpanded && _drawElementAtBottom && _lst.index >= 0 && _lst.index < property.arraySize)
                {
                    var pchild = property.GetArrayElementAtIndex(_lst.index);
                    var label2 = TempElementLabel(pchild, _lst.index); //(string.IsNullOrEmpty(_childPropertyAsLabel)) ? TempElementLabel(_lst.index) : GUIContent.none;

                    pchild.isExpanded = true;
                    float h;
                    if (_internalDrawer != null)
                    {
                        h = _internalDrawer.GetPropertyHeight(pchild, label2) + BOTTOM_PAD + TOP_PAD;
                    }
                    else if (pchild.hasChildren)
                    {
                        h = SPEditorGUI.GetDefaultPropertyHeight(pchild, label, true) + BOTTOM_PAD + TOP_PAD - EditorGUIUtility.singleLineHeight;
                    }
                    else
                    {
                        h = SPEditorGUI.GetDefaultPropertyHeight(pchild, label2, true) + BOTTOM_PAD + TOP_PAD;
                    }
                    var area = new Rect(position.x, position.yMax - h, position.width, h);
                    var drawArea = new Rect(area.x, area.y + TOP_PAD, area.width - MARGIN, area.height - TOP_PAD);

                    GUI.BeginGroup(area, label2, GUI.skin.box);
                    GUI.EndGroup();

                    EditorGUI.indentLevel++;
                    if (_internalDrawer != null)
                    {
                        _internalDrawer.OnGUI(drawArea, pchild, label2);
                    }
                    else if(pchild.hasChildren)
                    {
                        SPEditorGUI.FlatChildPropertyField(drawArea, pchild);
                    }
                    else
                    {
                        SPEditorGUI.DefaultPropertyField(drawArea, pchild, GUIContent.none, false);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                SPEditorGUI.DefaultPropertyField(position, property, label, false);
            }
        }

        #endregion

        #region Masks ReorderableList Handlers

        private void _maskList_DrawHeader(Rect area)
        {
            if(this.Label != null)
            {
                EditorGUI.LabelField(area, this.Label ?? _lst.serializedProperty.displayName);
            }
            else
            {
                EditorGUI.LabelField(area, _label ?? EditorHelper.TempContent(_lst.serializedProperty.displayName));
            }
        }

        private void _maskList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _lst.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;

            //EditorGUI.PropertyField(area, element, GUIContent.none, false);
            var attrib = this.attribute as ReorderableArrayAttribute;
            GUIContent label = null;
            if(this.FormatElementLabel != null)
            {
                string slbl = this.FormatElementLabel(element, index, isActive, isFocused);
                if (slbl != null) label = EditorHelper.TempContent(slbl);
            }
            else if(attrib != null)
            {
                if (attrib.ElementLabelFormatString != null)
                {
                    label = EditorHelper.TempContent(string.Format(attrib.ElementLabelFormatString, index));
                }
                if(attrib.ElementPadding > 0f)
                {
                    area = new Rect(area.xMin + attrib.ElementPadding, area.yMin, Mathf.Max(0f, area.width - attrib.ElementPadding), area.height);
                }
            }
            if (label == null) label = (_hideElementLabel) ? GUIContent.none : TempElementLabel(element, index);

            this.DrawElement(area, element, label, index);

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_lst, area, index, isActive, isFocused);
        }

        protected virtual void DrawElement(Rect area, SerializedProperty element, GUIContent label, int elementIndex)
        {

            if (_drawElementAtBottom)
            {
                SerializedProperty prop = string.IsNullOrEmpty(_childPropertyAsEntry) ? null : element.FindPropertyRelative(_childPropertyAsEntry);

                if (prop != null)
                {
                    SPEditorGUI.PropertyField(area, prop, label);
                }
                else
                {
                    EditorGUI.LabelField(area, label);
                }
            }
            else
            {
                if (_internalDrawer != null)
                {
                    _internalDrawer.OnGUI(area, element, label);
                }
                else if (ElementIsFlatChildField(element))
                {
                    //we don't draw this way if it's a built-in type from Unity

                    if (_hideElementLabel)
                    {
                        //no label
                        SPEditorGUI.FlatChildPropertyField(area, element);
                    }
                    else
                    {
                        //showing label
                        var labelArea = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(labelArea, label);
                        var childArea = new Rect(area.xMin, area.yMin + EditorGUIUtility.singleLineHeight + 1f, area.width, area.height - EditorGUIUtility.singleLineHeight);
                        SPEditorGUI.FlatChildPropertyField(childArea, element);
                    }
                }
                else
                {
                    SPEditorGUI.DefaultPropertyField(area, element, label, false);
                }
            }

        }

        protected virtual float GetElementHeight(SerializedProperty element, GUIContent label, bool elementIsAtBottom)
        {
            if (_internalDrawer != null)
            {
                return _internalDrawer.GetPropertyHeight(element, label);
            }
            else if (ElementIsFlatChildField(element))
            {
                //we don't draw this way if it's a built-in type from Unity
                element.isExpanded = true;
                if (_hideElementLabel || elementIsAtBottom)
                {
                    return SPEditorGUI.GetDefaultPropertyHeight(element, label, true) - EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    return SPEditorGUI.GetDefaultPropertyHeight(element, label, true);
                }
            }
            else
            {
                return SPEditorGUI.GetDefaultPropertyHeight(element, label, false);
            }
        }

        #endregion

        #region Drag & Drop

        protected virtual void DoDragAndDrop(SerializedProperty property, Rect listArea)
        {
            if (_allowDragAndDrop && this.DragDropElementType != null && Event.current != null)
            {
                var ev = Event.current;
                switch (ev.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        {
                            if (listArea.Contains(ev.mousePosition))
                            {
                                var refs = (from o in DragAndDrop.objectReferences let obj = ObjUtil.GetAsFromSource(this.DragDropElementType, o, false) where obj != null select obj);
                                DragAndDrop.visualMode = refs.Any() ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;

                                if (ev.type == EventType.DragPerform && refs.Any())
                                {
                                    DragAndDrop.AcceptDrag();
                                    AddObjectsToArray(property, refs.ToArray(), _childPropertyAsEntry);
                                    GUI.changed = true;
                                }
                            }
                        }
                        break;
                }
            }
        }

        #endregion


        private GUIContent TempElementLabel(SerializedProperty element, int index)
        {
            var target = EditorHelper.GetTargetObjectOfProperty(element);
            string slbl = ConvertUtil.ToString(com.spacepuppy.Dynamic.DynamicUtil.GetValue(target, _childPropertyAsLabel));

            if(string.IsNullOrEmpty(slbl))
            {
                var propLabel = (!string.IsNullOrEmpty(_childPropertyAsLabel)) ? element.FindPropertyRelative(_childPropertyAsLabel) : null;
                if (propLabel != null)
                    slbl = ConvertUtil.ToString(EditorHelper.GetPropertyValue(propLabel));
            }

            if(string.IsNullOrEmpty(slbl))
                slbl = string.Format("Element {0:00}", index);

            return EditorHelper.TempContent(slbl);
        }

        #region IArrayHandlingPropertyDrawer Interface

        PropertyDrawer IArrayHandlingPropertyDrawer.InternalDrawer
        {
            get
            {
                return _internalDrawer;
            }
            set
            {
                _internalDrawer = value;
            }
        }

        #endregion

        #region Static Utils

        private static bool ElementIsFlatChildField(SerializedProperty property)
        {
            //return property.hasChildren && property.objectReferenceValue is MonoBehaviour;
            return property.hasChildren && property.propertyType == SerializedPropertyType.Generic;
        }

        private static void AddObjectsToArray(SerializedProperty listProp, object[] objs, string optionalChildProp = null)
        {
            if (listProp == null) throw new System.ArgumentNullException("listProp");
            if (!listProp.isArray) throw new System.ArgumentException("Must be a SerializedProperty for an array/list.", "listProp");
            if (objs == null || objs.Length == 0) return;

            try
            {
                int start = listProp.arraySize;
                listProp.arraySize += objs.Length;
                for(int i = 0; i < objs.Length; i++)
                {
                    var element = listProp.GetArrayElementAtIndex(start + i);
                    if (!string.IsNullOrEmpty(optionalChildProp)) element = element.FindPropertyRelative(optionalChildProp);

                    if (element != null && element.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        element.objectReferenceValue = objs[i] as UnityEngine.Object;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #endregion

    }
}
