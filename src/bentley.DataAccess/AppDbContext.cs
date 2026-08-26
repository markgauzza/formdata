using bentley.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace bentley.DataAccess
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<FormData> FormData { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FormData>().HasQueryFilter(f => f.Active);


            modelBuilder.Entity<FormData>(entity =>
            {
                entity.ToTable("FormData");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("FormDataId");

                entity.Property(e => e.Subject)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasColumnType("text");

                entity.Property(e => e.Critical)
                      .IsRequired()
                      .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.CreatedBy)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.UpdatedBy)
                      .HasMaxLength(50);
                
                entity.Property(e => e.Active)
                      .HasColumnName("Active")
                      .HasColumnType("bit")
                      .IsRequired()
                      .HasDefaultValue(true);
            });
        }
    }
}
