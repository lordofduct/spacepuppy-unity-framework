using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{
    public class CustomTimeLayersData : ScriptableObject, ISerializationCallbackReceiver, IEnumerable<string>
    {

        #region Static Interface

        private static CustomTimeLayersData _instance;
        private static bool _loaded;

        public static IList<string> Layers
        {
            get
            {
                if (!_loaded)
                {
                    _instance = Resources.Load(@"CustomTimeLayersData") as CustomTimeLayersData;
                    if (_instance == null) _instance = ScriptableObject.CreateInstance<CustomTimeLayersData>();
                    _loaded = true;
                }
                return (_instance != null) ? _instance._readonlyLayers : null;
            }
        }

        #endregion

        #region Fields

        [SerializeField()]
        private List<string> _customTimeLayers;

        [System.NonSerialized()]
        private IList<string> _readonlyLayers;

        #endregion

        #region ISerializationCallbackReceiver Interface
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _readonlyLayers = new System.Collections.ObjectModel.ReadOnlyCollection<string>(_customTimeLayers);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
        #endregion

        #region IEnumerable Interface
        public IEnumerator<string> GetEnumerator()
        {
            return (_customTimeLayers != null) ? (_customTimeLayers as IEnumerable<string>).GetEnumerator() : Enumerable.Empty<string>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_customTimeLayers != null) ? _customTimeLayers.GetEnumerator() : Enumerable.Empty<string>().GetEnumerator();
        }
        #endregion

        #region Special Types

        public class EditorHelper
        {
            private CustomTimeLayersData _data;

            public EditorHelper(CustomTimeLayersData data)
            {
                _data = data;
                if (_data._customTimeLayers == null) _data._customTimeLayers = new List<string>();
            }

            public CustomTimeLayersData Data { get { return _data; } }

            public List<string> Layers { get { return _data._customTimeLayers; } }

        }

        #endregion

    }
}
