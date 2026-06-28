using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Models;

namespace gestionpaises.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<CountryLanguage> CountryLanguages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANTE: siempre primero, configura las tablas de Identity
            base.OnModelCreating(modelBuilder);

            // --- Country ---
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(c => c.Code);
            });

            // --- City ---
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(c => c.ID);

                entity.HasOne(c => c.Country)
                      .WithMany(p => p.Cities)
                      .HasForeignKey(c => c.CountryCode)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- CountryLanguage ---
            modelBuilder.Entity<CountryLanguage>(entity =>
            {
                entity.HasKey(cl => new { cl.CountryCode, cl.Language });

                entity.HasOne(cl => cl.Country)
                      .WithMany(p => p.CountryLanguages)
                      .HasForeignKey(cl => cl.CountryCode)
                      .OnDelete(DeleteBehavior.Restrict);

                // IsOfficial en C# es bool, pero en MySQL es ENUM('T','F').
                // Esta conversión le dice a EF Core cómo traducir entre ambos.
                entity.Property(cl => cl.IsOfficial)
                      .HasConversion(
                          v => v ? "T" : "F",
                          v => v == "T");
            });
        }
    }
}