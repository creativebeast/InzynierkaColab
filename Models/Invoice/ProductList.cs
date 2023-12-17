using Inzynierka.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models
{
    public class ProductList
    {
        [Key]
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public Decimal TotalNettoValue { get; set; }
        public Decimal TotalBruttoValue { get; set; }
        public Decimal TotalPostDiscountValue { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
