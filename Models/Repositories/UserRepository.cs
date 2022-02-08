using Album2.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class UserRepository : IUserRepository
    {
        string constr = null;

        public UserRepository(string connectionString)
        {
            constr = connectionString;
        }

        async public Task Delete(string id)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.users WHERE id = @id";
                await db.ExecuteAsync(queryString, new { id });
            }
        }

        async public Task Add(string un, string email, Guid guid, string pwd, string date = null, string com = null)
        {
            //Guid guid = Guid.NewGuid();
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "INSERT INTO db1.users(id, username, email, password, birth_date, comment) " +
                    "VALUES (@id, @un, @email, @pwd, @date, @com)";
                await db.ExecuteAsync(queryString, new { id=guid, un, email, pwd, date, com });
            }
        }

        async public Task Add(User user)
        { 
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "INSERT INTO db1.users(id, username, email, password, birth_date, comment, created, iv) " +
                    "VALUES (@id, @un, @email, @pwd, @date, @com, @created, @iv)";
                await db.ExecuteAsync(queryString, new { id = user.Id, un = user.UserName, email = user.Email, 
                    pwd = user.Password, date = user.BirthDate, com = user.Comment, created = user.Created, iv = user.Iv });
            }
        }

        async public Task<User> Find(string un)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT id, username, email, password FROM db1.users WHERE username = @un";
                var arr = await db.QueryFirstOrDefaultAsync<User>(queryString, new { un });
                return arr;
            }
        }

        async public Task AddRole(User user, Role role)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "INSERT INTO db1.users_roles(user_id, role_id) VALUES (@uid, @rid)";
                await db.ExecuteAsync(queryString, new { uid = user.Id, rid = role.Id });
            }
        }

        async public Task<Role> GetRole(User user)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT role_id FROM db1.users_roles WHERE user_id = @uid";
                int rid = await db.QueryFirstAsync<int>(queryString, new { uid = user.Id });
                queryString = "SELECT id, name FROM db1.roles WHERE id = @rid";
                Role role = await db.QueryFirstAsync<Role>(queryString, new { rid });
                return role;
            }
        }

        async public Task<List<User>> GetAllUsers()
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT * FROM db1.users";
                var arr = await db.QueryAsync<User>(queryString);
                return arr.ToList();
            }
        }
    }
}
