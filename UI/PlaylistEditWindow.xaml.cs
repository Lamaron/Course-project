using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для PlaylistEditWindow.xaml
    /// </summary>
    public partial class PlaylistEditWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();
        private Playlist _playlist;

        private ObservableCollection<AudioFile> _available = new ObservableCollection<AudioFile>();
        private ObservableCollection<PlaylistTrack> _playlistTracks = new ObservableCollection<PlaylistTrack>();

        public PlaylistEditWindow(Playlist playlist = null)
        {
            InitializeComponent();
            _playlist = playlist ?? new Playlist();

            PlaylistNameTextBox.Text = _playlist.Name;

            foreach (var a in _db.AudioFiles.ToList()) _available.Add(a);
            AvailableTracksDataGrid.ItemsSource = _available;

            if (_playlist.Id != 0)
            {
                var tracks = _db.PlaylistTracks.Where(pt => pt.Playlist_Id == _playlist.Id).Include(pt => pt.AudioFile).ToList();
                foreach (var pt in tracks) _playlistTracks.Add(pt);
            }

            PlaylistTracksDataGrid.ItemsSource = _playlistTracks;
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableTracksDataGrid.SelectedItem is AudioFile a)
            {
                var pt = new PlaylistTrack { AudioFile = a, Audio_File_Id = a.Id };
                _playlistTracks.Add(pt);
            }
        }

        private void RemoveTrack_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistTracksDataGrid.SelectedItem is PlaylistTrack pt)
            {
                _playlistTracks.Remove(pt);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _playlist.Name = PlaylistNameTextBox.Text;

            if (_playlist.Id == 0)
            {
                _db.Playlists.Add(_playlist);
                _db.SaveChanges();
            }

            // Очистим старые записи
            var old = _db.PlaylistTracks.Where(p => p.Playlist_Id == _playlist.Id).ToList();
            if (old.Any())
            {
                _db.PlaylistTracks.RemoveRange(old);
                _db.SaveChanges();
            }

            foreach (var pt in _playlistTracks)
            {
                pt.Playlist_Id = _playlist.Id;
                pt.Audio_File_Id = pt.AudioFile.Id;
                _db.PlaylistTracks.Add(pt);
            }
            _db.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AvailableTracksDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
