using CreditCard.Data;         
using CreditCard.Helpers;      
using CreditCard.Models;       

namespace CreditCard.Services 
{
    // Klasa e shërbimit për menaxhimin e kredit kartelave
    public class CreditCardService
    {
        // Fusha private për kontekstin e bazës së të dhënave
        private readonly AppDbContext _db;
        // Fusha private për ndihmësin e enkriptimit
        private readonly EncryptionHelper _crypto;

        // Konstruktori që inicializon varësitë e shërbimit
        public CreditCardService(AppDbContext db, IConfiguration config)
        {
            // Inicializon kontekstin e bazës së të dhënave
            _db = db;
            // Krijon instancën e EncryptionHelper duke përdorur çelësin AES nga konfigurimi
            _crypto = new EncryptionHelper(config["EncryptionSettings:AESKey"]);
        }

        // Metodë për krijimin e një kredit kartele të re
        public void CreateCard(
            int userId,           
            string holderName,    
            string cardNumber,    
            string cvv,           
            int month,            
            int year)            
        {
            // Krijon një objekt të ri të kredit kartelës
            var card = new CreditCard.Models.CreditCard
            {
                // Vendos ID-në e përdoruesit
                UserId = userId,
                // Vendos emrin e zotëruesit
                CardHolderName = holderName,
                // Enkripton dhe vendos numrin e kartelës
                CardNumberEncrypted = _crypto.Encrypt(cardNumber),
                // Enkripton dhe vendos kodin CVV
                CvvEncrypted = _crypto.Encrypt(cvv),
                // Vendos muajin e skadencës
                ExpirationMonth = month,
                // Vendos vitin e skadencës
                ExpirationYear = year
            };

            // Shton kartelën në kontekstin e bazës së të dhënave
            _db.CreditCards.Add(card);
            // Ruaj ndryshimet në bazën e të dhënave
            _db.SaveChanges();
        }

        // Metodë për marrjen e të gjitha kredit kartelave të një përdoruesi
        public List<CreditCard.Models.CreditCard> GetUserCards(int userId)
        {
            // Kërkon dhe kthen të gjitha kartelat e përdoruesit të dhënë
            return _db.CreditCards
                .Where(c => c.UserId == userId)  // Filtron sipas ID-së së përdoruesit
                .ToList();                       // Kthen si listë
        }

        // Metodë për dekriptimin e numrit të kartelës
        public string DecryptCardNumber(string encrypted)
        {
            // Dekripton numrin e kartelës së enkriptuar
            return _crypto.Decrypt(encrypted);
        }
    }
}