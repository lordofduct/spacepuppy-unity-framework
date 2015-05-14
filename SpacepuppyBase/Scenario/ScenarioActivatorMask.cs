using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public class ScenarioActivatorMask
    {

        #region Fields

        public bool TestRoot = false;

        [SerializeField()]
        private LayerMask _layerMask = -1;

        [SerializeField()]
        private string[] _tags;

        #endregion

        #region CONSTRUCTOR

        public ScenarioActivatorMask(LayerMask mask)
        {
            _layerMask = mask;
        }

        public ScenarioActivatorMask(string[] tags)
        {
            _tags = tags;
        }

        public ScenarioActivatorMask(LayerMask mask, string[] tags)
        {
            _layerMask = mask;
            _tags = tags;
        }

        public ScenarioActivatorMask(LayerMask mask, bool bTestRoot)
        {
            _layerMask = mask;
            this.TestRoot = bTestRoot;
        }

        public ScenarioActivatorMask(string[] tags, bool bTestRoot)
        {
            _tags = tags;
            this.TestRoot = bTestRoot;
        }

        public ScenarioActivatorMask(LayerMask mask, string[] tags, bool bTestRoot)
        {
            _layerMask = mask;
            _tags = tags;
            this.TestRoot = bTestRoot;
        }

        #endregion

        #region Properties

        public LayerMask LayerMask
        {
            get { return _layerMask; }
            set { _layerMask = value; }
        }

        public string[] Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        #endregion

        #region Methods

        public bool Intersects(GameObject go)
        {
            if (go == null) return false;
            if (!go.IntersectsLayerMask(_layerMask)) return false;
            if (_tags != null && _tags.Length > 0 && !go.HasTag(_tags)) return false;

            if (this.TestRoot)
            {
                var root = go.FindRoot();
                if (root != go)
                {
                    if (!root.IntersectsLayerMask(_layerMask)) return false;
                    if (_tags != null && _tags.Length > 0 && !root.HasTag(_tags)) return false;
                }
            }

            return true;
        }

        public bool Intersects(Component comp)
        {
            if (comp == null) return false;
            return Intersects(comp.gameObject);
        }

        #endregion

    }
}
