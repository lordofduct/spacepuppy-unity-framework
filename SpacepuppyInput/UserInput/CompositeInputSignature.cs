using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.UserInput
{

    /// <summary>
    /// This allows multiple input signatures to register for a single input id. So say 2 buttons can perform the same action. 
    /// The order of precedence is in the order of the signatures in the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CompositeInputSignature<T> : BaseInputSignature where T : IInputSignature
    {

        #region Fields

        private UniqueList<T> _signatures = new UniqueList<T>();

        #endregion

        #region CONSTRUCTOR
        
        public CompositeInputSignature(string id)
            : base(id)
        {
        }

        public CompositeInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        #region Properties

        public IList<T> Signatures { get { return _signatures; } }

        #endregion

        #region Methods

        public override void Update()
        {
            for(int i = 0; i < _signatures.Count; i++)
            {
                _signatures[i].Update();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            for(int i = 0; i < _signatures.Count; i++)
            {
                _signatures[i].FixedUpdate();
            }
        }

        #endregion

    }

    public class CompositeButtonInputSignature : CompositeInputSignature<IButtonInputSignature>, IButtonInputSignature
    {

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #region CONSTRUCTOR

        public CompositeButtonInputSignature(string id)
            : base(id)
        {
        }

        public CompositeButtonInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        public ButtonState CurrentState
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return _currentFixed;
                }
                else
                {
                    return _current;
                }
            }
        }

        public ButtonState GetCurrentState(bool getFixedState)
        {
            return (getFixedState) ? _currentFixed : _current;
        }

        public bool GetPressed(float duration, bool getFixedState)
        {
            if (getFixedState)
            {
                return _currentFixed == ButtonState.Released && Time.unscaledTime - _lastDown <= duration;
            }
            else
            {
                return _current == ButtonState.Released && Time.unscaledTime - _lastDown <= duration;
            }
        }

        public override void Update()
        {
            base.Update();

            bool down = false;
            for(int i = 0; i < this.Signatures.Count; i++)
            {
                if(this.Signatures[i].GetCurrentState(false) >= ButtonState.Down)
                {
                    down = true;
                    break;
                }
            }
            _current = InputUtil.GetNextButtonState(_current, down);

            if (_current == ButtonState.Down)
                _lastDown = Time.unscaledTime;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool down = false;
            for (int i = 0; i < this.Signatures.Count; i++)
            {
                if (this.Signatures[i].GetCurrentState(true) >= ButtonState.Down)
                {
                    down = true;
                    break;
                }
            }
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, down);
        }
    }

    public class CompositeAxleInputSignature : CompositeInputSignature<IAxleInputSignature>, IAxleInputSignature
    {

        private CompositeAxlePrecedence _axlePrecedence;
        
        #region CONSTRUCTOR
        
        public CompositeAxleInputSignature(string id)
            : base(id)
        {
        }

        public CompositeAxleInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        public CompositeAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public float CurrentState
        {
            get
            {
                switch(_axlePrecedence)
                {
                    case CompositeAxlePrecedence.FirstActive:
                        for (int i = 0; i < this.Signatures.Count; i++)
                        {
                            if (this.Signatures[i].CurrentState > 0f) return this.Signatures[i].CurrentState;
                        }
                        break;
                    case CompositeAxlePrecedence.LastActive:
                        for (int i = this.Signatures.Count - 1; i >= 0; i--)
                        {
                            if (this.Signatures[i].CurrentState > 0f) return this.Signatures[i].CurrentState;
                        }
                        break;
                    case CompositeAxlePrecedence.Largest:
                        if (this.Signatures.Count > 0)
                        {
                            float v = 0f;
                            for (int i = 0; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState > v) v = this.Signatures[i].CurrentState;
                            }
                            return v;
                        }
                        break;
                    case CompositeAxlePrecedence.Smallest:
                        if(this.Signatures.Count > 0)
                        {
                            float v = this.Signatures[0].CurrentState;
                            for (int i = 1; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState < v) v = this.Signatures[i].CurrentState;
                            }
                            return v;
                        }
                        break;
                }
                return 0f;
            }
        }
    }

    public class CompositeDualAxleInputSignature : CompositeInputSignature<IDualAxleInputSignature>, IDualAxleInputSignature
    {
        
        private CompositeAxlePrecedence _axlePrecedence;
        
        #region CONSTRUCTOR
        
        public CompositeDualAxleInputSignature(string id)
            : base(id)
        {
        }

        public CompositeDualAxleInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        public CompositeAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public Vector2 CurrentState
        {
            get
            {
                const float EPSILON = 0.0001f;
                switch (_axlePrecedence)
                {
                    case CompositeAxlePrecedence.FirstActive:
                        for (int i = 0; i < this.Signatures.Count; i++)
                        {
                            if (this.Signatures[i].CurrentState.sqrMagnitude > EPSILON) return this.Signatures[i].CurrentState;
                        }
                        break;
                    case CompositeAxlePrecedence.LastActive:
                        for (int i = this.Signatures.Count - 1; i >= 0; i--)
                        {
                            if (this.Signatures[i].CurrentState.sqrMagnitude > EPSILON) return this.Signatures[i].CurrentState;
                        }
                        break;
                    case CompositeAxlePrecedence.Largest:
                        if (this.Signatures.Count > 0)
                        {
                            Vector2 v = Vector2.zero;
                            float mag = 0f;
                            for (int i = 0; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState.sqrMagnitude > mag)
                                {
                                    v = this.Signatures[i].CurrentState;
                                    mag = v.sqrMagnitude;
                                }
                            }
                            return v;
                        }
                        break;
                    case CompositeAxlePrecedence.Smallest:
                        if (this.Signatures.Count > 0)
                        {
                            Vector2 v = this.Signatures[0].CurrentState;
                            float mag = 0f;
                            for (int i = 1; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState.sqrMagnitude < v.sqrMagnitude)
                                {
                                    v = this.Signatures[i].CurrentState;
                                    mag = v.sqrMagnitude;
                                }
                            }
                            return v;
                        }
                        break;
                }
                return Vector2.zero;
            }
        }
    }

}
