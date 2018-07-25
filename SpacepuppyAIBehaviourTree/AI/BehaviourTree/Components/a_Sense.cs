#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree.Components
{
    public class a_Sense : AIActionComponent
    {

        public enum SelectionStyle
        {
            First = 0,
            Random = 1,
            Nearest = 2,
            Farthest = 3
        }

        [System.Flags()]
        public enum VariableUpdateOptions
        {
            ClearPreviousAspectIfUnseen = 1,
            OnlyUpdateAspectIfPreviousUnseen = 2,
            StoreOnlyPosition = 4,
            AlwaysUpdate = 8
        }

        #region Fields

        [SerializeField()]
        private Sensor _sensor;

        [SerializeField()]
        [AIVariableName()]
        [Tooltip("Name of the variable to set the found target to.")]
        private string _variable;

        [SerializeField()]
        [EnumFlags()]
        private VariableUpdateOptions _variableUpdateParams = VariableUpdateOptions.ClearPreviousAspectIfUnseen | VariableUpdateOptions.OnlyUpdateAspectIfPreviousUnseen;

        [SerializeField()]
        private SelectionStyle _selection;

        #endregion

        #region CONSTRUCTOR

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
            if (_sensor == null) return ActionResult.Failed;

            if (string.IsNullOrEmpty(_variable))
            {
                //siple find, no storing of found target
                return (_sensor.SenseAny()) ? ActionResult.Success : ActionResult.Failed;
            }
            else
            {
                if((_variableUpdateParams & VariableUpdateOptions.OnlyUpdateAspectIfPreviousUnseen) != 0)
                {
                    var old = ai.Variables[_variable] as IAspect;
                    if (old != null && _sensor.Visible(old)) return ActionResult.Success;
                }

                if((_variableUpdateParams & VariableUpdateOptions.ClearPreviousAspectIfUnseen) != 0)
                {
                    var old = ai.Variables[_variable] as IAspect;
                    if (old != null && !_sensor.Visible(old)) ai.Variables[_variable] = null;
                }

                IAspect target = null;
                switch (_selection)
                {
                    case SelectionStyle.First:
                        target = _sensor.Sense();
                        break;
                    case SelectionStyle.Random:
                        target = _sensor.SenseAll().PickRandom();
                        break;
                    case SelectionStyle.Nearest:
                        {
                            var pos = _sensor.transform.position;
                            target = (from a in _sensor.SenseAll() orderby (a.transform.position - pos).sqrMagnitude ascending select a).FirstOrDefault();
                        }
                        break;
                    case SelectionStyle.Farthest:
                        {
                            var pos = _sensor.transform.position;
                            target = (from a in _sensor.SenseAll() orderby (a.transform.position - pos).sqrMagnitude descending select a).FirstOrDefault();
                        }
                        break;
                }

                if (target == null)
                {
                    if ((_variableUpdateParams & VariableUpdateOptions.AlwaysUpdate) != 0)
                        ai.Variables[_variable] = null;

                    return ActionResult.Failed;
                }
                else
                {
                    if ((_variableUpdateParams & VariableUpdateOptions.StoreOnlyPosition) != 0)
                        ai.Variables[_variable] = target.transform.position;
                    else
                        ai.Variables[_variable] = target;

                    return ActionResult.Success;
                }
            }
        }

        #endregion

    }
}
