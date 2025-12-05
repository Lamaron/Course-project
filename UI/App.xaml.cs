using System.Configuration;
using System.Data;
using System.Windows;
using Data;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            using (var db = new MusicDbContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }

}
