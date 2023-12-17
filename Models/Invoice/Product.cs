using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        public int ProductListID { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public Decimal NettoValue { get; set; }
        public Decimal BruttoValue { get; set; }
        public Decimal PostDiscountNettoValue { get; set; }
        public Decimal Discount { get; set; } = 0;
        public Decimal VAT { get; set; }
        public Decimal TotalNettoValue { get; set; }
        public Decimal TotalBruttoValue { get; set; }
        public Decimal TotalValue { get; set; }
    }
}
