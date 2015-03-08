using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Internal
{
    internal static class PropertyDrawerActivator
    {

        public static PropertyDrawer Create(System.Type propertyDrawerType)
        {
            return System.Activator.CreateInstance(propertyDrawerType) as PropertyDrawer;
        }

        public static PropertyDrawer Create(System.Type propertyDrawerType, PropertyAttribute attrib, System.Reflection.FieldInfo fieldInfo)
        {
            var drawer = System.Activator.CreateInstance(propertyDrawerType) as PropertyDrawer;
            if(drawer != null) InitializePropertyDrawer(drawer, attrib, fieldInfo);
            return drawer;
        }

        public static void InitializePropertyDrawer(PropertyDrawer drawer, PropertyAttribute attrib, System.Reflection.FieldInfo fieldInfo)
        {
            if (drawer == null) throw new System.ArgumentNullException("drawer");
            ObjUtil.SetValue(drawer, "m_Attribute", attrib);
            ObjUtil.SetValue(drawer, "m_FieldInfo", fieldInfo);
        }


    }
}
