using UnityEngine;

namespace com.spacepuppy.Utils
{
    public static class AudioUtils
    {

        public static void Play(this AudioSource src, AudioClip clip, AudioInterruptMode mode)
        {
            if (src == null) throw new System.ArgumentNullException("src");
            if (clip == null) throw new System.ArgumentNullException("clip");
            
            switch(mode)
            {
                case AudioInterruptMode.StopIfPlaying:
                    if (src.isPlaying) src.Stop();
                    break;
                case AudioInterruptMode.DoNotPlayIfPlaying:
                    if (src.isPlaying) return;
                    break;
                case AudioInterruptMode.PlayOverExisting:
                    break;
            }

            src.PlayOneShot(clip);
        }

        public static void Play(this AudioSource src, AudioClip clip, float volumeScale, AudioInterruptMode mode)
        {
            if (src == null) throw new System.ArgumentNullException("src");
            if (clip == null) throw new System.ArgumentNullException("clip");

            switch (mode)
            {
                case AudioInterruptMode.StopIfPlaying:
                    if (src.isPlaying) src.Stop();
                    break;
                case AudioInterruptMode.DoNotPlayIfPlaying:
                    if (src.isPlaying) return;
                    break;
                case AudioInterruptMode.PlayOverExisting:
                    break;
            }

            src.PlayOneShot(clip, volumeScale);
        }

    }
}
