using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy;
using com.spacepuppy.Utils.Dynamic;

namespace com.spacepuppyeditor.Internal
{
    internal static class ScriptAttributeUtility
    {

        #region Fields

        private static TypeAccessWrapper _accessWrapper;

        private static System.Func<System.Type, System.Type> _imp_getDrawerTypeForType;
        private static System.Func<SerializedProperty, object> _imp_getHandler;


        private static PropertyHandlerCache _handlerCache = new PropertyHandlerCache();

        #endregion

        #region CONSTRUCTOR

        static ScriptAttributeUtility()
        {
            var klass = InternalTypeUtil.UnityEditorAssembly.GetType("UnityEditor.ScriptAttributeUtility");
            _accessWrapper = new TypeAccessWrapper(klass, true);
        }

        #endregion

        #region Methods

        public static System.Type GetDrawerTypeForType(System.Type tp)
        {
            if (_imp_getDrawerTypeForType == null) _imp_getDrawerTypeForType = _accessWrapper.GetStaticMethod("GetDrawerTypeForType", typeof(System.Func<System.Type, System.Type>)) as System.Func<System.Type, System.Type>;
            return _imp_getDrawerTypeForType(tp);
        }

        public static IPropertyHandler GetHandler(SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException("property");

            IPropertyHandler result = _handlerCache.GetHandler(property);
            if (result != null)
            {
                if (result.RequiresInternalUpdate) result.UpdateInternal(GetHandlerInternal(property));
                return result;
            }

            //TEST FOR SPECIAL CASE HANDLER
            var fieldInfo = EditorHelper.GetFieldInfoOfProperty(property);
            if (fieldInfo != null && System.Attribute.IsDefined(fieldInfo, typeof(SPPropertyAttribute)))
            {
                //var attribs = fieldInfo.GetCustomAttributes(typeof(SPPropertyAttribute), false);

            }

            //USE STANDARD HANDLER
            var internalPropHandler = GetHandlerInternal(property);
            result = (internalPropHandler != null) ? new StandardPropertyHandler(internalPropHandler) : (IPropertyHandler)null;
            _handlerCache.SetHandler(property, result);
            return result;
        }

        private static object GetHandlerInternal(SerializedProperty property)
        {
            if (_imp_getHandler == null) _imp_getHandler = _accessWrapper.GetStaticMethod("GetHandler", typeof(System.Func<SerializedProperty, object>)) as System.Func<SerializedProperty, object>;
            return _imp_getHandler(property);
        }

        #endregion

    }
}
