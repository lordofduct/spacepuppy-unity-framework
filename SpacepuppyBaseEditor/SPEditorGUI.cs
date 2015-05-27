using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;
using com.spacepuppy.Dynamic;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor
{
    public static class SPEditorGUI
    {

        #region DefaultPropertyField

        public static float GetDefaultPropertyHeight(SerializedProperty property)
        {
            return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.GetHeight(property, GUIContent.none);
        }

        public static float GetDefaultPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.GetHeight(property, label);
        }

        public static bool DefaultPropertyField(Rect position, SerializedProperty property)
        {
            return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUI(position, property, GUIContent.none, false);
        }

        public static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUI(position, property, label, false);
        }

        public static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return com.spacepuppyeditor.Internal.DefaultPropertyHandler.Instance.OnGUI(position, property, label, includeChildren);
        }

        #endregion

        #region PropertyFields

        public static bool PropertyField(Rect position, SerializedProperty property, bool includeChildren = false)
        {
            return PropertyField(position, property, (GUIContent)null, includeChildren);
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

        #region EnumPopup Inspector

        public static System.Enum EnumPopupExcluding(Rect position, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            return EnumPopupExcluding(position, GUIContent.none, enumValue, ignoredValues);
        }

        public static System.Enum EnumPopupExcluding(Rect position, string label, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            return EnumPopupExcluding(position, EditorHelper.TempContent(label), enumValue, ignoredValues);
        }

        public static System.Enum EnumPopupExcluding(Rect position, GUIContent label, System.Enum enumValue, params System.Enum[] ignoredValues)
        {
            var etp = enumValue.GetType();
            var evalues = System.Enum.GetValues(etp).Cast<System.Enum>().Except(ignoredValues).ToArray();
            if (evalues.Length == 0) throw new System.ArgumentException("Excluded all possible values, not a valid popup.");
            var names = (from e in evalues select EditorHelper.TempContent(e.ToString())).ToArray();
            var index = EditorGUI.Popup(position, label, evalues.IndexOf(enumValue), names);
            return (index >= 0) ? evalues[index] : evalues.First();
        }

        #endregion

        #region EnumFlag Inspector

        public static int EnumFlagField(Rect position, System.Type enumType, GUIContent label, int value)
        {
            var names = (from e in EnumUtil.GetUniqueEnumFlags(enumType) select e.ToString()).ToArray();
            return EditorGUI.MaskField(position, label, value, names);
        }

        public static System.Enum EnumFlagField(Rect position, GUIContent label, System.Enum value)
        {
            if (value == null) throw new System.ArgumentException("Enum value must be non-null.", "value");

            var enumType = value.GetType();
            int i = EnumFlagField(position, enumType, label, System.Convert.ToInt32(value));
            return System.Enum.ToObject(enumType, i) as System.Enum;
        }
        
        public static WrapMode WrapModeField(Rect position, string label, WrapMode mode, bool allowDefault = false)
        {
            return WrapModeField(position, EditorHelper.TempContent(label), mode, allowDefault);
        }

        public static WrapMode WrapModeField(Rect position, GUIContent label, WrapMode mode, bool allowDefault = false)
        {
            if(allowDefault)
            {
                int i = 0;
                switch(mode)
                {
                    case WrapMode.Default:
                        i = 0;
                        break;
                    case WrapMode.Once:
                    //case WrapMode.Clamp: //same as once
                        i = 1;
                        break;
                    case WrapMode.Loop:
                        i = 2;
                        break;
                    case WrapMode.PingPong:
                        i = 3;
                        break;
                    case WrapMode.ClampForever:
                        i = 4;
                        break;
                }
                i = EditorGUI.Popup(position, label, i, new GUIContent[] { EditorHelper.TempContent("Default"), EditorHelper.TempContent("Once|Clamp"), EditorHelper.TempContent("Loop"), EditorHelper.TempContent("PingPong"), EditorHelper.TempContent("ClampForever") });
                switch(i)
                {
                    case 0:
                        return WrapMode.Default;
                    case 1:
                        return WrapMode.Once;
                    case 2:
                        return WrapMode.Loop;
                    case 3:
                        return WrapMode.PingPong;
                    case 4:
                        return WrapMode.ClampForever;
                    default:
                        return WrapMode.Default;
                }
            }
            else
            {
                int i = 0;
                switch (mode)
                {
                    case WrapMode.Default:
                    case WrapMode.Once:
                        //case WrapMode.Clamp: //same as once
                        i = 0;
                        break;
                    case WrapMode.Loop:
                        i = 1;
                        break;
                    case WrapMode.PingPong:
                        i = 2;
                        break;
                    case WrapMode.ClampForever:
                        i = 3;
                        break;
                }
                i = EditorGUI.Popup(position, label, i, new GUIContent[] {EditorHelper.TempContent("Once|Clamp"), EditorHelper.TempContent("Loop"), EditorHelper.TempContent("PingPong"), EditorHelper.TempContent("ClampForever") });
                switch (i)
                {
                    case 0:
                        return WrapMode.Once;
                    case 1:
                        return WrapMode.Loop;
                    case 2:
                        return WrapMode.PingPong;
                    case 3:
                        return WrapMode.ClampForever;
                    default:
                        return WrapMode.Default;
                }
            }
        }

        #endregion

        #region Type Dropdown

        public static System.Type TypeDropDown(Rect position, GUIContent label, System.Type baseType, System.Type selectedType, bool allowAbstractTypes = false, bool allowInterfaces = false, System.Type defaultType = null, TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            if (!TypeUtil.IsType(selectedType, baseType)) selectedType = null;

            //var knownTypes = (from ass in System.AppDomain.CurrentDomain.GetAssemblies()
            //                  from tp in ass.GetTypes()
            //                  where TypeUtil.IsType(tp, baseType) && (allowAbstractTypes || !tp.IsAbstract) && (allowInterfaces || !tp.IsInterface)
            //                  orderby tp.FullName.Substring(tp.FullName.LastIndexOf(".") + 1) ascending
            //                  select tp).ToArray();
            var knownTypes = (from tp in TypeUtil.GetTypesAssignableFrom(baseType)
                              where (allowAbstractTypes || !tp.IsAbstract) && (allowInterfaces || !tp.IsInterface)
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

            //vRot.x = MathUtil.NormalizeAngle(vRot.x, useRadians);
            //vRot.y = MathUtil.NormalizeAngle(vRot.y, useRadians);
            //vRot.z = MathUtil.NormalizeAngle(vRot.z, useRadians);

            EditorGUI.BeginChangeCheck();
            var vNewRot = EditorGUI.Vector3Field(position, label, vRot);
            if(EditorGUI.EndChangeCheck())
            {
                vNewRot.x = MathUtil.NormalizeAngle(vNewRot.x, useRadians);
                vNewRot.y = MathUtil.NormalizeAngle(vNewRot.y, useRadians);
                vNewRot.z = MathUtil.NormalizeAngle(vNewRot.z, useRadians);
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
            else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !typeof(IComponent).IsAssignableFrom(inheritsFromType)) throw new TypeArgumentMismatchException(inheritsFromType, typeof(IComponent), "Type must inherit from IComponent or Component.", "inheritsFromType");
            if (value != null && !inheritsFromType.IsAssignableFrom(value.GetType())) throw new TypeArgumentMismatchException(value.GetType(), inheritsFromType, "value must inherit from " + inheritsFromType.Name, "value");

            if (TypeUtil.IsType(inheritsFromType, typeof(Component)))
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
            else if (!typeof(Component).IsAssignableFrom(inheritsFromType) && !typeof(IComponent).IsAssignableFrom(inheritsFromType)) throw new TypeArgumentMismatchException(inheritsFromType, typeof(IComponent), "Type must inherit from IComponent or Component.", "inheritsFromType");
            if (targetComponentType == null) throw new System.ArgumentNullException("targetComponentType");
            if (!typeof(Component).IsAssignableFrom(targetComponentType)) throw new TypeArgumentMismatchException(targetComponentType, typeof(Component), "targetComponentType");
            if (value != null && !targetComponentType.IsAssignableFrom(value.GetType())) throw new TypeArgumentMismatchException(value.GetType(), inheritsFromType, "value must inherit from " + inheritsFromType.Name, "value");

            if (TypeUtil.IsType(inheritsFromType, typeof(Component)))
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
                        if (TypeUtil.IsType(c.GetType(), targetComponentType))
                        {
                            return c as Component;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Component Selection From Source

        public static Component SelectComponentFromSourceField(Rect position, string label, GameObject source, Component selectedComp, System.Predicate<Component> filter = null)
        {
            return SelectComponentFromSourceField(position, EditorHelper.TempContent(label), source, selectedComp, filter);
        }

        public static Component SelectComponentFromSourceField(Rect position, GUIContent label, GameObject source, Component selectedComp, System.Predicate<Component> filter = null)
        {
            if (source == null) throw new System.ArgumentNullException("source");

            var selectedType = (selectedComp != null) ? source.GetType() : null;
            System.Type[] availableMechanismTypes;
            if (filter != null)
                availableMechanismTypes = (from c in source.GetComponents<Component>() where filter(c) select c.GetType()).ToArray();
            else
                availableMechanismTypes = (from c in source.GetComponents<Component>() select c.GetType()).ToArray();
            var availableMechanismTypeNames = availableMechanismTypes.Select((tp) => EditorHelper.TempContent(tp.Name)).ToArray();

            var index = System.Array.IndexOf(availableMechanismTypes, selectedType);
            index = EditorGUI.Popup(position, label, index, availableMechanismTypeNames);
            return (index >= 0) ? source.GetComponent(availableMechanismTypes[index]) : null;
        }

        #endregion

    }
}
