using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree.Components
{

    [System.Serializable()]
    public class GameObjectConfigurableAIActionGroup : ConfigurableAIActionGroup
    {

        #region Fields

        [SerializeField()]
        private GameObject _actionSequenceContainer;

        #endregion

        #region Properties

        public GameObject ActionSequenceContainer
        {
            get { return _actionSequenceContainer; }
            set { _actionSequenceContainer = value; }
        }

        public override string DisplayName
        {
            get
            {
                if (_actionSequenceContainer != null)
                    return string.Format("{0} : {1}", base.DisplayName, _actionSequenceContainer.name);
                else
                    return base.DisplayName;
            }
        }

        #endregion

        #region Methods

        public void SyncActions()
        {
            if (_actionSequenceContainer.IsNullOrDestroyed())
            {
                this.SetEmpty();
            }
            else
            {
                this.SyncActions(_actionSequenceContainer, true);
            }
        }

        #endregion
        
    }

}
