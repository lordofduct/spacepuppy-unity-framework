using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;

using com.spacepuppy.Utils;


namespace com.spacepuppyeditor.Internal
{

    public class SPReorderableList : ReorderableList
    {

        #region CONSTRUCTOR

        public SPReorderableList(SerializedObject serializedObj, SerializedProperty property)
            : base(serializedObj, property)
        {

        }
        public SPReorderableList(System.Collections.IList elements, System.Type elementType)
            : base(elements, elementType)
        {

        }
        public SPReorderableList(System.Collections.IList elements, System.Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
            : base(elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {

        }
        public SPReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
            : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {

        }

        #endregion

        #region Methods

        public new void DoLayoutList()
        {
            //var beforeRect = GUILayoutUtility.GetLastRect();
            //base.DoLayoutList();
            //var afterRect = GUILayoutUtility.GetLastRect();

            //var area = new Rect(afterRect.xMin, beforeRect.yMax, afterRect.width, this.headerHeight);
            //this.DoHeaderContextMenu(area);

            var area = EditorGUILayout.GetControlRect(false, this.GetHeight());
            this.DoList(area);

            var headerArea = new Rect(area.xMin, area.yMin, area.width, this.headerHeight);
            this.DoHeaderContextMenu(headerArea);
        }

        public new void DoList(Rect rect)
        {
            base.DoList(rect);

            var area = new Rect(rect.xMin, rect.yMin, rect.width, this.headerHeight);
            this.DoHeaderContextMenu(area);
        }

        private void DoHeaderContextMenu(Rect area)
        {
            if(ReorderableListHelper.IsClickingArea(area, MouseUtil.BTN_RIGHT))
            {
                Event.current.Use();

                if(this.serializedProperty != null)
                {
                    var menu = new GenericMenu();
                    var prop = this.serializedProperty;
                    menu.AddItem(new GUIContent("Clear"), false, () =>
                    {
                        prop.serializedObject.Update();
                        prop.arraySize = 0;
                        prop.serializedObject.ApplyModifiedProperties();
                    });
                    menu.ShowAsContext();
                }
                else if(this.list != null)
                {

                    var menu = new GenericMenu();
                    var lst = this.list;
                    menu.AddItem(new GUIContent("Clear"), false, () =>
                    {
                        lst.Clear();
                    });
                    menu.ShowAsContext();
                }
            }
        }

        #endregion

    }

    public sealed class CachedReorderableList : SPReorderableList
    {

        public GUIContent Label;

        private CachedReorderableList(SerializedObject serializedObj, SerializedProperty property)
            : base(serializedObj, property)
        {
        }

        private CachedReorderableList(System.Collections.IList memberList)
            : base(memberList, null)
        {
        }

        #region Methods

        private static FieldInfo _m_SerializedObject;
        private void ReInit(SerializedObject obj, SerializedProperty prop)
        {
            try
            {
                if (_m_SerializedObject == null)
                    _m_SerializedObject = typeof(ReorderableList).GetField("m_SerializedObject", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                _m_SerializedObject.SetValue(this, obj);
            }
            catch
            {
                UnityEngine.Debug.LogWarning("This version of Spacepuppy Framework does not support the version of Unity it's being used with (CachedReorderableList).");
            }

            this.serializedProperty = prop;
            this.list = null;
        }

        private void ReInit(System.Collections.IList memberList)
        {
            try
            {
                if (_m_SerializedObject == null)
                    _m_SerializedObject = typeof(ReorderableList).GetField("m_SerializedObject", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                _m_SerializedObject.SetValue(this, null);
            }
            catch
            {
                UnityEngine.Debug.LogWarning("This version of Spacepuppy Framework does not support the version of Unity it's being used with (CachedReorderableList).");
            }

            this.serializedProperty = null;
            this.list = memberList;
        }

        #endregion


        #region Static Factory

        private static Dictionary<int, CachedReorderableList> _lstCache = new Dictionary<int, CachedReorderableList>();

        public static CachedReorderableList GetListDrawer(SerializedProperty property, ReorderableList.HeaderCallbackDelegate drawHeaderCallback, ReorderableList.ElementCallbackDelegate drawElementCallback,
                                                          ReorderableList.AddCallbackDelegate onAddCallback = null, ReorderableList.RemoveCallbackDelegate onRemoveCallback = null, ReorderableList.SelectCallbackDelegate onSelectCallback = null,
                                                          ReorderableList.ChangedCallbackDelegate onChangedCallback = null, ReorderableList.ReorderCallbackDelegate onReorderCallback = null, ReorderableList.CanRemoveCallbackDelegate onCanRemoveCallback = null,
                                                          ReorderableList.AddDropdownCallbackDelegate onAddDropdownCallback = null)
        {
            if (property == null) throw new System.ArgumentNullException("property");
            if (!property.isArray) throw new System.ArgumentException("SerializedProperty must be a property for an Array or List", "property");

            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetIndexRespectingPropertyHash(property);
            CachedReorderableList lst;
            if (_lstCache.TryGetValue(hash, out lst))
            {
                lst.ReInit(property.serializedObject, property);
            }
            else
            {
                lst = new CachedReorderableList(property.serializedObject, property);
                _lstCache[hash] = lst;
            }

            lst.drawHeaderCallback = drawHeaderCallback;
            lst.drawElementCallback = drawElementCallback;
            lst.onAddCallback = onAddCallback;
            lst.onRemoveCallback = onRemoveCallback;
            lst.onSelectCallback = onSelectCallback;
            lst.onChangedCallback = onChangedCallback;
            lst.onReorderCallback = onReorderCallback;
            lst.onCanRemoveCallback = onCanRemoveCallback;
            lst.onAddDropdownCallback = onAddDropdownCallback;

            return lst;
        }

        /// <summary>
        /// Creates a cached ReorderableList that can be used on a IList. The serializedProperty passed is used for look-up and is not used in the ReorderableList itself.
        /// </summary>
        /// <param name="memberList"></param>
        /// <param name="tokenProperty"></param>
        /// <param name="drawHeaderCallback"></param>
        /// <param name="drawElementCallback"></param>
        /// <param name="onAddCallback"></param>
        /// <param name="onRemoveCallback"></param>
        /// <param name="onSelectCallback"></param>
        /// <param name="onChangedCallback"></param>
        /// <param name="onReorderCallback"></param>
        /// <param name="onCanRemoveCallback"></param>
        /// <param name="onAddDropdownCallback"></param>
        /// <returns></returns>
        public static CachedReorderableList GetListDrawer(System.Collections.IList memberList, SerializedProperty tokenProperty, ReorderableList.HeaderCallbackDelegate drawHeaderCallback, ReorderableList.ElementCallbackDelegate drawElementCallback,
                                                  ReorderableList.AddCallbackDelegate onAddCallback = null, ReorderableList.RemoveCallbackDelegate onRemoveCallback = null, ReorderableList.SelectCallbackDelegate onSelectCallback = null,
                                                  ReorderableList.ChangedCallbackDelegate onChangedCallback = null, ReorderableList.ReorderCallbackDelegate onReorderCallback = null, ReorderableList.CanRemoveCallbackDelegate onCanRemoveCallback = null,
                                                  ReorderableList.AddDropdownCallbackDelegate onAddDropdownCallback = null)
        {
            if (memberList == null) throw new System.ArgumentNullException("memberList");
            if (tokenProperty == null) throw new System.ArgumentNullException("property");

            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetIndexRespectingPropertyHash(tokenProperty);
            CachedReorderableList lst;
            if (_lstCache.TryGetValue(hash, out lst))
            {
                lst.ReInit(memberList);
            }
            else
            {
                lst = new CachedReorderableList(memberList);
                _lstCache[hash] = lst;
            }

            lst.drawHeaderCallback = drawHeaderCallback;
            lst.drawElementCallback = drawElementCallback;
            lst.onAddCallback = onAddCallback;
            lst.onRemoveCallback = onRemoveCallback;
            lst.onSelectCallback = onSelectCallback;
            lst.onChangedCallback = onChangedCallback;
            lst.onReorderCallback = onReorderCallback;
            lst.onCanRemoveCallback = onCanRemoveCallback;
            lst.onAddDropdownCallback = onAddDropdownCallback;

            return lst;
        }

        #endregion

    }
}
