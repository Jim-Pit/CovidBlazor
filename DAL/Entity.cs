using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign
{
    public abstract class Entity<TKey, TEntity> : SharedClasses.Abstractions.Entity<TKey>
		where TEntity : Entity<TKey, TEntity>
	{
		//public TKey Id { get; set; }

		internal static void OnModelCreating(EntityTypeBuilder<TEntity> e)
		{
			e.HasKey(x => x.Id);
		}
	}
}
