using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Spawn
{

    //[CustomEditor(typeof(i_Spawner), true)]
    [System.Obsolete("i_Spawner no longer inherits from AbstractSpawnPoint, so this implementation of its inspector is no longer necessary.")]
    public class SpawnPointInspector : AbstractSpawnerInspector
    {

        #region Fields

        ReorderableList _lst;

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            _lst = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty("_prefabs"), true, true, true, true);
            _lst.drawHeaderCallback = this._prefabList_DrawHeader;
            _lst.drawElementCallback = this._prefabList_DrawElement;
            _lst.onAddCallback = this._prefabList_OnAdded;
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawBaseProperties();


            _lst.DoLayoutList();
            this.DrawOtherProperties("_prefabs");


            this.DrawInformationBox();

            this.serializedObject.ApplyModifiedProperties();
        }


        #region Anim ReorderableList Handlers

        private void _prefabList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Prefabs");
        }

        private void _prefabList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _lst.serializedProperty.GetArrayElementAtIndex(index);

            //EditorGUI.PropertyField(area, element, false);
            element.objectReferenceValue = EditorGUI.ObjectField(area, element.objectReferenceValue, typeof(GameObject), false);

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_lst, area, index, isActive, isFocused);
        }

        private void _prefabList_OnAdded(ReorderableList lst)
        {
            lst.serializedProperty.arraySize++;
            lst.index = lst.serializedProperty.arraySize - 1;

            var prefabProp = lst.serializedProperty.GetArrayElementAtIndex(lst.index);
            prefabProp.objectReferenceValue = null;
        }

        #endregion

    }

}
