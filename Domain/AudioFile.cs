using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AudioFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string URL { get; set; }
        public int? Album_Id { get; set; }


        [ForeignKey("Album_Id")]
        public Album Album { get; set; }


        public ICollection<PlaylistTrack> PlaylistTracks { get; set; }
    }
}
