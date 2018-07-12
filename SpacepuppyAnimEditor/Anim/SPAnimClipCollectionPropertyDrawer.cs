using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Anim;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Anim
{

    [CustomPropertyDrawer(typeof(SPAnimClipCollection), true)]
    public class SPAnimClipCollectionPropertyDrawer : PropertyDrawer
    {

        //SPAnimationState._masks is a MaskCollection wich has a list inside it called _masks
        private const string PROP_ANIMSTATES = "_serializedStates";

        private static GUIStyle _nameLabelGUIStyle = new GUIStyle("Label")
        {
            alignment = TextAnchor.MiddleRight
        };

        #region Fields

        private CachedReorderableList _animList;

        private SPAnimClipCollection _target;
        private GUIContent _currentLabel;
        private int _defaultLayer;
        private bool _isStaticColl;
        private bool _hideDetailRegion;
        private string _entryPrefix;

        private SPAnimClipPropertyDrawer _clipDrawer = new SPAnimClipPropertyDrawer();


        #endregion

        #region CONSTRUCTOR

        private void Init(SerializedProperty property, GUIContent label, bool searchForAttribs)
        {
            _target = EditorHelper.GetTargetObjectOfProperty(property) as SPAnimClipCollection;
            _currentLabel = label;
            _defaultLayer = 0;
            _isStaticColl = false;
            _hideDetailRegion = false;

            _animList = CachedReorderableList.GetListDrawer(property.FindPropertyRelative(PROP_ANIMSTATES), _animList_DrawHeader, _animList_DrawElement, _animList_OnAdded);
            if (_animList.index >= _animList.count) _animList.index = -1;

            if (searchForAttribs && this.fieldInfo != null)
            {
                //config
                var configAttrib = this.fieldInfo.GetCustomAttributes(typeof(SPAnimClipCollection.ConfigAttribute), false).FirstOrDefault() as SPAnimClipCollection.ConfigAttribute;
                if (configAttrib != null)
                {
                    _hideDetailRegion = configAttrib.HideDetailRegion;
                    _defaultLayer = configAttrib.DefaultLayer;
                    _entryPrefix = configAttrib.Prefix;
                    if (!string.IsNullOrEmpty(configAttrib.Hash))
                        _currentLabel.text = string.Format("{0} - ({1})", _currentLabel.text, configAttrib.Hash);
                }
                else
                {
                    _entryPrefix = null;
                }

                var staticCollAttrib = this.fieldInfo.GetCustomAttributes(typeof(SPAnimClipCollection.StaticCollectionAttribute), false).FirstOrDefault() as SPAnimClipCollection.StaticCollectionAttribute;
                if (staticCollAttrib != null)
                {
                    _isStaticColl = true;
                    var statesProp = property.FindPropertyRelative(PROP_ANIMSTATES);
                    int cnt = staticCollAttrib.Names.Length;
                    if (statesProp.arraySize > cnt)
                    {
                        statesProp.arraySize = cnt;
                    }
                    else if (statesProp.arraySize < cnt)
                    {
                        int sz = statesProp.arraySize;
                        statesProp.arraySize = cnt;
                        for (int i = sz; i < cnt; i++)
                        {
                            var stateProp = statesProp.GetArrayElementAtIndex(i);
                            stateProp.FindPropertyRelative("_name").stringValue = staticCollAttrib.Names[i];
                            stateProp.FindPropertyRelative("_clip").objectReferenceValue = null;
                            stateProp.FindPropertyRelative(SPAnimClip.PROP_WEIGHT).floatValue = 1f;
                            stateProp.FindPropertyRelative(SPAnimClip.PROP_SPEED).floatValue = 1f;
                            stateProp.FindPropertyRelative(SPAnimClip.PROP_LAYER).intValue = _defaultLayer;
                            stateProp.FindPropertyRelative(SPAnimClip.PROP_WRAPMODE).intValue = 0;
                            stateProp.FindPropertyRelative(SPAnimClip.PROP_BLENDMODE).intValue = 0;
                            stateProp.FindPropertyRelative(SPAnimClip.PROP_MASK).objectReferenceValue = null;
                        }
                    }
                    for (int i = 0; i < statesProp.arraySize; i++)
                    {
                        statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue = staticCollAttrib.Names[i];
                    }

                    _animList.displayAdd = false;
                    _animList.displayRemove = false;
                    _animList.draggable = false;
                    if (_animList.index >= cnt) _animList.index = cnt - 1;
                }
                else
                {
                    _isStaticColl = false;
                    _animList.displayAdd = true;
                    _animList.displayRemove = true;
                    _animList.draggable = true;
                }
            }
        }

        #endregion

        #region Methods

        private float GetDetailHeight()
        {
            if (_hideDetailRegion) return 0f;

            if (_animList.index >= 0)
            {
                try
                {
                    return _clipDrawer.GetDetailHeight(_animList.serializedProperty.GetArrayElementAtIndex(_animList.index)) + EditorGUIUtility.singleLineHeight + 2f;
                }
                catch
                {
                    return 0f;
                }
            }
            else
            {
                return 0f;
            }
        }

        #endregion

        #region OnGUI

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            this.Init(property, label, true);

            h = 0f;
            if (!property.isExpanded)
            {
                h = EditorGUIUtility.singleLineHeight;
            }
            else
            {
                h = _animList.GetHeight() + this.GetDetailHeight();
            }

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;

            this.Init(property, label, true);
            _currentLabel = label;
            if (this.fieldInfo.DeclaringType == typeof(SPAnimationController)) _currentLabel.text = "Animation States";

            property.isExpanded = SPEditorGUI.PrefixFoldoutLabel(position, property.isExpanded, GUIContent.none);

            if (!property.isExpanded)
            {
                //here we emulate the ReorderableList header
                //ReorderableListHelper.DrawRetractedHeader(EditorGUI.IndentedRect(position), label);
                ReorderableListHelper.DrawRetractedHeader(position, label);
            }
            else
            {
                //var cache = SPGUI.DisableIfPlaying();
                _animList.displayAdd = !Application.isPlaying;
                _animList.displayRemove = !Application.isPlaying;
                _animList.draggable = !Application.isPlaying;

                EditorGUI.BeginChangeCheck();
                //_animList.DoList(EditorGUI.IndentedRect(position));
                _animList.DoList(position);
                if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();

                if (!_hideDetailRegion && _animList.index >= 0)
                {
                    try
                    {
                        var detailRect = new Rect(position.xMin, position.yMin + _animList.GetHeight(), position.width, this.GetDetailHeight());
                        var stateProp = _animList.serializedProperty.GetArrayElementAtIndex(_animList.index);
                        var nameProp = stateProp.FindPropertyRelative("_name");

                        GUI.BeginGroup(detailRect, nameProp.stringValue, GUI.skin.box);
                        GUI.EndGroup();

                        //draw basic details
                        var buffer = EditorGUIUtility.singleLineHeight + 2f;
                        detailRect = new Rect(detailRect.xMin, detailRect.yMin + buffer, detailRect.width, detailRect.height - buffer);
                        _clipDrawer.DrawDetails(detailRect, stateProp);
                    }
                    catch
                    {
                        //failed to draw details because poorly selected nonsense
                    }
                }

                //cache.Reset();
            }
        }

        #endregion

        #region Anim ReorderableList Handlers

        private void _animList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, _currentLabel);
        }

        private void _animList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _animList.serializedProperty.GetArrayElementAtIndex(index);

            _clipDrawer.DrawClip(area, element, EditorHelper.TempContent(index.ToString()), Mathf.Min(area.width * 0.08f, 30f), _nameLabelGUIStyle, _isStaticColl);

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_animList, area, index, isActive, isFocused);
        }

        private void _animList_OnAdded(ReorderableList lst)
        {
            lst.serializedProperty.arraySize++;
            lst.index = lst.serializedProperty.arraySize - 1;

            var stateProp = lst.serializedProperty.GetArrayElementAtIndex(lst.index);

            string prefix = string.IsNullOrEmpty(_entryPrefix) ? "State" : _entryPrefix;
            int cnt = lst.serializedProperty.arraySize;
            while (_target.Keys.Contains(prefix + cnt.ToString("00")))
            {
                cnt++;
            }

            stateProp.FindPropertyRelative("_name").stringValue = prefix + cnt.ToString("00");
            stateProp.FindPropertyRelative("_clip").objectReferenceValue = null;
            stateProp.FindPropertyRelative(SPAnimClip.PROP_WEIGHT).floatValue = 1f;
            stateProp.FindPropertyRelative(SPAnimClip.PROP_SPEED).floatValue = 1f;
            stateProp.FindPropertyRelative(SPAnimClip.PROP_LAYER).intValue = _defaultLayer;
            stateProp.FindPropertyRelative(SPAnimClip.PROP_WRAPMODE).intValue = 0;
            stateProp.FindPropertyRelative(SPAnimClip.PROP_BLENDMODE).intValue = 0;
            stateProp.FindPropertyRelative(SPAnimClip.PROP_MASK).objectReferenceValue = null;
        }
        
        #endregion

    }

    /*
     * OBSOLETE - held onto for posterity/ease
     * 
    [CustomPropertyDrawer(typeof(SPAnimClipCollection), true)]
    public class SPAnimClipCollectionPropertyDrawer : PropertyDrawer
    {

        //SPAnimationState._masks is a MaskCollection wich has a list inside it called _masks
        private const string PROP_ANIMSTATES = "_serializedStates";

        private static GUIStyle _nameLabelGUIStyle = new GUIStyle("Label")
                                                         {
                                                             alignment = TextAnchor.MiddleRight
                                                         };

        #region Fields

        private CachedReorderableList _animList;

        private SPAnimClipCollection _target;
        private GUIContent _currentLabel;
        private int _defaultLayer;
        private bool _isStaticColl;
        private bool _hideDetailRegion;
        private string _entryPrefix;

        private SPAnimClipPropertyDrawer _clipDrawer = new SPAnimClipPropertyDrawer();


        #endregion

        #region CONSTRUCTOR

        private void Init(SerializedProperty property, GUIContent label, bool searchForAttribs)
        {
            _target = EditorHelper.GetTargetObjectOfProperty(property) as SPAnimClipCollection;
            _currentLabel = label;
            _defaultLayer = 0;
            _isStaticColl = false;
            _hideDetailRegion = false;

            _animList = CachedReorderableList.GetListDrawer(property.FindPropertyRelative(PROP_ANIMSTATES), _animList_DrawHeader, _animList_DrawElement, _animList_OnAdded, null, _animList_OnSelect);
            if (_animList.index >= _animList.count) _animList.index = -1;

            if(searchForAttribs && this.fieldInfo != null)
            {
                //config
                var configAttrib = this.fieldInfo.GetCustomAttributes(typeof(SPAnimClipCollection.ConfigAttribute), false).FirstOrDefault() as SPAnimClipCollection.ConfigAttribute;
                if(configAttrib != null)
                {
                    _hideDetailRegion = configAttrib.HideDetailRegion;
                    _defaultLayer = configAttrib.DefaultLayer;
                    _entryPrefix = configAttrib.Prefix;
                    if(!string.IsNullOrEmpty(configAttrib.Hash))
                        _currentLabel.text= string.Format("{0} - ({1})", _currentLabel.text, configAttrib.Hash);
                }
                else
                {
                    _entryPrefix = null;
                }

                var staticCollAttrib = this.fieldInfo.GetCustomAttributes(typeof(SPAnimClipCollection.StaticCollectionAttribute), false).FirstOrDefault() as SPAnimClipCollection.StaticCollectionAttribute;
                if (staticCollAttrib != null)
                {
                    _isStaticColl = true;
                    var statesProp = property.FindPropertyRelative(PROP_ANIMSTATES);
                    int cnt = staticCollAttrib.Names.Length;
                    if (statesProp.arraySize > cnt)
                    {
                        statesProp.arraySize = cnt;
                    }
                    else if (statesProp.arraySize < cnt)
                    {
                        int sz = statesProp.arraySize;
                        statesProp.arraySize = cnt;
                        for (int i = sz; i < cnt; i++)
                        {
                            var stateProp = statesProp.GetArrayElementAtIndex(i);
                            stateProp.FindPropertyRelative("_name").stringValue = staticCollAttrib.Names[i];
                            stateProp.FindPropertyRelative("_clip").objectReferenceValue = null;
                            stateProp.FindPropertyRelative("_weight").floatValue = 1f;
                            stateProp.FindPropertyRelative("_speed").floatValue = 1f;
                            stateProp.FindPropertyRelative("_layer").intValue = _defaultLayer;
                            stateProp.FindPropertyRelative("_wrapMode").intValue = 0;
                            stateProp.FindPropertyRelative("_blendMode").intValue = 0;
                            stateProp.FindPropertyRelative(SPAnimClipPropertyDrawer.PROP_ANIMSTATE_MASKS).arraySize = 0;
                        }
                    }
                    for (int i = 0; i < statesProp.arraySize; i++)
                    {
                        statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue = staticCollAttrib.Names[i];
                    }

                    _animList.displayAdd = false;
                    _animList.displayRemove = false;
                    _animList.draggable = false;
                    if (_animList.index >= cnt) _animList.index = cnt - 1;
                }
                else
                {
                    _isStaticColl = false;
                    _animList.displayAdd = true;
                    _animList.displayRemove = true;
                    _animList.draggable = true;
                }
            }
        }

        #endregion

        #region Methods

        private float GetDetailHeight()
        {
            if (_hideDetailRegion) return 0f;

            if (_animList.index >= 0)
            {
                try
                {
                    return _clipDrawer.GetDetailHeight(_animList.serializedProperty.GetArrayElementAtIndex(_animList.index)) + EditorGUIUtility.singleLineHeight + 2f;
                }
                catch
                {
                    return 0f;
                }
            }
            else
            {
                return 0f;
            }
        }

        #endregion

        #region OnGUI

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            this.Init(property, label, true);

            h = 0f;
            if (!property.isExpanded)
            {
                h = EditorGUIUtility.singleLineHeight;
            }
            else
            {
                h = _animList.GetHeight() + this.GetDetailHeight();
            }

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;

            this.Init(property, label, true);
            _currentLabel = label;
            if (this.fieldInfo.DeclaringType == typeof(SPAnimationController)) _currentLabel.text = "Animation States";

            property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);

            if (!property.isExpanded)
            {
                //here we emulate the ReorderableList header
                //ReorderableListHelper.DrawRetractedHeader(EditorGUI.IndentedRect(position), label);
                ReorderableListHelper.DrawRetractedHeader(position, label);
            }
            else
            {
                //var cache = SPGUI.DisableIfPlaying();
                _animList.displayAdd = !Application.isPlaying;
                _animList.displayRemove = !Application.isPlaying;
                _animList.draggable = !Application.isPlaying;

                EditorGUI.BeginChangeCheck();
                //_animList.DoList(EditorGUI.IndentedRect(position));
                _animList.DoList(position);
                if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();

                if (!_hideDetailRegion && _animList.index >= 0)
                {
                    try
                    {
                        var detailRect = new Rect(position.xMin, position.yMin + _animList.GetHeight(), position.width, this.GetDetailHeight());
                        var stateProp = _animList.serializedProperty.GetArrayElementAtIndex(_animList.index);
                        var nameProp = stateProp.FindPropertyRelative("_name");

                        GUI.BeginGroup(detailRect, nameProp.stringValue, GUI.skin.box);
                        GUI.EndGroup();

                        //draw basic details
                        var buffer = EditorGUIUtility.singleLineHeight + 2f;
                        detailRect = new Rect(detailRect.xMin, detailRect.yMin + buffer, detailRect.width, detailRect.height - buffer);
                        _clipDrawer.DrawDetails(detailRect, stateProp);
                    }
                    catch
                    {
                        //failed to draw details because poorly selected nonsense
                    }
                }
                
                //cache.Reset();
            }
        }

        #endregion

        #region Anim ReorderableList Handlers

        private void _animList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, _currentLabel);
        }

        private void _animList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _animList.serializedProperty.GetArrayElementAtIndex(index);

            _clipDrawer.DrawClip(area, element, EditorHelper.TempContent(index.ToString()), Mathf.Min(area.width * 0.08f, 30f), _nameLabelGUIStyle, _isStaticColl);

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_animList, area, index, isActive, isFocused);
        }

        private void _animList_OnAdded(ReorderableList lst)
        {
            lst.serializedProperty.arraySize++;
            lst.index = lst.serializedProperty.arraySize - 1;

            var stateProp = lst.serializedProperty.GetArrayElementAtIndex(lst.index);

            string prefix = string.IsNullOrEmpty(_entryPrefix) ? "State" : _entryPrefix;
            int cnt = lst.serializedProperty.arraySize;
            while (_target.Keys.Contains(prefix + cnt.ToString("00")))
            {
                cnt++;
            }

            stateProp.FindPropertyRelative("_name").stringValue = prefix + cnt.ToString("00");
            stateProp.FindPropertyRelative("_clip").objectReferenceValue = null;
            stateProp.FindPropertyRelative("_weight").floatValue = 1f;
            stateProp.FindPropertyRelative("_speed").floatValue = 1f;
            stateProp.FindPropertyRelative("_layer").intValue = _defaultLayer;
            stateProp.FindPropertyRelative("_wrapMode").intValue = 0;
            stateProp.FindPropertyRelative("_blendMode").intValue = 0;
            stateProp.FindPropertyRelative(SPAnimClipPropertyDrawer.PROP_ANIMSTATE_MASKS).arraySize = 0;
        }

        private void _animList_OnSelect(ReorderableList lst)
        {
            _clipDrawer.Reset();
        }

        #endregion

    }
    */
}
