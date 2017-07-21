using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.BehaviourTree;

namespace com.spacepuppyeditor.AI.BehaviourTree
{

    [CustomEditor(typeof(AITreeController), true)]
    public class AITreeControllerInspector : SPEditor
    {

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

    }

}
