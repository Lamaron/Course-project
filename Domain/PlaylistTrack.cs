using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class PlaylistTrack
    {
        public int Id { get; set; }


        public int Playlist_Id { get; set; }
        public Playlist Playlist { get; set; }


        public int Audio_File_Id { get; set; }
        public AudioFile AudioFile { get; set; }
    }
}
