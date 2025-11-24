using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign.Entities.ValueObjects
{
    public class Address
    {
        public string Street { get; set; }
        public int Number { get; set; }
        public string Zip { get; set; }

        public static void CopyValues(Address from, Address to)
        {
            to.Street = from.Street;
            to.Number = from.Number;
            to.Zip = from.Zip;
        }

        public override string ToString()
        {
            return $"{Street} {Number}{(string.IsNullOrWhiteSpace(Zip) ? "" : $", {Zip}")}";
        }
    }
}
