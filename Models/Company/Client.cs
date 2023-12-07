using Inzynierka.DAL;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Threading.Channels;

namespace Inzynierka.Models
{
    public class Client
    {
        [Key]
        public int ID { get; set; }
        public int RelatedCompanyID { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Street { get; set; }
        public string LocalNumber { get; set; }
        public string ContactNumber { get; set; }
        public string ContactMail { get; set; }
        public string NIP { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public bool IsCompany { get; set; }
        public string? LastModified { get; set; }

        public static List<Client>? GetClientsRelatedToCompany(ProjectContext context, int companyId)
        {
            //List<Client>? relatedClients = context.Clients.Where(c => c.RelatedCompanyID == companyId)?.ToList();
            //if (relatedClients == null || relatedClients.Count == 0)
            //    return null;

            //return relatedClients;

            try
            {
                List<Client> clients = new List<Client>();
                using (SqlConnection conn = new SqlConnection(context.Database.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "Select * From Clients where Clients.RelatedCompanyId = @companyId";

                    SqlCommand command = new SqlCommand(sql, conn);
                    command.Parameters.AddWithValue("@companyId", companyId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Client comp = new Client()
                            {
                                ID = int.Parse(reader["ID"].ToString()),
                                RelatedCompanyID = int.Parse(reader["RelatedCompanyID"].ToString()),
                                Name = reader["Name"].ToString(),
                                OwnerName = reader["OwnerName"].ToString(),
                                PostalCode = reader["PostalCode"].ToString(),
                                City = reader["City"].ToString(),
                                Province = reader["Province"].ToString(),
                                Street = reader["Street"].ToString(),
                                LocalNumber = reader["LocalNumber"].ToString(),
                                BankAccountNumber = reader["BankAccountNumber"].ToString(),
                                BankName = reader["BankName"].ToString(),
                                ContactMail = reader["ContactMail"].ToString(),
                                ContactNumber = reader["ContactNumber"].ToString(),
                                NIP = reader["NIP"].ToString(),
                                IsCompany = bool.Parse(reader["isCompany"].ToString()),
                                LastModified = reader["Name"].ToString()
                            };

                            clients.Add(comp);
                        }
                    }
                    conn.Close();
                    return clients;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Client? GetClientByID(ProjectContext context, int clientID)
        {
            Client? client = context.Clients.FirstOrDefault(c => c.ID == clientID);
            return client;
        }

        public static bool CreateNewClient(ProjectContext context, IFormCollection collection, int companyId, out string companyName)
        {
            Client newClient = new Client()
            {
                RelatedCompanyID = companyId,
                Name = collection["clientName"].ToString() ?? String.Empty,
                OwnerName = collection["clientOwnerName"].ToString() ?? String.Empty,
                PostalCode = collection["clientPostalCode"].ToString() ?? String.Empty,
                City = collection["clientCity"].ToString() ?? String.Empty,
                Province = collection["clientProvince"].ToString() ?? String.Empty,
                Street = collection["clientStreet"].ToString() ?? String.Empty,
                LocalNumber = collection["clientLocalNumber"].ToString() ?? String.Empty,
                ContactNumber = collection["clientContactNumber"].ToString() ?? String.Empty,
                ContactMail = collection["clientContactMail"].ToString() ?? String.Empty,
                NIP = collection["clientNIP"].ToString() ?? String.Empty,
                BankName = collection["clientBankName"].ToString() ?? String.Empty,
                BankAccountNumber = collection["clientBankAccountNumber"].ToString() ?? String.Empty,
                IsCompany = collection["clientIsCompany"].ToString() == "0" ? false : true,
                LastModified = DateTime.Now.ToString()
            };

            companyName = newClient.Name;

            //Check for empty values
            foreach (PropertyInfo property in newClient.GetType().GetProperties())
            {
                if (property.Name != "Province" && property.PropertyType == typeof(string))
                {
                    string? value = (string?)property.GetValue(newClient, null);
                    if (String.IsNullOrEmpty(value))
                        return false;
                }
            }

            context.Clients.Add(newClient);
            if (context.SaveChanges() == 0)
                return false;

            return true;
        }

        public static bool UpdateTargetCompany(ProjectContext context, Dictionary<string,string> changes, int clientID, int companyID)
        {
            Client? clientToChange = context.Clients.FirstOrDefault(c => c.RelatedCompanyID == companyID && c.ID == clientID);

            if (clientToChange == null)
                return false;

            foreach (var change in changes)
            {
                PropertyInfo? prop = clientToChange.GetType().GetProperty(change.Key);
                if (prop == null)
                    continue;
                else if (prop.Name == "IsCompany")
                    prop.SetValue(clientToChange, change.Value == "0" ? false : true);
                else
                    prop.SetValue(clientToChange, change.Value, null);
            }

            clientToChange.LastModified = DateTime.Now.ToString();
            context.Clients.Update(clientToChange);
            if (context.SaveChanges() == 0)
                return false;

            return true;
        }

        public static bool DeleteTargetCompany(ProjectContext context, int clientID, int relatedCompanyID, int relatedCompanyOwnerID)
        {
            Client? clientToDelete = context.Clients.
                Where(
                    c => c.ID == clientID && 
                    context.Companies.Where(com => com.ID == relatedCompanyID && com.OwnerID == relatedCompanyOwnerID).Any()
                )?.FirstOrDefault();

            if (clientToDelete == null)
                return false;

            context.Clients.Remove(clientToDelete);
            if (context.SaveChanges() == 0)
                return false;

            return true;
        }
    }
}
