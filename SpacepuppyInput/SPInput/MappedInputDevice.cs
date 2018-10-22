using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace com.spacepuppy.SPInput
{

    /// <summary>
    /// InputDevice based on a mapping value instead of a hash. This mapping value should usually be an enum, you can also use an int/long/etc if you want.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MappedInputDevice<T> : IMappedInputDevice<T> where T : struct, System.IConvertible
    {

        #region Fields

        private string _id;
        private bool _active = true;
        private MappedInputSignatureCollection<T> _signatures;

        #endregion

        #region CONSTRUCTOR

        public MappedInputDevice(string id)
        {
            _id = id;
            _signatures = new MappedInputSignatureCollection<T>();
        }

        public MappedInputDevice(string id, MappedInputSignatureCollection<T> sig)
        {
            _id = id;
            _signatures = sig ?? new MappedInputSignatureCollection<T>();
        }

        public MappedInputDevice(string id, IEqualityComparer<T> comparer)
        {
            _id = id;
            _signatures = new MappedInputSignatureCollection<T>(comparer);
        }

        #endregion

        #region Properties

        public MappedInputSignatureCollection<T> InputSignatures { get { return _signatures; } }

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
            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Update();
            }
        }
        
        public virtual void FixedUpdate()
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

        public bool AnyInputActivated
        {
            get
            {
                var e = _signatures.GetEnumerator();
                while(e.MoveNext())
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

        #region IMappedInputDevice Interface

        IInputSignature IMappedInputDevice<T>.GetSignature(T mapping)
        {
            return _signatures.GetSignature(mapping);
        }

        public ButtonState GetButtonState(T mapping, bool consume = false)
        {
            if (!_active) return ButtonState.None;

            var sig = _signatures.GetSignature(mapping);
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
        
        public float GetAxleState(T mapping)
        {
            if (!_active) return 0f;

            var sig = _signatures.GetSignature(mapping);
            if (sig is IAxleInputSignature)
                return (sig as IAxleInputSignature).CurrentState;
            else if (sig is IDualAxleInputSignature)
                return (sig as IDualAxleInputSignature).CurrentState.x;
            else if (sig is IButtonInputSignature)
                return (sig as IButtonInputSignature).CurrentState > ButtonState.None ? 1f : 0f;
            else
                return 0f;
        }

        public Vector2 GetDualAxleState(T mapping)
        {
            if (!_active) return Vector2.zero;

            var sig = _signatures.GetSignature(mapping);
            if (sig is IDualAxleInputSignature)
                return (sig as IDualAxleInputSignature).CurrentState;
            else if (sig is IAxleInputSignature)
                return new Vector2((sig as IAxleInputSignature).CurrentState, 0f);
            else if (sig is IButtonInputSignature)
                return new Vector2((sig as IButtonInputSignature).CurrentState > ButtonState.None ? 1f : 0f, 0f);
            else
                return Vector2.zero;
        }

        public Vector2 GetCursorState(T mapping)
        {
            if (!_active) return Vector2.zero;

            var sig = _signatures.GetSignature(mapping);
            if (sig == null) return Vector2.zero;
            if (!(sig is ICursorInputSignature)) return Vector2.zero;

            return (sig as ICursorInputSignature).CurrentState;
        }

        #endregion


    }

}
