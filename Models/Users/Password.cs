using Inzynierka.Helpers;

namespace Inzynierka.Models
{
    public class Password
    {
        public int ID { get; set; }
        public string UserPassword { 
            get  => _userPassword;
            set {
                if(value != null)
                {
                    _userPassword = Encryption.Encrypt(value);
                }
            }
        }

        private string _userPassword;
        public int? LastModified { get; set; }
        public string? ResetToken { get; set; }

        public int UserID { get; set; }
    }
}
