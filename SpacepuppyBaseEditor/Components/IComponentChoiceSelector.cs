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

        void BeforeGUI(SelectableComponentPropertyDrawer drawer, SerializedProperty property, System.Type restrictionType);

        Component[] GetComponents();
        GUIContent[] GetPopupEntries();
        int GetPopupIndexOfComponent(Component comp);
        Component GetComponentAtPopupIndex(int index);

        void GUIComplete(SerializedProperty property);

    }

    public class DefaultComponentChoiceSelector : IComponentChoiceSelector
    {

        private SelectableComponentPropertyDrawer _drawer;
        private SerializedProperty _property;
        private System.Type _restrictionType;
        private Component[] _components;

        public SelectableComponentPropertyDrawer Drawer { get { return _drawer; } }
        public SerializedProperty Property { get { return _property; } }
        public System.Type RestrictionType { get { return _restrictionType; } }
        public Component[] Components { get { return _components; } }



        protected virtual Component[] DoGetComponents()
        {
            if(_drawer.ForceOnlySelf)
            {
                return GetComponentsFromSerializedObjectTarget(_property, _restrictionType, _drawer.SearchChildren);
            }
            else
            {
                return GetComponentsFromPropertyObjectReference(_property, _restrictionType, _drawer.SearchChildren);
            }
        }

        protected virtual void OnBeforeGUI()
        {

        }

        protected virtual void OnGUIComplete()
        {

        }

        #region IComponentChoiceSelector Interface

        void IComponentChoiceSelector.BeforeGUI(SelectableComponentPropertyDrawer drawer, SerializedProperty property, System.Type restrictionType)
        {
            _drawer = drawer;
            _property = property;
            _restrictionType = restrictionType;
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
            var lst = com.spacepuppy.Collections.TempCollection<GUIContent>.GetCollection();
            lst.Add(new GUIContent("Nothing..."));
            if (_drawer.SearchChildren)
            {
                lst.AddRange(from s in DefaultComponentChoiceSelector.GetUniqueComponentNamesWithOwner(_components) select new GUIContent(s));
            }
            else
            {
                lst.AddRange(from s in DefaultComponentChoiceSelector.GetUniqueComponentNames(_components) select new GUIContent(s));
            }

            var result = lst.ToArray();
            lst.Release();
            return result;
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

        void IComponentChoiceSelector.GUIComplete(SerializedProperty property)
        {
            this.OnGUIComplete();
            _property = null;
            _restrictionType = null;
            _components = null;
        }

        #endregion



        public static Component[] GetComponentsFromPropertyObjectReference(SerializedProperty property, System.Type restrictionType, bool searchChildren)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(property.objectReferenceValue);
            if (go == null) return new Component[] { };

            if(searchChildren)
                return go.GetComponentsInChildren(restrictionType);
            else
                return go.GetComponents(restrictionType);
        }

        public static Component[] GetComponentsFromSerializedObjectTarget(SerializedProperty property, System.Type restrictionType, bool searchChildren)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (go == null) return new Component[] { };

            if (searchChildren)
                return go.GetComponentsInChildren(restrictionType);
            else
                return go.GetComponents(restrictionType);
        }

        private static Dictionary<System.Type, int> _uniqueCount = new Dictionary<System.Type, int>();
        public static IEnumerable<string> GetUniqueComponentNames(Component[] components)
        {
            _uniqueCount.Clear();
            for (int i = 0; i < components.Length; i++)
            {
                var tp = components[i].GetType();
                if (_uniqueCount.ContainsKey(tp))
                {
                    _uniqueCount[tp]++;
                    yield return tp.Name + " " + _uniqueCount[tp].ToString();
                }
                else
                {
                    _uniqueCount.Add(tp, 1);
                    yield return tp.Name;
                }
            }
            _uniqueCount.Clear();
        }

        public static IEnumerable<string> GetUniqueComponentNamesWithOwner(Component[] components)
        {
            _uniqueCount.Clear();
            for (int i = 0; i < components.Length; i++)
            {
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

}
