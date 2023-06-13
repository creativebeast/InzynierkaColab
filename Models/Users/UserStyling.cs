using System;

namespace Inzynierka.Models
{
    public class UserStyling
    {
        public int ID { get; set; }
        public string StylingsToken { get; set; }
        public int TableStylingID { get; set; }
        public int TextStylingID { get; set; }
        public int SpecialStylingID { get; set; }
    }
}
