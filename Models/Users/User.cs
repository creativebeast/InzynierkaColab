using System;

namespace Inzynierka.Models.Users
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }

        public string Email { get; set; }

        public string? Phone { get; set; }

        public int StylingID { get; set; }

        public int Privilage { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
