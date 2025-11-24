using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbDesign.Entities;

namespace CoViDAccountant.Mappings
{
    public static class Mapper
    {
		public static DiagnosticCenter Map(DiagnosticCenterModel model, DiagnosticCenter entity)
		{
			if (entity == null)
				return new DiagnosticCenter
				{
					Name = model.Name,
					Email = model.Email,
					Phone = model.Phone,
					Burg = model.Burg,
					Address = model.Address,
					Code = $"{nameof(DiagnosticCenter)}_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}"
				};

			entity.Name = model.Name;
			entity.Email = model.Email;
			entity.Phone = model.Phone;
			entity.Burg = model.Burg;
			entity.Address = model.Address;
			return entity;
		}

		public static DiagnosticCenterModel Map(DiagnosticCenter entity)
		{
			return entity == null 
				? new DiagnosticCenterModel()
				: new DiagnosticCenterModel
				{
					Id = entity.Id,
					Name = entity.Name,
					Email = entity.Email,
					Phone = entity.Phone,
					Address = entity.Address,
					Burg = entity.Burg,
					//City = entity.Burg.City
				};
		}
	}
}
