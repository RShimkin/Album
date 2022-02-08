using Album2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public interface ICrypt
    {
        public string GetNewIv();

        public string decrypt(string src, string plainiv);

        public string encrypt(string src, string plainiv);
    }
}
