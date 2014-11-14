using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{
    public static class SPEditorGUI
    {

        #region PropertyFields

        public static bool PropertyField(Rect position, SerializedProperty property, bool includeChildren = false)
        {
            return com.spacepuppyeditor.Internal.ScriptAttributeUtility.GetHandler(property).OnGUI(position, property, null, includeChildren);
        }

        public static bool PropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren = false)
        {
            return com.spacepuppyeditor.Internal.ScriptAttributeUtility.GetHandler(property).OnGUI(position, property, label, includeChildren);
        }

        #endregion

        #region LayerMaskField

        public static LayerMask LayerMaskField(Rect position, string label, int selectedMask)
        {
            return EditorGUI.MaskField(position, label, selectedMask, LayerUtil.GetAllLayerNames());
        }

        #endregion

        #region EnumFlag Inspector

        public static int EnumFlagField(Rect position, System.Type enumType, GUIContent label, int value)
        {
            var names = (from e in ObjUtil.GetUniqueEnumFlags(enumType) select e.ToString()).ToArray();
            return EditorGUI.MaskField(position, label, value, names);
        }

        public static System.Enum EnumFlagField(Rect position, GUIContent label, System.Enum value)
        {
            if (value == null) throw new System.ArgumentException("Enum value must be non-null.", "value");

            var enumType = value.GetType();
            int i = EnumFlagField(position, enumType, label, System.Convert.ToInt32(value));
            return System.Enum.ToObject(enumType, i) as System.Enum;
        }

        #endregion

        #region Type Dropdown

        public static System.Type TypeDropDown(Rect position, GUIContent label, System.Type baseType, System.Type selectedType, bool allowAbstractTypes = false, bool allowInterfaces = false, System.Type defaultType = null, TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            if (!ObjUtil.IsType(selectedType, baseType)) selectedType = null;

            var knownTypes = (from ass in System.AppDomain.CurrentDomain.GetAssemblies()
                              from tp in ass.GetTypes()
                              where ObjUtil.IsType(tp, baseType) && (allowAbstractTypes || !tp.IsAbstract) && (allowInterfaces || !tp.IsInterface)
                              orderby tp.FullName.Substring(tp.FullName.LastIndexOf(".") + 1) ascending
                              select tp).ToArray();
            GUIContent[] knownTypeNames = null;
            switch (listType)
            {
                case TypeDropDownListingStyle.Namespace:
                    knownTypeNames = knownTypes.Select((tp) =>
                    {
                        return new GUIContent(tp.FullName.Replace(".", "/"));
                    }).ToArray();
                    break;
                case TypeDropDownListingStyle.Flat:
                    knownTypeNames = (from tp in knownTypes select new GUIContent(tp.Name)).ToArray();
                    break;
                case TypeDropDownListingStyle.ComponentMenu:
                    knownTypeNames = knownTypes.Select((tp) =>
                    {
                        var menuAttrib = tp.GetCustomAttributes(typeof(AddComponentMenu), false).FirstOrDefault() as AddComponentMenu;
                        if (menuAttrib != null && !string.IsNullOrEmpty(menuAttrib.componentMenu))
                        {
                            return new GUIContent(menuAttrib.componentMenu);
                        }
                        else if (tp.FullName == tp.Name)
                        {
                            return new GUIContent("Scripts/" + tp.Name);
                        }
                        else
                        {
                            if (tp.FullName.StartsWith("UnityEngine."))
                            {
                                return new GUIContent(tp.FullName.Replace(".", "/"));
                            }
                            else
                            {
                                return new GUIContent("Scripts/" + tp.FullName.Replace(".", "/"));
                            }
                        }
                    }).ToArray();
                    break;
                default:
                    knownTypeNames = new GUIContent[0];
                    break;
            }

            if (defaultType == null)
            {
                knownTypes = knownTypes.Prepend(null).ToArray();
                knownTypeNames = knownTypeNames.Prepend(new GUIContent("Nothing")).ToArray();
            }

            int index = knownTypes.IndexOf(selectedType);
            index = EditorGUI.Popup(position, label, index, knownTypeNames);
            return (index >= 0) ? knownTypes[index] : defaultType;
        }

        #endregion

        #region Quaternion Field

        public static Quaternion QuaternionField(Rect position, GUIContent label, Quaternion value, bool useRadians = false)
        {
            Vector3 vRot = value.eulerAngles;
            if (useRadians)
            {
                vRot.x = vRot.x * Mathf.Deg2Rad;
                vRot.y = vRot.y * Mathf.Deg2Rad;
                vRot.z = vRot.z * Mathf.Deg2Rad;
            }

            vRot.x = MathUtil.NormalizeAngle(vRot.x, false);
            vRot.y = MathUtil.NormalizeAngle(vRot.y, false);
            vRot.z = MathUtil.NormalizeAngle(vRot.z, false);

            var vNewRot = EditorGUI.Vector3Field(position, label, vRot);

            vNewRot.x = MathUtil.NormalizeAngle(vNewRot.x, false);
            vNewRot.y = MathUtil.NormalizeAngle(vNewRot.y, false);
            vNewRot.z = MathUtil.NormalizeAngle(vNewRot.z, false);

            if (vRot != vNewRot)
            {
                if (useRadians)
                {
                    vNewRot.x = vNewRot.x * Mathf.Rad2Deg;
                    vNewRot.y = vNewRot.y * Mathf.Rad2Deg;
                    vNewRot.z = vNewRot.z * Mathf.Rad2Deg;
                }
                return Quaternion.Euler(vNewRot);
            }
            else
            {
                return value;
            }
        }

        #endregion

        #region IComponentField

        public static Component ComponentField(Rect position, GUIContent label, Component value, System.Type inheritsFromType, bool allowSceneObjects)
        {
            if (inheritsFromType == null) inheritsFromType = typeof(Component);
            else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !typeof(IComponent).IsAssignableFrom(inheritsFromType)) throw new System.ArgumentException("Type must inherit from IComponent or Component.", "inheritsFromType");
            if (value != null && !inheritsFromType.IsAssignableFrom(value.GetType())) throw new System.ArgumentException("value must inherit from " + inheritsFromType.Name, "value");

            if (ObjUtil.IsType(inheritsFromType, typeof(Component)))
            {
                return EditorGUI.ObjectField(position, label, value, inheritsFromType, true) as Component;
            }
            else
            {
                value = EditorGUI.ObjectField(position, label, value, typeof(Component), true) as Component;
                var go = GameObjectUtil.GetGameObjectFromSource(value);
                if (go != null)
                {
                    return go.GetFirstLikeComponent(inheritsFromType);
                }
            }

            return null;
        }

        public static Component ComponentField(Rect position, GUIContent label, Component value, System.Type inheritsFromType, bool allowSceneObjects, System.Type targetComponentType)
        {
            if (inheritsFromType == null) inheritsFromType = typeof(Component);
            else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !typeof(IComponent).IsAssignableFrom(inheritsFromType)) throw new System.ArgumentException("Type must inherit from IComponent or Component.", "inheritsFromType");
            if (targetComponentType == null) throw new System.ArgumentNullException("targetComponentType");
            if (!typeof(Component).IsAssignableFrom(targetComponentType)) throw new System.ArgumentException("Type must inherit from Component.", "targetComponentType");
            if (value != null && !targetComponentType.IsAssignableFrom(value.GetType())) throw new System.ArgumentException("value must inherit from " + targetComponentType.Name, "value");

            if (ObjUtil.IsType(inheritsFromType, typeof(Component)))
            {
                return EditorGUI.ObjectField(position, label, value, inheritsFromType, true) as Component;
            }
            else
            {
                value = EditorGUI.ObjectField(position, label, value, typeof(Component), true) as Component;
                var go = GameObjectUtil.GetGameObjectFromSource(value);
                if (go != null)
                {
                    foreach (var c in go.GetLikeComponents(inheritsFromType))
                    {
                        if (ObjUtil.IsType(c.GetType(), targetComponentType))
                        {
                            return c as Component;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

    }
}
