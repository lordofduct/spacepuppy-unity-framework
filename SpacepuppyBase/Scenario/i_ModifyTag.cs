#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;

namespace com.spacepuppy.Scenario
{

    public class i_ModifyTag : AutoTriggerableMechanism
    {

        public enum Modes
        {
            Set = 0,
            Add = 1,
            Remove = 2
        }

        #region Fields

        [SerializeField]
        private TriggerableTargetObject _target;

        [SerializeField]
        [TagSelector(AllowUntagged = true)]
        private string _tag;

        [SerializeField]
        private Modes _mode;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public Modes Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        #endregion

        #region Triggerable Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var go = _target.GetTarget<GameObject>(arg);
            if (go == null) return false;

            switch(_mode)
            {
                case Modes.Set:
                    go.SetTag(_tag);
                    break;
                case Modes.Add:
                    go.AddTag(_tag);
                    break;
                case Modes.Remove:
                    go.RemoveTag(_tag);
                    break;
            }
            return true;
        }

        #endregion

    }

}
