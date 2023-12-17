namespace Inzynierka.Models.ViewModels
{
    public class ProductTemplate
    {
        public int ID { get; set; }
        public int ProductListID { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? NettoValue { get; set; }
        public string? BruttoValue { get; set; }
        public string? PostDiscountNettoValue { get; set; }
        public int? Discount { get; set; } = 0;
        public string? VAT { get; set; }
        public string? TotalNettoValue { get; set; }
        public string? TotalBruttoValue { get; set; }
        public string? TotalValue { get; set; }
    }
}
