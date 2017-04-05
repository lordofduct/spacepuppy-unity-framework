
namespace com.spacepuppy.Movement
{
    public class WaitForBeforeUpdateMovement : ImmediatelyResumingYieldInstruction, IResettingYieldInstruction
    {

        private MovementMotor _motor;

        public WaitForBeforeUpdateMovement(MovementMotor motor)
        {
            if (motor == null) throw new System.ArgumentNullException("motor");
            _motor = motor;
        }

        #region Methods

        protected void Reset()
        {
            this.ResetSignal();
            _motor.BeforeUpdateMovement -= this.OnBeforeUpdateMovement;
            _motor.BeforeUpdateMovement += this.OnBeforeUpdateMovement;
        }

        #endregion

        #region Handlers

        private void OnBeforeUpdateMovement(object sender, System.EventArgs e)
        {
            _motor.BeforeUpdateMovement -= this.OnBeforeUpdateMovement;

            this.SetSignal();
        }

        #endregion

        #region IResettingYieldInstruction Interface

        void IResettingYieldInstruction.Reset()
        {
            this.Reset();
        }

        #endregion

    }
}
