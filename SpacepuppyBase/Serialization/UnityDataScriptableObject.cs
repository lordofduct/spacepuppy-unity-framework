using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Serialization
{
    public class UnityDataScriptableObject : ScriptableObject, IUnityData
    {

        #region Fields

        [SerializeField()]
        private byte[] _data;

        [SerializeField()]
        private UnityEngine.Object[] _unityObjectReferences;

        #endregion

        #region Properties

        public int Size
        {
            get
            {
                var dl = (_data != null) ? _data.Length : 0;
                var rl = (_unityObjectReferences != null) ? _unityObjectReferences.Length : 0;
                return dl + rl;
            }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            _data = new byte[] { };
            _unityObjectReferences = new UnityEngine.Object[] { };
        }

        void IUnityData.SetData(System.IO.Stream data, Object[] refs)
        {
            _data = data.ToByteArray();
            _unityObjectReferences = refs ?? new UnityEngine.Object[] { };
        }

        void IUnityData.GetData(System.IO.Stream data, out Object[] refs)
        {
            data.Write(_data, 0, _data.Length);
            refs = _unityObjectReferences;
        }

        #endregion

    }
}
