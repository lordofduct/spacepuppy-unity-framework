#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{


    [System.Serializable()]
    public class SPAnimClipCollection : ICollection<SPAnimClip>, System.IDisposable, ISerializationCallbackReceiver
    {

        #region Fields

        [System.NonSerialized()]
        private SPAnimationController _controller;
        [System.NonSerialized()]
        private Dictionary<string, SPAnimClip> _dict = new Dictionary<string, SPAnimClip>();
        [System.NonSerialized()]
        private string _uniqueHash;

        [SerializeField()]
        private SPAnimClip[] _serializedStates;

        #endregion

        #region CONSTRUCTOR

        protected internal SPAnimClipCollection()
        {
        }

        #endregion

        #region Properties

        public bool Initialized { get { return !object.ReferenceEquals(_controller, null); } }

        public SPAnimationController Controller { get { return _controller; } }

        public string UniqueHash { get { return _uniqueHash; } }

        public SPAnimClip this[string name]
        {
            get
            {
                SPAnimClip result = null;
                _dict.TryGetValue(name, out result);
                return result;
            }
        }

        public ICollection<string> Keys { get { return _dict.Keys; } }

        #endregion

        #region Methods

        /// <summary>
        /// Associate the collection as a master collection for an SPAnimationController
        /// </summary>
        /// <param name="controller"></param>
        internal void InitMasterCollection(SPAnimationController controller)
        {
            if (controller == null) throw new System.ArgumentNullException("container");
            if (this.Initialized) throw new System.InvalidOperationException("SPAnimClipCollection already has been initilized.");
            _controller = controller;
            _uniqueHash = null;
        }

        /// <summary>
        /// Call only once to add all animations from a master collection with the SPAnimationController.
        /// </summary>
        internal void SyncMasterAnims()
        {
            if (!this.Initialized) throw new System.InvalidOperationException("SPAnimationClipCollection must be initialized as a master collection first.");
            if(_controller.States != this) throw new System.InvalidOperationException("Attempted to sync non-master SPAnimClipCollection as if it were a master collection.");

            //var e = _dict.GetEnumerator();
            //while (e.MoveNext())
            //{
            //    this.AddToMasterList(e.Current.Value.Name, e.Current.Value, true);
            //}

            //we loop the serialized states in-case anims were added in between InitMasterCollection and SyncMasterAnims
            foreach (var st in _serializedStates)
            {
                this.AddToMasterList(st.Name, st, true);
            }
        }

        /// <summary>
        /// Initialize a SPAnimClipCollection with a SPAnimationController.
        /// 
        /// (internal: this is only used for non-master collections)
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="uniqueHash"></param>
        public void Init(SPAnimationController controller, string uniqueHash = null)
        {
            if (controller == null) throw new System.ArgumentNullException("container");
            if (this.Initialized) throw new System.InvalidOperationException("SPAnimClipCollection already has been initilized.");
            _controller = controller;
            _uniqueHash = uniqueHash;
            
            var e = _dict.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.Init(_controller, _uniqueHash);
            }
        }

        internal void AddToMasterList(string id, SPAnimClip clip)
        {
            if (!this.Initialized) throw new System.InvalidOperationException("Unauthorized attempt to initialize the SPAnimationController SPAnimClip master list, no SPAnimationController found.");
            if (clip.Controller != _controller) throw new System.InvalidOperationException("Unauthorized attempt to initialize the SPAnimationController SPAnimClip master list, clip's controller is not the same as master controller.");
            if (_dict.ContainsKey(id)) throw new DuplicateKeyException();

            this.AddToMasterList(id, clip, false);
        }

        /// <summary>
        /// This is only ever called on the spanim clip master list when SPAnimClips are initialized...
        /// </summary>
        /// <param name="clip"></param>
        private void AddToMasterList(string id, SPAnimClip clip, bool isSelfInitializing)
        {
            //***SEE NOTES IN SPAnimClip DESCRIPTION
            //if (_firstFrame > 0 || _lastFrame >= 0)
            //{
            //    int total = Mathf.RoundToInt(_clip.length * _clip.frameRate);
            //    int s = (_firstFrame < 0) ? 0 : _firstFrame;
            //    int l = (_lastFrame < 0) ? total : _lastFrame;
            //    if (l <= s) l = s + 1;
            //    _controller.animation.AddClip(_clip, _name, s, l, _wrapMode == UnityEngine.WrapMode.Loop);
            //}
            //else
            //{
            //    _controller.animation.AddClip(_clip, _name);
            //}

            var ac = clip.AnimationClip;
            AnimationState st = null;
            if(ac != null)
            {
                _controller.animation.AddClip(ac, id);
                st = _controller.animation[id];
                if (st == null)
                {
                    Debug.LogWarning("An added animation failed to be generated.");
                }
            }
            clip.SetAnimState(_controller, id, st);

            if (!isSelfInitializing)
            {
                _dict[id] = clip;
            }
        }




        public ISPAnim Play(string clipId, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_controller == null) throw new AnimationInvalidAccessException();
            var state = this[clipId];
            //if (state == null) throw new UnknownStateException(clipId);
            if (state == null) return null;

            return state.Play(queueMode, playMode);
        }

        public ISPAnim CrossFade(string clipId, float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            if (_controller == null) throw new AnimationInvalidAccessException();
            var state = this[clipId];
            //if (state == null) throw new UnknownStateException(clipId);
            if (state == null) return null;

            return state.CrossFade(fadeLength, queueMode, playMode);
        }


        public SPAnimClip Add(string name, AnimationClip clip)
        {
            if (name == null) throw new System.ArgumentNullException("name");
            if (clip == null) throw new System.ArgumentNullException("clip");

            if (_dict.ContainsKey(name))
            {
                throw new DuplicateKeyException();
            }

            var spclip = new SPAnimClip(name, clip);
            if (this.Initialized)
            {
                spclip.Init(_controller, _uniqueHash);
                if (_controller.States != this) _dict[spclip.Name] = spclip; //if not master, then add by name
            }
            else
            {
                _dict[name] = spclip;
            }
            return spclip;
        }

        public SPAnimClip[] ToArray()
        {
            return _dict.Values.ToArray();
        }

        public bool ContainsKey(string name)
        {
            if (name == null) return false;
            return _dict.ContainsKey(name);
        }

        #endregion

        #region ICollection

        public void Add(SPAnimClip clip)
        {
            if (_dict.ContainsKey(clip.ClipID))
            {
                throw new DuplicateKeyException();
            }

            if (this.Initialized)
            {
                if (!clip.Initialized)
                    clip.Init(_controller, _uniqueHash);
                else if (clip.Controller != _controller)
                    throw new System.ArgumentException("The added clip is already registered with another SPAnimationController.");

                if (_controller.States != this) _dict[clip.Name] = clip; //if not master, then add by name
            }
            else
            {
                _dict[clip.Name] = clip;
            }
        }

        public void Clear()
        {
            foreach (var a in _dict.Values)
            {
                a.Dispose();
            }
            _dict.Clear();
        }

        public bool Contains(SPAnimClip item)
        {
            return _dict.Values.Contains(item);
        }

        public void CopyTo(SPAnimClip[] array, int arrayIndex)
        {
            _dict.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(SPAnimClip item)
        {
            if (item == null) return false;
            if (_dict.Remove(item.Name))
            {
                item.Dispose();
                return true;
            }
            else
                return false;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<SPAnimClip> IEnumerable<SPAnimClip>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            if (object.ReferenceEquals(_controller, null)) return;
            
            var e = _dict.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Value.Dispose();
            }
            _controller = null;
        }

        #endregion

        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _dict.Clear();
            for (int i = 0; i < _serializedStates.Length; i++)
            {
                _dict.Add(_serializedStates[i].Name, _serializedStates[i]);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //do nothing
        }

        #endregion



        #region SpecialTypes

        public struct Enumerator : IEnumerator<SPAnimClip>
        {

            private SPAnimClipCollection _coll;
            private Dictionary<string, SPAnimClip>.Enumerator _e;

            public Enumerator(SPAnimClipCollection coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _coll = coll;
                _e = _coll._dict.GetEnumerator();
            }

            public SPAnimClip Current
            {
                get { return _e.Current.Value; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _e.Current.Value; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as System.Collections.IEnumerator).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }
        }

        public class ConfigAttribute : System.Attribute
        {
            public bool HideDetailRegion;
            public int DefaultLayer;
            public string Prefix;
            public string Hash;
        }

        public class StaticCollectionAttribute : System.Attribute
        {

            public string[] Names;

            public StaticCollectionAttribute(params string[] names)
            {
                this.Names = names;
            }

        }

        #endregion

    }


}
