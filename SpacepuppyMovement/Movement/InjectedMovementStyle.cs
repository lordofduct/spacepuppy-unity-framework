using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Movement
{
    public class InjectedMovementStyle : SPComponent, IMovementStyle
    {

        #region Fields

        private System.Action _updateMovementCallback;
        private RadicalCoroutine _routine;

        #endregion

        #region Methods

        private void Purge()
        {
            if (_routine != null) _routine.Cancel();

            _updateMovementCallback = null;
            _routine = null;
        }

        #endregion

        #region IMovementStyle Interface

        public void OnActivate(IMovementStyle lastStyle, ActivationReason reason)
        {

        }

        public void OnDeactivate(IMovementStyle nextStyle, ActivationReason reason)
        {
            if(reason == ActivationReason.Standard)
            {
                Object.Destroy(this);
            }
        }

        public void OnPurgedFromStack()
        {
            Object.Destroy(this);
        }

        public void UpdateMovement()
        {
            if(_updateMovementCallback != null)
            {
                _updateMovementCallback();
            }
            else if(_routine != null)
            {
                _routine.ManualTick(this);
            }
        }

        public void OnUpdateMovementComplete()
        {

        }

        #endregion

        #region Static Factory

        public static InjectedMovementStyle InjectMovement(MovementMotor motor, System.Action updateCallback, float precedence = 0)
        {
            if (motor == null) throw new System.ArgumentNullException("motor");
            if (updateCallback == null) throw new System.ArgumentNullException("updateCallback");

            var style = motor.AddComponent<InjectedMovementStyle>();
            style.Purge();
            style._updateMovementCallback = updateCallback;
            motor.States.ChangeState(style, precedence);
            return style;
        }

        public static InjectedMovementStyle InjectMovement(MovementMotor motor, System.Func<System.Collections.IEnumerator> routine, float precedence = 0)
        {
            if (motor == null) throw new System.ArgumentNullException("motor");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var style = motor.AddComponent<InjectedMovementStyle>();
            style.Purge();
            style._routine = new RadicalCoroutine(routine());
            motor.States.ChangeState(style, precedence);
            return style;
        }

        public static InjectedMovementStyle InjectMovement(MovementMotor motor, System.Collections.IEnumerable routine, float precedence = 0)
        {
            if (motor == null) throw new System.ArgumentNullException("motor");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var style = motor.AddComponent<InjectedMovementStyle>();
            style.Purge();
            style._routine = new RadicalCoroutine(routine.GetEnumerator());
            motor.States.ChangeState(style, precedence);
            return style;
        }

        public static InjectedMovementStyle StackInjectMovement(MovementMotor motor, System.Action updateCallback, float precedence = 0)
        {
            if (motor == null) throw new System.ArgumentNullException("motor");
            if (updateCallback == null) throw new System.ArgumentNullException("updateCallback");

            var style = motor.AddComponent<InjectedMovementStyle>();
            style.Purge();
            style._updateMovementCallback = updateCallback;
            motor.States.StackState(style, precedence);
            return style;
        }

        public static InjectedMovementStyle StackInjectMovement(MovementMotor motor, System.Func<System.Collections.IEnumerator> routine, float precedence = 0)
        {
            if (motor == null) throw new System.ArgumentNullException("motor");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var style = motor.AddComponent<InjectedMovementStyle>();
            style.Purge();
            style._routine = new RadicalCoroutine(routine());
            motor.States.StackState(style, precedence);
            return style;
        }

        public static InjectedMovementStyle StackInjectMovement(MovementMotor motor, System.Collections.IEnumerable routine, float precedence = 0)
        {
            if (motor == null) throw new System.ArgumentNullException("motor");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var style = motor.AddComponent<InjectedMovementStyle>();
            style.Purge();
            style._routine = new RadicalCoroutine(routine.GetEnumerator());
            motor.States.StackState(style, precedence);
            return style;
        }

        #endregion

    }
}
