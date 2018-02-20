using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.SPInput
{
    public class GenericInputDevice : IInputDevice
    {

        #region Fields

        private string _id;
        private bool _active = true;
        private IInputSignatureCollection _signatures;

        #endregion

        #region CONSTRUCTOR

        public GenericInputDevice(string id)
        {
            _id = id;
            _signatures = new InputSignatureCollection();
        }

        public GenericInputDevice(string id, IInputSignatureCollection coll)
        {
            _id = id;
            _signatures = coll ?? new InputSignatureCollection();
        }

        #endregion

        #region Properties

        public IInputSignatureCollection InputSignatures { get { return _signatures; } }

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
        
        public virtual void Update()
        {
            //_signatures.Sort();

            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Update();
            }
        }

        public virtual void FixedUpdate()
        {
            //_signatures.Sort();

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

        public bool Contains(string id)
        {
            return _signatures.Contains(id);
        }

        public ButtonState GetButtonState(string id)
        {
            if (!_active) return ButtonState.None;

            var sig = _signatures.GetSignature(id);
            if (sig == null) return ButtonState.None;
            if (!(sig is IButtonInputSignature)) return ButtonState.None;

            return (sig as IButtonInputSignature).CurrentState;
        }
        
        public bool GetButtonPressed(string id, float duration)
        {
            if (!_active) return false;

            var sig = _signatures.GetSignature(id);
            if (sig == null) return false;
            if (!(sig is IButtonInputSignature)) return false;

            return (sig as IButtonInputSignature).GetPressed(duration, GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate);
        }
        
        public float GetAxleState(string id)
        {
            if (!_active) return 0f;

            var sig = _signatures.GetSignature(id);
            if (sig == null) return 0f;
            if (!(sig is IAxleInputSignature)) return 0f;

            return (sig as IAxleInputSignature).CurrentState;
        }
        
        public Vector2 GetDualAxleState(string id)
        {
            if (!_active) return Vector2.zero;

            var sig = _signatures.GetSignature(id);
            if (sig == null) return Vector2.zero;
            if (!(sig is IDualAxleInputSignature)) return Vector2.zero;

            return (sig as IDualAxleInputSignature).CurrentState;
        }
        
        public Vector2 GetCursorState(string id)
        {
            if (!_active) return Vector2.zero;

            var sig = _signatures.GetSignature(id);
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
