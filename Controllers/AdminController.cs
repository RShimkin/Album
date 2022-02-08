
using Album2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using log4net;
using System.Threading.Tasks;

namespace Album2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));
        IImageRepository _picrep;
        IUserRepository _urep;
        IAlbumRepository _arep;
        IComRepository _comrep;

        public AdminController(IImageRepository prep, IUserRepository urep, IAlbumRepository arep, IComRepository crep)
        {
            _picrep = prep;
            _urep = urep;
            _arep = arep;
            _comrep = crep;
        }

        public IActionResult Index()
        {
            //log.Info("Home Controller -> Index");
            return RedirectToAction("Users");
        }

        async public Task<IActionResult> Users()
        {
            var userList = await _urep.GetAllUsers();
            return View(userList);
        }

        async public Task<JsonResult> DeleteUser(string id)
        {
            var albums = await _arep.GetByUser(new Models.User() { Id = id });
            foreach (var album in albums)
            {
                var pics = await _picrep.GetByAlbum(album.Id);
                foreach (var pic in pics)
                {
                    await _comrep.DeleteByPicture(pic.Id);
                }
                await _picrep.DeleteByAlbum(album.Id);
                await _comrep.DeleteByAlbum(album.Id);
            }
            await _arep.DeleteByUser(id);
            await _urep.Delete(id);
            return Json(id);
        }

        async public Task<JsonResult> DeleteAlbum(string id)
        {
            var pics = await _picrep.GetByAlbum(id);
            foreach (var pic in pics)
            {
                await _comrep.DeleteByPicture(pic.Id);
            }
            await _picrep.DeleteByAlbum(id);
            await _comrep.DeleteByAlbum(id);
            await _arep.DeleteById(id);
            return Json(id);
        }

        async public Task<JsonResult> DeletePicture(string id)
        {
            var pic = await _picrep.GetById(id);
            await _comrep.DeleteByPicture(pic.Id);
            await _picrep.DeleteById(id);
            await _arep.Decrement(pic.Album);
            return Json(id);
        }

        async public Task<JsonResult> GetAlbumsByUser(string id)
        {
            var albums = await _arep.GetByUser(new Models.User() { Id = id });
            return Json(albums);
        }

        async public Task<JsonResult> GetPicturesByAlbum(string id)
        {
            var pics = await _picrep.GetByAlbum(id);
            return Json(pics);
        }
    }
}
