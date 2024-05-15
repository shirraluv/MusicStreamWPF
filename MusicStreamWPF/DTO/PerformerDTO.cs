using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamWPF.DTO
{
    public class PerformerDTO
    {
        public int Id { get; set; }

        public string Nick { get; set; }

        public int Auditions { get; set; }

        public DateTime Regdate { get; set; }

        public string Password { get; set; }
        
        public string Imagesource { get; set; }
    }
}
