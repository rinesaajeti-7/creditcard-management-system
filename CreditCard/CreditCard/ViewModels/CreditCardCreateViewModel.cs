namespace CreditCard.ViewModels
{
    public class CreditCardCreateViewModel
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
    }
}
