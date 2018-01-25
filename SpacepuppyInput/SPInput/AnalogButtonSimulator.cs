using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput
{

    [System.Serializable()]
    public class AnalogButtonSimulator
    {

        [Range(0f, 1f)]
        [SerializeField()]
        public float DeadZone = 0.8f;
        [System.NonSerialized()]
        private ButtonState _currentState = ButtonState.None;

        public ButtonState Button { get { return _currentState; } }

        public void Update(float impact)
        {
            if (impact > this.DeadZone)
            {
                switch (_currentState)
                {
                    case ButtonState.None:
                    case ButtonState.Released:
                        _currentState = ButtonState.Down;
                        break;
                    case ButtonState.Down:
                    case ButtonState.Held:
                        _currentState = ButtonState.Held;
                        break;
                }
            }
            else
            {
                switch (_currentState)
                {
                    case ButtonState.None:
                    case ButtonState.Released:
                        _currentState = ButtonState.None;
                        break;
                    case ButtonState.Down:
                    case ButtonState.Held:
                        _currentState = ButtonState.Released;
                        break;
                }
            }
        }

        public void Reset()
        {
            _currentState = ButtonState.None;
        }
    }
}
