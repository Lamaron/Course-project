using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace UI
{
    public partial class PlaylistViewWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();
        private Playlist _playlist;
        private int _currentIndex = -1;

        public PlaylistViewWindow(int playlistId)
        {
            InitializeComponent();

            _playlist = _db.Playlists
                .Include(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.AudioFile)
                .ThenInclude(a => a.Album)
                .FirstOrDefault(p => p.Id == playlistId);

            if (_playlist == null)
            {
                MessageBox.Show("Плейлист не найден.");
                Close();
                return;
            }

            PlaylistNameText.Text = _playlist.Name;

            TracksGrid.ItemsSource = _playlist.PlaylistTracks;
        }

        private void TracksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TracksGrid.SelectedItem is PlaylistTrack pt)
            {
                _currentIndex = TracksGrid.Items.IndexOf(pt);
                PlayerController.PlayTrack(pt.AudioFile);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new PlaylistEditWindow(_playlist);
            editWindow.ShowDialog();

            _db = new MusicDbContext();

            _playlist = _db.Playlists
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.AudioFile)
                .FirstOrDefault(p => p.Id == _playlist.Id);

            TracksGrid.ItemsSource = _playlist.PlaylistTracks;
            PlaylistNameText.Text = _playlist.Name;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PlayCurrent()
        {
            if (_currentIndex < 0 || _currentIndex >= _playlist.PlaylistTracks.Count)
                return;

            var track = _playlist.PlaylistTracks.ElementAt(_currentIndex).AudioFile;

            PlayerController.PlayTrack(track);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex == -1 && _playlist.PlaylistTracks.Count > 0)
                _currentIndex = 0;

            var track = _playlist.PlaylistTracks.ElementAt(_currentIndex).AudioFile;
            PlayerController.PlayTrack(track);
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            PlayerController.Pause?.Invoke();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (TracksGrid.Items.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= _playlist.PlaylistTracks.Count)
                _currentIndex = 0;

            PlayCurrent();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (TracksGrid.Items.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _playlist.PlaylistTracks.Count - 1;

            PlayCurrent();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayerController.SetVolume?.Invoke(VolumeSlider.Value);
        }

    }
}
