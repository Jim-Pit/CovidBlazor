using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign.Entities
{
    public class Vaccine : Entity<short, Vaccine>
    {
        public string Code { get; set;}
        public string Description { get; set; }
        public int Cuts { get; set; }

        internal static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Vaccine>(
                e =>
                {
                    OnModelCreating(e);
                    e.HasIndex(x => x.Code).IsUnique();
                });
        }
    }
}
