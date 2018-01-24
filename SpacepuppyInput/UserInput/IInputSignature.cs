using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.UserInput
{

    public interface IInputSignature
    {

        string Id { get; }
        int Hash { get; }
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

    }

    public interface IDualAxleInputSignature : IInputSignature
    {
        Vector2 CurrentState { get; }
    }

    public interface ICursorInputSignature : IInputSignature
    {
        Vector2 CurrentState { get; }
    }

    public abstract class BaseInputSignature : IInputSignature
    {

        #region Static Interface

        private static TinyUidGenerator _uidGenerator = new TinyUidGenerator();

        public static int GetNextHash()
        {
            return _uidGenerator.Next().GetHashCode();
        }

        #endregion


        #region Fields

        private string _id;
        private int _hash;

        #endregion
        
        #region CONSTRUCTOR

        public BaseInputSignature(string id)
        {
            _id = id;
            _hash = BaseInputSignature.GetNextHash();
        }

        public BaseInputSignature(string id, int hash)
        {
            _id = id;
            _hash = hash;
        }

        #endregion

        #region IInputSignature Interfacce

        public string Id { get { return _id; } }

        public int Hash { get { return _hash; } }

        public float Precedence { get; set; }

        public abstract void Update();

        public virtual void FixedUpdate()
        {

        }

        #endregion

        #region HashCodeOverride

        public override int GetHashCode()
        {
            return _hash;
        }

        #endregion

    }

}
