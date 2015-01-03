using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{

    [System.Serializable()]
    public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {

        #region Fields

        [SerializeField()]
        private TKey[] _serializedKeys;
        [SerializeField()]
        private TValue[] _serializedValues;

        #endregion


        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();

            int cnt = Mathf.Min(_serializedKeys.Length, _serializedValues.Length);
            for(int i = 0; i < cnt; i++)
            {
                this.Add(_serializedKeys[i], _serializedValues[i]);
            }
            _serializedKeys = null;
            _serializedValues = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _serializedKeys = this.Keys.ToArray();
            _serializedValues = this.Values.ToArray();
        }

        #endregion

    }
}
