using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [System.Serializable()]
    public class TagMask : ICollection<string>
    {

        #region Fields

        [SerializeField()]
        private bool _intersectAll = true;
        [SerializeField()]
        private List<string> _tags = new List<string>();

        #endregion

        #region CONSTRUCTOR

        public TagMask()
        {

        }

        public TagMask(bool intersectsAll)
        {
            _intersectAll = intersectsAll;
        }

        public TagMask(params string[] tags)
        {
            foreach (var t in tags)
            {
                this.Add(t);
            }
        }

        public TagMask(IEnumerable<string> tags)
        {
            if (tags == null) throw new System.ArgumentNullException("tags");
            foreach (var t in tags)
            {
                this.Add(t);
            }
        }

        #endregion

        #region Properties

        public bool IntersectAll
        {
            get { return _intersectAll; }
            set
            {
                _intersectAll = value;
                if (_intersectAll) _tags.Clear();
            }
        }

        #endregion

        #region Methods

        public bool Intersects(GameObject go)
        {
            if (_intersectAll) return true;
            return go.HasTag(_tags);
        }

        public bool Intersects(Component c)
        {
            if (_intersectAll) return true;
            return c.HasTag(_tags);
        }

        public bool Intersects(string tag)
        {
            if (_intersectAll) return true;
            return _tags.Contains(tag);
        }

        public bool Intersects(params string[] tags)
        {
            if (_intersectAll) return true;
            return _tags.ContainsAny(tags);
        }

        public bool Intersects(IEnumerable<string> tags)
        {
            if (_intersectAll) return true;
            return _tags.ContainsAny(tags);
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _tags.Count; }
        }

        public void Add(string tag)
        {
            if (_intersectAll) return;
            if (!MultiTag.IsValidTag(tag)) return;

            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                if (_tags.SimilarTo(TagData.Tags))
                {
                    _tags.Clear();
                    _intersectAll = true;
                }
            }
        }

        public void Clear()
        {
            _tags.Clear();
            _intersectAll = false;
        }

        public bool Remove(string tag)
        {
            if (_intersectAll)
            {
                if (!MultiTag.IsValidTag(tag)) return false;
                _tags.AddRange(TagData.Tags);
                _tags.Remove(tag);
                _intersectAll = false;
                return true;
            }
            else
            {
                return _tags.Remove(tag);
            }
        }
        bool ICollection<string>.Contains(string item)
        {
            return this.Intersects(item);
        }

        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
            if (_tags != null) _tags.CopyTo(array, arrayIndex);
        }

        bool ICollection<string>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable Interface

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            if (_intersectAll) return TagData.Tags.GetEnumerator();
            return (_tags != null) ? (_tags as IEnumerable<string>).GetEnumerator() : Enumerable.Empty<string>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_intersectAll) return TagData.Tags.GetEnumerator();
            return (_tags != null) ? _tags.GetEnumerator() : Enumerable.Empty<string>().GetEnumerator();
        }

        #endregion

    }

}
