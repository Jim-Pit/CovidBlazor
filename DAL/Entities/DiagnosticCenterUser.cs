using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign.Entities
{
    public class DiagnosticCenterUser : Entity<long, DiagnosticCenterUser>
    {
        public DiagnosticCenter DiagnosticCenter { get; set; }
        public User User { get; set; }

        internal static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DiagnosticCenterUser>(
                e =>
                {
                    OnModelCreating(e);
                    e.HasOne(x => x.DiagnosticCenter).WithMany().IsRequired();//.OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.User).WithMany().IsRequired();//.OnDelete(DeleteBehavior.Cascade);
                    e.HasIndex("DiagnosticCenterId", "UserId").IsUnique();
                });
        }
    }
}
