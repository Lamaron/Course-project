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

        /// <summary>
        /// Воспроизводит файл по прямому URL или локальному пути
        /// </summary>
        public void Play(string url)
        {
            try
            {
                _currentUrl = url;

                _player.Source = new Uri(url, UriKind.Absolute);
                _player.LoadedBehavior = MediaState.Manual;
                _player.UnloadedBehavior = MediaState.Manual;

                _player.Play();
                _isPaused = false;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка воспроизведения: " + ex.Message);
            }
        }

        /// <summary>
        /// Переключатель Play/Pause
        /// </summary>
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

        /// <summary>
        /// Остановить воспроизведение
        /// </summary>
        public void Stop()
        {
            _player.Stop();
            _isPaused = false;
        }
    }
}
