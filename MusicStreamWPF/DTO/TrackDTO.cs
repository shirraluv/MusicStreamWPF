using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamWPF.DTO
{
    public class TrackDTO
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Duration { get; set; }

        public string Filename { get; set; }

        public int Auditions { get; set; }

        public int Albumid { get; set; }

        public string Imagesource { get; set; }

        public string Name { get; set; }
    }
}
