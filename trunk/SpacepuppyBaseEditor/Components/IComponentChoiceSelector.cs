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

        void BeforeGUI(SerializedProperty property, System.Type restrictionType);

        Component[] GetComponents();
        GUIContent[] GetPopupEntries();
        int GetPopupIndexOfComponent(Component comp);
        Component GetComponentAtPopupIndex(int index);

        void GUIComplete(SerializedProperty property);

    }

    public class DefaultComponentChoiceSelector : IComponentChoiceSelector
    {

        private SerializedProperty _property;
        private System.Type _restrictionType;
        private Component[] _components;

        public SerializedProperty Property { get { return _property; } }
        public System.Type RestrictionType { get { return _restrictionType; } }
        public Component[] Components { get { return _components; } }



        public virtual Component[] DoGetComponents(SerializedProperty property, System.Type restrictionType)
        {
            return GetComponentsFromPropertyObjectReference(property, restrictionType);
        }

        protected virtual void OnBeforeGUI()
        {

        }

        protected virtual void OnGUIComplete()
        {

        }

        #region IComponentChoiceSelector Interface

        void IComponentChoiceSelector.BeforeGUI(SerializedProperty property, System.Type restrictionType)
        {
            _property = property;
            _restrictionType = restrictionType;
            _components = this.DoGetComponents(property, restrictionType);
            this.OnBeforeGUI();
        }

        Component[] IComponentChoiceSelector.GetComponents()
        {
            return _components;
        }

        public virtual GUIContent[] GetPopupEntries()
        {
            //return (from c in components select new GUIContent(c.GetType().Name)).ToArray();
            return (from s in DefaultComponentChoiceSelector.GetUniqueComponentNames(_components) select new GUIContent(s)).ToArray();
        }

        public virtual int GetPopupIndexOfComponent(Component comp)
        {
            if (_components == null) return -1;
            return _components.IndexOf(comp);
        }

        public virtual Component GetComponentAtPopupIndex(int index)
        {
            if (_components == null) return null;
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



        public static Component[] GetComponentsFromPropertyObjectReference(SerializedProperty property, System.Type restrictionType)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(property.objectReferenceValue);
            if (go == null) return new Component[] { };

            return go.GetComponents(restrictionType);
        }

        public static Component[] GetComponentsFromSerializedObjectTarget(SerializedProperty property, System.Type restrictionType)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (go == null) return new Component[] { };

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
    }
}
