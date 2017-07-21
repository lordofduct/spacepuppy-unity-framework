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

        private PropertyDrawer _internalDrawer;

        #endregion

        #region CONSTRUCTOR

        private CachedReorderableList GetList(SerializedProperty property, GUIContent label)
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
            }

            _label = label;

            _lst = this.GetList(property, label);
            if (_lst.index >= _lst.count) _lst.index = -1;
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

                if(_disallowFoldout)
                {
                    this.StartOnGUI(property, label);
                    //_lst.DoList(EditorGUI.IndentedRect(position));
                    _lst.DoList(position);
                    this.EndOnGUI(property, label);
                }
                else
                {
                    if (property.isExpanded)
                    {
                        this.StartOnGUI(property, label);
                        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
                        //_lst.DoList(EditorGUI.IndentedRect(position));
                        _lst.DoList(position);
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
                EditorGUI.PropertyField(position, property, label, false);
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
            if(attrib != null)
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

            if(_drawElementAtBottom)
            {
                SerializedProperty prop = string.IsNullOrEmpty(_childPropertyAsEntry) ? null : element.FindPropertyRelative(_childPropertyAsEntry);

                if(prop != null)
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

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_lst, area, index, isActive, isFocused);
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

        #endregion

    }
}
