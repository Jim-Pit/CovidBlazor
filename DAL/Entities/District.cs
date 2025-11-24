using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign.Entities
{
    public class District : Entity<int, District>
    {
        public string Name { get; set; }
        public City City { get; set; }

        internal static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<District>(
                e =>
                {
                    OnModelCreating(e);
                    e.HasIndex(x => x.Name).IsUnique();
                    e.HasOne(x => x.City).WithMany(y => y.Districts).IsRequired().OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
