using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;


namespace com.spacepuppyeditor.Internal
{
    internal sealed class CachedReorderableList : ReorderableList
    {

        public GUIContent Label;

        private CachedReorderableList(SerializedObject serializedObj, SerializedProperty property)
            : base(serializedObj, property)
        {
        }


        #region Static Factory

        private static Dictionary<int, CachedReorderableList> _lstCache = new Dictionary<int, CachedReorderableList>();
        public static CachedReorderableList GetListDrawer(SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException("property");
            if (!property.isArray) throw new System.ArgumentException("SerializedProperty must be a property for an Array or List", "property");

            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            CachedReorderableList lst;
            if (_lstCache.TryGetValue(hash, out lst))
            {
                lst.serializedProperty = property;
                return lst;
            }

            lst = new CachedReorderableList(property.serializedObject, property);
            _lstCache[hash] = lst;
            return lst;
        }

        #endregion

    }
}
