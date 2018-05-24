using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{

    [System.Obsolete("Use SPAnimationMask Intead")]
    [System.Serializable()]
    public class MaskCollection : ICollection<TransformMask>
    {

        public System.EventHandler Changed;

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
                if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
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
            if (_owner != null && m.Transform != null) _owner.RemoveMixingTransform(m.Transform);
            if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
        }

        public void AddRange(IEnumerable<TransformMask> coll)
        {
            foreach (var m in coll)
            {
                this.Add(m);
            }
            if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
        }

        public void AddRange(MaskCollection coll)
        {
            for (int i = 0; i < coll._masks.Count; i++)
            {
                this.Add(coll._masks[i]);
            }
            if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
        }

        public void Copy(IEnumerable<TransformMask> coll)
        {
            this.SilentClear();
            foreach (var m in coll)
            {
                this.Add(m);
            }
            if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
        }

        public void Copy(MaskCollection coll)
        {
            this.SilentClear();
            for(int i = 0; i < coll._masks.Count; i++)
            {
                this.Add(coll._masks[i]);
            }
            if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
        }

        public void Apply(AnimationState state)
        {
            foreach (var m in _masks)
            {
                if (m.Transform != null) state.AddMixingTransform(m.Transform, m.Recursive);
            }
        }



        private void SilentClear()
        {
            if (_owner != null)
            {
                foreach (var m in _masks)
                {
                    if (m.Transform != null) _owner.RemoveMixingTransform(m.Transform);
                }
            }
            _masks.Clear();
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
                if (_owner != null && item.Transform != null) _owner.RemoveMixingTransform(item.Transform);
            }

            _masks.Add(item);
            if (_owner != null && item.Transform != null) _owner.AddMixingTransform(item.Transform, item.Recursive);
            if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
        }

        public void Clear()
        {
            if (_owner != null)
            {
                foreach (var m in _masks)
                {
                    if (m.Transform != null) _owner.RemoveMixingTransform(m.Transform);
                }
            }
            _masks.Clear();
            if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
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
                if (_owner != null && item.Transform != null) _owner.RemoveMixingTransform(item.Transform);
                if (this.Changed != null) this.Changed(this, System.EventArgs.Empty);
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
