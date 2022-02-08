using Album2.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Album2.Models
{
    public class CryptRepository : ICrypt 
    {
        string plainkey = "jepk6UjVW4w9miUCGqItDdWI/MjVd8CbsB0a6JuU2gk=";

        public CryptRepository() {}

        public string GetNewIv()
        {
            Aes aes = Aes.Create();
            aes.GenerateIV();
            byte[] iv = aes.IV;
            return Convert.ToBase64String(iv, 0, iv.Length);
        }
        
        public string decrypt(string src, string plainiv)//, string iv = "0123456789101112", string key = "0123456789101112ABCDEFGHabcdefgh")
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.KeySize = 256;
            encryptor.BlockSize = 128;
            encryptor.Padding = PaddingMode.PKCS7;

            byte[] aeskey = Convert.FromBase64String(plainkey);
            encryptor.Key = aeskey;
            encryptor.IV = Convert.FromBase64String(plainiv);

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

        public string encrypt(string src, string plainiv)//, string iv = "0123456789101112", string key = "0123456789101112ABCDEFGHabcdefgh")
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.KeySize = 256;
            encryptor.BlockSize = 128;
            encryptor.Padding = PaddingMode.PKCS7;

            byte[] aeskey = Convert.FromBase64String(plainkey);
            encryptor.Key = aeskey;
            encryptor.IV = Convert.FromBase64String(plainiv);

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
    }
}
