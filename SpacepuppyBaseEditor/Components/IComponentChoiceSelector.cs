using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Components
{
    public interface IComponentChoiceSelector
    {

        void BeforeGUI(SelectableComponentPropertyDrawer drawer, SerializedProperty property, System.Type restrictionType, bool allowProxy);

        Component[] GetComponents();
        GUIContent[] GetPopupEntries();
        int GetPopupIndexOfComponent(Component comp);
        Component GetComponentAtPopupIndex(int index);

        void GUIComplete(SerializedProperty property, int selectedIndex);

    }

    public class DefaultComponentChoiceSelector : IComponentChoiceSelector
    {

        private SelectableComponentPropertyDrawer _drawer;
        private SerializedProperty _property;
        private System.Type _restrictionType;
        private bool _allowProxy;
        private Component[] _components;

        public SelectableComponentPropertyDrawer Drawer { get { return _drawer; } }
        public SerializedProperty Property { get { return _property; } }
        public System.Type RestrictionType { get { return _restrictionType; } }
        public bool AllowProxy { get { return _allowProxy; } }
        public Component[] Components { get { return _components; } }



        protected virtual Component[] DoGetComponents()
        {
            return GetComponentsFromSerializedProperty(_property, _restrictionType, _drawer.ForceOnlySelf, _drawer.SearchChildren, _allowProxy);
        }

        protected virtual void OnBeforeGUI()
        {

        }

        protected virtual void OnGUIComplete(int selectedIndex)
        {

        }

        #region IComponentChoiceSelector Interface

        void IComponentChoiceSelector.BeforeGUI(SelectableComponentPropertyDrawer drawer, SerializedProperty property, System.Type restrictionType, bool allowProxy)
        {
            _drawer = drawer;
            _property = property;
            _restrictionType = restrictionType;
            _allowProxy = allowProxy;
            _components = this.DoGetComponents();
            this.OnBeforeGUI();
        }

        Component[] IComponentChoiceSelector.GetComponents()
        {
            return _components;
        }

        public virtual GUIContent[] GetPopupEntries()
        {
            //return (from c in components select new GUIContent(c.GetType().Name)).ToArray();
            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<GUIContent>())
            {
                lst.Add(new GUIContent("Nothing..."));
                if (_drawer.SearchChildren)
                {
                    lst.AddRange(from s in DefaultComponentChoiceSelector.GetUniqueComponentNamesWithOwner(_components) select new GUIContent(s));
                }
                else
                {
                    lst.AddRange(from s in DefaultComponentChoiceSelector.GetUniqueComponentNames(_components) select new GUIContent(s));
                }
                return lst.ToArray();
            }
        }

        public virtual int GetPopupIndexOfComponent(Component comp)
        {
            if (_components == null) return -1;
            return _components.IndexOf(comp) + 1; //adjust for Nothing...
        }

        public virtual Component GetComponentAtPopupIndex(int index)
        {
            if (_components == null) return null;
            if (index == 0) return null;
            index--; //adjust for Nothing...
            if (index < 0 || index >= _components.Length) return null;
            return _components[index];
        }

        void IComponentChoiceSelector.GUIComplete(SerializedProperty property, int selectedIndex)
        {
            this.OnGUIComplete(selectedIndex);
            _property = null;
            _restrictionType = null;
            _components = null;
        }

        #endregion



        public static GameObject GetGameObjectFromSource(SerializedProperty property, bool forceSelfOnly)
        {
            if (forceSelfOnly)
                return GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            else
                return GameObjectUtil.GetGameObjectFromSource(property.objectReferenceValue);
        }

        public static Component[] GetComponentsFromSerializedProperty(SerializedProperty property, System.Type restrictionType, bool forceSelfOnly, bool searchChildren, bool allowProxy)
        {
            if (!ComponentUtil.IsAcceptableComponentType(restrictionType))
                return ArrayUtil.Empty<Component>();

            var go = GetGameObjectFromSource(property, forceSelfOnly);
            if (go == null) return ArrayUtil.Empty<Component>();

            if(allowProxy)
            {
                if (searchChildren)
                    return (from c in go.GetComponentsInChildren<Component>() where ObjUtil.IsType(c, restrictionType, allowProxy) select c).ToArray();
                else
                    return (from c in go.GetComponents<Component>() where ObjUtil.IsType(c, restrictionType, allowProxy) select c).ToArray();
            }
            else
            {
                if (searchChildren)
                    return go.GetComponentsInChildren(restrictionType);
                else
                    return go.GetComponents(restrictionType);
            }
        }
        
        private static Dictionary<System.Type, int> _uniqueCount = new Dictionary<System.Type, int>();
        public static IEnumerable<string> GetUniqueComponentNames(Component[] components)
        {
            _uniqueCount.Clear();
            for (int i = 0; i < components.Length; i++)
            {
                //var tp = components[i].GetType();
                //if (_uniqueCount.ContainsKey(tp))
                //{
                //    _uniqueCount[tp]++;
                //    yield return tp.Name + " " + _uniqueCount[tp].ToString();
                //}
                //else
                //{
                //    _uniqueCount.Add(tp, 1);
                //    yield return tp.Name;
                //}

                var tp = components[i].GetType();
                if (_uniqueCount.ContainsKey(tp))
                {
                    _uniqueCount[tp]++;
                    yield return string.Format("{0} ({1} {2})", components[i].gameObject.name, tp.Name, _uniqueCount[tp]);
                }
                else
                {
                    _uniqueCount.Add(tp, 1);
                    yield return string.Format("{0} ({1})", components[i].gameObject.name, tp.Name);
                }

            }
            _uniqueCount.Clear();
        }

        public static IEnumerable<string> GetUniqueComponentNamesWithOwner(Component[] components)
        {
            _uniqueCount.Clear();
            for (int i = 0; i < components.Length; i++)
            {
                //TODO - maybe come up with a better naming scheme for this
                var tp = components[i].GetType();
                if (_uniqueCount.ContainsKey(tp))
                {
                    _uniqueCount[tp]++;
                    yield return components[i].gameObject.name + "/" + tp.Name + " " + _uniqueCount[tp].ToString();
                }
                else
                {
                    _uniqueCount.Add(tp, 1);
                    yield return components[i].gameObject.name + "/" + tp.Name;
                }
            }
            _uniqueCount.Clear();
        }

    }

    public class MultiTypeComponentChoiceSelector : DefaultComponentChoiceSelector
    {

        public System.Type[] AllowedTypes;

        protected override Component[] DoGetComponents()
        {
            return GetComponentsFromSerializedProperty(this.Property, this.AllowedTypes, this.RestrictionType, this.Drawer.ForceOnlySelf, this.Drawer.SearchChildren, this.AllowProxy);
        }
        
        public static Component[] GetComponentsFromSerializedProperty(SerializedProperty property, System.Type[] allowedTypes, System.Type restrictionType, bool forceSelfOnly, bool searchChildren, bool allowProxy)
        {
            if (allowedTypes == null || allowedTypes.Length == 0) return ArrayUtil.Empty<Component>();

            var go = DefaultComponentChoiceSelector.GetGameObjectFromSource(property, forceSelfOnly);
            if (go == null) return ArrayUtil.Empty<Component>();

            using (var set = com.spacepuppy.Collections.TempCollection.GetSet<Component>())
            {
                if (searchChildren)
                {
                    foreach (var c in go.GetComponentsInChildren<Component>())
                    {
                        if (!ObjUtil.IsType(c, restrictionType, allowProxy)) continue;
                        foreach (var tp in allowedTypes)
                        {
                            if (ObjUtil.IsType(c, tp, allowProxy)) set.Add(c);
                        }
                    }
                }
                else
                {
                    foreach (var c in go.GetComponents<Component>())
                    {
                        if (!ObjUtil.IsType(c, restrictionType, allowProxy)) continue;
                        foreach (var tp in allowedTypes)
                        {
                            if (ObjUtil.IsType(c, tp, allowProxy)) set.Add(c);
                        }
                    }
                }

                return (from c in set orderby c.GetType().Name select c).ToArray();
            }
        }

    }

}
