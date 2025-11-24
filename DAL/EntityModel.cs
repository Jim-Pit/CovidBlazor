using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign
{
    public abstract class EntityModel
		//<TKey, TEntity> : Entity<TKey>
		//where TEntity : EntityModel<TKey, TEntity>
    {
		public DateTime CreatedUtc { get; set; }
		public DateTime UpdatedUtc { get; set; }

		//internal static void OnModelCreating(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> e)
		//{
		//	e.HasKey(x => x.Id);
		//}
	}
}
