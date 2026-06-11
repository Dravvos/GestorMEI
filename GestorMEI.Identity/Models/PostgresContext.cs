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

    }
}
