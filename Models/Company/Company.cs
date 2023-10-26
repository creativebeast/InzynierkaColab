using Inzynierka.DAL;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Inzynierka.Models
{
    public class Company
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public int OwnerID { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Street { get; set; }
        public string LocalNumber { get; set; }
        public string NIP { get; set; }

        public string ?LastModified { get; set; }

        public static List<Company> getCompaniesRelatedToWorker(ProjectContext context, int userId)
        {
            try
            {
                List<Company> companies = new List<Company>();
                using (SqlConnection conn = new SqlConnection(context.Database.GetConnectionString()))
                {
                    conn.Open();

                    string sql = "Select * From Companies inner join Workers on Workers.companyId = Companies.Id where Workers.UserId = @userId";
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Company comp = new Company()
                            {
                                ID = int.Parse(reader["ID"].ToString()),
                                Name = reader["Name"].ToString(),
                                OwnerName = reader["OwnerName"].ToString(),
                                OwnerID = int.Parse(reader["OwnerID"].ToString()),
                                PostalCode = reader["PostalCode"].ToString(),
                                City = reader["City"].ToString(),
                                Province = reader["Province"].ToString(),
                                Street = reader["Street"].ToString(),
                                LocalNumber = reader["LocalNumber"].ToString(),
                                NIP = reader["NIP"].ToString(),
                                LastModified = reader["Name"].ToString()
                            };

                            companies.Add(comp);
                        }
                    }
                    conn.Close();
                    return companies;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<Company> getCompaniesRelatedToOwner(ProjectContext context, int userId)
        {
            try
            {
                List<Company> companies = new List<Company>();
                using (var conn = (SqlConnection)context.Database.GetDbConnection())
                {
                    conn.Open();
                    string sql = "Select * From Companies where OwnerID = @userId";
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Company comp = new Company()
                            {
                                ID = int.Parse(reader["ID"].ToString()),
                                Name = reader["Name"].ToString(),
                                OwnerName = reader["OwnerName"].ToString(),
                                OwnerID = int.Parse(reader["OwnerID"].ToString()),
                                PostalCode = reader["PostalCode"].ToString(),
                                City = reader["City"].ToString(),
                                Province = reader["Province"].ToString(),
                                Street = reader["Street"].ToString(),
                                LocalNumber = reader["LocalNumber"].ToString(),
                                NIP = reader["NIP"].ToString(),
                                LastModified = reader["Name"].ToString()
                            };

                            companies.Add(comp);
                        }
                    }
                    conn.Close();
                    return companies;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    
        public static bool removeUserFromCompany(ProjectContext context, int userId, int companyId)
        {
            Worker? workerUser = context.Workers.FirstOrDefault(w => w.UserID == userId && w.CompanyID == companyId);
            if (workerUser == null)
                return false;

            context.Remove(workerUser);
            if (context.SaveChanges() == 1)
                return true;

            return false; ;
        }

        public static Company? getCompanyByOwnerID(ProjectContext context, int ownerId, int companyId)
        {
            Company? targetCompany = context.Companies.Where(w => w.OwnerID == ownerId && w.ID == companyId)?.FirstOrDefault();
            return targetCompany;
        }

        public static Company? getCompanyByID(ProjectContext context, int companyId)
        {
            Company? targetCompany = context.Companies.Where(w => w.ID == companyId)?.FirstOrDefault();
            return targetCompany;
        }

        public static bool CreateNewCompany(ProjectContext context, IFormCollection collection, int ownerId, out string companyName)
        {
            Company newCompany = new Company()
            {
                Name = collection["companyName"].ToString() ?? String.Empty,
                OwnerName = collection["companyOwnerName"].ToString() ?? String.Empty,
                OwnerID = ownerId,
                PostalCode = collection["companyPostalCode"].ToString() ?? String.Empty,
                City = collection["companyOwnerName"].ToString() ?? String.Empty,
                Province = collection["companyProvince"].ToString() ?? String.Empty,
                Street = collection["companyName"].ToString() ?? String.Empty,
                LocalNumber = collection["companyOwnerName"].ToString() ?? String.Empty,
                NIP = collection["companyPostalCode"].ToString() ?? String.Empty,
                LastModified = DateTime.Now.ToString()
            };

            companyName = newCompany.Name;

            //Check for empty values
            foreach (PropertyInfo property in newCompany.GetType().GetProperties())
            {
                if (property.Name != "Province" && property.PropertyType == typeof(string))
                {
                    string? value = (string?)property.GetValue(newCompany, null);
                    if (String.IsNullOrEmpty(value))
                        return false;

                }
            }

            context.Companies.Add(newCompany);
            if (context.SaveChanges() == 0)
                return false;

            return true;
        }

        public static bool UpdateCompanyData(ProjectContext context, Dictionary<string, string> changes, int ownerId, int companyId)
        {
            Company? companyToChange = context.Companies.FirstOrDefault(c => c.OwnerID == ownerId && c.ID == companyId);

            if (companyToChange == null)
                return false;

            foreach (var change in changes)
            {
                companyToChange.GetType().GetProperty(change.Key)?.SetValue(companyToChange, change.Value, null);
            }

            companyToChange.LastModified = DateTime.Now.ToString();
            context.Companies.Update(companyToChange);
            if (context.SaveChanges() == 0)
                return false;

            return true;
        }

        public static bool DeleteCompanyByID(ProjectContext context, int ownerId, int companyId)
        {
            Company? targetCompany = context.Companies.FirstOrDefault(c => c.OwnerID == ownerId && c.ID == companyId);
            if (targetCompany == null)
                return false;

            context.Companies.Remove(targetCompany);
            if (context.SaveChanges() == 0)
                return false;

            return true;
        }
    }
}
