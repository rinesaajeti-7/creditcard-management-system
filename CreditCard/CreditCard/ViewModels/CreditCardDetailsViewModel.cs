namespace CreditCard.ViewModels
{
    public class CreditCardDetailsViewModel
    {
        public int Id { get; set; }

        public string CardHolderName { get; set; } = string.Empty;

        // Shfaqim vetëm 4 shifrat e fundit
        public string Last4Digits { get; set; } = string.Empty;

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }
    }
}