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

        public virtual void Reset()
        {
            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Reset();
            }
        }

        #endregion

        #region IPlayerInputDevice Interface

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public bool AnyInputActivated
        {
            get
            {
                var e = _signatures.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.GetInputIsActivated()) return true;
                }
                return false;
            }
        }

        public bool Contains(string id)
        {
            return _signatures.Contains(id);
        }

        IInputSignature IInputDevice.GetSignature(string id)
        {
            return _signatures.GetSignature(id);
        }

        public ButtonState GetButtonState(string id, bool consume = false)
        {
            if (!_active) return ButtonState.None;

            var sig = _signatures.GetSignature(id);
            if (sig is IButtonInputSignature)
            {
                var result = (sig as IButtonInputSignature).CurrentState;
                if (consume) (sig as IButtonInputSignature).Consume();
                return result;
            }
            else if (sig is IAxleInputSignature)
                return (sig as IAxleInputSignature).CurrentState > InputUtil.DEFAULT_AXLEBTNDEADZONE ? ButtonState.Held : ButtonState.None;
            else if (sig is IDualAxleInputSignature)
                return (sig as IDualAxleInputSignature).CurrentState.sqrMagnitude > InputUtil.DEFAULT_AXLEBTNDEADZONE * InputUtil.DEFAULT_AXLEBTNDEADZONE ? ButtonState.Held : ButtonState.None;
            else
                return ButtonState.None;
        }
        
        public float GetAxleState(string id)
        {
            if (!_active) return 0f;

            var sig = _signatures.GetSignature(id);
            if (sig is IAxleInputSignature)
                return (sig as IAxleInputSignature).CurrentState;
            else if (sig is IDualAxleInputSignature)
                return (sig as IDualAxleInputSignature).CurrentState.x;
            else if (sig is IButtonInputSignature)
                return (sig as IButtonInputSignature).CurrentState > ButtonState.None ? 1f : 0f;
            else
                return 0f;
        }
        
        public Vector2 GetDualAxleState(string id)
        {
            if (!_active) return Vector2.zero;

            var sig = _signatures.GetSignature(id);
            if (sig is IDualAxleInputSignature)
                return (sig as IDualAxleInputSignature).CurrentState;
            else if (sig is IAxleInputSignature)
                return new Vector2((sig as IAxleInputSignature).CurrentState, 0f);
            else if (sig is IButtonInputSignature)
                return new Vector2((sig as IButtonInputSignature).CurrentState > ButtonState.None ? 1f : 0f, 0f);
            else
                return Vector2.zero;
        }
        
        public Vector2 GetCursorState(string id)
        {
            if (!_active) return Vector2.zero;

            var sig = _signatures.GetSignature(id);
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
