using Album2.Models;
using Dapper;
using ExifLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class PicsRepository : IImageRepository
    {
        string constr = null;

        public PicsRepository(string connectionString)
        {
            constr = connectionString;
        }

        async public Task<Picture> GetById(string id)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT * FROM db1.images WHERE id=@id";
                var pic = await db.QueryFirstOrDefaultAsync<Picture>(queryString, new { id });
                return pic;
            }
        }

        async public Task Add(Picture pic)
        {
            
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT COUNT(ord) FROM db1.images WHERE album=@aid";
                var count = await db.QueryFirstOrDefaultAsync<int>(queryString, new { aid = pic.Album});
                int maxord = 0;
                if (count > 0)
                {
                    queryString = "SELECT MAX(ord) FROM db1.images WHERE album=@aid";
                    maxord = await db.QueryFirstOrDefaultAsync<int>(queryString, new { aid = pic.Album }); ;
                }
                queryString = "INSERT INTO db1.images(id, name, image, album, ord, upload, created, gps, camera, description) " +
                    "VALUES (@id, @name, @image, @album, @ord, @upload, @created, @gps, @camera, @descr)";
                await db.ExecuteAsync(queryString, new { 
                    id = pic.Id, name = pic.Name, image = pic.Image, album = pic.Album, upload = pic.Upload, 
                    ord = maxord + 1, created = pic.Created, gps = pic.GPS, camera = pic.Camera, descr = pic.Description
                });
            }
        }

        async public Task<IEnumerable<Picture>> GetByAlbum(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT id, name, image, album, ord, upload, created, gps, camera, " +
                    "description FROM db1.images WHERE album=@aid ORDER BY ord";
                var pics = await db.QueryAsync<Picture>(queryString, new { aid });
                return pics;
            }
        }

        async public Task OrderById(string id, int order)
        {
            string queryString = "UPDATE db1.images SET ord=@order WHERE id=@id ";
            using (IDbConnection db = new MySqlConnection(constr))
            {
                await db.ExecuteAsync(queryString, new { id, order });
            }
        }

        async public Task Test() ////////////////////////////////////////
        {
            DateTime now = DateTime.Now;
            string queryString = "INSERT INTO db1.date(datetime) VALUES (@date)";
            using (IDbConnection db = new MySqlConnection(constr))
            {
                await db.ExecuteAsync(queryString, new { date = now });
            }
        }

        async public Task DeleteByAlbum(string aid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.images WHERE album=@aid;";
                await db.ExecuteAsync(queryString, new { aid });
            }
        }

        async public Task DeleteById(string id)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE FROM db1.images WHERE id=@id;";
                await db.ExecuteAsync(queryString, new { id });
            }
        }

        async public Task<Picture> GetSharedPicture(string guid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "DELETE from db1.shared_pictures WHERE created < date_sub(current_timestamp(), interval @mins minute);";
                await db.ExecuteAsync(queryString, new { mins = 10 });
                queryString = "SELECT picture FROM db1.shared_pictures WHERE guid=@guid";
                string pid = await db.QueryFirstOrDefaultAsync<string>(queryString, new { guid });
                if (String.IsNullOrEmpty(pid))
                    return null;
                else
                    return await GetById(pid);
            }
        }

        async public Task DeleteSharePicture(string pid)
        {
            using IDbConnection db = new MySqlConnection(constr);
            string queryString = "DELETE FROM db1.shared_pictures WHERE picture=@pid;";
            await db.ExecuteAsync(queryString, new { pid });
        }

        async public Task<IEnumerable<string>> GetSharesByPicture(string pid)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "SELECT guid FROM db1.shared_pictures WHERE picture=@pid;";
                var shares = await db.QueryAsync<string>(queryString, new { pid });
                return shares;
            }
        }

        async public Task AddSharePicture(ShareAlbum sp)
        {
            using (IDbConnection db = new MySqlConnection(constr))
            {
                string queryString = "INSERT INTO db1.shared_pictures(guid, picture, created) VALUES (@guid, @pid, @created);";
                await db.ExecuteAsync(queryString, new { guid = sp.Guid, pid = sp.Album, created = sp.Created });
            }
        }
    }
}
