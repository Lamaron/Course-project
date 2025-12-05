using Domain;
using System;

namespace UI
{
    public static class PlayerController
    {
        public static Action<AudioFile> PlayExternal;
        public static Action Pause;
        public static Action<double> SetVolume;

        public static void PlayTrack(AudioFile file)
        {
            PlayExternal?.Invoke(file);
        }
    }
}