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

            _db = new MusicDbContext();
            _audioService = new AudioService(Player);

            _audioService.TrackChanged += track =>
                NowPlayingText.Text = $"{track.Name} — {track.Author}";

            InitTimer();
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


        private void MainDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MainDataGrid.SelectedItem is AudioFile track)
            {
                _audioService.SetPlaylist(_tracks, _tracks.IndexOf(track));
            }
        }


        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            _audioService.TogglePlayPause();
        }


        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _audioService.Next();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            _audioService.Previous();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_audioService == null)
                return;

            _audioService.SetVolume(e.NewValue);
        }

        private void OpenPlaylistManager_Click(object sender, RoutedEventArgs e)
        {
            var w = new PlaylistListWindow(_audioService);
            w.ShowDialog();
        }


        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
                TimelineSlider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
        }


        private void InitTimer()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += (s, e) =>
            {
                if (!_isDragging)
                    TimelineSlider.Value = _audioService.Position.TotalSeconds;

                TimelineSlider.Maximum = _audioService.Duration.TotalSeconds;

                CurrentTimeText.Text = _audioService.Position.ToString(@"mm\:ss");
                TotalTimeText.Text = _audioService.Duration.ToString(@"mm\:ss");
            };

            timer.Start();
        }



        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDragging || _audioService == null)
                return;

            _audioService.Seek(TimeSpan.FromSeconds(e.NewValue));
        }

        private void Timeline_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
        }

        private void Timeline_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _audioService.Seek(TimeSpan.FromSeconds(TimelineSlider.Value));
        }




    }
}
