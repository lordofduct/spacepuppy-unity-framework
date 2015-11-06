using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    public class CustomAddonDrawerAttribute : System.Attribute
    {

        private Type _inspectedType;
        public bool supportMultiObject;
        public bool displayAsFooter;

        public CustomAddonDrawerAttribute(Type inspectedType)
        {
            _inspectedType = inspectedType;
        }

        public Type InspectedType { get { return _inspectedType; } }

    }

    /// <summary>
    /// Allows defining custom add-on drawer for various script types to be placed at the beginning of, or at the end of, an inspector draw call. 
    /// This is great for things like all scripts that implement some interface get a custom footer relavant to that interface without every script 
    /// having to implement it in each inspector.
    /// </summary>
    public abstract class SPEditorAddonDrawer
    {

        #region Fields

        private UnityEditor.SerializedObject _serializedObject;
        private bool _isFooter;

        #endregion

        #region Properties

        public bool IsFooter
        {
            get { return _isFooter; }
        }

        public UnityEditor.SerializedObject SerializedObject
        {
            get { return _serializedObject; }
        }

        #endregion

        #region Methods

        public virtual void OnEnable()
        {

        }


        public virtual void OnInspectorGUI()
        {

        }

        #endregion



        #region Static Factory

        private static Dictionary<Type, object> _inspectedTypeToAddonDrawerType;

        private static void BuildAddonDrawerTypeTable()
        {
            _inspectedTypeToAddonDrawerType = new Dictionary<Type, object>();

            foreach (var tp in TypeUtil.GetTypesAssignableFrom(typeof(SPEditorAddonDrawer)))
            {
                if (tp.IsAbstract) continue;

                var attribs = tp.GetCustomAttributes(typeof(CustomAddonDrawerAttribute), false) as CustomAddonDrawerAttribute[];
                foreach(var attrib in attribs)
                {
                    object v;
                    if(_inspectedTypeToAddonDrawerType.TryGetValue(attrib.InspectedType, out v))
                    {
                        if(v is List<DrawerInfo>)
                        {
                            (v as List<DrawerInfo>).Add(new DrawerInfo()
                            {
                                DrawerType = tp,
                                SupportsMultiObject = attrib.supportMultiObject,
                                IsFooter = attrib.displayAsFooter
                            });
                        }
                        else if(v is DrawerInfo)
                        {
                            var lst = new List<DrawerInfo>();
                            lst.Add(v as DrawerInfo);
                            lst.Add(new DrawerInfo()
                            {
                                DrawerType = tp,
                                SupportsMultiObject = attrib.supportMultiObject,
                                IsFooter = attrib.displayAsFooter
                            });
                            _inspectedTypeToAddonDrawerType[attrib.InspectedType] = lst;
                        }
                    }
                    else
                    {
                        _inspectedTypeToAddonDrawerType.Add(attrib.InspectedType, new DrawerInfo()
                                                                                    {
                                                                                        DrawerType = tp,
                                                                                        SupportsMultiObject = attrib.supportMultiObject,
                                                                                        IsFooter = attrib.displayAsFooter
                                                                                    });
                    }
                }
            }
        }

        public static SPEditorAddonDrawer[] GetDrawers(UnityEditor.SerializedObject target)
        {
            if (target == null) return ArrayUtil.Empty<SPEditorAddonDrawer>();

            Type compType = typeof(UnityEngine.Component);
            Type targType = target.GetTargetType();
            if (!compType.IsAssignableFrom(targType)) return ArrayUtil.Empty<SPEditorAddonDrawer>();
            if (_inspectedTypeToAddonDrawerType == null) BuildAddonDrawerTypeTable();
            if (_inspectedTypeToAddonDrawerType.Count == 0) return ArrayUtil.Empty<SPEditorAddonDrawer>();

            using (var lst = TempCollection.GetList<SPEditorAddonDrawer>())
            {
                var tp = targType;
                while (tp != null && compType.IsAssignableFrom(tp))
                {
                    object v;
                    if (_inspectedTypeToAddonDrawerType.TryGetValue(tp, out v))
                    {
                        if (v is List<DrawerInfo>)
                        {
                            foreach(var info in (v as List<DrawerInfo>))
                            {
                                var d = info.CreateDrawer(target);
                                if (d != null) lst.Add(d);
                            }
                        }
                        else if (v is DrawerInfo)
                        {
                            var d = (v as DrawerInfo).CreateDrawer(target);
                            if (d != null) lst.Add(d);
                        }
                    }

                    tp = tp.BaseType;
                }

                foreach(var itp in targType.GetInterfaces())
                {
                    object v;
                    if (_inspectedTypeToAddonDrawerType.TryGetValue(itp, out v))
                    {
                        if (v is List<DrawerInfo>)
                        {
                            foreach (var info in (v as List<DrawerInfo>))
                            {
                                var d = info.CreateDrawer(target);
                                if (d != null) lst.Add(d);
                            }
                        }
                        else if (v is DrawerInfo)
                        {
                            var d = (v as DrawerInfo).CreateDrawer(target);
                            if (d != null) lst.Add(d);
                        }
                    }
                }

                return lst.ToArray();
            }
        }
        


        private class DrawerInfo
        {

            public Type DrawerType;
            public bool SupportsMultiObject;
            public bool IsFooter;



            public SPEditorAddonDrawer CreateDrawer(UnityEditor.SerializedObject target)
            {
                if (target.isEditingMultipleObjects && !this.SupportsMultiObject) return null;

                try
                {
                    var drawer = Activator.CreateInstance(DrawerType) as SPEditorAddonDrawer;
                    drawer._serializedObject = target;
                    drawer._isFooter = this.IsFooter;
                    drawer.OnEnable();
                    return drawer;
                }
                catch
                {
                    return null;
                }
            }

        }
        
        #endregion

    }
}
