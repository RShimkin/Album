
using Album2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using ExifLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using BC = BCrypt.Net.BCrypt;
using log4net;
using Album2.ViewModels;

namespace Album2.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));
        IImageRepository picrep;
        IHostingEnvironment env;

        public HomeController(/*ILogger<HomeController> logger,*/ IImageRepository pics, IHostingEnvironment henv)
        {
            picrep = pics;
            env = henv;
        }

        public IActionResult Index()
        {
            //log.Info("Home Controller -> Index");
            return View();
        }

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        public JsonResult Add(IEnumerable<IFormFile> Images, string param)
        {
            return Json(param);
            //return number1 + number2;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
