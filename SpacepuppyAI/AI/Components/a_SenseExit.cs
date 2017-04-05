#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Components
{
    public class a_SenseExit : AIActionComponent
    {

        public enum VariableUpdateOptions
        {
            DoNothing = -1,
            SetNullOnExit = 0,
            SetLastPositionOnExit = 1
        }

        #region Fields

        [SerializeField()]
        private Sensor _sensor;

        [SerializeField()]
        [AIVariableName()]
        [Tooltip("Name of the variable to set the found target to.")]
        private string _variable;

        [SerializeField()]
        private VariableUpdateOptions _variableUpdateParams;

        #endregion

        #region Properties

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; }
        }

        #endregion
        
        #region IAIAction Interface

        protected override ActionResult OnTick(IAIController ai)
        {
            if (_sensor.IsNullOrDestroyed()) return ActionResult.Failed;

            if (string.IsNullOrEmpty(_variable))
            {
                return ActionResult.Failed;
            }

            var aspect = ai.Variables[_variable] as IAspect;
            if (aspect.IsNullOrDestroyed())
            {
                return ActionResult.Success;
            }

            if(_sensor.Visible(aspect))
            {
                return ActionResult.Waiting;
            }
            else
            {
                switch (_variableUpdateParams)
                {
                    case VariableUpdateOptions.SetNullOnExit:
                        ai.Variables[_variable] = null;
                        break;
                    case VariableUpdateOptions.SetLastPositionOnExit:
                        ai.Variables[_variable] = (aspect.IsAlive()) ? (object)aspect.transform.position : null;
                        break;
                }

                return ActionResult.Success;
            }
        }

        #endregion

    }
}
