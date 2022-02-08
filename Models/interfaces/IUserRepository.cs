using Album2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Album2.Models
{
    public interface IUserRepository
    {
        public Task Delete(string id);

        public Task<List<User>> GetAllUsers();

        public Task Add(string un, string email, Guid guid, string pwd, string date = null, string com = null);

        public Task Add(User user);

        public Task<User> Find(string un);

        public Task AddRole(User user, Role role);

        public Task<Role> GetRole(User user);
    }
}
