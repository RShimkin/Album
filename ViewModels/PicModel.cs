using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.ViewModels
{
    public class PicModel
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        public string Album { get; set; }
    }
}
