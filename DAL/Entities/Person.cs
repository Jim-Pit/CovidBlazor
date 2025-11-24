using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign.Entities
{
    public class Person : Entity<Guid, Person>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => FirstName + " " + LastName;

        public string AMKA { get; set; }
        public string Tel { get; set; }
        
        public List<Record> Records { get; set; }

        internal static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Person>(
                e =>
                {
                    OnModelCreating(e);
                    e.Property(x => x.AMKA).IsRequired();
                    e.HasIndex(x => x.AMKA).IsUnique();
                });
        }
    }
}
