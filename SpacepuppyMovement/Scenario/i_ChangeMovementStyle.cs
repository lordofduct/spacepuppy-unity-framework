using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Movement;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_ChangeMovementStyle : AutoTriggerableMechanism
    {

        public enum ChangeMode
        {
            Change,
            Stack,
            Pop,
            PopAll,
            Purge
        }

        #region Fields

        [SerializeField]
        private MovementMotor _motor;
        [SerializeField]
        [TypeRestriction(typeof(IMovementStyle))]
        private Component _movementStyle;
        [SerializeField]
        private ChangeMode _mode;
        [SerializeField]
        private float _precedence;

        #endregion

        #region Properties

        public MovementMotor Motor
        {
            get { return _motor; }
            set { _motor = value; }
        }

        public IMovementStyle MovementStyle
        {
            get { return _movementStyle as IMovementStyle; }
            set { _movementStyle = value as Component; }
        }

        public ChangeMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public float Precedence
        {
            get { return _precedence; }
            set { _precedence = value; }
        }

        #endregion


        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            if (_motor == null) return false;
            
            try
            {
                switch(_mode)
                {
                    case ChangeMode.Change:
                        {
                            var style = this.MovementStyle;
                            if (style == null) return false;

                            if (_motor.States.Contains(style))
                            {
                                _motor.States.ChangeState(style, _precedence);
                                return true;
                            }
                        }
                        break;
                    case ChangeMode.Stack:
                        {
                            var style = this.MovementStyle;
                            if (style == null) return false;

                            if (_motor.States.Contains(style))
                            {
                                _motor.States.StackState(style, _precedence);
                                return true;
                            }
                        }
                        break;
                    case ChangeMode.Pop:
                        {
                            _motor.States.PopState(_precedence);
                            return true;
                        }
                    case ChangeMode.PopAll:
                        {
                            _motor.States.PopAllStates(_precedence);
                            return true;
                        }
                    case ChangeMode.Purge:
                        {
                            var style = this.MovementStyle;
                            if (style == null) return false;

                            if (_motor.States.Contains(style))
                            {
                                _motor.States.PurgeStackedState(style);
                                return true;
                            }
                        }
                        break;
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
            }

            return false;
        }
    }
}
