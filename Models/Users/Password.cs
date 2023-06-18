namespace Inzynierka.Models.Users
{
    public class Password
    {
        public int ID { get; set; }
        public string UserPassword { get; set; }
        public int? LastModified { get; set; }
        public string? ResetToken { get; set; }

        public int UserID { get; set; }
    }
}
