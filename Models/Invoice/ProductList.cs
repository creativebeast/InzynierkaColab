using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models
{
    public class ProductList
    {
        [Key]
        public int ID { get; set; }
        public int ProductListID { get; set; }
        public int InvoiceID { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
