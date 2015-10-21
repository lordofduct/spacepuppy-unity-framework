using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_OnStart : TriggerComponent
    {

        #region Fields

        public float Delay;

        #endregion

        #region Messages

        protected override void Start()
        {
            base.Start();

            if (this.Delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.ActivateTrigger(this);
                }, this.Delay);
            }
            else
            {
                this.ActivateTrigger(this);
            }

        }

        #endregion

    }
}
