using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Project
{

    [CustomPropertyDrawer(typeof(TextRef), true)]
    public class TextRefPropertyDrawer : PropertyDrawer
    {

        public const string PROP_TEXT = "_text";
        public const string PROP_OBJ = "_obj";

        private const float MARGIN_TOP = 3f;
        private const float MARGIN_BOTTOM = 2f;
        private const float MARGIN_TEXT = 2f;
        private const int TEXTSRC_MAX_SHOWN = 3;
        private const int TEXTSRC_AREA_LINES = 3;
        
        private enum Mode
        {
            Text,
            TextSource,
            TextAsset
        }

        #region Fields

        private bool _disallowFoldout;
        private System.Action<Rect, SerializedProperty, GUIContent, int> _elementLabelCallback;

        private ReorderableList _lst;
        private SerializedProperty _textProp;
        private SerializedProperty _objProp;
        private GUIContent _label;
        private Mode _mode;
        private bool _manuallyConfigured;

        #endregion

        #region CONSTRUCTOR

        public TextRefPropertyDrawer()
        {

        }

        public TextRefPropertyDrawer(bool disallowFoldout = false, System.Action<Rect, SerializedProperty, GUIContent, int> elementLabelCallback = null)
        {
            this.DisallowFoldout = disallowFoldout;
            this.ElementLabelCallback = elementLabelCallback;
            _manuallyConfigured = true;
        }

        #endregion

        #region Properties

        public bool DisallowFoldout
        {
            get { return _disallowFoldout; }
            set
            {
                _disallowFoldout = value;
                _manuallyConfigured = true;
            }
        }

        public System.Action<Rect, SerializedProperty, GUIContent, int> ElementLabelCallback
        {
            get { return _elementLabelCallback; }
            set
            {
                _elementLabelCallback = value;
                _manuallyConfigured = true;
            }
        }

        #endregion

        #region PropertyDrawer Interface

        private void ConfigureSettings()
        {
            if (_manuallyConfigured) return;

            bool done = false;
            if (this.fieldInfo != null)
            {
                var attrib = this.fieldInfo.GetCustomAttributes(typeof(TextRef.ConfigAttribute), true).FirstOrDefault() as TextRef.ConfigAttribute;
                if (attrib != null)
                {
                    _disallowFoldout = attrib.DisallowFoldout;
                    done = true;
                }
            }

            if (!done)
            {
                _disallowFoldout = false;
                this.ElementLabelCallback = null;
            }
        }

        private void OnGUIStart(SerializedProperty property, GUIContent label)
        {
            _textProp = property.FindPropertyRelative(PROP_TEXT);
            _objProp = property.FindPropertyRelative(PROP_OBJ);
            _label = label;
            _lst = CachedReorderableList.GetListDrawer(_textProp, _maskList_DrawHeader, _maskList_DrawElement);
            _lst.headerHeight = EditorGUIUtility.singleLineHeight + 2f;

            if (_objProp.objectReferenceValue == null)
            {
                _mode = Mode.Text;
                _lst.displayAdd = true;
                _lst.displayRemove = true;
                _lst.elementHeight = EditorGUIUtility.singleLineHeight * TEXTSRC_AREA_LINES + EditorGUIUtility.singleLineHeight + 3f;
            }
            else if (_objProp.objectReferenceValue is ITextSource)
            {
                _mode = Mode.TextSource;
                var src = _objProp.objectReferenceValue as ITextSource;
                int cnt = src.Count;
                _textProp.arraySize = cnt;

                _lst.displayAdd = false;
                _lst.displayRemove = false;
                _lst.elementHeight = EditorGUIUtility.singleLineHeight * TEXTSRC_AREA_LINES + EditorGUIUtility.singleLineHeight + 3f;
            }
            else if (_objProp.objectReferenceValue is TextAsset)
            {
                _mode = Mode.TextAsset;
                _textProp.arraySize = 1;

                _lst.displayAdd = false;
                _lst.displayRemove = false;
                _lst.elementHeight = EditorGUIUtility.singleLineHeight * TEXTSRC_AREA_LINES + EditorGUIUtility.singleLineHeight + 3f;
            }
        }

        private void OnGUIEnd()
        {
            if(_lst != null)
            {
                _lst.serializedProperty = null;
            }
            if(_textProp != null)
            {
                if (_mode != Mode.Text)
                    _textProp.arraySize = 0;
            }

            _lst = null;
            _textProp = null;
            _objProp = null;
            _label = null;
            _mode = Mode.Text;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.ConfigureSettings();

            if (_disallowFoldout || property.isExpanded)
            {
                float h = 0f;

                var objProp = property.FindPropertyRelative(PROP_OBJ);
                var textProp = property.FindPropertyRelative(PROP_TEXT);
                var lst = CachedReorderableList.GetListDrawer(textProp, _maskList_DrawHeader, _maskList_DrawElement);
                lst.headerHeight = EditorGUIUtility.singleLineHeight + 2f;

                if (objProp.objectReferenceValue == null)
                {
                    lst.displayAdd = true;
                    lst.displayRemove = true;
                    lst.elementHeight = EditorGUIUtility.singleLineHeight * TEXTSRC_AREA_LINES + EditorGUIUtility.singleLineHeight + 3f;
                    h += lst.GetHeight();
                }
                else if (objProp.objectReferenceValue is ITextSource)
                {
                    var src = objProp.objectReferenceValue as ITextSource;
                    int cnt = src.Count;
                    textProp.arraySize = cnt;

                    lst.displayAdd = false;
                    lst.displayRemove = false;
                    lst.elementHeight = EditorGUIUtility.singleLineHeight * TEXTSRC_AREA_LINES + EditorGUIUtility.singleLineHeight + 3f;
                    h += lst.GetHeight();
                }
                else if (objProp.objectReferenceValue is TextAsset)
                {
                    textProp.arraySize = 1;

                    lst.displayAdd = false;
                    lst.displayRemove = false;
                    lst.elementHeight = EditorGUIUtility.singleLineHeight * TEXTSRC_AREA_LINES + EditorGUIUtility.singleLineHeight + 3f;
                    h += lst.GetHeight();
                }

                return h;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.ConfigureSettings();

            if (_disallowFoldout)
            {
                this.OnGUIStart(property, label);
                _lst.DoList(position);
                this.OnGUIEnd();
            }
            else
            {
                if(property.isExpanded)
                {
                    this.OnGUIStart(property, label);
                    var foldoutRect = new Rect(position.xMin, position.yMin, 20f, EditorGUIUtility.singleLineHeight);
                    property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
                    _lst.DoList(position);
                    this.OnGUIEnd();
                }
                else
                {
                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
                    ReorderableListHelper.DrawRetractedHeader(position, label);
                }
            }
        }

        #endregion

        #region Text ReorderableList Handlers

        private void _maskList_DrawHeader(Rect area)
        {
            var objArea = new Rect(area.xMin, area.yMin + 1f, area.width, EditorGUIUtility.singleLineHeight);
            //_objProp.objectReferenceValue = EditorGUI.ObjectField(objArea, _label, _objProp.objectReferenceValue, typeof(UnityEngine.Object), true);
            _objProp.objectReferenceValue = SPEditorGUI.ObjectFieldX(objArea, _label, _objProp.objectReferenceValue, typeof(UnityEngine.Object), true);
        }

        private void _maskList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _textProp.GetArrayElementAtIndex(index);

            switch (_mode)
            {
                case Mode.Text:
                    {
                        var labelRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
                        var textRect = new Rect(area.xMin, area.yMin + labelRect.height + 2f, area.width, area.height - labelRect.height - 3f);

                        var label = EditorHelper.TempContent("Element " + index.ToString("00"));
                        if (this.ElementLabelCallback != null)
                            this.ElementLabelCallback(labelRect, element, label, index);
                        else
                            EditorGUI.LabelField(labelRect, label);

                        element.stringValue = EditorGUI.TextArea(textRect, element.stringValue);
                    }
                    break;
                case Mode.TextSource:
                    {
                        var src = _objProp.objectReferenceValue as ITextSource;
                        if (src == null) return;

                        var labelRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
                        var textRect = new Rect(area.xMin, area.yMin + labelRect.height + 2f, area.width, area.height - labelRect.height - 3f);

                        var label = EditorHelper.TempContent("Element " + index.ToString("00"));
                        if (this.ElementLabelCallback != null)
                            this.ElementLabelCallback(labelRect, element, label, index);
                        else
                            EditorGUI.LabelField(labelRect, label);

                        EditorGUI.TextArea(textRect, src[index]);
                    }
                    break;
                case Mode.TextAsset:
                    {
                        var src = _objProp.objectReferenceValue as TextAsset;
                        if (src == null) return;

                        var labelRect = new Rect(area.xMin, area.yMin, area.width, EditorGUIUtility.singleLineHeight);
                        var textRect = new Rect(area.xMin, area.yMin + labelRect.height + 2f, area.width, area.height - labelRect.height - 3f);

                        var label = EditorHelper.TempContent("Element " + index.ToString("00"));
                        if (this.ElementLabelCallback != null)
                            this.ElementLabelCallback(labelRect, element, label, index);
                        else
                            EditorGUI.LabelField(labelRect, label);

                        EditorGUI.TextArea(textRect, src.text);
                    }
                    break;
            }

        }

        #endregion
        
    }

}
