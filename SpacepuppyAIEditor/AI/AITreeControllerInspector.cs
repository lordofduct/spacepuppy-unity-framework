using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI;

namespace com.spacepuppyeditor.AI
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
