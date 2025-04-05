using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }
        public DbSet<StationDistance> StationDistances { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Station>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<Station>()
                .HasIndex(s => s.Order)
                .IsUnique();

            modelBuilder.Entity<StationDistance>()
                .HasOne(sd => sd.Station1)
                .WithMany()
                .HasForeignKey(sd => sd.Station1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StationDistance>()
                .HasOne(sd => sd.Station2)
                .WithMany()
                .HasForeignKey(sd => sd.Station2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = 1,
                    MinFare = 20.0000m,
                    FarePerKm = 5.0000m,
                    QrCodeValidTime = 1440,
                    QrCodeTicketValidTime = 2880,
                    TripTimeLimit = 120,
                    CreatedAt = new DateTime(2025, 03, 14, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 03, 14, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<User>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(s => s.Phone)
                .IsUnique();
        }
    }
}
