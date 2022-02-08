using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class Comment
    {
        public Comment()
        {
            Created = DateTime.Now;
        }
        public string Id { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }

        public string Auid { get; set; }

        public string Album { get; set; }

        public string Pic { get; set; }

        public DateTime Created { get; set; }
    }
}
