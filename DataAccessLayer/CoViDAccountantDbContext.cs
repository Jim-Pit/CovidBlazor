using DbDesign.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace DbDesign
{
    public class CoViDAccountantDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public CoViDAccountantDbContext(DbContextOptions opts) : base(opts) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<DiagnosticCenter> DiagnosticCenters { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<Person> Persons { get; set; }

        public DbSet<Record> Records { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
        public DbSet<CovidTest> CovidTests { get; set; }

        public DbSet<DiagnosticCenterUser> DiagnosticCenterUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(
                userRole =>
                {
                    userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                    userRole.HasOne(ur => ur.User)
                            .WithOne(r => r.UserRole)
                            .HasForeignKey<UserRole>(ur => ur.UserId)
                            .IsRequired();
                    
                    userRole.HasOne(ur => ur.Role)
                            .WithMany()
                            .HasForeignKey(ur => ur.RoleId)
                            .IsRequired();
                });

            builder.Entity<User>(
                user =>
                {
                    user.OwnsOne(p => p.Address);
                });

            City.OnModelCreating(builder);
            District.OnModelCreating(builder);
            DiagnosticCenter.OnModelCreating(builder);
            Vaccine.OnModelCreating(builder);
            Record.OnModelCreating(builder);
            Person.OnModelCreating(builder);
            
            Record.OnModelCreating(builder);
            Vaccination.OnModelCreating(builder);
            CovidTest.OnModelCreating(builder);

            DiagnosticCenterUser.OnModelCreating(builder);
        }
    }
}
