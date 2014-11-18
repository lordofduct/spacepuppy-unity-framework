using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{
    public sealed class TagData : ScriptableObject, ISerializationCallbackReceiver, IEnumerable<string>
    {

        #region Static Interface

        private static TagData _instance;
        private static bool _loaded;

        public static IList<string> Tags
        {
            get
            {
                if (!_loaded)
                {
                    _instance = Resources.Load(@"TagData") as TagData;
                    _loaded = true;
                }
                return (_instance != null) ? _instance._readonlyTags : null;
            }
        }

        public static bool IsValidTag(string stag)
        {
            return TagData.Tags.Contains(stag);
        }

        #endregion

        #region Fields

        [SerializeField()]
        private string[] _tags;

        [System.NonSerialized()]
        private IList<string> _readonlyTags;

        #endregion

        #region ISerializationCallbackReceiver Interface
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _readonlyTags = new System.Collections.ObjectModel.ReadOnlyCollection<string>(_tags);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
        #endregion

        #region IEnumerable Interface
        public IEnumerator<string> GetEnumerator()
        {
            return (_tags != null) ? (_tags as IEnumerable<string>).GetEnumerator() : Enumerable.Empty<string>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_tags != null) ? _tags.GetEnumerator() : Enumerable.Empty<string>().GetEnumerator();
        }
        #endregion

        #region Special Types

        public class EditorHelper
        {
            private TagData _data;

            public EditorHelper(TagData data)
            {
                _data = data;
            }

            public IList<string> Tags { get { return _data._readonlyTags; } }

            public void UpdateTags(IEnumerable<string> tags)
            {
                if (tags == null) throw new System.ArgumentNullException("tags");
                _data._tags = tags.ToArray();
                _data._readonlyTags = new System.Collections.ObjectModel.ReadOnlyCollection<string>(_data._tags);
            }

        }

        #endregion



    }
}
