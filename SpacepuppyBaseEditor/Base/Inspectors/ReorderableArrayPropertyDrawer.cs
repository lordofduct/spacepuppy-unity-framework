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

        private const float BOTTOM_PAD = 2f;

        #region Fields

        public string Label;
        private ReorderableList _lst;
        private GUIContent _label;
        private bool _disallowFoldout;
        private bool _removeBackgroundWhenCollapsed;
        private bool _draggable = true;
        private bool _drawElementAtBottom;
        private string _childPropertyAsLabel;

        private PropertyDrawer _internalDrawer;

        #endregion

        #region CONSTRUCTOR

        private CachedReorderableList GetList(SerializedProperty property, GUIContent label)
        {
            var lst = CachedReorderableList.GetListDrawer(property, _maskList_DrawHeader, _maskList_DrawElement);
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
                _childPropertyAsLabel = attrib.ChildPropertyToDrawAsElementLabel;
            }
            else
            {
                _disallowFoldout = false;
                _removeBackgroundWhenCollapsed = false;
                _draggable = true;
                _drawElementAtBottom = false;
                _childPropertyAsLabel = null;
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
                        bool cache = pchild.isExpanded;
                        pchild.isExpanded = true;
                        if (_internalDrawer != null)
                        {
                            h += _internalDrawer.GetPropertyHeight(pchild, label) + BOTTOM_PAD;
                        }
                        else
                        {
                            h += SPEditorGUI.GetDefaultPropertyHeight(pchild, label, true) + BOTTOM_PAD;
                        }
                        pchild.isExpanded = cache;
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
                if(_disallowFoldout)
                {
                    this.StartOnGUI(property, label);
                    //_lst.DoList(EditorGUI.IndentedRect(position));
                    _lst.DoList(position);
                    this.EndOnGUI(property, label);
                }
                else
                {
                    const float WIDTH_FOLDOUT = 5f;
                    if (property.isExpanded)
                    {
                        this.StartOnGUI(property, label);
                        property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, WIDTH_FOLDOUT, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);
                        //_lst.DoList(EditorGUI.IndentedRect(position));
                        _lst.DoList(position);
                        this.EndOnGUI(property, label);
                    }
                    else
                    {
                        if(_removeBackgroundWhenCollapsed)
                        {
                            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
                        }
                        else
                        {
                            property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, WIDTH_FOLDOUT, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);
                            //ReorderableListHelper.DrawRetractedHeader(EditorGUI.IndentedRect(position), label);
                            ReorderableListHelper.DrawRetractedHeader(position, label);
                        }
                    }
                }

                if(_drawElementAtBottom && _lst.index >= 0 && _lst.index < property.arraySize)
                {
                    var pchild = property.GetArrayElementAtIndex(_lst.index);
                    var label2 = TempElementLabel(pchild, _lst.index); //(string.IsNullOrEmpty(_childPropertyAsLabel)) ? TempElementLabel(_lst.index) : GUIContent.none;

                    pchild.isExpanded = true;
                    float h;
                    if (_internalDrawer != null)
                    {
                        h = _internalDrawer.GetPropertyHeight(pchild, label2) + BOTTOM_PAD;
                    }
                    else
                    {
                        h = SPEditorGUI.GetDefaultPropertyHeight(pchild, label2, true) + BOTTOM_PAD;
                    }
                    var area = new Rect(position.x, position.yMax - h, position.width, h);

                    GUI.BeginGroup(area, label2, GUI.skin.box);
                    GUI.EndGroup();

                    if (_internalDrawer != null)
                    {
                        _internalDrawer.OnGUI(area, pchild, label2);
                    }
                    else
                    {
                        SPEditorGUI.DefaultPropertyField(area, pchild, GUIContent.none, true);
                    }
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
            GUIContent label = GUIContent.none;
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

            if(_drawElementAtBottom)
            {
                EditorGUI.LabelField(area, TempElementLabel(element, index));
                //var propLabel = (!string.IsNullOrEmpty(_childPropertyAsLabel)) ? element.FindPropertyRelative(_childPropertyAsLabel) : null;
                //if(propLabel != null)
                //{
                //    SPEditorGUI.PropertyField(area, propLabel);
                //}
                //else
                //{
                //    EditorGUI.LabelField(area, TempElementLabel(element, index));
                //}
            }
            else
            {
                if (_internalDrawer != null)
                {
                    _internalDrawer.OnGUI(area, element, label);
                }
                else
                {
                    SPEditorGUI.DefaultPropertyField(area, element, label, false);
                }
            }

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_lst, area, index, isActive, isFocused);
        }

        #endregion


        //private static Rect ElementIndentedRect(Rect rect)
        //{
        //    return new Rect(rect.x + 15f, rect.y, rect.width - 15f, rect.height);
        //}

        private GUIContent TempElementLabel(SerializedProperty element, int index)
        {
            var propLabel = (!string.IsNullOrEmpty(_childPropertyAsLabel)) ? element.FindPropertyRelative(_childPropertyAsLabel) : null;
            string slbl = null;
            
            if (propLabel != null)
                slbl = ConvertUtil.ToString(EditorHelper.GetPropertyValue(propLabel));

            if (string.IsNullOrEmpty(slbl))
                slbl = string.Format("Element {0:00}", index);

            return EditorHelper.TempContent(slbl);
            //return EditorHelper.TempContent(string.Format("Element {0:00}", index));
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
        
    }
}
