using Domain;
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
        private bool _isPaused = false;
        private string _currentUrl = null;

        public AudioService(MediaElement player)
        {
            _player = player;
        }

        public void Play(string url)
        {
            if (_isPaused)
            {
                _player.Play();
                _isPaused = false;
                return;
            }

            _player.Source = new Uri(url);
            _player.Play();
            _isPaused = false;
        }

        public void TogglePlayPause()
        {
            if (_player.Source == null && _currentUrl != null)
            {
                Play(_currentUrl);
                return;
            }

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


        public void Stop()
        {
            _player.Stop();
            _isPaused = false;
        }

        public void Pause()
        {
            _player.Pause();
            _isPaused = true;
        }
    }
}
