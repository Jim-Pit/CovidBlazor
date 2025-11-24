using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign.Entities
{
    public abstract class Record : Entity<long, Record>
    {
        public DateTime EventDate { get; set; }
        public Person Person { get; set; }
        public DiagnosticCenter DiagnosticCenter { get; set; }

        //public abstract RecordType GetRecordType();
        public abstract string GetRecordType();

        internal static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Record>(
                e =>
                {
                    OnModelCreating(e);
                    OnBaseModelCreating<Record>(builder);
                    e.HasOne(x => x.Person).WithMany(p => p.Records).IsRequired().OnDelete(DeleteBehavior.Restrict);
                });
        }

        internal static void OnBaseModelCreating<TRecord>(ModelBuilder builder)
            where TRecord : Record
        {
            builder.Entity<TRecord>(
                e =>
                {
                    e.HasDiscriminator<string>("RecordType");
                    e.Property("RecordType").HasMaxLength(128);
                    e.HasIndex("RecordType");
                    e.HasOne(x => x.DiagnosticCenter).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);
                });
        }
    }

    public class Vaccination : Record
    {
        [System.ComponentModel.DataAnnotations.Required]
        public Vaccine Vaccine { get; set; }

        //public override RecordType GetRecordType() => RecordType.Vaccination;
        public override string GetRecordType() => nameof(Vaccination);

        internal static new void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Vaccination>(
                e =>
                {
                    OnBaseModelCreating<Record>(builder);
                    e.HasOne(x => x.Vaccine).WithMany()/*.IsRequired()*/.OnDelete(DeleteBehavior.Restrict);
                });
        }
    }

    public class CovidTest : Record
    {
        public string TestType { get; set; }
        public bool? Result { get; set; }

        //public override RecordType GetRecordType() => RecordType.CovidTest;
        public override string GetRecordType() => nameof(CovidTest);

        internal static new void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Vaccination>(
                e =>
                {
                    OnBaseModelCreating<Record>(builder);
                });
        }
    }

    public static class CovidTestTypes
    {
        public const string Rapid = nameof(Rapid);
        public const string PCR = nameof(PCR);
        public const string IgA = nameof(IgA);
        public const string IgC = nameof(IgC);
    }

    public enum RecordType
    {
        Vaccination,
        CovidTest
    }

    public enum CovidTestResult
    {
        Negative,
        Void,
        Positive
    }
}
