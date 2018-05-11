using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Project
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
                if (!_loaded) Reload();
                return (_instance != null) ? _instance._readonlyTags : null;
            }
        }

        public static bool IsValidTag(string stag)
        {
            return TagData.Tags.Contains(stag);
        }

        public static TagData Asset
        {
            get
            {
                if (!_loaded) Reload();
                return _instance;
            }
        }

        private static void Reload()
        {
            _instance = Resources.Load(@"TagData") as TagData;
            if (_instance == null) _instance = ScriptableObject.CreateInstance<TagData>();
            _loaded = true;
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



        #region Static Utils

        public static bool IsDefaultUnityTag(string tag)
        {
            switch(tag)
            {
                case SPConstants.TAG_UNTAGGED:
                case SPConstants.TAG_RESPAWN:
                case SPConstants.TAG_FINISH:
                case SPConstants.TAG_EDITORONLY:
                case SPConstants.TAG_MAINCAMERA:
                case SPConstants.TAG_GAMECONTROLLER:
                case SPConstants.TAG_PLAYER:
                    return true;
                default:
                    return false;
            }
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

            public TagData Target { get { return _data; } }

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
