using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace UI
{
    public partial class PlaylistViewWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();
        private Playlist _playlist;
        private AudioService _audioService;
        private bool _isDragging;



        public PlaylistViewWindow(int playlistId, AudioService audioService)
        {
            InitializeComponent();


            _playlist = _db.Playlists
                .Include(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.AudioFile)
                .FirstOrDefault(p => p.Id == playlistId);

            if (_playlist == null)
            {
                MessageBox.Show("Плейлист не найден.");
                Close();
                return;
            }
            _audioService = audioService;

            PlaylistNameText.Text = _playlist.Name;
            TracksGrid.ItemsSource = _playlist.PlaylistTracks;

            InitTimer();
        }

        private void TracksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TracksGrid.SelectedItem is PlaylistTrack pt)
            {
                var tracks = _playlist.PlaylistTracks
                    .Select(t => t.AudioFile)
                    .ToList();

                _audioService.SetPlaylist(tracks, tracks.IndexOf(pt.AudioFile));
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


        private void Play_Click(object sender, RoutedEventArgs e)
        {
            _audioService.TogglePlayPause();
        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _audioService.Next();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            _audioService.Previous();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_audioService == null)
                return;

            _audioService.SetVolume(e.NewValue);
        }



        private void InitTimer()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += (s, e) =>
            {
                if (_audioService == null) return;

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
            if (_audioService == null || _isDragging)
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
