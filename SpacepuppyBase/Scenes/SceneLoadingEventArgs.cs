using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenes
{

    public class SceneLoadingEventArgs : System.EventArgs
    {

        private SceneManager _manager;
        private ISceneBehaviour _scene;
        private ISceneBehaviourLoadOptions _options;

        private bool _stall;
        private object _instruction;

        public SceneLoadingEventArgs(SceneManager manager, ISceneBehaviour scene, ISceneBehaviourLoadOptions loadOptions)
        {
            _manager = manager;
            _scene = scene;
            _options = loadOptions;
        }

        public SceneManager Manager { get { return _manager; } }

        public ISceneBehaviour Scene { get { return _scene; } }

        public ISceneBehaviourLoadOptions LoadOptions { get { return _options; } }


        public void RequestManagerToStall(object instruction)
        {
            _stall = true;
            if (_instruction == null)
                _instruction = instruction;
            //TODO!!!!!
        }

        internal void PurgeYieldInstructions()
        {

        }

    }

}
