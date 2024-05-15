using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamWPF.DTO
{
    public class PlaylistDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Imagesource { get; set; }

        public string Duration { get; set; }

        public int Userid { get; set; }

        public DateTime Date { get; set; }

        public int Trackid { get; set; }
    }
}
