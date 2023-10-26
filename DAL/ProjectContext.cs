using Inzynierka.Models;
using Inzynierka.Models.Generic;
using Inzynierka.Models.Stylings;
using Microsoft.EntityFrameworkCore;

namespace Inzynierka.DAL
{
    public class ProjectContext : DbContext
    {
        //User related Tables
        public Microsoft.EntityFrameworkCore.DbSet<User> Users { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Password> Passwords { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<UserStyling> UserStylings { get; set; }
        //Company related Tables
        public Microsoft.EntityFrameworkCore.DbSet<AuthToken> AuthTokens { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Company> Companies { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Client> Clients { get; set; }
        //Invoice related Tables
        public Microsoft.EntityFrameworkCore.DbSet<Invoice> Invoices { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Product> Products { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ProductList> ProductsList { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ArchInvoice> ArchInvoices { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ArchProduct> ArchProducts { get; set; }
        //Generic Tables
        public Microsoft.EntityFrameworkCore.DbSet<DefaultStyling> DefaultStylings { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Links> Links { get; set; }
        //Styling Tables
        public Microsoft.EntityFrameworkCore.DbSet<Styling> Stylings { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TextStyling> TextStyling { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TableStyling> TableStyling { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<SpecialStyling> SpecialStyling { get; set; }
        //Tables linking other tables together
        public Microsoft.EntityFrameworkCore.DbSet<InvoiceHistory> InvoiceHistory { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Worker> Workers { get; set; }

        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {

        }

    }
}
