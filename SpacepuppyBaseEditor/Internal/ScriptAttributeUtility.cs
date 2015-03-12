using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.Dynamic;

namespace com.spacepuppyeditor.Internal
{
    internal static class ScriptAttributeUtility
    {

        #region Fields

        private static PropertyHandlerCache _handlerCache = new PropertyHandlerCache();



        //Internal Wrapper Fields

        private static TypeAccessWrapper _accessWrapper;

        private static System.Func<System.Type, System.Type> _imp_getDrawerTypeForType;

        private delegate System.Reflection.FieldInfo GetFieldInfoFromPropertyDelegate(SerializedProperty property, out System.Type type);
        private static GetFieldInfoFromPropertyDelegate _imp_getFieldInfoFromProperty;

        private static System.Func<SerializedProperty, System.Type> _imp_getScriptTypeFromProperty;

        #endregion

        #region CONSTRUCTOR

        static ScriptAttributeUtility()
        {
            var klass = InternalTypeUtil.UnityEditorAssembly.GetType("UnityEditor.ScriptAttributeUtility");
            _accessWrapper = new TypeAccessWrapper(klass, true);
        }

        #endregion

        #region Methods

        //#######################
        // GetDrawerTypeForType

        public static System.Type GetDrawerTypeForType(System.Type tp)
        {
            if (_imp_getDrawerTypeForType == null) _imp_getDrawerTypeForType = _accessWrapper.GetStaticMethod("GetDrawerTypeForType", typeof(System.Func<System.Type, System.Type>)) as System.Func<System.Type, System.Type>;
            return _imp_getDrawerTypeForType(tp);
        }

        //#######################
        // GetHandler
        
        public static IPropertyHandler GetHandler(SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException("property");

            IPropertyHandler result = _handlerCache.GetHandler(property);
            if (result != null)
            {
                return result;
            }

            //TEST FOR SPECIAL CASE HANDLER
            var fieldInfo = ScriptAttributeUtility.GetFieldInfoFromProperty(property);
            if (fieldInfo != null && System.Attribute.IsDefined(fieldInfo, typeof(SPPropertyAttribute)))
            {
                var attribs = fieldInfo.GetCustomAttributes(typeof(SPPropertyAttribute), false).Cast<SPPropertyAttribute>().ToArray();
                if (attribs[0].HandlesEntireArray)
                {
                    result = new SPPropertyAttributePropertyHandler(fieldInfo, attribs);
                    _handlerCache.SetHandler(property, result);
                    return result;
                }
            }

            //USE STANDARD HANDLER if none was found
            return StandardPropertyHandler.Instance;
        }

        //#######################
        // GetFieldInfoFromProperty

        public static System.Reflection.FieldInfo GetFieldInfoFromProperty(SerializedProperty property)
        {
            if (_imp_getFieldInfoFromProperty == null) _imp_getFieldInfoFromProperty = _accessWrapper.GetStaticMethod("GetFieldInfoFromProperty", typeof(GetFieldInfoFromPropertyDelegate)) as GetFieldInfoFromPropertyDelegate;
            System.Type type;
            return _imp_getFieldInfoFromProperty(property, out type);
        }

        public static System.Reflection.FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out System.Type type)
        {
            if (_imp_getFieldInfoFromProperty == null) _imp_getFieldInfoFromProperty = _accessWrapper.GetStaticMethod("GetFieldInfoFromProperty", typeof(GetFieldInfoFromPropertyDelegate)) as GetFieldInfoFromPropertyDelegate;
            return _imp_getFieldInfoFromProperty(property, out type);
        }

        /*
        /// <summary>
        /// Returns the fieldInfo of the property. If the property is an Array/List, the fieldInfo for the Array is returned.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoFromProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            var scriptType = GetScriptTypeFromProperty(prop);
            var elements = path.Split('.');

            FieldInfo result = null;
            System.Type tp = scriptType;
            foreach (var element in elements)
            {
                if (element.Contains('['))
                {
                    var name = element.Substring(0, element.IndexOf('['));
                    FieldInfo info = null;
                    for (var tp2 = tp; info == null && tp2 != null; tp2 = tp2.BaseType)
                        info = tp2.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (info == null)
                    {
                        return null;
                    }
                    else
                    {
                        result = info;
                        tp = info.FieldType;
                    }

                    if (ObjUtil.IsListType(tp))
                    {
                        tp = ObjUtil.GetElementTypeOfListType(tp);
                    }
                }
                else
                {
                    FieldInfo info = null;
                    for (var tp2 = tp; info == null && tp2 != null; tp2 = tp2.BaseType)
                        info = tp2.GetField(element, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (info == null)
                    {
                        return null;
                    }
                    else
                    {
                        result = info;
                        tp = info.FieldType;
                    }
                }
            }

            return result;
        }
         */

        //#######################
        // GetScriptTypeFromProperty

        public static System.Type GetScriptTypeFromProperty(SerializedProperty property)
        {
            if (_imp_getScriptTypeFromProperty == null) _imp_getScriptTypeFromProperty = _accessWrapper.GetStaticMethod("GetScriptTypeFromProperty", typeof(System.Func<SerializedProperty, System.Type>)) as System.Func<SerializedProperty, System.Type>;
            return _imp_getScriptTypeFromProperty(property);
        }

        /*
        public static System.Type GetScriptTypeFromProperty(SerializedProperty prop)
        {
            SerializedProperty scriptProp = prop.serializedObject.FindProperty(PROP_SCRIPT);
            if (scriptProp == null)
                return null;
            MonoScript monoScript = scriptProp.objectReferenceValue as MonoScript;
            if ((UnityEngine.Object)monoScript == (UnityEngine.Object)null)
                return null;
            else
                return monoScript.GetClass();
        }
         */

        #endregion

    }
}
