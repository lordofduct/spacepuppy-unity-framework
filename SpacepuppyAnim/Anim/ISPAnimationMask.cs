#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{

    public interface ISPAnimationMask
    {
        
        void Apply(SPAnimationController controller, AnimationState state);
        void Redact(SPAnimationController controller, AnimationState state);

    }

    [CreateAssetMenu(fileName = "SPAnimMask", menuName = "Spacepuppy/SPAnim Mask")]
    public sealed class SPAnimationMaskAsset : ScriptableObject, ICollection<SPAnimationMaskAsset.MaskEntry>, ISPAnimationMask
    {

        #region Fields

        [SerializeField]
        private Transform _avatar;
        [SerializeField]
        private List<MaskEntry> _masks;
        
        #endregion

        #region CONSTRUCTOR
        
        #endregion

        #region Properties

        public int Count
        {
            get { return _masks.Count; }
        }
        
        public Transform Avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }

        #endregion

        #region Methods

        public void Apply(SPAnimationController controller, AnimationState state)
        {
            foreach(var entry in _masks)
            {
                var t = controller.transform.Find(entry.Path);
                if(t != null)
                {
                    state.AddMixingTransform(t, entry.Recurse);
                }
            }
        }

        public void Redact(SPAnimationController controller, AnimationState state)
        {
            foreach (var entry in _masks)
            {
                var t = controller.transform.Find(entry.Path);
                if (t != null)
                {
                    state.RemoveMixingTransform(t);
                }
            }
        }

        public IEnumerable<Transform> GetTransforms(SPAnimationController controller)
        {
            foreach (var entry in _masks)
            {
                var t = controller.transform.Find(entry.Path);
                if (t != null)
                {
                    yield return t;
                }
            }
        }


        public MaskEntry Find(string path)
        {
            for (int i = 0; i < _masks.Count; i++)
            {
                if (_masks[i].Path == path) return _masks[i];
            }

            return default(MaskEntry);
        }

        public void Add(string path, bool recurse)
        {
            this.Add(new MaskEntry()
            {
                Path = path,
                Recurse = recurse
            });
        }

        public void Remove(string path)
        {
            for (int i = 0; i < _masks.Count; i++)
            {
                if (_masks[i].Path == path)
                {
                    _masks.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion

        #region ICollection Interface

        bool ICollection<MaskEntry>.IsReadOnly
        {
            get { return false; }
        }

        public void Add(MaskEntry entry)
        {
            if (!entry.IsValid) return;

            _masks.Add(entry);
        }

        public bool Remove(MaskEntry entry)
        {
            return _masks.Remove(entry);
        }

        public void Clear()
        {
            _masks.Clear();
        }

        public bool Contains(MaskEntry entry)
        {
            return _masks.Contains(entry);
        }

        public void CopyTo(MaskEntry[] array, int arrayIndex)
        {
            _masks.CopyTo(array, arrayIndex);
        }
        
        #endregion

        #region IEnumerable Interface

        public List<MaskEntry>.Enumerator GetEnumerator()
        {
            return _masks.GetEnumerator();
        }

        IEnumerator<MaskEntry> IEnumerable<MaskEntry>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Special Types

        [System.Serializable]
        public struct MaskEntry
        {
            public string Path;
            public bool Recurse;

            public bool IsValid
            {
                get { return !string.IsNullOrEmpty(Path); }
            }

        }

        internal struct Mask
        {
            public Transform Transform;
            public bool Recurse;
        }

        #endregion

    }
    
    [System.Serializable]
    public sealed class SPAnimMaskSerializedRef : Project.SerializableInterfaceRef<ISPAnimationMask>
    {
        
    }

}
