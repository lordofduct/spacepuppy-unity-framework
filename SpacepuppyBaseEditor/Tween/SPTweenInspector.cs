using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Tween
{

    [CustomEditor(typeof(SPTween))]
    public class SPTweenInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.DrawDefaultInspector();

            var obj = this.target as SPTween;
            if(obj != null && Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Active Tweens: " + obj.RunningTweenCount.ToString(), MessageType.Info);
            }
        }

    }
}
