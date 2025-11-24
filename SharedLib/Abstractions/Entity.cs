using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.Abstractions
{
    public abstract class Entity<TKey>
    {
        public TKey Id { get; set; }
        //public byte[] RowVersion { get; set; }

        public bool? HasDefaultId()
        {
            switch (Id)
            {
                case long id:
                    return id == default;
                case int id:
                    return id == default;
                default:
                    return (bool?)null;
            }
        }
    }
}
