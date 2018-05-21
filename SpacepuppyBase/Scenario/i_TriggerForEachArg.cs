#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    /// <summary>
    /// Loops over collection of args and calls 'Action' for each one.
    /// </summary>
    [Infobox("Loops over collection of args and calls 'Action' for each one.")]
    public class i_TriggerForEachArg : AutoTriggerableMechanism
    {

        public enum TargetMode
        {
            All = 0,
            First = 1,
            FirstUnique = 2
        }

        #region Fields

        [SerializeField]
        [Tooltip("When running each query which entries should be used. Note that 'FirstUnique' is the slowest.")]
        private TargetMode _targetSearchMode = TargetMode.All;

        [SerializeField]
        [ReorderableArray]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Object), AlwaysExpanded = true)]
        private TriggerableTargetObject[] _targets;

        [SerializeField]
        private Trigger _action;

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            
            switch(_targetSearchMode)
            {
                case TargetMode.All:
                    foreach (var t1 in _targets)
                    {
                        foreach (var t2 in t1.GetTargets<UnityEngine.Object>(arg))
                        {
                            if (t2 != null) _action.ActivateTrigger(this, t2);
                        }
                    }
                    break;
                case TargetMode.First:
                    foreach (var t1 in _targets)
                    {
                        var t2 = t1.GetTarget<UnityEngine.Object>(arg);
                        if (t2 != null) _action.ActivateTrigger(this, t2);
                    }
                    break;
                case TargetMode.FirstUnique:
                    using(var set = TempCollection.GetSet<UnityEngine.Object>())
                    {
                        foreach (var t1 in _targets)
                        {
                            foreach (var t2 in t1.GetTargets<UnityEngine.Object>(arg))
                            {
                                if(t2 != null && !set.Contains(t2))
                                {
                                    set.Add(t2);
                                    _action.ActivateTrigger(this, t2);
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
            return true;
        }

        #endregion


    }
}
