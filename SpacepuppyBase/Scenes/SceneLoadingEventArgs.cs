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

        private List<object> _stallInstructions;

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
            if (_stallInstructions != null && _stallInstructions.Count > 0)
            {
                if(_stallInstructions[0] == null)
                {
                    _stallInstructions[0] = instruction;
                }
                else if(instruction != null)
                {
                    _stallInstructions.Add(instruction);
                }
            }
            else
            {
                if (_stallInstructions == null) _stallInstructions = new List<object>();
                _stallInstructions.Add(instruction);
            }
        }

        internal bool ShouldStall(out object[] yieldInstructions)
        {
            if (_stallInstructions == null || _stallInstructions.Count == 0)
            {
                yieldInstructions = null;
                return false;
            }

            yieldInstructions = _stallInstructions.ToArray();
            _stallInstructions.Clear();
            return true;
        }

    }

}
