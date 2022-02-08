using Album2.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.ViewModels
{
    public class AlbumPicCommentVM
    {
        public Album Album { get; set; }
        public List<Picture> Pics{ get; set; }

        public List<Comment> Coms { get; set; }
    }
}
