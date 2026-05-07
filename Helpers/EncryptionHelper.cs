using System.Security.Cryptography;
using System.Text;

namespace CreditCard.Helpers
{
    // Klasa për enkriptim dhe dekriptim të të dhënave duke përdorur algoritmin AES
    public class EncryptionHelper
    {
        // Fusha private që ruan çelësin e enkriptimit si bajt array
        private readonly byte[] _key;

        // Konstruktori që merr çelësin e enkriptimit si string dhe e konverton në bajt array
        public EncryptionHelper(string key)
        {
            _key = Encoding.UTF8.GetBytes(key);
        }

        // Metodë për enkriptimin e tekstit të thjeshtë (plain text)
        public string Encrypt(string plainText)
        {
            // Krijon një instancë të algoritmit AES
            using var aes = Aes.Create();
            // Vendos çelësin e enkriptimit
            aes.Key = _key;
            // Gjeneron vektorin e inicializimit (IV) të rastësishëm
            aes.GenerateIV();

            // Krijon enkriptuesin duke përdorur çelësin dhe IV
            var encryptor = aes.CreateEncryptor();
            // Konverton tekstin në bajt array
            var bytes = Encoding.UTF8.GetBytes(plainText);
            // Kryen enkriptimin
            var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            // Kombinon IV dhe tekstin e enkriptuar në një array
            var result = aes.IV.Concat(encrypted).ToArray();
            // Kthen rezultatin si string të koduar në Base64
            return Convert.ToBase64String(result);
        }

        // Metodë për dekriptimin e tekstit të enkriptuar (cipher text)
        public string Decrypt(string cipherText)
        {
            // Konverton stringun Base64 në bajt array
            var full = Convert.FromBase64String(cipherText);

            // Krijon një instancë të algoritmit AES
            using var aes = Aes.Create();
            // Vendos çelësin e enkriptimit
            aes.Key = _key;

            // Nxjerr vektorin e inicializimit (IV) - 16 bajtët e parë
            var iv = full.Take(16).ToArray();
            // Nxjerr tekstin e enkriptuar - pjesën e mbetur pas IV
            var cipher = full.Skip(16).ToArray();

            // Vendos IV për dekriptim
            aes.IV = iv;
            // Krijon dekriptuesin
            var decryptor = aes.CreateDecryptor();
            // Kryen dekriptimin
            var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            // Kthen tekstin e dekriptuar si string
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}