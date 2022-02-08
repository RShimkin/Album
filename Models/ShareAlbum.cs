using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class ShareAlbum
    {
        public ShareAlbum()
        {
            Created = DateTime.Now;
        }
        public string Guid { get; set; }

        public string Album { get; set; }

        public DateTime Created { get; set; }
    }
}
