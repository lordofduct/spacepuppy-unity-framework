#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_TriggerRandomElimination : AutoTriggerableMechanism
    {

        public enum TriggerMode
        {
            Manual = 0,
            AllAtOnce = 1
        }

        #region Fields

        [SerializeField()]
        [Trigger.Config(Weighted = true)]
        private Trigger _targets;

        [SerializeField]
        [Tooltip("Leave 0 or negative to mean trigger all.")]
        private int _selectionCount;

        [SerializeField]
        private TriggerMode _mode;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("PassAlongTriggerArg")]
        private bool _passAlongTriggerArg;

        [SerializeField]
        private Trigger _onComplete;

        [System.NonSerialized]
        private HashSet<TriggerTarget> _used = new HashSet<TriggerTarget>();
        [System.NonSerialized]
        private bool _blockTrigger;

        #endregion

        #region Properties

        public Trigger Targets
        {
            get { return _targets; }
        }

        public int SelectionCount
        {
            get { return _selectionCount; }
            set { _selectionCount = value; }
        }

        public TriggerMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public Trigger OnComplete
        {
            get { return _onComplete; }
        }

        #endregion

        #region Methods

        public void ResetSelection()
        {
            _used.Clear();
        }

        public bool TargetHasBeenUsed(int index)
        {
            if (index < 0) return false;
            if (index >= _targets.Targets.Count) return false;

            return _used.Contains(_targets.Targets[index]);
        }
        
        private int GetNormalizedSelectionCount()
        {
            return (_selectionCount <= 0 || _selectionCount > _targets.Targets.Count) ? _targets.Targets.Count : _selectionCount;
        }

        private bool SelectAndActivate(object arg)
        {
            if (_used.Count >= _targets.Targets.Count)
            {
                //we should not have gotten here, but just incase
                _used.Clear();
            }

            var targ = _targets.Where((t) => !_used.Contains(t)).PickRandom((t) => t.Weight);
            if (targ != null)
            {
                _used.Add(targ);
                targ.Trigger(this, _passAlongTriggerArg ? arg : null);

                int cnt = this.GetNormalizedSelectionCount();
                if (_used.Count >= cnt)
                {
                    _used.Clear();
                    if (_onComplete.Count > 0) _onComplete.ActivateTrigger(this, null);
                }

                return true;
            }

            return false;
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && !_blockTrigger;
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            switch (_mode)
            {
                case TriggerMode.Manual:
                    {
                        this.SelectAndActivate(arg);
                    }
                    break;
                case TriggerMode.AllAtOnce:
                    {
                        _blockTrigger = true;
                        _used.Clear();
                        int cnt = this.GetNormalizedSelectionCount();
                        for (int i = 0; i < cnt; i++)
                        {
                            this.SelectAndActivate(arg);
                        }
                        _blockTrigger = false;
                    }
                    break;
            }

            return false;
        }

        #endregion

    }
}
