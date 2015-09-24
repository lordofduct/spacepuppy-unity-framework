using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{
    [CustomPropertyDrawer(typeof(ForceRootTagAttribute))]
    public class ForceRootTagHeaderDrawer : ComponentHeaderDrawer
    {


        public override float GetHeight(SerializedObject serializedObject)
        {
            return 0f;
        }

        public override void OnGUI(Rect position, SerializedObject serializedObject)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(serializedObject.targetObject);
            if (go != null && !go.HasTag(SPConstants.TAG_ROOT)) go.AddTag(SPConstants.TAG_ROOT);
        }

    }
}
