namespace Inzynierka.Models.Company
{
    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public int OwnerID { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Street { get; set; }
        public string LocalNumber { get; set; }
        public string NIP { get; set; }

        public string ?LastModified { get; set; }

    }
}
