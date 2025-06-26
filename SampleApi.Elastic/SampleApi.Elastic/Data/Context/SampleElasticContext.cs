using Microsoft.EntityFrameworkCore;
using SampleApi.Elastic.Models;

namespace SampleApi.Elastic.Data.Context
{
    public class SampleElasticContext : DbContext
    {
        public SampleElasticContext(DbContextOptions<SampleElasticContext> options) : base(options)
        {
                
        }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

                entity.Property(u => u.FirstName)
                    .HasColumnName("firstName")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.LastName)
                    .HasColumnName("lastName")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasColumnName("role")
                    .HasMaxLength(50)
                    .IsRequired();
            }); 

            
        }
    }
}
