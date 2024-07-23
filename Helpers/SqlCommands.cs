using Inzynierka.DAL;
using Inzynierka.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;
using NuGet.Common;

namespace Inzynierka.Helpers
{

    public class SqlCommandsManager
    {
        public ProjectContext context;
        public SqlCommandsManager(ProjectContext context)
        {
            this.context = context;
        }

        public string GetConnFromSettings(IConfiguration configuration)
        {
            string connString = configuration.GetSection("ConnectionStrings").GetSection("InzynierkaContext").Value;
            if (String.IsNullOrEmpty(connString))
                throw new Exception("Cannot get Value from appsettings or it's empty");

            return connString;
        }
        #region AccountCreation
        //Add user + add them to target company workers db
        public int CreateAccount(User userToAdd, string password, string referalCode)
        {
            string procedureName = "sp_Add_User_Referal";
            try
            {
                using (var conn = (SqlConnection)context.Database.GetDbConnection())
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procedureName;
                    command.Parameters.AddWithValue("@Username", userToAdd.Username);
                    command.Parameters.AddWithValue("@Email", userToAdd.Email);
                    if (!String.IsNullOrEmpty(userToAdd.Phone))
                        command.Parameters.AddWithValue("@Phone", userToAdd.Phone);
                    else
                        command.Parameters.AddWithValue("@Phone", DBNull.Value); // Zmiana na DBNull.Value
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@ReferalCode", referalCode);
                    command.Parameters.AddWithValue("@IsOwner", 0);

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                return 0;
            }
        }


        //Add User + company they own
        public int CreateAccount(User userToAdd, string password, Company comapnyToAdd)
        {
            string procedureName = "sp_Add_User";
            try
            {
                using (var conn = (SqlConnection)context.Database.GetDbConnection())
                {
                    conn.Open();
                    var command = conn.CreateCommand();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procedureName;
                    command.Parameters.AddWithValue("@Username", userToAdd.Username);
                    command.Parameters.AddWithValue("@Email", userToAdd.Email);
                    if (!String.IsNullOrEmpty(userToAdd.Phone))
                        command.Parameters.AddWithValue("@Phone", userToAdd.Phone);
                    else
                        command.Parameters.AddWithValue("@Phone", null);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@IsOwner", 1);

                    command.Parameters.AddWithValue("@Companyname", comapnyToAdd.Name);
                    command.Parameters.AddWithValue("@Username", null);
                    command.Parameters.AddWithValue("@CompanyPostalCode", comapnyToAdd.PostalCode);
                    command.Parameters.AddWithValue("@CompanyCity", comapnyToAdd.City);
                    command.Parameters.AddWithValue("@CompanyProvince", comapnyToAdd.Province);
                    command.Parameters.AddWithValue("@CompanyStreet", comapnyToAdd.Street);
                    command.Parameters.AddWithValue("@CompanyLocalNumber", comapnyToAdd.LocalNumber);
                    command.Parameters.AddWithValue("@CompanyNIP", comapnyToAdd.NIP);

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public User CheckForUserLogin(string username, string password)
        {
            //List<User> users = context.Users.FromSqlInterpolated($"sp_Login_User @Username = {username}, @Password = {password}").ToList();
            var user = context.Users.Where(u => u.Username == username).FirstOrDefault();
            if (user == null)
                return new User();

            var checkPassword = context.Passwords.Where(p => p.UserID == user.ID && p.UserPassword == password).Any();

            if (checkPassword)
                return user;
            else
                return new User();
        }
        #endregion

        #region AuthTokens
        public int CreateAuthToken(string token, string id, bool isCompany)
        {
            string procedureName = "sp_Add_Token";
            try
            {
                using (var conn = (SqlConnection)context.Database.GetDbConnection())
                {
                    conn.Open();
                    var command = conn.CreateCommand();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procedureName;
                    command.Parameters.AddWithValue("@Token", token);
                    if (isCompany)
                        command.Parameters.AddWithValue("@CompanyID", int.Parse(id));
                    else
                        command.Parameters.AddWithValue("@UserID", int.Parse(id));

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        //Find matching token for login action
        public AuthToken FindLoginAuthToken(string token, out bool foundMatch)
        {
            try
            {
                var authToken = context?.AuthTokens?.FromSqlInterpolated($"sp_Find_Token @Token = {token}").ToList().FirstOrDefault();

                if (authToken != null)
                {
                    foundMatch = true;
                    context?.Remove(authToken);
                    context?.SaveChanges();
                    return authToken;
                }
                else
                {
                    foundMatch = false;
                    return null;
                }
                    
            }
            catch (Exception e)
            {
                foundMatch = false;
                return null;
            }
        }
        #endregion

        #region Stylings
        public int CreateStyling(XElement textStyling, XElement tableStyling, XElement specialStyling, 
            string creatorName, int creatorId, string stylingName, string referenceToken)
        {
            string procedureName = "sp_Add_Stylings";
            try
            {
                using (var conn = (SqlConnection)context.Database.GetDbConnection())
                {
                    conn.Open();
                    var command = conn.CreateCommand();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procedureName;
                    command.Parameters.AddWithValue("@textStyling", textStyling.ToString());
                    command.Parameters.AddWithValue("@tableStyling", tableStyling.ToString());
                    command.Parameters.AddWithValue("@specialStyling", specialStyling.ToString());
                    command.Parameters.AddWithValue("@stylingName", stylingName);
                    command.Parameters.AddWithValue("@creatorId", creatorId);
                    command.Parameters.AddWithValue("@creatorUsername", creatorName);
                    command.Parameters.AddWithValue("@referenceToken", referenceToken);

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        #endregion
    }
}
