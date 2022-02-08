using Album2.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.ViewModels
{
    public class AlbumInfo
    {
        public string Album { get; set; }

        //public IFormFile Image { get; set; }
        public IEnumerable<IFormFile> Images { get; set; }

        public string Text { get; set; }
    }
}
