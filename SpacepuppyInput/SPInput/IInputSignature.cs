using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput
{

    public interface IInputSignature
    {

        string Id { get; }
        float Precedence { get; set; }
        
        /// <summary>
        /// Only called by the IPlayerInputDevice on Update, if called independent of IPlayerInputDevice, state of the input signature will be inaccurate.
        /// </summary>
        void Update();

        /// <summary>
        /// Only called by the IPlayerInputDevice on FixedUpdate, if called independent of IPlayerInputDevice, state of the input signature will be inaccurate.
        /// </summary>
        void FixedUpdate();

    }

    public interface IButtonInputSignature : IInputSignature
    {
        ButtonState CurrentState { get; }

        ButtonState GetCurrentState(bool getFixedState);

        /// <summary>
        /// Consumes any 'Down' or 'Released' ButtonState so no other object can react to it by converting 'Down' to 'Held' and 'Released' to 'None'.
        /// </summary>
        void Consume();

        /// <summary>
        /// Last time 'Down' was signaled (as realTimeSinceStartup)
        /// </summary>
        float LastDownTime { get; }
        
    }

    public interface IAxleInputSignature : IInputSignature
    {
        float CurrentState { get; }

        float DeadZone { get; set; }
        DeadZoneCutoff Cutoff { get; set; }

    }

    public interface IDualAxleInputSignature : IInputSignature
    {
        Vector2 CurrentState { get; }

        float DeadZone { get; set; }
        DeadZoneCutoff Cutoff { get; set; }
        float RadialDeadZone { get; set; }
        DeadZoneCutoff RadialCutoff { get; set; }

    }

    public interface ICursorInputSignature : IInputSignature
    {
        Vector2 CurrentState { get; }
    }

    public abstract class BaseInputSignature : IInputSignature
    {
        
        #region Fields

        private string _id;

        #endregion
        
        #region CONSTRUCTOR

        public BaseInputSignature(string id)
        {
            _id = id;
        }
        
        #endregion

        #region IInputSignature Interfacce

        public string Id { get { return _id; } }
        
        public float Precedence { get; set; }
        
        public abstract void Update();

        public virtual void FixedUpdate()
        {

        }

        #endregion
        
    }

}
