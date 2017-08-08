#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_TriggerRandom : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [Trigger.Config(Weighted=true)]
        private Trigger _targets;
        
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("PassAlongTriggerArg")]
        private bool _passAlongTriggerArg;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay = 0f;

        #endregion

        #region Properties

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Methdos

        private void DoTriggerNext(object arg)
        {
            if (this._passAlongTriggerArg)
                _targets.ActivateRandomTrigger(this, arg, true);
            else
                _targets.ActivateRandomTrigger(this, null, true);
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (this._delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.DoTriggerNext(arg);
                }, this._delay);
            }
            else
            {
                this.DoTriggerNext(arg);
            }

            return true;
        }

        #endregion

    }
}
