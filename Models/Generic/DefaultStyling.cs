using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models
{
    public class DefaultStyling
    {
        [Key]
        public int ID { get; set; }
        public string StylingsToken { get; set; }
        public int TableStylingID { get; set; }
        public int TextStylingID { get; set; }
        public int SpecialStylingID { get; set; }
    }
}
