using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Project;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Project
{

    [CustomEditor(typeof(CustomTimeLayersData))]
    public class CustomTimeLayersDataInspector : SPEditor
    {

        private CustomTimeLayersData.EditorHelper _timeLayersHelper;
        private SPReorderableList _timeLayersListDrawer;

        protected override void OnEnable()
        {
            base.OnEnable();

            var timeLayersData = this.target as CustomTimeLayersData;
            if (timeLayersData != null)
            {
                _timeLayersHelper = new CustomTimeLayersData.EditorHelper(timeLayersData);
                _timeLayersListDrawer = new SPReorderableList(_timeLayersHelper.Layers, typeof(string));
                _timeLayersListDrawer.drawHeaderCallback = _timeLayersList_DrawHeader;
                _timeLayersListDrawer.drawElementCallback = _timeLayers_DrawElement;
                _timeLayersListDrawer.onAddCallback = _timeLayers_AddElement;
            }
        }

        protected override void OnSPInspectorGUI()
        {
            if (_timeLayersListDrawer == null) return;

            EditorGUI.BeginChangeCheck();
            _timeLayersListDrawer.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_timeLayersHelper.Data);
                AssetDatabase.SaveAssets();
            }
        }

        #region Time Layers Draw Callback

        private void _timeLayersList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Custom Time Layers");
        }

        private void _timeLayers_AddElement(ReorderableList lst)
        {
            _timeLayersHelper.Layers.Add(null);
        }

        private void _timeLayers_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            string layerName = _timeLayersHelper.Layers[index] as string;

            EditorGUI.BeginChangeCheck();
            layerName = EditorGUI.TextField(area, layerName);
            if (EditorGUI.EndChangeCheck())
                _timeLayersHelper.Layers[index] = layerName;

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_timeLayersListDrawer, area, index, isActive, isFocused);
        }

        #endregion

    }
}
