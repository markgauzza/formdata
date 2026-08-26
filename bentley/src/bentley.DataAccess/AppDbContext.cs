using bentley.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace bentley.DataAccess
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<FormData> FormData { get; set; }
    }
}
