using Inzynierka.DAL;
using MessagePack;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Inzynierka.Models
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

        public static User GetUserById(ProjectContext context, int userId)
        {
            try
            {
                User targetUser;
                using (SqlConnection conn = new SqlConnection(context.Database.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "Select * From Users where ID = @userId";
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            targetUser = new User()
                            {
                                ID = userId,
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                StylingID = int.Parse(reader["StylingID"].ToString()),
                                Privilage = int.Parse(reader["Privilage"].ToString()),
                                CreationDate = DateTime.Parse(reader["CreationDate"].ToString())
                            };

                            conn.Close();
                            return targetUser;
                        }

                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    
        public static User? GetUserByUsernamePassword(ProjectContext context, string username, string userPassword)
        {
            User? user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return null;
            //return user;
            Password password = context.Passwords.FirstOrDefault(p => p.UserID == user.ID);
            if (password == null)
                return null;
            string pass = password.UserPassword;
            if (pass == userPassword)
                return user;
            //string password = context.Passwords.Where(p => p.UserID == user.ID).FirstOrDefault().UserPassword;
            return null;
        }
        public static int UpdateUserPassword(ProjectContext context, int userId, string password)
        {
            Password? userPassword = context.Passwords.Where(p => p.UserID == userId)?.FirstOrDefault();
            if (userPassword == null)
                return 0;

            userPassword.UserPassword = password;
            context.Update(userPassword);
            context.SaveChanges();
            return 1;
        }

        public static bool CheckIfPasswordMatch(ProjectContext context, int userId, string password)
        {
            string? oldPassword = context.Passwords.FirstOrDefault(p => p.UserID == userId)?.UserPassword;
            if (String.IsNullOrEmpty(oldPassword) || oldPassword != password)
                return false;

            return true;
        }

        public static bool UpdatePhoneNumber(ProjectContext context, int userId, string newNumber)
        {
            User? targetUser = context.Users.FirstOrDefault(u => u.ID == userId);
            if (targetUser == null)
                return false;

            targetUser.Phone = newNumber;
            context.Update(targetUser);
            if (context.SaveChanges() != 1)
                return false;

            return true;
        }

    }
}
