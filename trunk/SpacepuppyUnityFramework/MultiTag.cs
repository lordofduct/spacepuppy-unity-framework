using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [AddComponentMenu("SpacePuppy/Multi Tag")]
    public class MultiTag : com.spacepuppy.SPComponent
    {

        [SerializeField]
        private string[] _tags;

        // Use this for initialization
        protected override void Awake()
        {
            base.Awake();

            this.gameObject.tag = SPConstants.TAG_MULTITAG;
        }

        public void UpdateTags(string[] tags)
        {
            _tags = tags;
        }

        public IEnumerable<string> GetTags()
        {
            if (_tags == null) yield break;

            foreach (var tag in _tags) yield return tag;
        }

        public bool ContainsTag(string tag)
        {
            if (_tags == null) return false;
            return System.Array.IndexOf(_tags, tag) >= 0;
        }

        public bool ContainsTag(params string[] tags)
        {
            if (_tags == null) return false;
            return _tags.ContainsAny(tags);
        }

        public void AddTag(string tag)
        {
            if (_tags == null)
            {
                _tags = new string[] { tag };
            }
            else
            {
                if (!_tags.Contains(tag))
                {
                    System.Array.Resize(ref _tags, _tags.Length + 1);
                    _tags[_tags.Length - 1] = tag;
                }
            }
        }

        public bool RemoveTag(string tag)
        {
            if (_tags == null) return false;

            if (_tags.Contains(tag))
            {
                var lst = new List<string>(_tags);
                if (lst.Remove(tag))
                {
                    _tags = lst.ToArray();
                }
            }

            return false;
        }
    }

}