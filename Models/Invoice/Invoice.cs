namespace Inzynierka.Models
{
    public class Invoice
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string SellerName { get; set; }
        public string SellerAdress { get; set; }
        public string SellerPostalCode { get; set; }
        public string ?SellerPhone { get; set; }
        public string ?SellerEmail { get; set; }
        public string SellerBankName { get; set; }
        public string SellerBankNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDate { get; set; }
        public bool IncludesDelivery { get; set; }
        public int ProductListId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAdress { get; set; }
        public string BuyerPostalCode { get; set; }
        public string ?BuyerPhone { get; set; }
        public string ?BuyerEmail { get; set; }
        public string BuyerBankName { get; set; }
        public string BuyerBankNumber { get; set; }

    }
}
