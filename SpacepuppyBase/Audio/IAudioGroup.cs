using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Audio
{

    public interface IAudioGroup : IEnumerable<AudioSource>
    {

        float Volume { get; set; }
        bool Paused { get; }

        void Pause();
        void UnPause();
        void PlayAll();
        bool Contains(AudioSource src);

    }

}
