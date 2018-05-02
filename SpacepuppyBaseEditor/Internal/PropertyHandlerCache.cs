using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Modifiers;

namespace com.spacepuppyeditor.Internal
{
    internal class PropertyHandlerCache
    {

        private Dictionary<int, IPropertyHandler> _table = new Dictionary<int, IPropertyHandler>();


        public IPropertyHandler GetHandler(SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException("property");

            var hash = GetPropertyHash(property);
            IPropertyHandler result;
            if (_table.TryGetValue(hash, out result))
                return result;
            else
                return null;

        }

        public void SetHandler(SerializedProperty property, IPropertyHandler handler)
        {
            if (property == null) throw new System.ArgumentNullException("property");

            var hash = GetPropertyHash(property);
            if(handler == null)
            {
                if (_table.ContainsKey(hash)) _table.Remove(hash);
            }
            else
            {
                _table[hash] = handler;
            }
        }

        public void Clear()
        {
            _table.Clear();
        }

        public static int GetPropertyHash(SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException("property");
            if (property.serializedObject.targetObject == null)
                return 0;

            var spath = property.propertyPath;
            int index = spath.IndexOf(".Array.data[");
            int len = 0;
            while (index >= 0)
            {
                len = spath.IndexOf(']', index) - index;
                spath = spath.Remove(index, len);
                index = spath.IndexOf(".Array.data[");
            }

            int num = property.serializedObject.targetObject.GetInstanceID() ^ spath.GetHashCode();
            if (property.propertyType == SerializedPropertyType.ObjectReference)
                num ^= property.objectReferenceInstanceIDValue;

            return num;
        }

        /// <summary>
        /// Unlike GetPropertyHash, this will respect the index in an array. Useful if you need uniqueness over an array of elements.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static int GetIndexRespectingPropertyHash(SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException("property");
            if (property.serializedObject.targetObject == null)
                return 0;

            var spath = property.propertyPath;

            int num = property.serializedObject.targetObject.GetInstanceID() ^ spath.GetHashCode();
            if (property.propertyType == SerializedPropertyType.ObjectReference)
                num ^= property.objectReferenceInstanceIDValue;

            return num;
        }
        
    }
}
