namespace Inzynierka.Models.Users
{
    public class Password
    {
        public string UserPassword { get; set; }
        public int? LastModified { get; set; }
        public string? ResetToken { get; set; }

        public int UserID { get; set; }
    }
}
