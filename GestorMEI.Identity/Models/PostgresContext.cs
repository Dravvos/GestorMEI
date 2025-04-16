using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestorMEI.Identity.Models
{
    public class PostgresContext : IdentityDbContext<ApplicationUser>
    {
        public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
        {
        }

        public PostgresContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = Environment.GetEnvironmentVariable("MEIConn");
            optionsBuilder.UseNpgsql(config);
        }
    }
}
