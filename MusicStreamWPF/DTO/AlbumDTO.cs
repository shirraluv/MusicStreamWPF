using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamWPF.DTO
{
    public class AlbumDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Creatorid { get; set; }

        public string Duration { get; set; }

        public string Imagesource { get; set; }

        public DateTime Date { get; set; }
    }
}
