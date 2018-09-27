#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.SPInput;

namespace com.spacepuppy.Scenario
{

    public class t_OnKey : SPComponent, IObservableTrigger
    {
        
        #region Fields

        [SerializeField]
        [ReorderableArray]
        private List<KeyInfo> _keys;

        #endregion

        #region Properties

        public List<KeyInfo> Keys
        {
            get
            {
                return _keys;
            }
        }

        #endregion

        #region Methods

        private void Update()
        {
            foreach(var key in _keys)
            {
                if (key == null) continue;

                switch(key.State)
                {
                    case ButtonState.Released:
                        if(Input.GetKeyUp(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                    case ButtonState.None:
                        if (!Input.GetKey(key.Key) && !Input.GetKeyUp(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                    case ButtonState.Down:
                        if(Input.GetKeyDown(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                    case ButtonState.Held:
                        if (Input.GetKey(key.Key) && !Input.GetKeyDown(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                }
            }
        }

        #endregion

        #region IObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return (from k in _keys where k != null select k.OnKey).ToArray();
        }

        #endregion

        #region Special Types

        [System.Serializable]
        public class KeyInfo
        {
            public KeyCode Key = KeyCode.Return;
            public ButtonState State = ButtonState.Down;
            [SerializeField]
            [SPEvent.Config(AlwaysExpanded = true)]
            private Trigger _onKey = new Trigger();

            public Trigger OnKey
            {
                get { return _onKey; }
            }
        }

        #endregion

    }

}
