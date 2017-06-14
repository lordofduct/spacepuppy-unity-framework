using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomEditor(typeof(Animation), true)]
    public class AdvancedAnimationInspector : SPEditor
    {
        private const string PROP_ANIMATION = "m_Animation";
        private const string PROP_ANIMATIONS = "m_Animations";
        private const string PROP_PLAYAUTOMATICALLY = "m_PlayAutomatically";
        private const string PROP_ANIMATE_PHYSICS = "m_AnimatePhysics";
        private const string PROP_CULLINGTYPE = "m_CullingType";

        #region Fields

        private ReorderableList _animList;

        #endregion

        #region OnGUI

        protected override void OnEnable()
        {
            base.OnEnable();

            _animList = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty(PROP_ANIMATIONS), true, true, true, true);
            _animList.drawHeaderCallback = this._animList_DrawHeader;
            _animList.drawElementCallback = this._animList_DrawElement;
            _animList.onAddCallback = this._animList_OnAdded;
            _animList.onRemoveCallback = this._animList_OnRemoved;

            _animList.draggable = !Application.isPlaying;
            _animList.displayAdd = !Application.isPlaying;
            _animList.displayRemove = !Application.isPlaying;
        }

        protected override void OnSPInspectorGUI()
        {
            if (!EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, true))
            {
                this.DrawDefaultStandardInspector();
                return;
            }

            this.serializedObject.Update();

            this.DrawPlayAnimPopup();

            //_animList.DoLayoutList();
            var rect = EditorGUILayout.GetControlRect(false, _animList.GetHeight());
            _animList.DoList(rect);

            rect = new Rect(rect.xMin, rect.yMax - EditorGUIUtility.singleLineHeight + 1f, Mathf.Min(EditorGUIUtility.labelWidth, 60f), EditorGUIUtility.singleLineHeight - 1f);
            if(GUI.Button(rect, "Clear All"))
            {
                _animList.serializedProperty.arraySize = 0;
            }


            this.DrawPropertyField(PROP_ANIMATE_PHYSICS);
            this.DrawPropertyField(PROP_CULLINGTYPE);

            this.serializedObject.ApplyModifiedProperties();
        }

        private void DrawPlayAnimPopup()
        {
            var clips = new List<AnimationClip>();
            for (int i = 0; i < _animList.serializedProperty.arraySize; i++)
            {
                var clip = _animList.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as AnimationClip;
                if(clip != null)
                {
                    clips.Add(clip);
                }
            }

            var names = from c in clips select c.name;

            var animToPlayProp = this.serializedObject.FindProperty(PROP_ANIMATION);
            int index = clips.IndexOf(animToPlayProp.objectReferenceValue as AnimationClip) + 1;
            index = EditorGUILayout.Popup(new GUIContent("Play Automatically"), index, (from n in names.Prepend("Play Nothing") select new GUIContent(n)).ToArray());
            animToPlayProp.objectReferenceValue = (index > 0) ? clips[index - 1] : null;


            this.serializedObject.FindProperty(PROP_PLAYAUTOMATICALLY).boolValue = animToPlayProp.objectReferenceValue != null;
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        #endregion

        #region Anim ReorderableList Handlers

        private void _animList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Animation States");
        }

        private void _animList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _animList.serializedProperty.GetArrayElementAtIndex(index);

            GUIContent label;
            if (Application.isPlaying && !this.serializedObject.isEditingMultipleObjects)
            {
                var targ = this.target as Animation;
                var clip = element.objectReferenceValue as AnimationClip;
                if (targ.IsPlaying(clip.name))
                {
                    label = EditorHelper.TempContent("Anim " + index.ToString("00") + " : (Playing)");
                }
                else
                {
                    label = EditorHelper.TempContent("Anim " + index.ToString("00"));
                }
            }
            else
            {
                label = EditorHelper.TempContent("Anim " + index.ToString("00"));
            }

            area.height = EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(area, element, label);

            area = EditorGUI.PrefixLabel(area, label);
            if(Application.isPlaying && !this.serializedObject.isEditingMultipleObjects)
            {
                if(SPEditorGUI.PlayButton(ref area))
                {
                    var targ = this.target as Animation;
                    var clip = element.objectReferenceValue as AnimationClip;
                    if (targ != null && clip != null)
                    {
                        targ.Play(clip.name, PlayMode.StopSameLayer);
                    }
                }
            }
            EditorGUI.PropertyField(area, element, GUIContent.none);


            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_animList, area, index, isActive, isFocused);

        }

        private void _animList_OnAdded(ReorderableList lst)
        {
            lst.serializedProperty.arraySize++;
            lst.index = lst.serializedProperty.arraySize - 1;

            var element = lst.serializedProperty.GetArrayElementAtIndex(lst.serializedProperty.arraySize - 1);
            element.objectReferenceValue = null;
        }

        private void _animList_OnRemoved(ReorderableList lst)
        {
            if(lst.index >= 0)
            {
                var element = lst.serializedProperty.GetArrayElementAtIndex(lst.index);
                var animToPlay = this.serializedObject.FindProperty(PROP_ANIMATION);
                if(animToPlay.objectReferenceValue != null && animToPlay.objectReferenceValue == element.objectReferenceValue)
                {
                    animToPlay.objectReferenceValue = null;
                    this.serializedObject.FindProperty(PROP_PLAYAUTOMATICALLY).boolValue = false;
                }
                element.objectReferenceValue = null;
                lst.serializedProperty.DeleteArrayElementAtIndex(lst.index);
            }
        }

        #endregion

    }
}
