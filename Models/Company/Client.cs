namespace Inzynierka.Models
{
    public class Client
    {
        public int ID { get; set; }
        public int RelatedCompanyID { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Street { get; set; }
        public string LocalNumber { get; set; }
        public string NIP { get; set; }
        public bool IsCompany { get; set; }
        public string? LastModified { get; set; }
    }
}
