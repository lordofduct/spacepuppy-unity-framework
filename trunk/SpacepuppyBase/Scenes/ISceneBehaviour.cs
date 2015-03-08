using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenes
{
    public interface ISceneBehaviour : IComponent
    {

        IProgressingYieldInstruction LoadScene();
        void BeginScene();
        void EndScene();
    }

    public interface ISceneSequenceBehaviour : ISceneBehaviour
    {
        IProgressingYieldInstruction LoadNextScene();
    }

}
