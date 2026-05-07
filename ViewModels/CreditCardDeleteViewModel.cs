namespace CreditCard.ViewModels
{
    public class CreditCardDeleteViewModel
    {
        public int Id { get; set; }

        public string CardHolderName { get; set; } = string.Empty;

        public string Last4Digits { get; set; } = string.Empty;

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }
    }
}