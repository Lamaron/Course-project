using System;
using System.Collections.Generic;
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
        public string Url { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; } 

        public ICollection<PlaylistTrack> PlaylistTracks { get; set; } 
    }
}
