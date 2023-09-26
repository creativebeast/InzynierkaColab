namespace Inzynierka.Models.Stylings
{
    public class Styling
    {
        public int ID { get; set; }
        public int TableStylingId { get; set; }
        public int TextStylingId { get; set; }
        public int SpecialStylingId { get; set; }
        public string StylingName { get; set; }
        public string ReferenceToken { get; set; }

    }
}
