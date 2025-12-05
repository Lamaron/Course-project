using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class MainWindow : Window
    {
        private MusicDbContext _db;
        private AudioService _audioService;

        private List<AudioFile> _tracks;
        private int _currentTrackIndex = -1;

        public MainWindow()
        {
            InitializeComponent();

            _db = new MusicDbContext();
            _audioService = new AudioService(Player);

            PlayerController.PlayExternal = PlayTrack;
            PlayerController.Pause = () => _audioService.Pause();
            PlayerController.SetVolume = (v) => Player.Volume = v;

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
            PlayerController.SetVolume = (v) => Player.Volume = v;
        }

        private void OpenPlaylistManager_Click(object sender, RoutedEventArgs e)
        {
            var w = new PlaylistListWindow();
            w.ShowDialog();
        }



    }
}
