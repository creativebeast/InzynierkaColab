using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models.Generic
{
    public class Links
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string ?AltText { get; set; }
        public string TargetURL { get; set; }
    }
}
