#pragma warning disable 1066 // default parameter values ignored

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity
{

    public class ConfigurableInputProfile<TInputId> : IConfigurableInputProfile<TInputId> where TInputId : struct, System.IConvertible
    {

        #region Fields

        private IInputProfile<TInputId> _innerProfile;
        private Dictionary<TInputId, InputToken> _customTable = new Dictionary<TInputId, InputToken>();

        #endregion

        #region CONSTRUCTOR

        public ConfigurableInputProfile()
        {

        }

        public ConfigurableInputProfile(IInputProfile<TInputId> innerProfile)
        {
            _innerProfile = innerProfile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// An ID field that can be used for identification purposes.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        public IInputProfile<TInputId> InnerProfile
        {
            get { return _innerProfile; }
            set { _innerProfile = value; }
        }

        public bool ContainsCustomLayout
        {
            get { return _customTable.Count > 0; }
        }

        #endregion

        #region Methods

        public void ApplyToToken(InputLayoutToken<TInputId> token)
        {
            var e = _customTable.GetEnumerator();
            while (e.MoveNext())
            {
                token.Inputs.Add(new InputLayoutToken<TInputId>.InputTokenKeyValuePair()
                {
                    InputId = e.Current.Key,
                    InputToken = e.Current.Value
                });
            }
        }

        public void LoadFromToken(InputLayoutToken<TInputId> token)
        {
            _customTable.Clear();
            if (token == null || token.Inputs == null) return;

            var e = token.Inputs.GetEnumerator();
            while (e.MoveNext())
            {
                _customTable[e.Current.InputId] = e.Current.InputToken;
            }
        }

        #endregion

        #region IConfigurableInputProfile Interface

        public bool Contains(TInputId id)
        {
            return _customTable.ContainsKey(id) || (_innerProfile != null && _innerProfile.Contains(id));
        }

        public InputToken GetMapping(TInputId id)
        {
            InputToken token;
            if (_customTable.TryGetValue(id, out token)) return token;

            if (_innerProfile != null) return _innerProfile.GetMapping(id);

            return InputToken.Unknown;
        }

        public void Reset()
        {
            _customTable.Clear();
        }

        public void SetAxisMapping(TInputId id, InputToken token)
        {
            _customTable[id] = token;
        }

        public void SetButtonMapping(TInputId id, InputToken token)
        {
            _customTable[id] = token;
        }

        bool IInputProfile<TInputId>.TryPollAxis(out TInputId axis, out float value, Joystick joystick = Joystick.All, float deadZone = 0.707F)
        {
            //TODO - should we poll the custom inputs? No?
            if (_innerProfile != null)
            {
                return _innerProfile.TryPollAxis(out axis, out value, joystick, deadZone);
            }
            else
            {
                axis = default(TInputId);
                value = 0f;
                return false;
            }
        }

        bool IInputProfile<TInputId>.TryPollButton(out TInputId button, Joystick joystick = Joystick.All)
        {
            //TODO - should we poll the custom inputs? No?
            if (_innerProfile != null)
            {
                return _innerProfile.TryPollButton(out button, joystick);
            }
            else
            {
                button = default(TInputId);
                return false;
            }
        }

        #endregion

    }

}
