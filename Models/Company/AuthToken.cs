namespace Inzynierka.Models.Company
{
    public class AuthToken
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Token { get; set; }
    }
}
