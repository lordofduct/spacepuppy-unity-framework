using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Audio
{

    [DisallowMultipleComponent()]
    public class AudioGroup : SPComponent, IAudioGroup, ICollection<AudioSource>
    {

        #region Multiton Interface

        private static AudioGroupPool _pool = new AudioGroupPool();
        public static AudioGroupPool Pool
        {
            get { return _pool; }
        }

        #endregion


        #region Fields

        [ReorderableArray()]
        [SerializeField()]
        private List<AudioSource> _managedAudioSources = new List<AudioSource>();

        [SerializeField()]
        private bool _ignoreGlobalPause = true;
        [SerializeField()]
        private bool _ignoreGlobalVolume = true;

        [Range(0, 1f)]
        [SerializeField()]
        private float _volume = 1.0f;
        [SerializeField()]
        private bool _paused;

        [System.NonSerialized()]
        private bool _globallyPaused;

        [System.NonSerialized()]
        private List<AudioSource> _pausedPool;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _pool.AddReference(this);
            _volume = Mathf.Clamp01(_volume);
        }

        protected override void Start()
        {
            base.Start();

            foreach (var src in this.transform.GetChildComponents<AudioSource>(true))
            {
                this.Add(src);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < _managedAudioSources.Count; i++)
            {
                if (_managedAudioSources[i] != null) Object.Destroy(_managedAudioSources[i]);
            }
            _managedAudioSources.Clear();
            _pool.RemoveReference(this);
        }

        #endregion

        #region Properties

        public bool IgnoreGlobalPause
        {
            get { return _ignoreGlobalPause; }
            set
            {
                if (_ignoreGlobalPause == value) return;
                _ignoreGlobalPause = value;
                for (int i = 0; i < _managedAudioSources.Count; i++)
                {
                    _managedAudioSources[i].ignoreListenerPause = _ignoreGlobalPause;
                }
            }
        }

        public bool IgnoreGlobalVolume
        {
            get { return _ignoreGlobalVolume; }
            set
            {
                if (_ignoreGlobalVolume == value) return;
                _ignoreGlobalVolume = value;
                for(int i = 0; i < _managedAudioSources.Count; i++)
                {
                    _managedAudioSources[i].ignoreListenerVolume = _ignoreGlobalVolume;
                }
            }
        }

        #endregion

        #region Methods

        internal void SetGloballyPaused(bool paused)
        {
            if (_globallyPaused == paused) return;
            _globallyPaused = paused;
            
        }

        public bool TryAdd(AudioSource item)
        {
            if (_pool.IsManaged(item))
            {
                //Debug.LogWarning("AudioSource is already managed by another group. An AudioSource can only be a member of one group at a time.", item);
                return false;
            }

            this.Add_Imp(item);
            return true;
        }

        private void Add_Imp(AudioSource item)
        {
            item.ignoreListenerPause = _ignoreGlobalPause;
            item.ignoreListenerVolume = _ignoreGlobalVolume;
            item.volume = _volume;
            _managedAudioSources.Add(item);

            if (_paused && item.isPlaying)
            {
                if (_pausedPool == null) _pausedPool = new List<AudioSource>();
                item.Pause();
                _pausedPool.Add(item);
            }
        }

        #endregion

        #region IAudioGroup Interface

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = Mathf.Clamp01(value);
                for (int i = 0; i < _managedAudioSources.Count; i++)
                {
                    _managedAudioSources[i].volume = _volume;
                }
            }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        public void Pause()
        {
            _paused = true;
            if (_pausedPool == null) _pausedPool = new List<AudioSource>();

            for (int i = 0; i < _managedAudioSources.Count; i++)
            {
                var src = _managedAudioSources[i];
                if (src.isPlaying)
                {
                    src.Pause();
                    _pausedPool.Add(src);
                }
            }
        }

        public void UnPause()
        {
            _paused = false;
            if (_pausedPool == null) return;

            for (int i = 0; i < _pausedPool.Count; i++)
            {
                _pausedPool[i].Play();
            }
            _pausedPool.Clear();
        }

        public void PlayAll()
        {
            for(int i = 0; i < _managedAudioSources.Count; i++)
            {
                _managedAudioSources[i].Play();
            }
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _managedAudioSources.Count; }
        }

        bool ICollection<AudioSource>.IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Add(AudioSource item)
        {
            if (_pool.IsManaged(item))
            {
                throw new System.ArgumentException("AudioSource is already managed by another group. An AudioSource can only be a member of one group at a time.", "item");
                //Debug.LogWarning("AudioSource is already managed by another group. An AudioSource can only be a member of one group at a time.", item);
                //return;
            }

            this.Add_Imp(item);
        }

        public void Clear()
        {
            for(int i = 0; i < _managedAudioSources.Count; i++)
            {
                _managedAudioSources[i].ignoreListenerPause = false;
                _managedAudioSources[i].ignoreListenerVolume = false;
            }
            _managedAudioSources.Clear();
        }

        public bool Contains(AudioSource item)
        {
            return _managedAudioSources.Contains(item);
        }

        public void CopyTo(AudioSource[] array, int arrayIndex)
        {
            _managedAudioSources.CopyTo(array, arrayIndex);
        }

        public bool Remove(AudioSource item)
        {
            if(_managedAudioSources.Remove(item))
            {
                if(item != null)
                {
                    //because the AudioSource may have been destroyed
                    item.ignoreListenerPause = false;
                    item.ignoreListenerVolume = false;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable Interface

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<AudioSource> GetEnumerator()
        {
            return _managedAudioSources.GetEnumerator();
        }

        #endregion

        #region Special Types

        public class AudioGroupPool : com.spacepuppy.Collections.MultitonPool<AudioGroup>
        {
            public bool IsManaged(AudioSource src)
            {
                if (src == null) return false;

                var e = this.GetEnumerator();
                while(e.MoveNext())
                {
                    if (e.Current.Contains(src)) return true;
                }
                return false;
            }
        }

        #endregion

    }

}
