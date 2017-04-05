using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Anim;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Anim
{

    [CustomEditor(typeof(SPAnimationController), true)]
    public class SPAnimationControllerInspector : SPEditor
    {

        public const string PROP_STATES = "_states";
        public const string PROP_ANIMATEPHYSICS = "_animatePhysics";
        public const string PROP_ANIMCULLING = "_animCullingType";
        public const string PROP_TIMESUPPLIER = "_timeSupplier";
        public const string PROP_SPEED = "_speed";
        public const string PROP_STATES_SERIALIZEDSTATES = "_serializedStates";
        public const string PROP_ANIMTOPLAYONSTART = "_animToPlayOnStart";


        private GUIContent _playOnStartLabel = new GUIContent("Play On Start", "Which animation to play if any when this component starts.");

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPlayAnimPopup();
            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_ANIMTOPLAYONSTART, PROP_STATES, PROP_ANIMATEPHYSICS, PROP_ANIMCULLING, PROP_TIMESUPPLIER, PROP_SPEED);
            this.DrawSPAnimationControllerProps();

            this.serializedObject.ApplyModifiedProperties();
        }


        private void DrawPlayAnimPopup()
        {
            var statesProp = this.serializedObject.FindProperty(PROP_STATES);
            var serialStatesProp = statesProp.FindPropertyRelative(PROP_STATES_SERIALIZEDSTATES);
            var names = new List<string>();
            for (int i = 0; i < serialStatesProp.arraySize; i++)
            {
                names.Add(serialStatesProp.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue);
            }

            var animToPlayProp = this.serializedObject.FindProperty("_animToPlayOnStart");
            int index = names.IndexOf(animToPlayProp.stringValue) + 1;
            index = EditorGUILayout.Popup(_playOnStartLabel, index, (from n in names.Prepend("Play Nothing") select new GUIContent(n)).ToArray());
            animToPlayProp.stringValue = (index > 0) ? names[index - 1] : "";
        }

        protected void DrawSPAnimationControllerProps()
        {
            if(Application.isPlaying)
            {
                var targ = this.target as SPAnimationController;
                if (targ == null) return;

                EditorGUI.BeginChangeCheck();
                var b1 = EditorGUILayout.Toggle("Animate Physics", targ.AnimatePhysics);
                if (EditorGUI.EndChangeCheck())
                    targ.AnimatePhysics = b1;


                EditorGUI.BeginChangeCheck();
                var ectp = (AnimationCullingType)EditorGUILayout.EnumPopup("Culling Type", targ.CullingType);
                if (EditorGUI.EndChangeCheck())
                    targ.CullingType = ectp;
                
                var cache = SPGUI.Disable();
                this.DrawPropertyField(PROP_TIMESUPPLIER);
                this.DrawPropertyField(PROP_SPEED);
                cache.Reset();
                this.DrawPropertyField(PROP_STATES);
            }
            else
            {
                this.DrawPropertyField(PROP_ANIMATEPHYSICS, "Animate Physics", false);
                this.DrawPropertyField(PROP_ANIMCULLING, "Culling Type", false);
                this.DrawPropertyField(PROP_TIMESUPPLIER);
                this.DrawPropertyField(PROP_SPEED);
                this.DrawPropertyField(PROP_STATES);
            }
        }

        #region Utils

        private static SkinnedMeshRenderer FindRenderer(GameObject targ)
        {
            foreach (Transform child in targ.transform)
            {
                var c = child.GetComponent<SkinnedMeshRenderer>();
                if (c != null) return c;
            }

            return null;
        }

        #endregion

    }
}
