#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_TriggerSequence : SPComponent, ITriggerableMechanism
    {

        public enum SequenceMode
        {
            Oblivion,
            Clamp,
            Loop,
            PingPong
        }


        #region Fields

        [SerializeField()]
        private int _order;

        [SerializeField()]
        private SequenceMode _mode;

        [SerializeField()]
        private Trigger _trigger;

        [SerializeField()]
        private bool _passAlongTriggerArg;

        [System.NonSerialized()]
        private int _currentIndex = 0;
        
        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties
        
        public SequenceMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public Trigger Trigger
        {
            get
            {
                return _trigger;
            }
        }

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }

        public int CurrentIndexNormalized
        {
            get
            {
                switch (_mode)
                {
                    case SequenceMode.Oblivion:
                        return _currentIndex;
                    case SequenceMode.Clamp:
                        return Mathf.Clamp(_currentIndex, 0, _trigger.Targets.Count - 1);
                    case SequenceMode.Loop:
                        return _currentIndex % _trigger.Targets.Count;
                    case SequenceMode.PingPong:
                        return (int)Mathf.PingPong(_currentIndex, _trigger.Targets.Count - 1);
                    default:
                        return _currentIndex;
                }
            }
        }

        #endregion

        #region Methods

        #endregion

        #region ITriggerableMechanism Interface

        public int Order
        {
            get { return _order; }
        }

        public bool CanTrigger
        {
            get { return this.isActiveAndEnabled; }
        }

        public void ActivateTrigger()
        {
            this.ActivateTrigger(null);
        }

        public bool ActivateTrigger(object arg)
        {
            if (!this.CanTrigger) return false;

            if (_passAlongTriggerArg)
                _trigger.ActivateTriggerAt(this.CurrentIndexNormalized, arg);
            else
                _trigger.ActivateTriggerAt(this.CurrentIndexNormalized);

            _currentIndex++;

            return true;
        }

        void ITriggerableMechanism.Trigger()
        {
            this.ActivateTrigger(null);
        }

        bool ITriggerableMechanism.Trigger(object arg)
        {
            return this.ActivateTrigger(arg);
        }

        #endregion

    }
}
