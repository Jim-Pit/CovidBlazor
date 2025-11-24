using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DbDesign.Entities
{
    public class City : Entity<int, City>
    {
        public string Name { get; set; }
        public List<District> Districts { get; set; }

        internal static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<City>(
                e =>
                {
                    OnModelCreating(e);
                    e.HasIndex(x => x.Name).IsUnique();
                });
        }
    }
}
