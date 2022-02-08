using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class ComRepository : IComRepository
    {
        string constr = null;

        public ComRepository(string connectionString)
        {
            constr = connectionString;
        }

        async public Task Add(Comment com)
        {
            com.Id = Guid.NewGuid().ToString();
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "INSERT INTO db1.comments(id, text, author, auid, created, album, pic) " +
                    "VALUES (@id, @text, @author, @auid, @created, @album, @pic)";
                await db.ExecuteAsync(queryString, new { 
                    id = com.Id, text = com.Text, author = com.Author, auid = com.Auid, album = com.Album, 
                    created = com.Created, pic = com.Pic
                });
            }
        }

        async public Task<IEnumerable<Comment>> GetByAlbum(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT COUNT(id) FROM db1.comments WHERE album=@aid";
                //int count = await db.QueryFirstOrDefaultAsync<int>(queryString, new { aid });
                queryString = "SELECT * FROM db1.comments WHERE album=@aid ORDER BY created";
                var coms = await db.QueryAsync<Comment>(queryString, new { aid });
                return coms;
            }
        }

        async public Task<IEnumerable<Comment>> GetByPicture(string pid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT * FROM db1.comments WHERE pic=@pid ORDER BY created";
                var coms = await db.QueryAsync<Comment>(queryString, new { pid });
                return coms;
            }
        }

        async public Task DeleteByAlbum(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.comments WHERE album=@aid";
                await db.ExecuteAsync(queryString, new { aid });
            }
        }

        async public Task DeleteByPicture(string pid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.comments WHERE pic=@pid";
                await db.ExecuteAsync(queryString, new { pid });
            }
        }
    }
}
