using Album2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public interface IRoleRepository
    {
        public Task Delete(int id);

        public Task Add(string name);

        public Task<Role> Find(string name);
    }
}
