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
        /// The button was pressed and released in a set amount of time.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="getFixedState"></param>
        /// <returns></returns>
        bool GetPressed(float duration, bool getFixedState);
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
