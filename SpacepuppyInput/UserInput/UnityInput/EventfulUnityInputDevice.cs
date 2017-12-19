using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace com.spacepuppy.UserInput.UnityInput
{

    public class EventfulUnityInputDevice : IPlayerInputDevice, IUpdateable
    {

        public const string INPUT_ID = "sp.internal.evu.device";

        #region Singleton Interface

        private static EventfulUnityInputDevice _device;
        public static EventfulUnityInputDevice GetDevice()
        {
            if (_device != null) return _device;
            
            _device = new EventfulUnityInputDevice();
            GameLoopEntry.UpdatePump.Add(_device);
            return _device;
        }

        /* OLD
        private static EventfulUnityInputDevice _device;
        private static IGameInputManager _inputManager;
        public static EventfulUnityInputDevice GetDevice()
        {
            if (_device != null) return _device;
            
            if (_inputManager == null)
            {
                _inputManager = Services.Get<IGameInputManager>();
                if(_inputManager != null)
                {
                    _inputManager.ServiceUnregistered += (s, e) =>
                    {
                        _inputManager = null;
                        _device = null;
                    };
                }
                else
                {
                    return null;
                }
            }

            _device = new EventfulUnityInputDevice();
            _inputManager.Add(INPUT_ID, _device);
            return _device;
        }
        */

        #endregion

        #region Fields

        private Dictionary<string, System.Action<string>> _buttonPressTable = new Dictionary<string, Action<string>>();
        private bool _inUpdate;
        private System.Action _lateRegister;

        #endregion

        #region CONSTRUCTOR

        private EventfulUnityInputDevice()
        {
            //only one allowed
        }

        #endregion

        #region Methods

        public void RegisterButtonPress(string id, System.Action<string> callback)
        {
            if (id == null) throw new System.ArgumentNullException("id");
            if (callback == null) return;

            System.Action<string> d;
            if(_buttonPressTable.TryGetValue(id, out d))
            {
                callback = d + callback;
            }

            if(_inUpdate)
            {
                _lateRegister += () =>
                {
                    _buttonPressTable[id] = callback;
                    this.Active = true;
                };
            }
            else
            {
                _buttonPressTable[id] = callback;
                this.Active = true;
            }
        }

        public void UnregisterButtonPress(string id, System.Action<string> callback)
        {
            if (id == null) throw new System.ArgumentNullException("id");
            if (callback == null) return;

            if(_inUpdate)
            {
                _lateRegister += () =>
                {
                    this.UnregisterButtonPress(id, callback);
                };
            }
            else
            {
                System.Action<string> d;
                if (_buttonPressTable.TryGetValue(id, out d))
                {
                    d -= callback;
                    if (d == null)
                    {
                        _buttonPressTable.Remove(id);
                        if (_buttonPressTable.Count == 0)
                            this.Active = false;
                    }
                    else
                    {
                        _buttonPressTable[id] = callback;
                    }
                }
            }
        }

        public void Update()
        {
            _inUpdate = true;
            var e = _buttonPressTable.GetEnumerator();
            while (e.MoveNext())
            {
                if (Input.GetButtonDown(e.Current.Key)) e.Current.Value(e.Current.Key);
            }
            _inUpdate = false;

            if (_lateRegister != null)
            {
                var a = _lateRegister;
                _lateRegister = null;
                a();
            }
        }

        #endregion

        #region IPlayerInputDevice Interface

        public string Id
        {
            get
            {
                return INPUT_ID;
            }
        }

        public bool Active
        {
            get;
            set;
        }

        public int Hash
        {
            get
            {
                return INPUT_ID.GetHashCode();
            }
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

        void IInputSignature.FixedUpdate()
        {
        }

        public float GetCurrentAxleState(int hash)
        {
            return 0f;
        }

        public float GetCurrentAxleState(string id)
        {
            return Input.GetAxis(id);
        }

        public ButtonState GetCurrentButtonState(int hash)
        {
            return ButtonState.None;
        }

        public ButtonState GetCurrentButtonState(string id)
        {
            if (Input.GetButtonDown(id))
                return ButtonState.Down;
            else if (Input.GetButtonUp(id))
                return ButtonState.Released;
            else if (Input.GetButton(id))
                return ButtonState.Held;
            else
                return ButtonState.None;
        }

        public bool GetButtonPressed(string id, float duration)
        {
            //TODO - how to track this?
            throw new System.NotSupportedException();
        }

        public bool GetButtonPressed(int hash, float duration)
        {
            //TODO - how to track this?
            throw new System.NotSupportedException();
        }

        public Vector2 GetCurrentCursorState(int hash)
        {
            return Vector2.zero;
        }

        public Vector2 GetCurrentCursorState(string id)
        {
            return Vector2.zero;
        }

        public Vector2 GetCurrentDualAxleState(int hash)
        {
            return Vector2.zero;
        }

        public Vector2 GetCurrentDualAxleState(string id)
        {
            return Vector2.zero;
        }

        #endregion

    }

}
