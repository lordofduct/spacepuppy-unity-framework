using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;


namespace com.spacepuppyeditor.Internal
{
    public sealed class CachedReorderableList : ReorderableList
    {

        public GUIContent Label;

        private CachedReorderableList(SerializedObject serializedObj, SerializedProperty property)
            : base(serializedObj, property)
        {
        }


        #region Static Factory

        private static Dictionary<int, CachedReorderableList> _lstCache = new Dictionary<int, CachedReorderableList>();

        public static CachedReorderableList GetListDrawer(SerializedProperty property, ReorderableList.HeaderCallbackDelegate drawHeaderCallback, ReorderableList.ElementCallbackDelegate drawElementCallback,
                                                          ReorderableList.AddCallbackDelegate onAddCallback = null, ReorderableList.RemoveCallbackDelegate onRemoveCallback = null, ReorderableList.SelectCallbackDelegate onSelectCallback = null,
                                                          ReorderableList.ChangedCallbackDelegate onChangedCallback = null, ReorderableList.ReorderCallbackDelegate onReorderCallback = null, ReorderableList.CanRemoveCallbackDelegate onCanRemoveCallback = null,
                                                          ReorderableList.AddDropdownCallbackDelegate onAddDropdownCallback = null)
        {
            if (property == null) throw new System.ArgumentNullException("property");
            if (!property.isArray) throw new System.ArgumentException("SerializedProperty must be a property for an Array or List", "property");

            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            CachedReorderableList lst;
            if (_lstCache.TryGetValue(hash, out lst))
            {
                lst.serializedProperty = property;
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

        #endregion

    }
}
