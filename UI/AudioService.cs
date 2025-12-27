using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UI
{
    public class AudioService
    {
        private readonly MediaElement _player;

        private List<AudioFile> _playlist = new();
        private int _currentIndex = -1;

        private bool _isPaused;
        private double _volume = 0.5;

        public event Action<AudioFile> TrackChanged;

        public AudioService(MediaElement player)
        {
            _player = player;
            _player.LoadedBehavior = MediaState.Manual;
            _player.UnloadedBehavior = MediaState.Stop;

            _player.MediaEnded += Player_MediaEnded;
        }


        public TimeSpan Position => _player.Position;

        public TimeSpan Duration =>
            _player.NaturalDuration.HasTimeSpan
                ? _player.NaturalDuration.TimeSpan
                : TimeSpan.Zero;


        public void SetPlaylist(List<AudioFile> tracks, int startIndex)
        {
            if (tracks == null || tracks.Count == 0)
                return;

            _playlist = tracks;
            _currentIndex = Math.Clamp(startIndex, 0, tracks.Count - 1);

            PlayCurrent();
        }


        public void PlayCurrent()
        {
            if (_currentIndex < 0 || _currentIndex >= _playlist.Count)
                return;

            var track = _playlist[_currentIndex];

            _player.Source = new Uri(track.URL);
            _player.Volume = _volume;
            _player.Play();

            _isPaused = false;
            TrackChanged?.Invoke(track);
        }

        public void TogglePlayPause()
        {
            if (_player.Source == null)
                return;

            if (_isPaused)
            {
                _player.Play();
                _isPaused = false;
            }
            else
            {
                _player.Pause();
                _isPaused = true;
            }
        }


        public void Next()
        {
            if (_playlist.Count == 0)
                return;

            _currentIndex++;
            if (_currentIndex >= _playlist.Count)
                _currentIndex = 0;

            PlayCurrent();
        }

        public void Previous()
        {
            if (_playlist.Count == 0)
                return;

            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _playlist.Count - 1;

            PlayCurrent();
        }


        public void Seek(TimeSpan position)
        {
            if (_player.Source == null)
                return;

            _player.Position = position;
        }


        public void SetVolume(double volume)
        {
            _volume = Math.Clamp(volume, 0, 1);
            _player.Volume = _volume;
        }


        private void Player_MediaEnded(object sender, EventArgs e)
        {
            Next();
        }
    }
}
