using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class TestPic
    {
        public TestPic()
        {
            Uploaded = DateTime.Now;
            coords = null;
            Camera = null;
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Image { get; set; }

        public DateTime Uploaded { get; set; }

        public DateTime Created { get; set; }

        public string coords { get; set; }

        public string Camera { get; set; }
    }
}
