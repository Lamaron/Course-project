using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Data;
using Domain;
using System.Collections.ObjectModel;
using System.IO;
using TagLib;

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

        private AudioFile CreateAudioFromFile(string filePath)
        {
            var tagFile = TagLib.File.Create(filePath);

            string title = string.IsNullOrWhiteSpace(tagFile.Tag.Title)
                ? Path.GetFileNameWithoutExtension(filePath)
                : tagFile.Tag.Title;

            string author = tagFile.Tag.FirstPerformer ?? "Unknown";

            TimeSpan dur = tagFile.Properties.Duration;

            byte[] cover = null;

            if (tagFile.Tag.Pictures != null && tagFile.Tag.Pictures.Length > 0)
            {
                cover = tagFile.Tag.Pictures[0].Data.Data;
            }

            return new AudioFile
            {
                Name = title,
                Author = author,
                URL = new Uri(filePath).AbsoluteUri,
                duration = dur,


                CoverImage = cover ?? new byte[0]
            };
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
                    var af = CreateAudioFromFile(f);
                    _db.AudioFiles.Add(af);
                }

                _db.SaveChanges();
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
