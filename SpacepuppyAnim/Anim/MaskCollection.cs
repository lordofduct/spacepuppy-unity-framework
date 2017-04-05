using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{

    [System.Serializable()]
    public class MaskCollection : ICollection<TransformMask>
    {

        #region Fields

        [System.NonSerialized()]
        private AnimationState _owner;
        [SerializeField()]
        private List<TransformMask> _masks = new List<TransformMask>();

        #endregion

        #region CONSTRUCTOR

        public MaskCollection()
        {

        }

        internal void SetState(AnimationState state)
        {
            _owner = state;
            if (_owner != null && _masks.Count > 0) this.Apply(_owner);
        }

        #endregion

        #region Methods

        public void Add(Transform t, bool recursive)
        {
            this.Add(new TransformMask(t, recursive));
        }

        public bool Remove(Transform t)
        {
            int i = this.IndexOfTransform(t);
            if (i >= 0)
            {
                _masks.RemoveAt(i);
                return true;
            }
            return false;
        }

        internal int IndexOfTransform(Transform t)
        {
            for (int i = 0; i < _masks.Count; i++)
            {
                if (_masks[i].Transform == t) return i;
            }
            return -1;
        }

        internal void RemoveAt(int index)
        {
            var m = _masks[index];
            _masks.RemoveAt(index);
            if (_owner != null) _owner.RemoveMixingTransform(m.Transform);
        }

        public void AddRange(IEnumerable<TransformMask> coll)
        {
            foreach (var m in coll)
            {
                this.Add(m);
            }
        }

        public void Copy(IEnumerable<TransformMask> coll)
        {
            this.Clear();
            foreach (var m in coll)
            {
                this.Add(m);
            }
        }

        public void Apply(AnimationState state)
        {
            foreach (var m in _masks)
            {
                state.AddMixingTransform(m.Transform, m.Recursive);
            }
        }

        #endregion

        #region ICollection Interface

        public void Add(TransformMask item)
        {
            if (item.Transform == null) throw new System.ArgumentException("Mask contains no transform.");

            var i = this.IndexOfTransform(item.Transform);
            if (i >= 0)
            {
                _masks.RemoveAt(i);
                if (_owner != null) _owner.RemoveMixingTransform(item.Transform);
            }

            _masks.Add(item);
            if (_owner != null) _owner.AddMixingTransform(item.Transform, item.Recursive);
        }

        public void Clear()
        {
            if (_owner != null)
            {
                foreach (var m in _masks)
                {
                    _owner.RemoveMixingTransform(m.Transform);
                }
            }
            _masks.Clear();
        }

        public bool Contains(TransformMask item)
        {
            return _masks.Contains(item);
        }

        public void CopyTo(TransformMask[] array, int arrayIndex)
        {
            _masks.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _masks.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TransformMask item)
        {
            if (_masks.Contains(item))
            {
                _masks.Remove(item);
                if (_owner != null) _owner.RemoveMixingTransform(item.Transform);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<TransformMask> GetEnumerator()
        {
            return _masks.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _masks.GetEnumerator();
        }

        #endregion

    }

}
