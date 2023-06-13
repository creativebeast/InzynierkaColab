using Inzynierka.Models;
using Inzynierka.Models.Users;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Inzynierka.DAL
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(): base("ProjectContext")
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
