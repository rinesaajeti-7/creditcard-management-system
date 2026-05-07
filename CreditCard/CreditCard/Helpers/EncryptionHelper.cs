using System.Security.Cryptography;
using System.Text;

namespace CreditCard.Helpers
{
    public class EncryptionHelper
    {
        private readonly byte[] _key;

        public EncryptionHelper(string key)
        {
            _key = Encoding.UTF8.GetBytes(key);
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor();
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            var result = aes.IV.Concat(encrypted).ToArray();
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            var full = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = full.Take(16).ToArray();
            var cipher = full.Skip(16).ToArray();

            aes.IV = iv;
            var decryptor = aes.CreateDecryptor();
            var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
