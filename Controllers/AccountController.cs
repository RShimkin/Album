using Album2.Models;
using Album2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using BC = BCrypt.Net.BCrypt;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;

namespace Album2.Controllers
{
    public class AccountController : Controller
    {
        IUserRepository _urep;
        IRoleRepository _rrep;
        ICrypt _crep;
        string adminPWD = "Admin123";

        public AccountController(IUserRepository userrepository, IRoleRepository rolerepository, ICrypt crep)
        {
            _urep = userrepository;
            _rrep = rolerepository;
            _crep = crep;
            Check();
        }

        async private void Check()
        {
            Role role = await _rrep.Find("Admin");
            if (role == null)
            {
                await _rrep.Add("Admin");
            }

            role = await _rrep.Find("User");
            if (role == null)
            {
                await _rrep.Add("User");
            }

            User admin = await _urep.Find("Admin");
            if (admin == null)
            {
                Guid guid = Guid.NewGuid();
                await _urep.Add("Admin", "Admin@gmail.com", guid, BC.HashPassword(blend(adminPWD, guid.ToString())));
                admin = await _urep.Find("Admin");
                Role adminRole = await _rrep.Find("Admin");
                await _urep.AddRole(admin, adminRole);
            }
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _urep.Find(model.Username);
                if (user != null && BC.Verify(blend(model.Password,user.Id), user.Password))
                {
                    Role role = await _urep.GetRole(user);
                    await AuthenticateAsync(model.Username, role.Name);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные данные");
            }
            return View(model);
        }

        private string blend(string pas, string uid) 
        {
            string piece = uid.Split('-')[3];
            return piece + pas;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _urep.Find(model.Username);
                if (user == null)
                {
                    Guid guid = Guid.NewGuid();
                    string hash = BC.HashPassword(blend(model.Password, guid.ToString()));
                    
                    user = new User()
                    {
                        Id = guid.ToString(),
                        Password = hash,
                        Created = DateTime.Now,
                        UserName = model.Username,
                        Email = model.Email,
                        BirthDate = model.Birthdate,
                        Comment = model.Comment,
                    };
                    user = encryptUser(user);
                    await _urep.Add(user);
                    Role userRole = await _rrep.Find("User");
                    await _urep.AddRole(user, userRole);
                    await AuthenticateAsync(model.Username, "User");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь уже существует...");
                }
            }
            return View(model);
        }

        // не уверен в этих 2 функциях
        // но сам CryptRepository работает правильно
        private User encryptUser(User user)
        {
            string iv = _crep.GetNewIv();
            user.Email = _crep.encrypt(user.Email, iv);
            user.BirthDate = _crep.encrypt(user.BirthDate, iv);
            user.Iv = iv;
            return user;
        }

        private User decryptUser(User user)
        {
            string iv = user.Iv;
            user.Email = _crep.decrypt(user.Email, iv);
            user.BirthDate = _crep.decrypt(user.BirthDate, iv);
            return user;
        }

        public async Task AuthenticateAsync(string userName, string userRole)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Crypt()
        {
            string src = "Only english";

            var ivstr = _crep.GetNewIv();
            var crypted = _crep.encrypt(src, ivstr);
            string decrypted = _crep.decrypt(crypted, ivstr);
            var lst = new List<string> { src, decrypted, ivstr};

            return View(lst);
        }
        /*
        private string encrypt(string src)//, string iv = "0123456789101112", string key = "0123456789101112ABCDEFGHabcdefgh")
        { 
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.KeySize = 256;
            encryptor.BlockSize = 128;
            encryptor.Padding = PaddingMode.PKCS7;

            encryptor.Key = aeskey;
            encryptor.IV = iv;

            MemoryStream ms = new MemoryStream();
            ICryptoTransform crypt = encryptor.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write);

            byte[] plainbytes = ASCIIEncoding.ASCII.GetBytes(src);
            cs.Write(plainbytes, 0, plainbytes.Length);
            cs.FlushFinalBlock();

            byte[] cipherbytes = ms.ToArray();
            ms.Close();
            cs.Close();
            string ciphertext = Convert.ToBase64String(cipherbytes, 0, cipherbytes.Length);
            return ciphertext;
        }

        private string decrypt(string src)//, string iv = "0123456789101112", string key = "0123456789101112ABCDEFGHabcdefgh")
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.KeySize = 256;
            encryptor.BlockSize = 128;
            encryptor.Padding = PaddingMode.PKCS7;

            encryptor.Key = aeskey;
            encryptor.IV = iv;

            MemoryStream ms = new MemoryStream();
            ICryptoTransform crypt = encryptor.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write);

            string plaintext = String.Empty;
            try
            {
                byte[] cipherbytes = Convert.FromBase64String(src);
                cs.Write(cipherbytes, 0, cipherbytes.Length);
                cs.FlushFinalBlock();
                byte[] plainbytes = ms.ToArray();
                plaintext = ASCIIEncoding.ASCII.GetString(plainbytes, 0, plainbytes.Length);
            }
            finally
            {
                ms.Close();
                cs.Close();
            }
            
            return plaintext;
        }
        */
    }
}
