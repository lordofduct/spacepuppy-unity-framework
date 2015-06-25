using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(VariantCollection))]
    public class VariantCollectionPropertyDrawer : PropertyDrawer
    {

        #region Fields

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();
        private VariantCollection.EditorHelper _helper = new VariantCollection.EditorHelper();
        private ReorderableList _lst;
        private GUIContent _label;
        private SerializedProperty _currentProp;

        #endregion

        #region CONSTRUCTOR

        private void StartOnGUI(SerializedProperty property, GUIContent label)
        {
            _helper.UpdateCollection(EditorHelper.GetTargetObjectOfProperty(property) as VariantCollection);

            if (_lst == null)
            {
                _lst = new ReorderableList(_helper, _helper.EntryType, true, true, true, true);
                _lst.drawHeaderCallback = this._lst_DrawHeader;
                _lst.drawElementCallback = this._lst_DrawElement;
                _lst.onAddCallback = this._lst_OnAdd;
                _lst.draggable = false;
            }

            if (_lst.index >= _helper.Count) _lst.index = -1;

            _currentProp = property;
            _label = label;
        }

        private void EndOnGUI(SerializedProperty property, GUIContent label)
        {
            _helper.UpdateCollection(null);
            _currentProp = null;
            _label = null;
        }

        #endregion

        #region Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.StartOnGUI(property, label);

            var h = _lst.GetHeight();

            this.EndOnGUI(property, label);

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.StartOnGUI(property, label);

            EditorGUI.BeginChangeCheck();
            _lst.DoList(position);
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            this.EndOnGUI(property, label);
        }

        #endregion

        #region ReorderableList Handlers

        private void _lst_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, _label);
        }

        private void _lst_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var skey = _helper.GetNameAt(index);
            var variant = _helper.GetVariant(skey);
            if (variant == null) return;

            var w = area.width / 3f;
            var nameRect = new Rect(area.xMin, area.yMin, w, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            var newKey = EditorGUI.TextField(nameRect, skey);
            if(EditorGUI.EndChangeCheck())
            {
                if(_helper.ChangeEntryName(skey, newKey))
                {
                    skey = newKey;
                }
            }

            var variantRect = new Rect(nameRect.xMax + 1f, area.yMin, area.width - (nameRect.width + 1f), area.height);
            _variantDrawer.DrawValueField(variantRect, _currentProp.serializedObject, variant);
        }

        private void _lst_OnAdd(ReorderableList lst)
        {
            _helper.AddEntry();
        }
        
        #endregion

    }
}
