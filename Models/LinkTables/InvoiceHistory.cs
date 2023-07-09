using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models
{
    public class InvoiceHistory
    {
        [Key]
        public int ID { get; set; }
        public int CompanyID { get; set; }
        public int InvoiceID { get; set; }
    }
}
