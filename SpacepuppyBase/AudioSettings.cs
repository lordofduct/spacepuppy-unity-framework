using UnityEngine;

namespace com.spacepuppy
{

    [System.Flags]
    public enum AudioSettingsMask
    {
        Clip = 1,
        Volume = 2,
        Loop = 4
    }

    [System.Serializable]
    public struct AudioSettings
    {

        #region Fields

        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        public bool loop;

        #endregion

        #region Methods
        
        public void Apply(AudioSource src)
        {
            src.clip = this.clip;
            src.volume = this.volume;
            src.loop = this.loop;
        }

        public void Apply(AudioSource src, AudioSettingsMask mask)
        {
            if ((mask & AudioSettingsMask.Clip) != 0) src.clip = this.clip;
            if ((mask & AudioSettingsMask.Volume) != 0) src.volume = this.volume;
            if ((mask & AudioSettingsMask.Loop) != 0) src.loop = this.loop;
        }

        #endregion

        #region Static Interface

        public static readonly AudioSettings Default = new AudioSettings()
        {
            clip = null,
            volume = 1f,
            loop = false
        };

        public static AudioSettings From(AudioSource src)
        {
            return new AudioSettings()
            {
                clip = src.clip,
                volume = src.volume,
                loop = src.loop
            };
        }

        #endregion


    }

}
