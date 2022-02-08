using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class Picture
    {
        public Picture()
        {
            Created = DateTime.Now;
            Ord = 0;
            GPS = String.Empty;
            Camera = String.Empty;
            Description = String.Empty;
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public byte[] Image { get; set; }

        public string Album { get; set; }

        public int Ord { get; set; }

        public DateTime Upload { get; set; }

        public DateTime Created { get; set; }

        public string GPS { get; set; }

        public string Camera { get; set; }

        public string Description { get; set; }
    }
}
