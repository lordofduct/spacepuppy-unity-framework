#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Scenario
{

    /// <summary>
    /// Loops over collection of args and calls 'Action' for each one.
    /// </summary>
    [Infobox("Loops over collection of args and calls 'Action' for each one.")]
    public class i_TriggerForEachArg : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        [SelectableObject()]
        [ReorderableArray]
        private UnityEngine.Object[] _args;

        [SerializeField]
        private Trigger _action;

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            foreach(var obj in _args)
            {
                _action.ActivateTrigger(this, obj);
            }
            return true;
        }

        #endregion


    }
}
