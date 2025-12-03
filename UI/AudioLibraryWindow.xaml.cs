using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Data;
using Domain;
using System.Collections.ObjectModel;
using System.IO;

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для AudioLibraryWindow.xaml
    /// </summary>
    public partial class AudioLibraryWindow : Window
    {
        private MusicDbContext _db = new MusicDbContext();
        private ObservableCollection<AudioFile> _files = new ObservableCollection<AudioFile>();

        public AudioLibraryWindow()
        {
            InitializeComponent();
            LoadFiles();
            AudioFilesDataGrid.ItemsSource = _files;
        }

        private void LoadFiles()
        {
            _files.Clear();
            foreach (var f in _db.AudioFiles.ToList()) _files.Add(f);
        }

        private void ImportFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Выберите папку",
                Filter = "Folders|\n",
                Title = "Выбор папки"
            };

            if (dlg.ShowDialog() == true)
            {
                string folder = System.IO.Path.GetDirectoryName(dlg.FileName);

                var supported = new[] { ".mp3", ".wav", ".wma", ".m4a" };
                var files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => supported.Contains(Path.GetExtension(f).ToLower()))
                    .ToList();

                foreach (var f in files)
                {
                    var af = new AudioFile
                    {
                        Name = Path.GetFileNameWithoutExtension(f),
                        Author = "Unknown",
                        URL = new Uri(f).AbsoluteUri
                    };

                    _db.AudioFiles.Add(af);
                }

                _db.SaveChanges();
                LoadFiles();
            }
        }

        private void EditTags_Click(object sender, RoutedEventArgs e)
        {
            if (AudioFilesDataGrid.SelectedItem is AudioFile af)
            {
                var w = new AudioDataEditWindow(af.Id);
                w.ShowDialog();
                LoadFiles();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AudioFilesDataGrid.SelectedItem is AudioFile af)
            {
                if (MessageBox.Show("Удалить выбранный трек?", "Подтвердите", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _db.AudioFiles.Remove(_db.AudioFiles.Find(af.Id));
                    _db.SaveChanges();
                    LoadFiles();
                }
            }
        }
    }
}
