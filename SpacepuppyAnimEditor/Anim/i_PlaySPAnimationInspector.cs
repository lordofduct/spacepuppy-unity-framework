#pragma warning disable 0618 // ignore obsolete since this is the editor for said obsolete type
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Anim;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Anim
{

    [CustomEditor(typeof(i_PlaySPAnimation), true)]
    [CanEditMultipleObjects()]
    public class i_PlaySPAnimationInspector : SPEditor
    {

        public const string PROP_STARTTIME = "_startTime";


        

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawDefaultInspector();


            if(Application.isPlaying && !this.serializedObject.isEditingMultipleObjects)
            {
                var targ = this.serializedObject.targetObject as i_PlaySPAnimation;
                if (targ == null) return;

                var anim = targ.CurrentAnimState;
                if (anim != null)
                {
                    EditorGUI.BeginChangeCheck();
                    float t = EditorGUILayout.Slider("Current Time", anim.Time, 0f, anim.Duration);
                    if (EditorGUI.EndChangeCheck())
                        anim.Time = t;
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }


        public override bool RequiresConstantRepaint()
        {
            if (base.RequiresConstantRepaint()) return true;

            var targ = this.serializedObject.targetObject as i_PlaySPAnimation;
            if (targ == null) return false;

            var anim = targ.CurrentAnimState;
            if (anim == null) return false;

            return anim.IsPlaying;
        }

    }
}
