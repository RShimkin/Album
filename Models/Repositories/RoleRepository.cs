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
    public class RoleRepository : IRoleRepository
    {
        string constr = null;

        public RoleRepository(string connectionString)
        {
            constr = connectionString;
        }
        async public Task Delete(int id)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.roles WHERE id = @id";
                await db.ExecuteAsync(queryString, new { id }); 
            }
        }

        async public Task Add(string name)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                //var exists = Find(name) == null ? false : true;
                if (!(Find(name) == null))
                {
                    string queryString = "INSERT INTO db1.roles(name) VALUES (@name)";
                    await db.ExecuteAsync(queryString, new { name });
                }
            }
        }

        async public Task<Role> Find(string name)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT id, name FROM db1.roles WHERE name = @name";
                var role = await db.QueryFirstOrDefaultAsync<Role>(queryString, new { name });
                return role;
            }
        }
    }
}
