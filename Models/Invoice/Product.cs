using MessagePack;
using Microsoft.EntityFrameworkCore;

namespace Inzynierka.Models
{
    [PrimaryKey(nameof(ID))]
    public class Product
    {
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public Decimal NettoValue { get; set; }
        public Decimal Discount { get; set; } = 0;
        public Decimal PostDiscountNettoValue { get; set; }
        public Decimal VAT { get; set; }
        public Decimal TotalNettoValue { get; set; }
        public Decimal VATValue { get; set; }
        public Decimal BruttoValue { get; set; }
    }
}
