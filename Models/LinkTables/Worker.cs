using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models
{
    public class Worker
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string WorkerId { get; set; }
        public int Privilages { get; set; }
        public string CompanyID { get; set; }
    }
}
