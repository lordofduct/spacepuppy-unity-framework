using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Audio
{

    public interface IAudioManager : IService
    {

        AudioSource BackgroundAmbientAudioSource { get; }

    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : ServiceComponent<IAudioManager>, IAudioManager
    {

        #region Fields

        [System.NonSerialized]
        private AudioSource _backgroundAmbientAudioSource;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            base.OnValidAwake();

            _backgroundAmbientAudioSource = this.AddOrGetComponent<AudioSource>();
        }

        #endregion

        #region IAudioManager Interface

        public AudioSource BackgroundAmbientAudioSource
        {
            get { return _backgroundAmbientAudioSource; }
        }

        #endregion

    }




    [System.Obsolete]
    internal static class AudioManager_Old
    {

        #region Singleton Interface

        #endregion

        #region Fields

        private static GlobalAudioSourceGroup _globalAudioSources = new GlobalAudioSourceGroup();
        private static UnmanagedAudioSourceGroup _unmanagedAudioSources = new UnmanagedAudioSourceGroup();

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
        
        #endregion
        
        #region Special Types
        
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
                var e = AudioGroup.Pool.GetEnumerator();
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
