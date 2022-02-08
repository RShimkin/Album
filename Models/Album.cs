using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class Album
    {
        public Album()
        {
            Comment = String.Empty;
            Num = 0;
        }

        public string Id { get; set; }

        public string User { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public DateTime Created { get; set; }

        public DateTime Changed { get; set; }

        public int Num { get; set; }
    }
}
