using Inzynierka.DAL;
using Inzynierka.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Data.SqlClient;

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
                    command.Parameters.AddWithValue("@ReferalCode", referalCode);
                    command.Parameters.AddWithValue("@IsOwner", 0);

                    return command.ExecuteNonQuery();
                }
                //string username = $"DECLARE @Username={userToAdd.Username}";
                //string email = $" DECLARE @Email={userToAdd.Email}";
                //string? phone = userToAdd.Phone != null ? $"DECLARE @Phone={userToAdd.Phone}" : null;
                //string pswd = $"DECLARE @Password={password}";
                //string refCode = $"DECLARE @ReferalCode={referalCode}";
                //string isOwner = $"DECLARE @IsOwner=0";

                //FormattableString dbQuery = $"EXECUTE {procedureName}, {username}, {email}, {phone}, {pswd}, {refCode}, {isOwner}"
                //return this.context.Database.ExecuteSqlInterpolated($"EXECUTE {procedureName}, {username}, {email}, {phone}, {pswd}, {refCode}, {isOwner}");
            } 
            catch (Exception e)
            {
                return 0;
            }
        }

        //Add User + company they own
        public int CreateAccount(User userToAdd, string password, Company comapnyToAdd)
        {
            return 0;
        }

        public User CheckForUserLogin(string username, string password)
        {
            User user;
            var users = context?.Users.FromSqlInterpolated($"sp_Login_User @Username = {username}, @Password = {password}").ToList();
            if (users?.Count == 1)
                user = users[0];
            else
                user = new User();

            return user;
        }
        #endregion 
    }
}
