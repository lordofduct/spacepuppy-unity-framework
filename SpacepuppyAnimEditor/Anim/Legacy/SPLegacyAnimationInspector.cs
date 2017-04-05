using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Anim;
using com.spacepuppy.Anim.Legacy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Anim.Legacy
{

    [System.Obsolete()]
    [CustomEditor(typeof(SPLegacyAnimation), true)]
    public class SPLegacyAnimationInspector : SPAnimationControllerInspector
    {
        
        //protected override void OnSPInspectorGUI()
        //{
        //    this.serializedObject.Update();

        //    this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
        //    this.DrawPlayAnimPopup();
        //    this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_STATES, PROP_ANIMATEPHYSICS, PROP_ANIMCULLING, PROP_TIMESUPPLIER, PROP_SPEED, PROP_ANIMTOPLAYONSTART);
        //    this.DrawSPAnimationControllerProps();

        //    this.serializedObject.ApplyModifiedProperties();
        //}

    }
}
