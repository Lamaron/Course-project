using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace UI
{
    public partial class PlaylistEditWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();
        private Playlist _playlist;

        private List<AudioFile> _allTracks;
        private List<PlaylistTrack> _playlistTracks;

        private ObservableCollection<AudioFile> _available = new ObservableCollection<AudioFile>();

        public PlaylistEditWindow(Playlist playlist = null)
        {
            InitializeComponent();
            _playlist = playlist ?? new Playlist
            {
                PlaylistTracks = new List<PlaylistTrack>()
            };

            PlaylistNameTextBox.Text = _playlist.Name;

            foreach (var a in _db.AudioFiles.ToList()) _available.Add(a);
            AvailableTracksDataGrid.ItemsSource = _available;

            if (_playlist.Id != 0)
            {
                var tracks = _db.PlaylistTracks.Where(pt => pt.Playlist_Id == _playlist.Id).Include(pt => pt.AudioFile).ToList();
                foreach (var pt in tracks) _playlistTracks.Add(pt);
            }

            PlaylistTracksDataGrid.ItemsSource = _playlistTracks;
            LoadTracks();
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableTracksDataGrid.SelectedItem is AudioFile track)
            {
                if (_playlistTracks.Any(pt => pt.Audio_File_Id == track.Id))
                    return;

                var pt = new PlaylistTrack
                {
                    AudioFile = track,
                    Audio_File_Id = track.Id
                };

                _playlistTracks.Add(pt);
                PlaylistTracksDataGrid.Items.Refresh();
            }
        }

        private void RemoveTrack_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistTracksDataGrid.SelectedItem is PlaylistTrack pt)
            {
                _playlistTracks.Remove(pt);
                PlaylistTracksDataGrid.Items.Refresh();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _playlist.Name = PlaylistNameTextBox.Text;
            _playlist.PlaylistTracks = _playlistTracks;

            if (_playlist.Id == 0)
                _db.Playlists.Add(_playlist);

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

        private void LoadTracks()
        {
            _allTracks = _db.AudioFiles.ToList();

            _playlistTracks = _playlist.PlaylistTracks.ToList();

            AvailableTracksDataGrid.ItemsSource = _allTracks;
            PlaylistTracksDataGrid.ItemsSource = _playlistTracks;
        }
    }
}
