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
    public class AlbumsRepository : IAlbumRepository
    {
        string constr = null;

        public AlbumsRepository(string connectionString)
        {
            constr = connectionString;
        }

        async public Task Add(string name, string uid, string com, DateTime created)
        {
            string id = Guid.NewGuid().ToString();
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "INSERT INTO db1.albums(id, user, name, comment, created, changed, num)" + 
                    "VALUES (@id, @uid, @name, @com, @created, @changed, @num)";
                await db.ExecuteAsync(queryString, new { id, uid, name, com, created, changed = created, num = 0 });
            }
        }

        async public Task<IEnumerable<Album>> GetByUser(User user)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                //var uid = user.Id;
                string queryString = "SELECT id, user, name, comment, created, changed, num FROM db1.albums WHERE user=@uid";
                var arr = await db.QueryAsync<Album>(queryString, new { uid = user.Id });
                return arr;
            }
        }

        async public Task<Album> GetAlbum(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT id, user, name, comment, created, changed, num FROM db1.albums WHERE id=@aid";
                Album album = await db.QueryFirstOrDefaultAsync<Album>(queryString, new { aid });
                return album;
            }
        }

        async public Task Increment(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "UPDATE db1.albums SET num=num+1 WHERE id=@aid";
                await db.ExecuteAsync(queryString, new { aid });
            }
        }

        async public Task AddShare(ShareAlbum sa)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "INSERT INTO db1.shared_albums(guid, album, created) VALUES (@guid, @aid, @created);";
                await db.ExecuteAsync(queryString, new { guid = sa.Guid, aid = sa.Album, created = sa.Created });
            }
        }

        async public Task<IEnumerable<string>> GetSharesByAlbum(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE from db1.shared_albums WHERE created < date_sub(current_timestamp(), interval @mins minute);";
                await db.ExecuteAsync(queryString, new { mins = 10 });
                queryString = "SELECT guid FROM db1.shared_albums WHERE album=@aid;";
                var shares = await db.QueryAsync<string>(queryString, new { aid });
                return shares;
            }
        }

        async public Task DeleteShare(string aid)
        {
            using IDbConnection db = new MySqlConnection(constr);
            string queryString = "DELETE FROM db1.shared_albums WHERE album=@aid;";
            await db.ExecuteAsync(queryString, new { aid });
        }
        
        async public Task<Album> GetSharedAlbum(string guid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE from db1.shared_albums WHERE created < date_sub(current_timestamp(), interval @mins minute);";
                await db.ExecuteAsync(queryString, new { mins = 10 });
                queryString = "SELECT album FROM db1.shared_albums WHERE guid=@guid";
                //ShareAlbum? sa = db.QueryFirst<ShareAlbum>(queryString, new { guid });
                string aid = await db.QueryFirstOrDefaultAsync<string>(queryString, new { guid });
                if (String.IsNullOrEmpty(aid))
                    return null;
                else
                    return await GetAlbum(aid);
            }
        }

        async public Task DeleteByUser(string uid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.albums WHERE user=@uid;";
                await db.ExecuteAsync(queryString, new { uid });
            }
        }

        async public Task DeleteById(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.albums WHERE id=@aid;";
                await db.ExecuteAsync(queryString, new { aid });
            }
        }

        async public Task Decrement(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "UPDATE db1.albums SET num=num-1 WHERE id=@aid";
                await db.ExecuteAsync(queryString, new { aid });
            }
        }
    }
}
