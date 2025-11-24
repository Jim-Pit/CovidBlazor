using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DbDesign.Entities
{
    public class DiagnosticCenter : Entity<int, DiagnosticCenter>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public ValueObjects.Address Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public District Burg { get; set; }

        internal static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DiagnosticCenter>(
                e =>
                {
                    OnModelCreating(e);
                    e.Property(x => x.Name).IsRequired();
                    e.Property(x => x.Code).IsRequired();
                    e.HasIndex(x => x.Code).IsUnique();
                    e.HasOne(x => x.Burg).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);
                    e.OwnsOne(p => p.Address);
                });
        }
    }

    public class DiagnosticCenterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public ValueObjects.Address Address { get; set; } = new ValueObjects.Address();
        [Required]
        [Phone(ErrorMessage = "Digits Only")]
        public string Phone { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A District is required")]
        public District Burg { get; set; }
        [Required(ErrorMessage = "A City is required")]
        public City City { get; set; }
    }
}
