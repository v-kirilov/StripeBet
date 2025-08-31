using StripeBet.Models;
using Microsoft.EntityFrameworkCore;

namespace StripeBet.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Balance).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Transaction>(tx =>
            {
                tx.HasKey(tx => tx.Id);
                tx.Property(tx => tx.Amount).HasColumnType("decimal(18,2)");
            });


            modelBuilder.Entity<BetResultViewModel>(tx =>
            {
                tx.HasKey(b => b.Id);
                tx.Property(b => b.BetAmount).HasColumnType("decimal(18,2)");
                tx.Property(b => b.NewBalance).HasColumnType("decimal(18,2)");
                tx.Property(b => b.Winnings).HasColumnType("decimal(18,2)");
            });


            // Seed initial data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "demo",
                    Password = "AQAAAAIAAYagAAAAENMcLD0OuaRDqiKH92oFZrEJNrcsCvBR67n6tPUeqGUHJV4VeQKDiLdgoLRGNZ5G8g==",
                    Email = "demo@example.com",
                    Balance = 1000.00m,
                    CreatedAt = new DateTime(2025, 1, 1, 10, 0, 0),
                }
            );
        }
    }
}