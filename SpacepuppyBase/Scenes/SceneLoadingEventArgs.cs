using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{

    public class SceneLoadingEventArgs : System.EventArgs
    {

        private SPSceneManager _manager;
        private ISceneLoadOptions _options;

        private List<object> _stallInstructions;

        public SceneLoadingEventArgs(SPSceneManager manager, ISceneLoadOptions loadOptions)
        {
            _manager = manager;
            _options = loadOptions;
        }

        public SPSceneManager Manager { get { return _manager; } }

        public ISceneLoadOptions LoadOptions { get { return _options; } }

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
