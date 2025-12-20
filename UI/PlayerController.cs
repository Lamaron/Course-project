using Domain;
using System;

namespace UI
{
    public static class PlayerController
    {
        public static Action<AudioFile> PlayExternal;
        public static Action Pause;
        public static Action<double> SetVolume;
        public static Func<TimeSpan> GetPosition;
        public static Func<TimeSpan?> GetDuration;
        public static Action<TimeSpan> Seek;
        public static Action Next;

        public static void PlayTrack(AudioFile file)
        {
            PlayExternal?.Invoke(file);
        }
    }
}