using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.EncryptionRepository
{
    public class PasswordRepository:IPasswordRepository
    {
        public async Task<string> GetEncryptedPassword(string password)
        {
            string encryptedPassword = Encryptword(password);
            return encryptedPassword;
        }
        private string Encryptword(string password)
        {
            string key = "1prt56";
            byte[] SrctArray;

            byte[] EnctArray = UTF8Encoding.UTF8.GetBytes(password);

            SrctArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider objcrpt = new MD5CryptoServiceProvider();

            SrctArray = objcrpt.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

            objcrpt.Clear();

            objt.Key = SrctArray;

            objt.Mode = CipherMode.ECB;

            objt.Padding = PaddingMode.PKCS7;

            ICryptoTransform crptotrns = objt.CreateEncryptor();

            byte[] resArray = crptotrns.TransformFinalBlock(EnctArray, 0, EnctArray.Length);

            objt.Clear();

            return Convert.ToBase64String(resArray, 0, resArray.Length);

        }
    }
}
