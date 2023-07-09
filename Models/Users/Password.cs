using Inzynierka.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Models
{
    public class Password
    {
        [Key]
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
        public DateTime? ModDate { get; set; }
        public string? ResetToken { get; set; }

        public int UserID { get; set; }
    }
}
