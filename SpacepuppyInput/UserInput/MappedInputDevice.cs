using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace com.spacepuppy.UserInput
{

    /// <summary>
    /// InputDevice based on a mapping value instead of a hash. This mapping value should usually be an enum, you can also use an int/long/etc if you want.
    /// 
    /// When creating IInputSignatures to add to this, it's best if its hash is set to the enum value that it maps to. It's not required, but the IInputDevice.Get...(int hash) methods will all fail. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MappedInputDevice<T> : IMappedInputDevice<T> where T : struct, System.IConvertible
    {

        #region Fields

        private string _id;
        private bool _active = true;
        private MappedInputSignaturCollection<T> _signatures = new MappedInputSignaturCollection<T>();

        #endregion

        #region CONSTRUCTOR

        public MappedInputDevice(string id)
        {
            _id = id;
        }

        #endregion

        #region Properties

        public MappedInputSignaturCollection<T> InputSignatures { get { return _signatures; } }

        #endregion

        #region IInputSignature Interface

        public string Id
        {
            get { return _id; }
        }

        public int Hash
        {
            get { return _id.GetHashCode(); }
        }

        float IInputSignature.Precedence
        {
            get
            {
                return 0f;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        void IInputSignature.Update()
        {
            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Update();
            }
        }

        void IInputSignature.FixedUpdate()
        {
            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.FixedUpdate();
            }
        }

        #endregion

        #region IInputDevice Interface

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public ButtonState GetButtonState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return ButtonState.None;
            if (!(sig is IButtonInputSignature)) return ButtonState.None;

            return (sig as IButtonInputSignature).CurrentState;
        }

        ButtonState IInputDevice.GetButtonState(int hash)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return ButtonState.None;
            if (!(sig is IButtonInputSignature)) return ButtonState.None;

            return (sig as IButtonInputSignature).CurrentState;
        }

        public bool GetButtonPressed(string id, float duration)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return false;
            if (!(sig is IButtonInputSignature)) return false;

            return (sig as IButtonInputSignature).GetPressed(duration, GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate);
        }

        bool IInputDevice.GetButtonPressed(int hash, float duration)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return false;
            if (!(sig is IButtonInputSignature)) return false;

            return (sig as IButtonInputSignature).GetPressed(duration, GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate);
        }

        public float GetAxleState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return 0f;
            if (!(sig is IAxleInputSignature)) return 0f;

            return (sig as IAxleInputSignature).CurrentState;
        }

        float IInputDevice.GetAxleState(int hash)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return 0f;
            if (!(sig is IAxleInputSignature)) return 0f;

            return (sig as IAxleInputSignature).CurrentState;
        }

        public Vector2 GetDualAxleState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return Vector2.zero;
            if (!(sig is IDualAxleInputSignature)) return Vector2.zero;

            return (sig as IDualAxleInputSignature).CurrentState;
        }

        Vector2 IInputDevice.GetDualAxleState(int hash)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return Vector2.zero;
            if (!(sig is IDualAxleInputSignature)) return Vector2.zero;

            return (sig as IDualAxleInputSignature).CurrentState;
        }

        public Vector2 GetCursorState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return Vector2.zero;
            if (!(sig is ICursorInputSignature)) return Vector2.zero;

            return (sig as ICursorInputSignature).CurrentState;
        }

        Vector2 IInputDevice.GetCursorState(int hash)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return Vector2.zero;
            if (!(sig is ICursorInputSignature)) return Vector2.zero;

            return (sig as ICursorInputSignature).CurrentState;
        }

        #endregion

        #region IMappedInputDevice Interface

        public ButtonState GetButtonState(T mapping)
        {
            var sig = _signatures.GetSignature(mapping);
            if (sig == null) return ButtonState.None;
            if (!(sig is IButtonInputSignature)) return ButtonState.None;

            return (sig as IButtonInputSignature).CurrentState;
        }

        public bool GetButtonPressed(T mapping, float duration)
        {
            var sig = _signatures.GetSignature(mapping);
            if (sig == null) return false;
            if (!(sig is IButtonInputSignature)) return false;

            return (sig as IButtonInputSignature).GetPressed(duration, GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate);
        }

        public float GetAxleState(T mapping)
        {
            var sig = _signatures.GetSignature(mapping);
            if (sig == null) return 0f;
            if (!(sig is IAxleInputSignature)) return 0f;

            return (sig as IAxleInputSignature).CurrentState;
        }

        public Vector2 GetDualAxleState(T mapping)
        {
            var sig = _signatures.GetSignature(mapping);
            if (sig == null) return Vector2.zero;
            if (!(sig is IDualAxleInputSignature)) return Vector2.zero;

            return (sig as IDualAxleInputSignature).CurrentState;
        }

        public Vector2 GetCursorState(T mapping)
        {
            var sig = _signatures.GetSignature(mapping);
            if (sig == null) return Vector2.zero;
            if (!(sig is ICursorInputSignature)) return Vector2.zero;

            return (sig as ICursorInputSignature).CurrentState;
        }

        #endregion


    }

}
