using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Geom
{
    public static class TransformAltContextMenu
    {

        [MenuItem("CONTEXT/ALT/Zero Out", priority = 0)]
        public static void ZeroOut()
        {
            foreach(var t in Selection.transforms)
            {
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
            }
        }

        [MenuItem("CONTEXT/ALT/Zero Out 3DS Max", priority = 1)]
        public static void ZeroOut3DSMAX()
        {
            foreach(var t in Selection.transforms)
            {
                t.localPosition = Vector3.zero;
                t.localRotation = SPConstants.ROT_3DSMAX_TO_UNITY;
                t.localScale = Vector3.one;
            }
        }

    }
}
