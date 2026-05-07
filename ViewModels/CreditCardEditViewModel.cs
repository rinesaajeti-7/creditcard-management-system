namespace CreditCard.ViewModels
{
    public class CreditCardEditViewModel
    {
        public int Id { get; set; }

        public string CardHolderName { get; set; } = string.Empty;

        // Vetëm për shfaqje (nuk editohet numri i kartës)
        public string Last4Digits { get; set; } = string.Empty;

        // CVV i ri – lëre bosh fillimisht për siguri
        public string Cvv { get; set; } = string.Empty;

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }
    }
}