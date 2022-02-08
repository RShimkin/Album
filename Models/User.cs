using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class User
    {
        public User()
        {
            BirthDate = String.Empty;
            Comment = String.Empty;
        }
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string BirthDate { get; set; }

        public string Comment { get; set; }

        public DateTime Created { get; set; }

        public string Iv { get; set; }

    }
}
