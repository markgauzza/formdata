using bentley.api.Models;
using Microsoft.EntityFrameworkCore;

namespace bentley.api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<FormData> FormData { get; set; }
    }
}
