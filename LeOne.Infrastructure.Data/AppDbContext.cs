using LeOne.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeOne.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<SpaService> SpaServices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SpaService configuration
            modelBuilder.Entity<SpaService>(entity =>
            {
                entity.ToTable("SpaServices");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                      .IsRequired();

                entity.Property(e => e.PriceInCents)
                      .IsRequired();

                entity.Property(e => e.DurationMinutes)
                      .IsRequired();

                entity.Property(e => e.Description);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                      .IsRequired();

                entity.Property(e => e.PriceInCents)
                      .IsRequired();

                entity.Property(e => e.Description);

                entity.Property(e => e.OrderedAt)
                      .IsRequired(false);
            });

            // Review configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Reviews");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.ByUserId)
                      .IsRequired();

                entity.Property(e => e.TransactionId)
                      .IsRequired();

                entity.Property(e => e.TransactionType)
                      .IsRequired();

                entity.Property(e => e.Mark)
                      .IsRequired();

                entity.Property(e => e.Description);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.OwnsOne(e => e.Name, name =>
                {
                    name.Property(n => n.FirstName)
                        .IsRequired();

                    name.Property(n => n.LastName)
                        .IsRequired();
                });

                entity.OwnsOne(e => e.Email, email =>
                {
                    email.Property(e => e.Value)
                         .IsRequired();
                });

                entity.OwnsOne(e => e.Password, password =>
                {
                    password.Property(p => p.Hash)
                            .IsRequired();

                    password.Property(p => p.Salt)
                            .IsRequired();
                });

                entity.Property(e => e.Role)
                      .IsRequired();
            });
        }
    }
}
