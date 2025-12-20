using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace UI
{

    public partial class MainWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();
        private AudioService _audioService;

        private List<AudioFile> _tracks;
        private int _currentTrackIndex = -1;

        private TimeSpan _totalDuration = TimeSpan.Zero;
        private bool _isDragging;


        public MainWindow()
        {
            InitializeComponent();
            InitTimer();
            PlayerController.GetPosition = () => Player.Position;

            PlayerController.GetDuration = () =>
                Player.NaturalDuration.HasTimeSpan
                    ? Player.NaturalDuration.TimeSpan
                    : (TimeSpan?)null;

            PlayerController.Seek = (ts) => Player.Position = ts;

            _db = new MusicDbContext();
            _audioService = new AudioService(Player);

            PlayerController.PlayExternal = PlayTrack;
            PlayerController.Pause = () => _audioService.Pause();
            PlayerController.SetVolume = (v) => Player.Volume = v;
            PlayerController.Next = () => NextButton_Click(null, null);
            LoadTracks();


        }

        private void LoadTracks()
        {
            _tracks = _db.AudioFiles.ToList();
            MainDataGrid.ItemsSource = _tracks;
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string q = SearchTextBox.Text.ToLower();

            var filtered = _tracks.Where(t =>
                t.Name.ToLower().Contains(q) ||
                t.Author.ToLower().Contains(q)).ToList();

            MainDataGrid.ItemsSource = filtered;
        }


        private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            PlaylistEditWindow w = new PlaylistEditWindow();
            if (w.ShowDialog() == true)
                LoadTracks();
        }

        private void OpenLibrary_Click(object sender, RoutedEventArgs e)
        {
            AudioLibraryWindow w = new AudioLibraryWindow();
            w.ShowDialog();
            LoadTracks();
        }

        private void OpenPayment_Click(object sender, RoutedEventArgs e)
        {
            PaymentWindow w = new PaymentWindow();
            w.ShowDialog();
        }


        private void MainDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MainDataGrid.SelectedItem is AudioFile track)
            {
                _currentTrackIndex = _tracks.IndexOf(track);
                PlayTrack(track);
            }
        }

        private void PlayTrack(AudioFile track)
        {

            NowPlayingText.Text = $"{track.Name} — {track.Author}";
            _audioService.Play(track.URL);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            _audioService.TogglePlayPause();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerController.PlayExternal = PlayTrack;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_tracks.Count == 0) return;

            _currentTrackIndex++;
            if (_currentTrackIndex >= _tracks.Count)
                _currentTrackIndex = 0;

            PlayTrack(_tracks[_currentTrackIndex]);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_tracks.Count == 0) return;

            _currentTrackIndex--;
            if (_currentTrackIndex < 0)
                _currentTrackIndex = _tracks.Count - 1;

            PlayTrack(_tracks[_currentTrackIndex]);
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Player != null)
                Player.Volume = e.NewValue;
        }

        private void OpenPlaylistManager_Click(object sender, RoutedEventArgs e)
        {
            var w = new PlaylistListWindow();
            w.ShowDialog();
        }


        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
                TimelineSlider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
        }

        DispatcherTimer _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        private void InitTimer()
        {
            _timer.Tick += (s, e) =>
            {
                var pos = PlayerController.GetPosition?.Invoke();
                var dur = PlayerController.GetDuration?.Invoke();

                if (pos == null || dur == null)
                    return;

                if (!_isDragging)
                    TimelineSlider.Value = pos.Value.TotalSeconds;

                TimelineSlider.Maximum = dur.Value.TotalSeconds;

                CurrentTimeText.Text = pos.Value.ToString(@"mm\:ss");
                TotalTimeText.Text = dur.Value.ToString(@"mm\:ss");
            };

            _timer.Start();
        }



        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDragging)
                PlayerController.Seek?.Invoke(
                    TimeSpan.FromSeconds(TimelineSlider.Value));
        }

        private void Timeline_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
        }

        private void Timeline_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            PlayerController.Seek?.Invoke(
                TimeSpan.FromSeconds(TimelineSlider.Value));
        }




    }
}
