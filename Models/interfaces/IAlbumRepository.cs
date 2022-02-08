using Album2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public interface IAlbumRepository
    {
        public Task Add(string name, string uid, string com, DateTime created);

        public Task<IEnumerable<Album>> GetByUser(User user);

        public Task<Album> GetAlbum(string aid);

        public Task Increment(string aid);

        public Task AddShare(ShareAlbum sa);

        public Task DeleteShare(string aid);

        public Task<Album> GetSharedAlbum(string id);

        public Task DeleteByUser(string uid);

        public Task DeleteById(string aid);

        public Task Decrement(string aid);

        public Task<IEnumerable<string>> GetSharesByAlbum(string aid);
    }
}
