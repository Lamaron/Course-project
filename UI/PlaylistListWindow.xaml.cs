using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace UI
{
    public partial class PlaylistListWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();
        private AudioService _audioService;

        public PlaylistListWindow(AudioService audioService)
        {
            InitializeComponent();
            _audioService = audioService;
            LoadPlaylists();
        }

        private void LoadPlaylists()
        {
            var playlists = _db.Playlists
                .Include(p => p.PlaylistTracks)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    TrackCount = p.PlaylistTracks.Count
                })
                .ToList();

            PlaylistsGrid.ItemsSource = playlists;
        }

        private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            var win = new PlaylistEditWindow(null);
            win.ShowDialog();
            LoadPlaylists();
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistsGrid.SelectedItem == null)
                return;

            dynamic selected = PlaylistsGrid.SelectedItem;
            int id = selected.Id;

            if (MessageBox.Show("Удалить плейлист?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var pl = _db.Playlists.Find(id);
                _db.Playlists.Remove(pl);
                _db.SaveChanges();
                LoadPlaylists();
            }
        }

        private void PlaylistsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PlaylistsGrid.SelectedItem == null) return;

            dynamic pl = PlaylistsGrid.SelectedItem;
            int id = pl.Id;

            var view = new PlaylistViewWindow(id, _audioService);
            view.ShowDialog();
        }
    }
}