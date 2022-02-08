using Album2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.Models
{
    public interface IImageRepository
    {

        public Task Add(Picture pic);

        public Task<Picture> GetById(string id);

        public Task<IEnumerable<Picture>> GetByAlbum(string aid);

        public Task Test();

        public Task DeleteByAlbum(string aid);

        public Task DeleteById(string id);

        public Task OrderById(string name, int order);

        public Task<Picture> GetSharedPicture(string guid);

        public Task DeleteSharePicture(string pid);

        public Task<IEnumerable<string>> GetSharesByPicture(string pid);

        public Task AddSharePicture(ShareAlbum sa);

    }
}
