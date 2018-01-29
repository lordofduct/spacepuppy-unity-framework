using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity
{

    [System.Serializable]
    public class InputLayoutToken<TInputId> where TInputId : struct, System.IConvertible
    {

        public string Id;
        [UnityEngine.SerializeField]
        private List<InputTokenKeyValuePair> _inputs = new List<InputTokenKeyValuePair>();

        public List<InputTokenKeyValuePair> Inputs
        {
            get { return _inputs; }
        }

        [System.Serializable]
        public struct InputTokenKeyValuePair
        {
            public TInputId InputId;
            public InputToken InputToken;

            public InputTokenKeyValuePair(TInputId id, InputToken token)
            {
                this.InputId = id;
                this.InputToken = token;
            }
        }

    }

}
