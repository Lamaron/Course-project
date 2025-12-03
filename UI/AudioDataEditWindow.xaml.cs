using System.Windows;
using Data;
using System.Linq;

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для AudioDataEditWindow.xaml
    /// </summary>
    public partial class AudioDataEditWindow : Window
    {
        private int _audioId;
        private MusicDbContext _db = new MusicDbContext();

        public AudioDataEditWindow(int audioId)
        {
            InitializeComponent();
            _audioId = audioId;
            var a = _db.AudioFiles.Find(audioId);
            if (a != null)
            {
                TitleTextBox.Text = a.Name;
                ArtistTextBox.Text = a.Author;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var a = _db.AudioFiles.Find(_audioId);
            if (a != null)
            {
                a.Name = TitleTextBox.Text;
                a.Author = ArtistTextBox.Text;
                _db.SaveChanges();
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
