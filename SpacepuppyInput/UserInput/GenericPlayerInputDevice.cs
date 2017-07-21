using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using System;

namespace com.spacepuppy.UserInput
{
    public class GenericPlayerInputDevice : IPlayerInputDevice
    {

        #region Fields

        private string _id;
        private bool _active = true;
        private InputSignatureCollection _signatures = new InputSignatureCollection();

        #endregion

        #region CONSTRUCTOR

        public GenericPlayerInputDevice(string id)
        {
            _id = id;
        }

        #endregion

        #region Properties

        public InputSignatureCollection InputSignatures { get { return _signatures; } }

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

        public float Precedence
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

        public virtual void Update()
        {
            _signatures.Sort();

            var e = _signatures.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Update();
            }
        }

        public virtual void FixedUpdate()
        {
            _signatures.Sort();

            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.FixedUpdate();
            }
        }

        #endregion

        #region IPlayerInputDevice Interface

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public ButtonState GetCurrentButtonState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return ButtonState.None;
            if (!(sig is IButtonInputSignature)) return ButtonState.None;

            return (sig as IButtonInputSignature).CurrentState;
        }

        public ButtonState GetCurrentButtonState(int hash)
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

        public bool GetButtonPressed(int hash, float duration)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return false;
            if (!(sig is IButtonInputSignature)) return false;

            return (sig as IButtonInputSignature).GetPressed(duration, GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate);
        }

        public float GetCurrentAxleState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return 0f;
            if (!(sig is IAxleInputSignature)) return 0f;

            return (sig as IAxleInputSignature).CurrentState;
        }

        public float GetCurrentAxleState(int hash)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return 0f;
            if (!(sig is IAxleInputSignature)) return 0f;

            return (sig as IAxleInputSignature).CurrentState;
        }

        public Vector2 GetCurrentDualAxleState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return Vector2.zero;
            if (!(sig is IDualAxleInputSignature)) return Vector2.zero;

            return (sig as IDualAxleInputSignature).CurrentState;
        }

        public Vector2 GetCurrentDualAxleState(int hash)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return Vector2.zero;
            if (!(sig is IDualAxleInputSignature)) return Vector2.zero;

            return (sig as IDualAxleInputSignature).CurrentState;
        }

        public Vector2 GetCurrentCursorState(string id)
        {
            var sig = _signatures.GetSignature(id);
            if (sig == null) return Vector2.zero;
            if (!(sig is ICursorInputSignature)) return Vector2.zero;

            return (sig as ICursorInputSignature).CurrentState;
        }

        public Vector2 GetCurrentCursorState(int hash)
        {
            var sig = _signatures.GetSignature(hash);
            if (sig == null) return Vector2.zero;
            if (!(sig is ICursorInputSignature)) return Vector2.zero;

            return (sig as ICursorInputSignature).CurrentState;
        }

        #endregion

        #region HashCodeOverride

        public override int GetHashCode()
        {
            return this.Hash;
        }

        #endregion

    }
}
