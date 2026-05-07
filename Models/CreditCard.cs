namespace CreditCard.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string CardHolderName { get; set; }
        public string CardNumberEncrypted { get; set; }
        public string CvvEncrypted { get; set; }

        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
    }
}
