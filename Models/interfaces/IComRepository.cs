using System.Collections.Generic;
using System.Threading.Tasks;

namespace Album2.Models
{
    public interface IComRepository
    {

        public Task Add(Comment com);

        public Task<IEnumerable<Comment>> GetByAlbum(string aid);

        public Task<IEnumerable<Comment>> GetByPicture(string pid);

        public Task DeleteByAlbum(string aid);

        public Task DeleteByPicture(string pid);

    }
}
