using Inzynierka.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Inzynierka.DAL
{
    public class ProjectContext : DbContext
    {
        //User related Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Password> Password { get; set; }
        public DbSet<UserStyling> UserStyling { get; set; }
        //Company related Tables
        public DbSet<AuthToken> AuthToken { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Client> Client { get; set; }
        //Invoice related Tables
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ArchInvoice> ArchInvoice { get; set; }
        public DbSet<ArchProduct> ArchProduct { get; set; }
        //Generic Tables
        //Tables linking other tables together
        public DbSet<InvoiceHistory> InvoiceHistory { get; set; }
        public DbSet<Worker> Worker { get; set; }

        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {

        }

    }
}
