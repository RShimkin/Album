using Album2.Models;
using Album2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using ExifLib;
using log4net;

namespace Album2.Controllers
{
    public class AlbumsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));
        readonly IHostingEnvironment env;
        IUserRepository _urep;
        IAlbumRepository _arep;
        IImageRepository _picrep;
        IComRepository _comrep;
        public AlbumsController(IUserRepository userrepository, IAlbumRepository albumrepository,
#pragma warning disable CS0618 // Тип или член устарел
            IImageRepository picrep, IHostingEnvironment henv, IComRepository comrep)
#pragma warning restore CS0618 // Тип или член устарел
        {
            _urep = userrepository;
            _arep = albumrepository;
            _picrep = picrep;
            _comrep = comrep;
            env = henv;
        }

        [HttpGet]
        async public Task<IActionResult> Create()
        {
            //var name = User.Identity.Name;
            User user = await _urep.Find(User.Identity.Name);

            return View();
        }

        [HttpPost]
        async public Task<IActionResult> Create(AlbumModel model)
        {
            User user = await _urep.Find(User.Identity.Name);
            string name = model.Name;
            string com = model.Comment;
            string date = DateTime.Now.ToString();
            string uid = user.Id;
            await _arep.Add(name, uid, com, DateTime.Now);
            return RedirectToAction("Index", "Albums");
        }

        //[IgnoreAntiforgeryToken]
        [HttpPost]
        async public Task<JsonResult> UploadPictures([FromForm] AlbumInfo model)//[FromForm] IEnumerable<IFormFile> Images, string album)
        {
            //string album = "22296823-20d9-4a70-9f73-50a478c678da";
            log.Info("Albums UploadPicture");
            List<string> res = new List<string>();
            foreach (var image in model.Images)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)image.Length);
                }
                string tempname = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                long size = image.Length;
                //var filePath = Path.GetTempFileName();

                var folder = Path.Combine(env.WebRootPath, "uploads");
                var fullpath = Path.Combine(folder, tempname);

                using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                {
                    image.CopyTo(fs);
                }

                Picture pic = new Picture
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = image.FileName,
                    Image = imageData,
                    Album = model.Album,
                    Upload = DateTime.Now
                };
                res.Add(pic.Id);

                try
                {
                    using (ExifReader reader = new ExifReader(fullpath))
                    {
                        DateTime taken;
                        if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out taken))
                        {
                            //info.Add(taken.ToString());
                            pic.Created = taken;
                        }
                        string camera;
                        if (reader.GetTagValue<string>(ExifTags.Model, out camera))
                        {
                            pic.Camera = camera;
                        };
                        string lon, lat;
                        if (reader.GetTagValue<string>(ExifTags.GPSLongitude, out lon))
                        {
                            if (reader.GetTagValue<string>(ExifTags.GPSLatitude, out lat))
                            {
                                pic.GPS = lat + lon;
                            }
                        };
                    }
                }
                catch (ExifLibException)
                {
                    //info.Add("Нет Exif блока (невозможно достать метаданные)");
                }

                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }

                await _picrep.Add(pic);
                await _arep.Increment(pic.Album);
            }
            return Json(res);
        }

        [HttpPost]
        async public Task<IActionResult> Album([FromForm] AlbumInfo model)
        {
            if (ModelState.IsValid && model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)image.Length);
                    }
                    string tempname = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    long size = image.Length;
                    //var filePath = Path.GetTempFileName();

                    var folder = Path.Combine(env.WebRootPath, "uploads");
                    var fullpath = Path.Combine(folder, tempname);

                    using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                    {
                        image.CopyTo(fs);
                    }

                    Picture pic = new Picture
                    {
                        Name = image.FileName,
                        Image = imageData,
                        Album = model.Album,
                        Upload = DateTime.Now
                    };

                    try
                    {
                        using (ExifReader reader = new ExifReader(fullpath))
                        {
                            DateTime taken;
                            if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out taken))
                            {
                                //info.Add(taken.ToString());
                                pic.Created = taken;
                            }
                            string camera;
                            if (reader.GetTagValue<string>(ExifTags.Model, out camera))
                            {
                                pic.Camera = camera;
                            };
                            string lon, lat;
                            if (reader.GetTagValue<string>(ExifTags.GPSLongitude, out lon))
                            {
                                if (reader.GetTagValue<string>(ExifTags.GPSLatitude, out lat))
                                {
                                    pic.GPS = lat + lon;
                                }
                            };
                        }
                    }
                    catch (ExifLibException)
                    {
                        //info.Add("Нет Exif блока (невозможно достать метаданные)");
                    }

                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }

                    await _picrep.Add(pic);
                    await _arep.Increment(pic.Album);
                }
            }
            else if (ModelState.IsValid && model.Text != null)
            {
                string author;
                if (User.Identity.IsAuthenticated)
                {
                    author = User.Identity.Name;
                }
                else
                {
                    author = "Unknown";
                }
                Comment com = new Comment
                {
                    Album = model.Album,
                    Text = model.Text,
                    Author = author,
                    Created = DateTime.Now
                };
                await _comrep.Add(com);
            }
            //IEnumerable<Pic> arr = _picrep.Get(user);
            return RedirectToAction("Album", "Albums", new { id = model.Album });
        }

        [HttpPost]
        async public Task<IActionResult> SharedComment([FromForm] AlbumInfo model)
        {
            if (ModelState.IsValid && model.Text != null)
            {
                string author;
                if (User.Identity.IsAuthenticated)
                {
                    author = User.Identity.Name;
                }
                else
                {
                    author = "Unknown";
                }
                Comment com = new Comment
                {
                    Album = model.Album,
                    Text = model.Text,
                    Author = author,
                    Created = DateTime.Now
                };
                await _comrep.Add(com);
            }
            return RedirectToActionPreserveMethod("GetShared", "Albums", new { id = model.Album });
        }

        [HttpPost]
        async public Task<JsonResult> SharedCommentAjax(AlbumInfo model)
        {
            if (ModelState.IsValid && model.Text != null)
            {
                string author;
                if (User.Identity.IsAuthenticated)
                {
                    author = User.Identity.Name;
                }
                else
                {
                    author = "Unknown";
                }
                var created = DateTime.Now;
                Comment com = new Comment
                {
                    Album = model.Album,
                    Text = model.Text,
                    Author = author,
                    Created = created
                };
                await _comrep.Add(com);
                return Json(created.ToString());
            }
            else return Json(-1);
        }

        [HttpGet]
        async public Task<JsonResult> SharedCommentPictureAjax(string pic, string text)
        {
            string author;
            if (User.Identity.IsAuthenticated)
                {
                    author = User.Identity.Name;
                }
            else
                {
                    author = "Unknown";
                }
            var created = DateTime.Now;
            Comment com = new Comment
            {
                    Pic = pic,
                    Text = text,
                    Author = author,
                    Created = created
            };
            await _comrep.Add(com);
            return Json(created.ToString());
        }

        /*[HttpPost]
        public IActionResult Album([FromForm] AlbumInfo model)
        {
            if (ModelState.IsValid && model.Image != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)model.Image.Length);
                }
                string tempname = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                long size = model.Image.Length;
                //var filePath = Path.GetTempFileName();

                var folder = Path.Combine(env.WebRootPath, "uploads");
                var fullpath = Path.Combine(folder, tempname);

                using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                {
                    model.Image.CopyTo(fs);
                }

                Picture pic = new Picture
                {
                    Name = model.Image.FileName,
                    Image = imageData,
                    Album = model.Album,
                    Upload = DateTime.Now
                };

                try
                {
                    using (ExifReader reader = new ExifReader(fullpath))
                    {
                        DateTime taken;
                        if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out taken))
                        {
                            //info.Add(taken.ToString());
                            pic.Created = taken;
                        }
                        string camera;
                        if (reader.GetTagValue<string>(ExifTags.Model, out camera))
                        {
                            pic.Camera = camera;
                        };
                        string lon, lat;
                        if (reader.GetTagValue<string>(ExifTags.GPSLongitude, out lon))
                        {
                            if (reader.GetTagValue<string>(ExifTags.GPSLatitude, out lat)) 
                            {
                                pic.GPS = lat + lon;
                            }
                        };
                    }
                }
                catch (ExifLibException)
                {
                    //info.Add("Нет Exif блока (невозможно достать метаданные)");
                }

                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                
                _picrep.Add(pic);
                _arep.Increment(pic.Album);
            }
            else if (ModelState.IsValid && model.Text != null)
            {
                string author;
                if (User.Identity.IsAuthenticated)
                {
                    author = User.Identity.Name;
                }
                else
                {
                    author = "Unknown";
                }
                Comment com = new Comment
                {
                    Album = model.Album,
                    Text = model.Text,
                    Author = author,
                    Created = DateTime.Now
                };
                _comrep.Add(com);
            }
            //IEnumerable<Pic> arr = _picrep.Get(user);
            return RedirectToAction("Album","Albums", new { id = model.Album });
        }*/

        [HttpGet]
        [Authorize]
        async public Task<IActionResult> Index()
        {
            User user = await _urep.Find(User.Identity.Name);
            var albums = await _arep.GetByUser(user);
            return View(albums.ToList());
        }

        [HttpGet]
        [Authorize]
        async public Task<IActionResult> Album(string id)
        {
            Album album = await _arep.GetAlbum(id);
            var pics = (await _picrep.GetByAlbum(album.Id)).ToList();
            var coms = (await _comrep.GetByAlbum(id)).ToList();
            foreach (var com in coms)
            {
                if (com.Author == User.Identity.Name)
                {
                    com.Author = "Вы";
                }
            }
            AlbumPicCommentVM vm = new AlbumPicCommentVM() {
                Album = album, Coms = coms, Pics = pics
            };
            var shares = await _arep.GetSharesByAlbum(id);
            ViewBag.shares = shares;
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        async public Task<IActionResult> Picture(string id)
        {
            Picture pic = await _picrep.GetById(id);
            var coms = (await _comrep.GetByPicture(id)).ToList();
            ViewBag.pic = pic;
            var shares = await _picrep.GetSharesByPicture(id);
            ViewBag.shares = shares;
            return View(coms);
        }

        [HttpPost]
        [Authorize]
        async public Task<IActionResult> Picture([FromForm] string Pic, string Text)
        {
            string author;
            if (User.Identity.IsAuthenticated)
            {
                author = User.Identity.Name;
            }
            else
            {
                author = "Unknown";
            }
            Comment com = new Comment
            {
                Pic = Pic,
                Text = Text,
                Author = author,
                Created = DateTime.Now
            };
            await _comrep.Add(com);
            return RedirectToAction("Picture", new { id = Pic });
        }

        [HttpGet]
        async public Task<JsonResult> ShareAlbumAjax(string id)
        {
            Guid guid = Guid.NewGuid();
            ShareAlbum sa = new ShareAlbum()
            {
                Created = DateTime.Now,
                Album = id,
                Guid = guid.ToString()
            };
            await _arep.AddShare(sa);
            var shares = await _arep.GetSharesByAlbum(id);
            return Json(shares);
        }

        [HttpGet]
        async public Task<JsonResult> SharePictureAjax(string id)
        {
            Guid guid = Guid.NewGuid();
            ShareAlbum sp = new ShareAlbum()
            {
                Created = DateTime.Now,
                Album = id,
                Guid = guid.ToString()
            };
            await _picrep.AddSharePicture(sp);
            var shares = await _picrep.GetSharesByPicture(id);
            return Json(shares);
        }

        async public Task<IActionResult> ShareAlbum(string id)
        {
            Guid guid = Guid.NewGuid();
            ShareAlbum sa = new ShareAlbum()
            {
                Created = DateTime.Now,
                Album = id,
                Guid = guid.ToString()
            };
            await _arep.AddShare(sa);
            ViewBag.guid = guid;
            return View();
        }

        [HttpPost]
        async public Task<IActionResult> GetShared(string id)
        {
            //Album? album = 
            Album album = await _arep.GetSharedAlbum(id);
            if (album != null)
            {
                AlbumPicCommentVM vm = new AlbumPicCommentVM()
                {
                    Album = album,
                    Pics = (await _picrep.GetByAlbum(album.Id)).ToList(),
                    Coms = (await _comrep.GetByAlbum(album.Id)).ToList()
                };
                return View("ShowAlbum", vm);//RedirectToAction("ShowAlbum", "Albums", new { album = vm });
            }
            Picture picture = await _picrep.GetSharedPicture(id);
            if (picture != null)
            {
                ViewBag.pic = picture;
                var coms = await _comrep.GetByPicture(picture.Id);
                return View("ShowPicture", coms);
            }
            else return View();
        }

        [HttpGet]
        public IActionResult ShowAlbum(AlbumPicCommentVM vm) => View(vm);

        [HttpGet]
        public IActionResult ShowPicture(AlbumPicCommentVM vm) => View(vm);

        [HttpGet]
        async public Task<IActionResult>  Edit(string id)
        {
            Album album = await _arep.GetAlbum(id);
            var pics = await _picrep.GetByAlbum(album.Id);
            ViewBag.album = album;
            return View(pics);
        }

        [HttpPost]
        async public Task<JsonResult> DragDrop(List<string> pics)
        {
            for (int i = 0; i < pics.Count; i++)
            {
                await _picrep.OrderById(pics[i], i);
            }
            return Json(pics.Count);
        }

        [HttpPost]
        async public Task<JsonResult> DeletePicture(string id)
        {
            string album = (await _picrep.GetById(id)).Album;
            await _picrep.DeleteById(id);
            await _arep.Decrement(album);
            return Json(id);
        }

        [HttpGet]
        async public Task<JsonResult> DeleteAlbumAjax(string id)
        {
            //Album album = _arep.GetAlbum(id);
            var pics = await _picrep.GetByAlbum(id);
            foreach (var pic in pics)
            {
                await _comrep.DeleteByPicture(pic.Id);
            }
            await _picrep.DeleteByAlbum(id);
            await _comrep.DeleteByAlbum(id);
            await _arep.DeleteById(id);
            await _arep.DeleteShare(id);
            return Json(id);
        }

        [HttpGet]
        async public Task<IActionResult> DeleteAlbum(string id)
        {
            await DeleteAlbumAjax(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        async public Task<JsonResult> PostComment(string text, string album)
        {
            string author;
            if (User.Identity.IsAuthenticated)
            {
                author = User.Identity.Name;
            }
            else
            {
                author = "Unknown";
            }
            var created = DateTime.Now;
            Comment com = new Comment
            {
                Album = album,
                Text = text,
                Author = author,
                Created = created
            };
            await _comrep.Add(com);
            return Json(created);
        }
    }
}
