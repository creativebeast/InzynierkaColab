using Inzynierka.DAL;
using System.ComponentModel.DataAnnotations;
using Inzynierka.Models;

namespace Inzynierka.Models
{
    public class AuthToken
    {
        [Key]
        public int ID { get; set; }
        public int? CompanyID { get; set; }
        public int? UserID { get; set; }
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }

        public static bool AddReferalToken(ProjectContext context, string token, int companyId)
        {
            AuthToken companyToken = new AuthToken()
            {
                CompanyID = companyId,
                Token = token,
                CreationDate = DateTime.Now
            };
            context.AuthTokens.Add(companyToken);
            if (context.SaveChanges() != 1)
                return false;

            return true;
        }
    }
}
