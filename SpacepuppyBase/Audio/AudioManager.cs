using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Audio
{

    public static class AudioManager
    {

        #region Singleton Interface

        #endregion

        #region Fields

        private static GlobalAudioSourceGroup _globalAudioSources = new GlobalAudioSourceGroup();
        private static UnmanagedAudioSourceGroup _unmanagedAudioSources = new UnmanagedAudioSourceGroup();
        private static ManagedAudioGroupCollection _groups = new ManagedAudioGroupCollection();

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        /// <summary>
        /// A group that represents all audiosources globally. This can be used for controlling the global volume and pausing. 
        /// Those groups flagged to ignore the global pausing/volume will not be effected here. 
        /// Note, looping over this can be very expensive as it utilizes Object.FindObjectsOfType, do not do so unless necessary.
        /// </summary>
        public static IAudioGroup Global { get { return _globalAudioSources; } }

        /// <summary>
        /// A collection that represents all AudioSources that aren't otherwise part of a group. 
        /// Looping over the entries in this collection is expensive as it utilizes a Object.FindObjectsOfType call, do not do so unless necessary!
        /// </summary>
        public static IEnumerable<AudioSource> Unmanaged { get { return _unmanagedAudioSources; } }

        public static ManagedAudioGroupCollection Groups { get { return _groups; } }

        #endregion

        #region Methods

        public static bool IsManaged(AudioSource src)
        {
            if (src == null) return false;
            int cnt = _groups.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (_groups[i].Contains(src)) return true;
            }
            return false;
        }

        #endregion


        #region Special Types

        public class ManagedAudioGroupCollection : IEnumerable<AudioGroup>
        {

            #region Fields

            private List<AudioGroup> _lst = new List<AudioGroup>();

            #endregion

            #region CONSTRUCTOR

            internal ManagedAudioGroupCollection()
            {

            }

            #endregion

            #region Properties

            public int Count { get { return _groups.Count; } }

            public AudioGroup this[int index] { get { return _groups[index]; } }

            /// <summary>
            /// Returns the first AudioGroup whose name matches.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public AudioGroup this[string name]
            {
                get
                {
                    for(int i = 0; i < _groups.Count; i++)
                    {
                        if (_groups[i].name == name) return _groups[i];
                    }
                    return null;
                }
            }

            #endregion

            #region Methods

            public AudioGroup Create(string name)
            {
                var go = new GameObject(name);
                var grp = go.AddComponent<AudioGroup>();
                _lst.Add(grp);
                return grp;
            }

            public bool Contains(AudioGroup grp)
            {
                return _lst.Contains(grp);
            }

            internal void Register(AudioGroup grp)
            {
                if (_lst.Contains(grp)) return;
                _lst.Add(grp);
            }

            internal void Unregister(AudioGroup grp)
            {
                _lst.Remove(grp);
            }

            #endregion

            #region IEnumerable Interface

            public IEnumerator<AudioGroup> GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            #endregion

        }

        private class GlobalAudioSourceGroup : IAudioGroup
        {

            internal GlobalAudioSourceGroup()
            {

            }

            #region IAudioGroup INterface

            public float Volume
            {
                get
                {
                    return AudioListener.volume;
                }
                set
                {
                    AudioListener.volume = value;
                }
            }

            public bool Paused
            {
                get { return AudioListener.pause; }
            }

            public void Pause()
            {
                AudioListener.pause = true;
            }

            public void UnPause()
            {
                AudioListener.pause = false;
            }

            public void PlayAll()
            {
                foreach(var src in this)
                {
                    src.Play();
                }
            }
            
            public bool Contains(AudioSource src)
            {
                return true;
            }

            public IEnumerator<AudioSource> GetEnumerator()
            {
                return (Object.FindObjectsOfType<AudioSource>() as IEnumerable<AudioSource>).GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

        }

        /// <summary>
        /// Represents all the audiosources that aren't part of a group.
        /// </summary>
        private class UnmanagedAudioSourceGroup : IEnumerable<AudioSource>
        {

            internal UnmanagedAudioSourceGroup()
            {

            }

            #region IEnumerable Interface

            public IEnumerator<AudioSource> GetEnumerator()
            {
                //var managedSources = (from g in AudioManager._groups from s in g select s).ToArray();
                //var allSources = Object.FindObjectsOfType<AudioSource>();
                //for (int i = 0; i < allSources.Length; i++)
                //{
                //    if (System.Array.IndexOf(managedSources, allSources[i]) < 0) yield return allSources[i];
                //}

                var e = AudioManager._groups.GetEnumerator();
                while(e.MoveNext())
                {
                    var e2 = e.Current.GetEnumerator();
                    while(e2.MoveNext())
                    {
                        yield return e2.Current;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }

}
