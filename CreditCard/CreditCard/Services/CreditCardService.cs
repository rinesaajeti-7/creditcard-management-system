using CreditCard.Data;
using CreditCard.Helpers;
using CreditCard.Models;

namespace CreditCard.Services
{
    public class CreditCardService
    {
        private readonly AppDbContext _db;
        private readonly EncryptionHelper _crypto;

        public CreditCardService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _crypto = new EncryptionHelper(config["EncryptionSettings:AESKey"]);
        }

        public void CreateCard(
            int userId,
            string holderName,
            string cardNumber,
            string cvv,
            int month,
            int year)
        {
            var card = new CreditCard.Models.CreditCard
            {
                UserId = userId,
                CardHolderName = holderName,
                CardNumberEncrypted = _crypto.Encrypt(cardNumber),
                CvvEncrypted = _crypto.Encrypt(cvv),
                ExpirationMonth = month,
                ExpirationYear = year
            };

            _db.CreditCards.Add(card);
            _db.SaveChanges();
        }

        public List<CreditCard.Models.CreditCard> GetUserCards(int userId)
        {
            return _db.CreditCards
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public string DecryptCardNumber(string encrypted)
        {
            return _crypto.Decrypt(encrypted);
        }
    }
}
