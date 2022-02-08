using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Album2.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно!")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Неверный пароль!")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        public string Birthdate { get; set; }

        [DataType(DataType.Text)]
        public string Comment { get; set; }
    }
}
