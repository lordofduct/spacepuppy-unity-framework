using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.SPInput
{

    /// <summary>
    /// This allows multiple input signatures to register for a single input id. So say 2 buttons can perform the same action. 
    /// The order of precedence is in the order of the signatures in the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MergedInputSignature<T> : BaseInputSignature where T : IInputSignature
    {

        #region Fields

        private UniqueList<T> _signatures = new UniqueList<T>();

        #endregion

        #region CONSTRUCTOR
        
        public MergedInputSignature(string id)
            : base(id)
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

    public class MergedButtonInputSignature : MergedInputSignature<IButtonInputSignature>, IButtonInputSignature
    {

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #region CONSTRUCTOR

        public MergedButtonInputSignature(string id)
            : base(id)
        {
        }

        public MergedButtonInputSignature(string id, IButtonInputSignature a, IButtonInputSignature b)
            : base(id)
        {
            this.Signatures.Add(a);
            this.Signatures.Add(b);
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

    public class MergedAxleInputSignature : MergedInputSignature<IAxleInputSignature>, IAxleInputSignature
    {

        private MergedAxlePrecedence _axlePrecedence;
        
        #region CONSTRUCTOR
        
        public MergedAxleInputSignature(string id)
            : base(id)
        {
        }

        public MergedAxleInputSignature(string id, IAxleInputSignature a, IAxleInputSignature b)
            : base(id)
        {
            this.Signatures.Add(a);
            this.Signatures.Add(b);
        }

        #endregion

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public MergedAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public float CurrentState
        {
            get
            {
                float result = 0f;
                switch(_axlePrecedence)
                {
                    case MergedAxlePrecedence.FirstActive:
                        for (int i = 0; i < this.Signatures.Count; i++)
                        {
                            if (this.Signatures[i].CurrentState > 0f) result = this.Signatures[i].CurrentState;
                        }
                        break;
                    case MergedAxlePrecedence.LastActive:
                        for (int i = this.Signatures.Count - 1; i >= 0; i--)
                        {
                            if (this.Signatures[i].CurrentState > 0f) result = this.Signatures[i].CurrentState;
                        }
                        break;
                    case MergedAxlePrecedence.Largest:
                        if (this.Signatures.Count > 0)
                        {
                            result = 0f;
                            for (int i = 0; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState > result) result = this.Signatures[i].CurrentState;
                            }
                        }
                        break;
                    case MergedAxlePrecedence.Smallest:
                        if(this.Signatures.Count > 0)
                        {
                            result = this.Signatures[0].CurrentState;
                            for (int i = 1; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState < result) result = this.Signatures[i].CurrentState;
                            }
                        }
                        break;
                }

                return InputUtil.CutoffAxis(result, this.DeadZone, this.Cutoff);
            }
        }
    }

    public class MergedDualAxleInputSignature : MergedInputSignature<IDualAxleInputSignature>, IDualAxleInputSignature
    {
        
        private MergedAxlePrecedence _axlePrecedence;
        
        #region CONSTRUCTOR
        
        public MergedDualAxleInputSignature(string id)
            : base(id)
        {
        }

        public MergedDualAxleInputSignature(string id, IDualAxleInputSignature a, IDualAxleInputSignature b)
            : base(id)
        {
            this.Signatures.Add(a);
            this.Signatures.Add(b);
        }

        #endregion

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public float RadialDeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff RadialCutoff
        {
            get;
            set;
        }

        public MergedAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public Vector2 CurrentState
        {
            get
            {
                const float EPSILON = 0.0001f;
                Vector2 result = Vector2.zero;
                switch (_axlePrecedence)
                {
                    case MergedAxlePrecedence.FirstActive:
                        for (int i = 0; i < this.Signatures.Count; i++)
                        {
                            if (this.Signatures[i].CurrentState.sqrMagnitude > EPSILON) result = this.Signatures[i].CurrentState;
                        }
                        break;
                    case MergedAxlePrecedence.LastActive:
                        for (int i = this.Signatures.Count - 1; i >= 0; i--)
                        {
                            if (this.Signatures[i].CurrentState.sqrMagnitude > EPSILON) result = this.Signatures[i].CurrentState;
                        }
                        break;
                    case MergedAxlePrecedence.Largest:
                        if (this.Signatures.Count > 0)
                        {
                            float mag = 0f;
                            for (int i = 0; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState.sqrMagnitude > mag)
                                {
                                    result = this.Signatures[i].CurrentState;
                                    mag = result.sqrMagnitude;
                                }
                            }
                        }
                        break;
                    case MergedAxlePrecedence.Smallest:
                        if (this.Signatures.Count > 0)
                        {
                            result = this.Signatures[0].CurrentState;
                            float mag = 0f;
                            for (int i = 1; i < this.Signatures.Count; i++)
                            {
                                if (this.Signatures[i].CurrentState.sqrMagnitude < result.sqrMagnitude)
                                {
                                    result = this.Signatures[i].CurrentState;
                                    mag = result.sqrMagnitude;
                                }
                            }
                        }
                        break;
                }

                return InputUtil.CutoffDualAxis(result, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            }
        }
    }

}
